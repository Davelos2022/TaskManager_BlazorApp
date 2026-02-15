namespace TaskManager.Data
{
    public class ApplicationConstants
    {
        #region Navigation Page
        public const string LOGIN_PAGE = "/Login";
        public const string LOGOUT_PAGE = "/Logout";
        public const string REGISTER_PAGE = "/Register";
        public const string HOME_PAGE = "/";
        #endregion

        #region JS scripts Name
        public const string SCRIPT_RESET_SCROLL = "resetModalScroll";
        public const string INITIALIZATION_DRAG_DROP = "initializeDragAndDrop";
        public const string ADD_MODAL_OPEN = "toggleBodyClass.addModalOpen";
        public const string REMOVE_MODAL_OPEN = "toggleBodyClass.removeModalOpen";
        #endregion

        #region Messages Pop Up
        public const string MSG_CONFRIM_DELETED_TASK = "Вы уверены, что хотите удалить эту задачу?";
        #endregion

        #region Texts Form
        public const string COMPLETED_TEXT = "Выполнено";
        public const string NOT_COMPLETED_TEXT = "Не выполнено";

        public const string NOT_ACTIVATE_TASK = "Нет активных задач.";
        public const string NOT_COMPLETED_TASK = "Нет выполненных задач.";
        #endregion

        #region Errors Text Modal
        public const string ERROR_EMTY = "Поле не должно быть пустым";
        public const string ERROR_VALUE = "Некорректное значение";
        public const string ERROR_VALUE_DATE = "Дата выполнение задачи должна быть не меньше текущей";
        public const string ERROR_VALUE_NAME_TASK = "Название задачи должно быть не менее {1} символов";

        public const string ERROR_VALUE_NAME_LEGHT_MAX = "Имя должно содержать не более {1} символов";
        public const string ERROR_VALUE_NAME_LEGHT_MIN = "Имя должно содержать не менее {1} символов";
        public const string ERROR_VALUE_PASSWORD_CHAR = "Пароль должен содержать хотя бы один специальный символ";
        public const string ERROR_VALUE_PASSWORD_UPPERCASE = "Пароль должен содержать хотя бы одну заглавную букву";
        public const string ERROR_VALUE_PASSWORD_DIGIT = "Пароль должен содержать хотя бы одну цифру";
        public const string ERROR_PASSWORD_CONFIM = "Необходимо подтвердить пароль";
        public const string ERROR_PASSWORD_CURRENT = "Введённый текущий пароль неверный.";
        public const string ERROR_LOGIN_SIGIN = "Неверный Email или Пароль";
        public const string ERROR_REGISTRATION_FAILED = "Ошибка при регистрации. Пожалуйста, попробуйте снова.";
        public const string ERROR_EXISTS_EMAIL = "{0} Такой Email уже зарегистрирован";

        public const string ERROR_VALUE_TG = "Некорректный Telegram ID. Ожидается формат: @username";
        public const string ERROR_VALUE_EMAIL = "Некорректный Email";

        public const string ERROR_VALUE_PASSWORD_LEGTH = "Пароль должен содержать не менее {0} символов";
        public const string ERROR_PASSWORD_COMPARE = "Пароли не совпадают";
        #endregion

        #region Notification

        public const string NOTIFICATION_TASK_DEAD_LINE = "Задача \"{0}\" должна быть выполнена до {1}.";
        public const string NOTIFICATION_TASK_DEAD = "Задача \"{0}\" просрочена, Сроки задачи были до {1}.";
        public const string NOTIFICATION_TASK_ADDED = "Добавлена новая задача \"{0}\" Приоритет {1}, Выполнить до {2}";
        public const string NOTIFICATION_TASK_CHANGED = "Задача \"{0}\" была обновлена";
        public const string NOTIFICATION_TASK_REMOVE = "Задача \"{0}\" удалена";
        public const string NOTIFICATION_TASK_COMPLETED = "Задача \"{0}\" выполнена!";
        public const string NOTIFICATION_ADMIN = "Администратор: {0}";
        #endregion

        #region Telegram
        public const string HELLO_MESSAGE = "👋 Добро пожаловать в {0} " +
                "Я помогу Вам зарегистрироваться в системе и управлять Вашими задачами.\n\n" +
                "Нажмите кнопку ниже, чтобы начать регистрацию.";

        public const string REGISTER_NEED_MESSAGE = "Для использования {0}, Вам необходимо зарегистрироваться. Воспользуйтесь меню для Регистрации";

        public const string MESSAGE_SIGIN_USER = "Уважаемый {0} Вы пользуетесь сервисом {1}, используете меню для управление Вашими задачами, а я о них напомню.";

        public const string REGISTER_STEP_ONE = "Спасибо, что решили зарегистрироваться! Начнем процесс регистрации.\n\n" +
                "Пожалуйста, введите Ваш email:";
        public const string REGISTER_CORRECT_EMAIL = "Пожалуйста, введите корректный email адрес.";

        public const string REGISTER_STEP_THREE = "Отлично! Теперь введите Ваше имя:";
        public const string REGISTER_CORRECT_NAME = "Пожалуйста, введите корректное имя оно должно быть минимум {0} символа и максимум {1}";

        public const string REGISTER_STER_FREE = "Спасибо! Теперь введите Ваш пароль:";
        public const string REGISTER_CORRECT_PASSWORD = "Пароль должен содержать минимум {0} символов, включая обычные и заглавные буквы, специальные символы, и цифры";

        public const string REGISTER_CONFIM_MESSAGE = "Пожалуйста, проверьте введенные данные:\n\n" +
                "Email: {0}\n" +
                "Пароль: {1}\n" +
                "Имя: {2}\n\n" +
                "Всё верно?";

        public const string REGISTER_COMPLETED_MESSAGE = "🎉 Регистрация успешно завершена! Теперь Вы можете войти в систему используя ваш email и пароль.\n\n" +
                        "Этот бот также будет отправлять вам уведомления о Ваших задачах.";

        public const string REGISTER_CANCEL_MESSAGE = "Регистрация отменена. Вы можете начать снова нажав на кнопку в меню";
        public const string REGISTER_ERROR_MESSAGE = "❌ Извините, произошла ошибка. Попробуйте начать регистрацию заново";

        public const string ERROR_MESSAGE = "Произошла ошибка попробуйте снова";
        public const string ALREADY_REGISTERED_MESSAGE = "Вы уже зарегистрированы";
        public const string NOT_FOUND_COMMAND_MESSAGE = "Не понимаю о чем Вы, Воспользуйтесь меню.";
        public const string START_APP_MESSAGE = "Для перехода в личный кабинет нажмите на кнопку";
        #endregion

        #region Optrions Value
        public const string NAME_APPLICATION = "Task Manager";
        public const int DELAY = 50;

        //Data base
        public const string NAME_CONNECTION_URL_APP = "UrlSite";
        public const string NAME_CONNECTION_BASE = "DefaultConnection";
        public const string NAME_CONNECTION_TELEGRAM = "TelegramTokenBot";
        public const string NAME_CONNECTION_TELEGRAMBOT_URL = "TelegramBotURL";

        public const string ERROR_CONNECTION_BY_BASE = "Connection string '{0}' not found.";
        public const string ERROR_CONNECTION_BY_TELEGRAM = "Connection telegram string '{0}' not found.";

        //Cookies 
        public const string NAME_COOKIE = "TaskManagerAuth";
        public const bool VALUE_TTP_ONLY = true;
        public const bool VALUE_SLIDING_EXPIRATION = true;
        public const double VALUE_EXPIRE_TIME_SPAN = 1;

        //Tasks
        public const int MAX_LEGHT_NAME_TASK = 50;
        public const int MAX_LEGHT_PREVIEW_DESCRIPTION = 10;
        public const int MAX_LEGHT_PREVIEW_NAME_TASK = 35;
        public const int MIN_LEGHT_NAME = 2;
        public const int MAX_LEGHT_NAME = 12;

        //Password
        public const int MIN_LEGHT_PASSWORD = 6;
        public const bool REQUIRE_SPECIAL_CHARACTER = true;
        public const bool REQUIRQ_DIGIT = true;
        public const bool REQUIRQ_UPPERCASE = true;
        public const bool REQUIRQ_LOWERCASE = true;

        //Notification Task
        public const int INTERVAL_MINUTES = 180;
        public const int INTERVAL_HOUSE = 70;
        public const int MIN_HOUSE_NOTIFICATION = 6;
        public const int MAX_HOUSE_NOTIFICATION = 18;

        //Telegram Bot
        public const int QR_SIZE_HEIGHT = 300;
        public const int QR_SIZE_WIDTH = 300;

        public const string START_COMMAND = "/start";

        public const string REGISTER_START_DATA_NAME_TG = "start_registration";
        public const string REGISTER_CANCEL_DATA_NAME_TG = "cancel_registration";
        public const string REGISTER_CONFIM_DATA_NAME_TG = "confirm_registration";

        public const string BUTTON_START_APP_TG = "📱 Приложение";
        public const string BUTTON_REGISTER_TG = "📝 Регистрация";
        public const string BUTTON_CANCEL_REGISTER_TG = "❌ Отменить регистрацию";
        public const string BUTTON_CANCEL_TG = "❌ Отменить";
        public const string BUTTON_CONFIM_TG = "✅ Подтвердить";

        // Register Telegram constants
        public const int REGISTRATION_EXPIRY_MINUTES = 15;
        #endregion
    }
}