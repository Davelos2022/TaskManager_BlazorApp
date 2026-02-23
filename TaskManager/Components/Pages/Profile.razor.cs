using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using TaskManager.Data;
using TaskManager.Models;

namespace TaskManager.Components.Pages
{
    public partial class Profile
    {
        #region Properties
        public ProfileModel? _profileUser { get; private set; }

        private NotificationSettingsModel? _settings = new();
        private NotificationSettingsModel? _tempNotification;

        private EditContext? _editContextProfile;
        private EditContext? _editContextSettings;
        private ValidationMessageStore? _messageStore;

        private ApplicationUser? _user;
        private IBrowserFile? _selectedFile;

        private bool _showProfileModal = false;
        private bool _showSettingsModal = false;
        private bool _showImageModal = false;
        private bool _showUploadModal = false;
        #endregion

        #region Initialiation
        protected override async Task OnInitializedAsync()
        {
            LoadingService.IsLoading = true;

            _user = await AccountService.GetCurrentUser();

            if (_user != null)
            {
                _profileUser = new ProfileModel(_user);

                var settings = await NotificationSettingsService.GetSettingsAsync(_user);
                _settings = settings ?? new NotificationSettingsModel { UserId = _user.Id };

                await NotificationSettingsService.SaveSettingsAsync(_settings);

                _editContextSettings = new EditContext(_settings);
                _editContextProfile = new EditContext(_profileUser);
                _messageStore = new ValidationMessageStore(_editContextProfile);
            }

            LoadingService.IsLoading = false;
        }
        
        #endregion

        #region Profile Modal
        private async Task ShowProfileModal()
        {
            _showProfileModal = true;
            await JSRuntime.InvokeVoidAsync(ApplicationConstants.ADD_MODAL_OPEN);
            StateHasChanged();
        }

        private async Task CloseProfileModal()
        {
            _showProfileModal = false;
            await JSRuntime.InvokeVoidAsync(ApplicationConstants.REMOVE_MODAL_OPEN);
            StateHasChanged();
        }
        #endregion

        #region Image Preview Modal
        private void ShowImagePreview()
        {
            _showImageModal = true;
            StateHasChanged();
        }

        private void CloseImagePreview()
        {
            _showImageModal = false;
            StateHasChanged();
        }
        #endregion

        #region Upload Modal

        private void ShowUploadModal()
        {
            _profileUser.ChangePasswordRequested = false;
            _showUploadModal = true;
            StateHasChanged();
        }

        private void CloseUploadModal()
        {
            _messageStore.Clear();
            _showUploadModal = false;
            StateHasChanged();
        }

        private void OnInputFileChange(InputFileChangeEventArgs e)
        {
            try
            {
                _selectedFile = e.File;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        private void ToggleChangePassword()
        {
            _profileUser.ChangePasswordRequested = !_profileUser.ChangePasswordRequested;
            StateHasChanged();
        }

        private async Task UploadPofile()
        {
            if (_editContextProfile.Validate())
            {
                LoadingService.IsLoading = true;

                try
                {
                    if (_user != null)
                    {
                        _user.FirstName = _profileUser.FirstName;
                        _user.TelegramID = _profileUser.TelegramID;

                        if (_selectedFile != null)
                        {
                            using (var memoryStream = new MemoryStream())
                            {
                                await _selectedFile.OpenReadStream().CopyToAsync(memoryStream);
                                _user.AvatarImage = memoryStream.ToArray();
                                _profileUser.PreviewImageUrl = $"data:{_selectedFile.ContentType};base64,{Convert.ToBase64String(_user.AvatarImage)}";
                            }

                            _selectedFile = null;
                        }

                        if (_profileUser.ChangePasswordRequested)
                        {
                            var pwdChangeResult = await AccountService.ChangePassword(_user, _profileUser.OldPassword, _profileUser.NewPassword);

                            if (!pwdChangeResult.Succeeded)
                            {
                                foreach (var error in pwdChangeResult.Errors)
                                {
                                    _messageStore.Add(_editContextProfile.Field(nameof(_profileUser.OldPassword)), error.Description);
                                }
                            }
                        }

                        await AccountService.UpdateUser(_user);
                        CloseUploadModal();
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex}");
                    _messageStore.Add(_editContextProfile.Field(nameof(_profileUser.PreviewImageUrl)), $"{ex.Message}");
                }
                finally
                {
                    _editContextProfile.NotifyValidationStateChanged();
                    LoadingService.IsLoading = false;

                    ClearForm();
                    StateHasChanged();
                }
            }
        }

        private void ClearForm()
        {
            if (_profileUser != null)
            {
                _profileUser.OldPassword = string.Empty;
                _profileUser.NewPassword = string.Empty;
                _profileUser.ConfirmPassword = string.Empty;
            }
        }
        #endregion

        #region Settings Modal
        private async Task ShowSettingsModal()
        {
            _showSettingsModal = true;
            InitializationTemp();
            await JSRuntime.InvokeVoidAsync(ApplicationConstants.ADD_MODAL_OPEN);
            StateHasChanged();
        }

        private async Task CloseSettingsModal()
        {
            if (_tempNotification != null) _settings = _tempNotification;
            _tempNotification = null;
            _showSettingsModal = false;
            await JSRuntime.InvokeVoidAsync(ApplicationConstants.REMOVE_MODAL_OPEN);
            StateHasChanged();
        }

        private void InitializationTemp()
        {
            _tempNotification = new NotificationSettingsModel();
            _tempNotification.NotifyOnTaskReminder = _settings.NotifyOnTaskReminder;
            _tempNotification.NotifyOnTaskChanged = _settings.NotifyOnTaskChanged;
            _tempNotification.NotifyOnTaskDeleted = _settings.NotifyOnTaskDeleted;
            _tempNotification.NotifyOnTaskAdded = _settings.NotifyOnTaskAdded;
            _tempNotification.NotifyOnTaskCompleted = _settings.NotifyOnTaskCompleted;
        }

        private async Task SaveSettings()
        {
            if (_editContextProfile.Validate())
            {
                LoadingService.IsLoading = true;

                _tempNotification = null;
                await NotificationSettingsService.SaveSettingsAsync(_settings);
                await CloseSettingsModal();

                LoadingService.IsLoading = false;
            }
        }
        #endregion

        #region Logout
        private void Logout()
        {
            NavigationManager.NavigateTo(ApplicationConstants.LOGOUT_PAGE, forceLoad: true);
        }
        #endregion
    }
}