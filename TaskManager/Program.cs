using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TaskManager.Bots.Telegram;
using TaskManager.Components;
using TaskManager.Components.Account;
using TaskManager.Data;
using TaskManager.Interfaces;
using TaskManager.Models;
using TaskManager.Repositories;
using TaskManager.Services;
using Telegram.Bot;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddScoped<ITaskService, TaskService>();
builder.Services.AddScoped<IAccountService, AccountService>();
builder.Services.AddScoped<ILoadingService, LoadingService>();
builder.Services.AddScoped<NotificationSettingsModel>();
builder.Services.AddScoped<INotificationSettingsService, NotificationSettingsService>();
builder.Services.AddScoped<IUtilService, UtilService>();
builder.Services.AddScoped<ITaskRepository, TaskRepository>();
builder.Services.AddScoped<INotificationService, NotificationTGService>();

builder.Services.AddCascadingAuthenticationState();
builder.Services.AddScoped<IdentityUserAccessor>();
builder.Services.AddScoped<IdentityRedirectManager>();
builder.Services.AddScoped<AuthenticationStateProvider, IdentityRevalidatingAuthenticationStateProvider>();

builder.Services.AddSingleton<ITelegramRegistrStateStorage, StorageStateTelegram>();
builder.Services.AddSingleton<ITelegramBotClient>(sp =>
{
    var token = builder.Configuration[ApplicationConstants.NAME_CONNECTION_TELEGRAM] ??
    throw new InvalidOperationException(string.Format(ApplicationConstants.ERROR_CONNECTION_BY_TELEGRAM, ApplicationConstants.NAME_CONNECTION_TELEGRAM));
    return new TelegramBotClient(token);
});

builder.Services.AddScoped<TelegramBot>(provider =>
{
    var botClient = provider.GetRequiredService<ITelegramBotClient>();
    var stateStorage = provider.GetRequiredService<ITelegramRegistrStateStorage>();
    var accountService = provider.GetRequiredService<IAccountService>();
    var utilService = provider.GetRequiredService<IUtilService>();

    var urlsSite = builder.Configuration[ApplicationConstants.NAME_CONNECTION_URL_APP];
    var urlTgBot = builder.Configuration[ApplicationConstants.NAME_CONNECTION_TELEGRAMBOT_URL];

    return new TelegramBot(botClient, stateStorage, accountService, utilService, urlsSite, urlTgBot);
});

builder.Services.AddHostedService<NotificationBackgroundService>();
builder.Services.AddHostedService<LongPollingTelegram>();

var connectionString = builder.Configuration.GetConnectionString(ApplicationConstants.NAME_CONNECTION_BASE) ??
    throw new InvalidOperationException(string.Format(ApplicationConstants.ERROR_CONNECTION_BY_BASE, ApplicationConstants.NAME_CONNECTION_BASE));
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;
    options.SignIn.RequireConfirmedEmail = false;

    options.Password.RequiredLength = ApplicationConstants.MIN_LEGHT_PASSWORD;
    options.Password.RequireNonAlphanumeric = ApplicationConstants.REQUIRE_SPECIAL_CHARACTER;
    options.Password.RequireDigit = ApplicationConstants.REQUIRQ_DIGIT;
    options.Password.RequireUppercase = ApplicationConstants.REQUIRQ_UPPERCASE;
    options.Password.RequireLowercase = ApplicationConstants.REQUIRQ_UPPERCASE;

    options.User.RequireUniqueEmail = true;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddSignInManager()
.AddDefaultTokenProviders();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = ApplicationConstants.LOGIN_PAGE;
    options.LogoutPath = ApplicationConstants.LOGIN_PAGE;
    options.AccessDeniedPath = ApplicationConstants.LOGIN_PAGE;

    options.Cookie.Name = ApplicationConstants.NAME_COOKIE;
    options.Cookie.HttpOnly = ApplicationConstants.VALUE_TTP_ONLY;
    options.ExpireTimeSpan = TimeSpan.FromDays(ApplicationConstants.VALUE_EXPIRE_TIME_SPAN);
    options.SlidingExpiration = ApplicationConstants.VALUE_SLIDING_EXPIRATION;

    options.Cookie.SameSite = SameSiteMode.None;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
});

builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders =
        ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
});

builder.Services.Configure<NotificationOptions>(
    builder.Configuration.GetSection(ApplicationConstants.NAME_NOTIFICATION_SETTINGS));

builder.Services.AddAuthorization();
builder.Services.AddRazorPages();
builder.Services.AddHttpContextAccessor();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAntiforgery();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.MapRazorPages();
app.Run();