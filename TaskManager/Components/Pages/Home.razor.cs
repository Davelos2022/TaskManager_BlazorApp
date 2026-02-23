using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using TaskManager.Data;
using TaskManager.Models;

namespace TaskManager.Components.Pages
{
    public partial class Home
    {
        #region Structs
        private struct PriorityDisplayInfo
        {
            public string Style { get; set; }
            public string DisplayText { get; set; }
        }
        #endregion

        #region Properties
        private ApplicationUser[] _users = Array.Empty<ApplicationUser>();
        private List<TaskModel> _allTasks = new List<TaskModel>();
        private List<TaskModel> _tasks = new List<TaskModel>();

        private TaskModel _newTask = new TaskModel();
        private TaskModel? _selectedTask;

        private List<FileUploadModel> _tempFiles = new();
        private List<AttachmentInfo> _attachments = new();
        private string? _fileSizeError;

        private enum SortOption { None, ByPriority, ByDueDate, ByTitle }
        private SortOption _currentSort = SortOption.None;

        private bool _isAdminMode = false;
        private string _selectedTelegramId = string.Empty;
        private string _textMessage = string.Empty;

        private bool _showCompleted = false;
        private bool _showAddModal = false;
        private bool _showDetailsModal = false;
        private bool _showEditModal = false;
        private bool _showMessageModal = false;

        private DotNetObjectReference<Home>? _selfRef;
        #endregion

