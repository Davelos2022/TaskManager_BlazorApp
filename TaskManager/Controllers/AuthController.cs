using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using TaskManager.Data;
using TaskManager.Interfaces;

namespace TaskManager.Controllers
{
    [ApiController]
    [AllowAnonymous]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IAccountService _authService;

        public AuthController(IAccountService authService) => _authService = authService;

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var q = QueryHelpers.ParseQuery(Request.QueryString.Value ?? "");
            var rawInitData = Request.QueryString.Value?.TrimStart('?') ?? "";

            if (string.IsNullOrWhiteSpace(rawInitData)) return Redirect(ApplicationConstants.REGISTER_PAGE);

            var ok = await _authService.SignInFromTelegramApp(rawInitData, HttpContext);
            return ok ? Redirect(ApplicationConstants.HOME_PAGE) : Redirect(ApplicationConstants.REGISTER_PAGE);
        }
    }
}
