using QRCoder;
using QRCodeGenerator = QRCoder.QRCodeGenerator;

namespace TaskManager.Components.Pages
{
    public partial class Register
    {
        #region Properties
        private string _qrCodeImageAsDataUri = string.Empty;
        private string _telegramBotLink = string.Empty; 
        #endregion

        #region Initialiation
        protected override async Task OnInitializedAsync()
        {
            await GenerateQrCode();
        }
        #endregion

        #region Methods
        private async Task GenerateQrCode()
        {
            LoadingService.IsLoading = true;
            await Task.Yield();

            try
            {
                _telegramBotLink = TelegramBot.TelegramBotUrl;

                if (string.IsNullOrEmpty(_telegramBotLink))
                    throw new InvalidOperationException("TelegramBotUrl не задан");

                using var qrGenerator = new QRCodeGenerator();
                using var qrData = qrGenerator.CreateQrCode(_telegramBotLink, QRCodeGenerator.ECCLevel.Q);
                var qrPngBytes = new PngByteQRCode(qrData).GetGraphic(20);

                _qrCodeImageAsDataUri =
                    $"data:image/png;base64,{Convert.ToBase64String(qrPngBytes)}";
            }
            catch (Exception ex)
            {
                _qrCodeImageAsDataUri = "";
            }
            finally
            {
                LoadingService.IsLoading = false;
                StateHasChanged();
            }

        }
        #endregion
    }
}