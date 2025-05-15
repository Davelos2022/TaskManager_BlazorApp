using SkiaSharp;
using SkiaSharp.QrCode.Image;
using TaskManager.Data;

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

                var options = new Vector2Slim(ApplicationConstants.QR_SIZE_HEIGHT,ApplicationConstants.QR_SIZE_WIDTH);
                var qrCode = new QrCode(_telegramBotLink, options);

                using var memoryStream = new MemoryStream();
                qrCode.GenerateImage(memoryStream); 

                memoryStream.Seek(0, SeekOrigin.Begin);

                using var image = SKImage.FromEncodedData(memoryStream.ToArray());
                using var data = image.Encode(SKEncodedImageFormat.Png, 100);

                _qrCodeImageAsDataUri = string.Format("data:image/png;base64,{0}", Convert.ToBase64String(data.ToArray()));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error generating QR code: {ex}");
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