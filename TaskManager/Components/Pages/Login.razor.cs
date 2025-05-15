using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using TaskManager.Data;
using TaskManager.Models;

namespace TaskManager.Components.Pages
{
    public partial class Login
    {
        [SupplyParameterFromForm]
        private LoginModel? _loginModel { get; set; } = new LoginModel();

        private EditContext? _editContext;
        private ValidationMessageStore? _messageStore;

        protected override void OnInitialized()
        {
            _editContext = new EditContext(_loginModel);
            _messageStore = new ValidationMessageStore(_editContext);
        }

        private async Task LoginUser()
        {
            _messageStore.Clear();
            _editContext.NotifyValidationStateChanged();

            try
            {
                var result = await AccountService.LoginUser(_loginModel);

                if (!result.Succeeded)
                {
                    _messageStore.Add(_editContext.Field(nameof(_loginModel)), ApplicationConstants.ERROR_LOGIN_SIGIN);
                    _editContext.NotifyValidationStateChanged();
                    return;
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return;
            }

            NavigationManager.NavigateTo(ApplicationConstants.HOME_PAGE, true);
        }
    }
}