using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types;
using Telegram.Bot;
using TaskManager.Data;
using TaskManager.Interfaces;
using TaskManager.Models;
using Telegram.Bot.Types.ReplyMarkups;

namespace TaskManager.Bots.Telegram
{
    public class TelegramBot
    {
        private readonly ITelegramBotClient _botClient;
        private readonly ITelegramRegistrStateStorage _stateStorage;
        private readonly IAccountService _accountService;
        private readonly IUtilService _utilService;

        private string _urlSite;
        public string TelegramBotUrl {  get; private set; }

        public TelegramBot(ITelegramBotClient botClient, ITelegramRegistrStateStorage stateStorage,
            IAccountService accountService, IUtilService utilService, string urlSite, string urlTgBot)
        {
            _botClient = botClient;
            _stateStorage = stateStorage;
            _accountService = accountService;
            _utilService = utilService;
            _urlSite = urlSite;
            TelegramBotUrl = urlTgBot;
        }

        public async Task HandleUpdateAsync(Update update)
        {
            try
            {
                if (update.Type == UpdateType.Message && update.Message?.Type == MessageType.Text)
                {
                    var message = update.Message;
                    var chatId = message.Chat.Id;

                    if (message.Text.Equals(ApplicationConstants.START_COMMAND, StringComparison.OrdinalIgnoreCase))
                    {
                        await SendWelcomeMessageAsync(chatId);
                        return;
                    }

                    if (message.Text.Equals(ApplicationConstants.BUTTON_REGISTER_TG, StringComparison.OrdinalIgnoreCase))
                    {
                        await StartRegistrationProcessAsync(chatId, message.From?.Username);
                        return;
                    }

                    if (message.Text.Equals(ApplicationConstants.BUTTON_START_APP_TG, StringComparison.OrdinalIgnoreCase))
                    {
                        var user = await _accountService.GetUserByIDTelegram($"{message.From.Username}");

                        if (user != null)
                        {
                            await StartApplication(message.From.Username, $"{chatId}");
                        }
                        else
                        {
                            await _botClient.SendMessage(chatId,
                                string.Format(ApplicationConstants.REGISTER_NEED_MESSAGE,
                                ApplicationConstants.NAME_APPLICATION));
                        }

                        return;
                    }

                    var state = await _stateStorage.GetStateAsync(chatId);

                    if (state != null)
                    {
                        if (state.IsExpired)
                        {
                            await _botClient.SendMessage(chatId, ApplicationConstants.REGISTER_CANCEL_MESSAGE);
                            return;
                        }

                        await ProcessRegistrationStepAsync(state, message.Text);
                    }
                    else if (!message.Text.StartsWith("/"))
                    {
                        await SendRegistrationSuggestionAsync(chatId, message.From.Username);
                    }

                }
                else if (update.Type == UpdateType.CallbackQuery)
                {
                    var callbackQuery = update.CallbackQuery;
                    var chatId = callbackQuery.Message.Chat.Id;
                    var data = callbackQuery.Data;

                    if (data == ApplicationConstants.REGISTER_START_DATA_NAME_TG)
                    {
                        await StartRegistrationProcessAsync(chatId, callbackQuery.From.Username);
                    }
                    else if (data == ApplicationConstants.REGISTER_CANCEL_DATA_NAME_TG)
                    {
                        await _stateStorage.RemoveStateAsync(chatId);
                        await _botClient.SendMessage(
                            chatId,
                            ApplicationConstants.REGISTER_CANCEL_MESSAGE
                        );
                    }
                    else if (data == ApplicationConstants.REGISTER_CONFIM_DATA_NAME_TG)
                    {
                        await ConfirmRegistrationAsync(chatId);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex}");
            }
        }

        private async Task SendWelcomeMessageAsync(long chatId)
        {
            await _botClient.SendMessage(
                chatId, string.Format(ApplicationConstants.HELLO_MESSAGE, ApplicationConstants.NAME_APPLICATION), replyMarkup: GetMenuApp()
            );
        }

        private async Task SendRegistrationSuggestionAsync(long chatId, string username)
        {
            var user = await _accountService.GetUserByIDTelegram(username);


            if (user != null)
            {
                await _botClient.SendMessage(
                    chatId,
                    string.Format(ApplicationConstants.MESSAGE_SIGIN_USER, username, ApplicationConstants.NAME_APPLICATION), replyMarkup: GetMenuApp()
                );
            }
            else
            {
                await _botClient.SendMessage(chatId, ApplicationConstants.NOT_FOUND_COMMAND_MESSAGE, replyMarkup: GetMenuApp());
            }
        }

        private async Task StartRegistrationProcessAsync(long chatId, string? username)
        {
            ApplicationUser user = await _accountService.GetUserByIDTelegram(username);

            if (user == null)
            {
                var state = new TelegramRegistrState
                {
                    ChatId = chatId,
                    CurrentStep = RegistrationStepModel.WaitingForEmail,
                    TelegramUsername = username
                };

                await _stateStorage.SaveStateAsync(state);

                await _botClient.SendMessage(
                    chatId,
                    ApplicationConstants.REGISTER_STEP_ONE,
                    replyMarkup: new InlineKeyboardMarkup(new[]
                    {
                    new[] { InlineKeyboardButton.WithCallbackData(ApplicationConstants.BUTTON_CANCEL_REGISTER_TG,
                    ApplicationConstants.REGISTER_CANCEL_DATA_NAME_TG) }
                    })
                );
            }
            else
            {
                await _botClient.SendMessage(
                    chatId, ApplicationConstants.ALREADY_REGISTERED_MESSAGE);
            }
        }

        private async Task ProcessRegistrationStepAsync(TelegramRegistrState state, string messageText)
        {
            switch (state.CurrentStep)
            {
                case RegistrationStepModel.WaitingForEmail:
                    await ProcessEmailStepAsync(state, messageText);
                    break;
                case RegistrationStepModel.WaitingForFirstName:
                    await ProcessFirstNameStepAsync(state, messageText);
                    break;
                case RegistrationStepModel.WaitingForPassword:
                    await ProcessPasswordStepAsync(state, messageText);
                    break;
                default:
                    await _botClient.SendMessage(
                        state.ChatId,
                        ApplicationConstants.REGISTER_ERROR_MESSAGE
                    );
                    await _stateStorage.RemoveStateAsync(state.ChatId);
                    break;
            }
        }

        private async Task ProcessEmailStepAsync(TelegramRegistrState state, string email)
        {
            if (!_utilService.IsValidEmail(email))
            {
                await _botClient.SendMessage(
                    state.ChatId,
                    ApplicationConstants.REGISTER_CORRECT_EMAIL
                );
                return;
            }

            var existsEmail = await _accountService.GetUserByEmail(email);

            if (existsEmail == null)
            {
                state.Email = email;
                state.CurrentStep = RegistrationStepModel.WaitingForFirstName;
                await _stateStorage.SaveStateAsync(state);

                await _botClient.SendMessage(
                    state.ChatId,
                    ApplicationConstants.REGISTER_STEP_THREE
                );
            }
            else
            {
                await _botClient.SendMessage(
                      state.ChatId, string.Format(
                       ApplicationConstants.ERROR_EXISTS_EMAIL, email)
                 );
            }
        }

        private async Task ProcessFirstNameStepAsync(TelegramRegistrState state, string firstName)
        {
            if (string.IsNullOrWhiteSpace(firstName) || firstName.Length > ApplicationConstants.MAX_LEGHT_NAME 
                || firstName.Length < ApplicationConstants.MIN_LEGHT_NAME)
            {
                await _botClient.SendMessage(
                    state.ChatId,
                    string.Format(ApplicationConstants.REGISTER_CORRECT_NAME, 
                    ApplicationConstants.MIN_LEGHT_NAME, 
                    ApplicationConstants.MAX_LEGHT_NAME)
                );
                return;
            }

            state.FirstName = firstName;
            state.CurrentStep = RegistrationStepModel.WaitingForPassword;
            await _stateStorage.SaveStateAsync(state);

            await _botClient.SendMessage(
                state.ChatId,
                string.Format(ApplicationConstants.REGISTER_STER_FREE, ApplicationConstants.MIN_LEGHT_PASSWORD)
            );
        }

        private async Task ProcessPasswordStepAsync(TelegramRegistrState state, string password)
        {
            if (!_utilService.IsStrongPassword(password))
            {
                await _botClient.SendMessage(
                    state.ChatId,
                    string.Format(ApplicationConstants.REGISTER_CORRECT_PASSWORD, ApplicationConstants.MIN_LEGHT_PASSWORD)
                );
                return;
            }

            state.Password = password;
            state.CurrentStep = RegistrationStepModel.WaitingForConfirmation;
            await _stateStorage.SaveStateAsync(state);

            var confirmationMessage = string.Format(ApplicationConstants.REGISTER_CONFIM_MESSAGE,
                state.Email, state.Password, state.FirstName);

            var inlineKeyboard = new InlineKeyboardMarkup(new[]
            {
                new[]
                {
                    InlineKeyboardButton.WithCallbackData(ApplicationConstants.BUTTON_CONFIM_TG,
                    ApplicationConstants.REGISTER_CONFIM_DATA_NAME_TG),
                    InlineKeyboardButton.WithCallbackData(ApplicationConstants.BUTTON_CANCEL_TG,
                    ApplicationConstants.REGISTER_CANCEL_DATA_NAME_TG)
                }
            });

            await _botClient.SendMessage(
                state.ChatId,
                confirmationMessage,
                replyMarkup: inlineKeyboard
            );
        }

        private async Task ConfirmRegistrationAsync(long chatId)
        {
            var state = await _stateStorage.GetStateAsync(chatId);
            if (state == null || !state.IsComplete)
            {
                await _botClient.SendMessage(
                    chatId,
                    ApplicationConstants.REGISTER_ERROR_MESSAGE
                );
                return;
            }

            try
            {
                var registerModel = new RegisterModel
                {
                    Email = state.Email,
                    Password = state.Password,
                    FirstName = state.FirstName,
                    TelegramID = state.TelegramUsername,
                    ChatID = $"{chatId}"              
                };

                var result = await _accountService.RegisterUser(registerModel);

                if (result.Succeeded)
                {
                    await _botClient.SendMessage(
                        chatId,
                        ApplicationConstants.REGISTER_COMPLETED_MESSAGE
                    );

                    await _stateStorage.RemoveStateAsync(chatId);
                }
                else
                {
                    var errors = string.Join("\n", result.Errors.Select(e => e.Description));
                    await _botClient.SendMessage(
                        chatId,
                        ApplicationConstants.REGISTER_ERROR_MESSAGE + $" {errors}"
                    );
                    await _stateStorage.RemoveStateAsync(chatId);
                }
            }
            catch (Exception ex)
            {
                await _botClient.SendMessage(
                    chatId,
                    ApplicationConstants.ERROR_REGISTRATION_FAILED
                );
                Console.WriteLine($"Error: {ex}");
                await _stateStorage.RemoveStateAsync(chatId);
            }
        }

        private async Task StartApplication(string userName, string chatID)
        {
            try
            {
                var webAppButton = new InlineKeyboardButton(ApplicationConstants.BUTTON_START_APP_TG)
                {
                    WebApp = new WebAppInfo { Url = _urlSite }
                };

                var inlineKeyboard = new InlineKeyboardMarkup(new[] { new[] { webAppButton } });

                await _botClient.SendMessage(
                    chatId: chatID,
                    text: ApplicationConstants.START_APP_MESSAGE,
                    replyMarkup: inlineKeyboard
                );
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                await _botClient.SendMessage(chatID, ApplicationConstants.ERROR_MESSAGE);
            }
        }

        private ReplyKeyboardMarkup GetMenuApp()
        {
            var replyKeyboard = new ReplyKeyboardMarkup(new[]
{
                 new KeyboardButton[] {ApplicationConstants.BUTTON_START_APP_TG, ApplicationConstants.BUTTON_REGISTER_TG}
            })
            {
                ResizeKeyboard = true,
                OneTimeKeyboard = false
            };

            return replyKeyboard;
        }
    }
}