        #region Initialization
        protected override async Task OnInitializedAsync()
        {
            try
            {
                _selfRef = DotNetObjectReference.Create(this);
                _allTasks = await TaskService.GetTasksFromSQL();
                _isAdminMode = await AccountService.CheckAdminStatus();

                if (_isAdminMode) _users = await AccountService.GetAllUsers();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
            finally
            {
                ShowTasks(false);
            }
        }
        #endregion

        #region Controller Methods
        private PriorityDisplayInfo GetPriorityInfo(PriorityModel priority)
        {
            return priority switch
            {
                PriorityModel.Low => new PriorityDisplayInfo { Style = "color: green;", DisplayText = "Низкий" },
                PriorityModel.Medium => new PriorityDisplayInfo { Style = "color: orange;", DisplayText = "Средний" },
                PriorityModel.High => new PriorityDisplayInfo { Style = "color: #CD5C5C;", DisplayText = "Высокий" },
                _ => new PriorityDisplayInfo { Style = "", DisplayText = priority.ToString() }
            };
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await JSRuntime.InvokeVoidAsync(ApplicationConstants.INITIALIZATION_DRAG_DROP, _selfRef);
        }

        private async Task OnSelectedUser(ChangeEventArgs e)
        {
            _selectedTelegramId = $"{e.Value}";
            LoadingService.IsLoading = true;

            if (!string.IsNullOrWhiteSpace(_selectedTelegramId))
            {
                ApplicationUser? user = await AccountService.GetUserByIDTelegram(_selectedTelegramId);
                _allTasks = await TaskService.GetTasksFromSQL(user);
            }
            else
            {
                _allTasks = await TaskService.GetTasksFromSQL();
            }

            ShowTasks(_showCompleted);
            LoadingService.IsLoading = false;
        }

        private async Task AddTask(EditContext context)
        {
            if (context.Validate())
            {
                LoadingService.IsLoading = true;
                ApplicationUser? user = await GetUser();

                try
                {
                    _textMessage = GetMessageTasks(_newTask, NotificationTypeModel.TaskAdded);

                    if (user != null)
                    {
                        await TaskService.AddTaskToUser(user, _newTask);

                        foreach (var f in _tempFiles)
                            if (f.IsNewFile) await TaskRepository.UploadAsync(_newTask.Id, f.FileName, f.ContentType!, f.Content!);

                        await NotificationService.SendNotification(user, _textMessage, NotificationTypeModel.TaskAdded);
                        await RefreshTasks();
                        await CloseAddModal();
                    }
                }
                finally
                {
                    LoadingService.IsLoading = false;
                    ShowTasks(_showCompleted);
                }
            }
        }

        private async Task RemoveTask(TaskModel taskToRemove)
        {
            try
            {
                bool confirmed = await JSRuntime.InvokeAsync<bool>
                ("confirm", ApplicationConstants.MSG_CONFRIM_DELETED_TASK);

                if (confirmed)
                {
                    LoadingService.IsLoading = true;
                    ApplicationUser? user = await GetUser();

                    _textMessage = GetMessageTasks(taskToRemove, NotificationTypeModel.TaskDeleted);

                    if (user != null)
                    {
                        await TaskService.RemoveTask(taskToRemove);
                        await NotificationService.SendNotification(user, _textMessage, NotificationTypeModel.TaskDeleted);
                        await RefreshTasks();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
            finally
            {
                LoadingService.IsLoading = false;
                ShowTasks(_showCompleted);
            }
        }

        private async Task UpdateTask()
        {
            if (_selectedTask != null)
            {
                try
                {
                    LoadingService.IsLoading = true;
                    ApplicationUser? user = await GetUser();

                    if (user != null)
                    {
                        if (_selectedTask.IsCompleted)
                        {
                            _textMessage = GetMessageTasks(_selectedTask, NotificationTypeModel.TaskCompleted);
                            await NotificationService.SendNotification(user, _textMessage, NotificationTypeModel.TaskCompleted);
                        }
                        else if (_selectedTask.UserId != user.Id)
                        {
                            _textMessage = GetMessageTasks(_selectedTask, NotificationTypeModel.TaskAdded);
                            await NotificationService.SendNotification(user, _textMessage, NotificationTypeModel.TaskAdded);
                        }
                        else
                        {
                            _textMessage = GetMessageTasks(_selectedTask, NotificationTypeModel.TaskChanged);
                            await NotificationService.SendNotification(user, _textMessage, NotificationTypeModel.TaskChanged);
                        }

                        if (_tempFiles.Count > 0)
                        {
                            foreach (var f in _tempFiles)
                            {
                                if (f.IsDeleted) await TaskRepository.DeleteAttachmentAsync(f.Id);
                                else if (f.IsNewFile) await TaskRepository.UploadAsync(_selectedTask.Id, f.FileName, f.ContentType!, f.Content!);
                            }
                        }

                        await TaskService.UpdateTask(user, _selectedTask);
                        await RefreshTasks();
                        await CloseEdit();

                        ShowTasks(_showCompleted);
                    }
                }
                finally
                {
                    LoadingService.IsLoading = false;
                }
            }
        }

        private async Task OnFilesSelected(InputFileChangeEventArgs e)
        {
            try
            {
                LoadingService.IsLoading = true;
                _fileSizeError = null;

                foreach (var file in e.GetMultipleFiles())
                {
                    if (file.Size > ApplicationConstants.MAX_SIZE_FILE)
                    {
                        _fileSizeError = string.Format(ApplicationConstants.ERROR_UPLOAD_FILE, 
                            file.Name, UtilService.FormatFileSize(ApplicationConstants.MAX_SIZE_FILE));

                        continue;
                    }

                    await using var ms = new MemoryStream();
                    await file.OpenReadStream(ApplicationConstants.MAX_SIZE_FILE).CopyToAsync(ms);

                    _tempFiles.Add(new FileUploadModel
                    {
                        FileName = file.Name,
                        ContentType = file.ContentType,
                        Content = ms.ToArray(),
                        IsNewFile = true
                    });
                }
            }
            catch (Exception ex)
            {
                _tempFiles.Clear();
                Console.WriteLine(ex.ToString());
            }
            finally
            {
                LoadingService.IsLoading = false;
            }
        }

        private async Task LoadAttachmentInfos(Guid taskId)
        {
            _attachments = (await TaskRepository.GetAttachmentsInfoAsync(taskId)).ToList();
            _tempFiles = _attachments.Select(a => new FileUploadModel
            {
                FileName = a.FileName,
                Id = a.Id
            }).ToList();
        }

        private async Task<ApplicationUser?> GetUser()
        {
            ApplicationUser? user;

            if (_isAdminMode && !string.IsNullOrWhiteSpace(_selectedTelegramId))
            {
                user = await AccountService.GetUserByIDTelegram(_selectedTelegramId);
            }
            else
            {
                user = await AccountService.GetCurrentUser();
            }

            return user;
        }

        private void ShowTasks(bool completed)
        {
            _showCompleted = completed;
            _tasks = _allTasks.Where(t => t.IsCompleted == _showCompleted).ToList();

            switch (_currentSort)
            {
                case SortOption.ByPriority:
                    _tasks = _tasks.OrderBy(t => t.Priority).ToList();
                    break;
                case SortOption.ByDueDate:
                    _tasks = _tasks.OrderBy(t => t.DueDate).ToList();
                    break;
                case SortOption.ByTitle:
                    _tasks = _tasks.OrderBy(t => t.Title).ToList();
                    break;
                case SortOption.None:
                default:
                    _tasks = _tasks.OrderBy(t => t.SortOrder).ToList();
                    break;
            }

            StateHasChanged();
        }

        private void ChangeSort(SortOption option)
        {
            _currentSort = option;
            ShowTasks(_showCompleted);
        }

        private async Task RefreshTasks()
        {
            ApplicationUser? user = await GetUser();
            _allTasks = _isAdminMode ? await TaskService.GetTasksFromSQL(user) : await TaskService.GetTasksFromSQL();
        }

        private async void SendMessageFromAdmin()
        {
            if (_isAdminMode && !string.IsNullOrWhiteSpace(_textMessage))
            {
                try
                {
                    LoadingService.IsLoading = true;

                    var tasks = _users.Select(user => NotificationService.SendNotification(
                    user, string.Format(ApplicationConstants.NOTIFICATION_ADMIN, _textMessage), NotificationTypeModel.General));
                    await Task.WhenAll(tasks);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                }
                finally
                {
                    LoadingService.IsLoading = false;
                    await CloseSendMessage();
                }
            }
        }

        private string GetMessageTasks(TaskModel task, NotificationTypeModel notificationType)
        {
            string message = string.Empty;

            switch (notificationType)
            {
                case NotificationTypeModel.TaskAdded:
                    message = string.Format(ApplicationConstants.NOTIFICATION_TASK_ADDED, task.Title, GetPriorityInfo(task.Priority).DisplayText,
                        UtilService.FormatDate(task.DueDate.Value));
                    break;
                case NotificationTypeModel.TaskDeleted:
                    message = string.Format(ApplicationConstants.NOTIFICATION_TASK_REMOVE, task.Title);
                    break;
                case NotificationTypeModel.TaskChanged:
                    message = string.Format(ApplicationConstants.NOTIFICATION_TASK_CHANGED, task.Title);
                    break;
                case NotificationTypeModel.TaskCompleted:
                    message = string.Format(ApplicationConstants.NOTIFICATION_TASK_COMPLETED, task.Title);
                    break;
            }

            return message;
        }

        [JSInvokable]
        public async Task UpdateTasksOrder(List<string> sortOrder)
        {
            try
            {
                await TaskService.UpdateTasksSorting(_tasks, sortOrder);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                throw;
            }
        }
        #endregion

        #region PopUp Modal Methods
        private async Task ShowAddModal()
        {
            _newTask = new TaskModel();
            _fileSizeError = null;
            _showAddModal = true;

            await ResetScroll();
            await ScrollSiteControll(true);
            StateHasChanged();
        }

        private async Task CloseAddModal()
        {
            _showAddModal = false;
            _tempFiles.Clear();

            await ScrollSiteControll(false);
            StateHasChanged();
        }

        private async Task ShowDetails(TaskModel task)
        {
            LoadingService.IsLoading = true;
            _selectedTask = task.Clone();

            await LoadAttachmentInfos(_selectedTask.Id);
            await ResetScroll();
            await ScrollSiteControll(true);

            _showDetailsModal = true;
            LoadingService.IsLoading = false;
            StateHasChanged();
        }

        private async Task CloseDetails()
        {
            _showDetailsModal = false;
            _selectedTask = null;
            _tempFiles.Clear();
            _attachments.Clear();

            await ScrollSiteControll(false);
            StateHasChanged();
        }

        private async Task ShowEdit(TaskModel task)
        {
            LoadingService.IsLoading = true;
            _fileSizeError = null;
            _selectedTask = task.Clone();

            await LoadAttachmentInfos(_selectedTask.Id);
            await ResetScroll();
            await ScrollSiteControll(true);

            _showEditModal = true;
            LoadingService.IsLoading = false;
            StateHasChanged();
        }

        private async Task CloseEdit()
        {
            _showEditModal = false;
            _selectedTask = null;
            _tempFiles.Clear();

            await ScrollSiteControll(false);
            StateHasChanged();
        }

        private async Task ShowSendMessage()
        {
            _showMessageModal = true;
            _textMessage = string.Empty;
            await ScrollSiteControll(true);
            StateHasChanged();
        }

        private async Task CloseSendMessage()
        {
            _showMessageModal = false;
            await ScrollSiteControll(false);
            StateHasChanged();
        }

        private async Task ResetScroll()
        {
            try
            {
                await Task.Delay(ApplicationConstants.DELAY);
                await JSRuntime.InvokeVoidAsync(ApplicationConstants.SCRIPT_RESET_SCROLL);
            }
            catch (JSException ex)
            {
                Console.WriteLine($"{ex.Message}");
            }
        }

        private async Task ScrollSiteControll(bool control)
        {
            if (control) await JSRuntime.InvokeVoidAsync(ApplicationConstants.ADD_MODAL_OPEN);
            else await JSRuntime.InvokeVoidAsync(ApplicationConstants.REMOVE_MODAL_OPEN);
        }
        #endregion
    }
}