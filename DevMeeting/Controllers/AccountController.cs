using System;
using System.Threading.Tasks;
using DevMeeting.Data.Entities.User;
using DevMeeting.Data.Repositories;
using DevMeeting.Models.Account;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using DevMeeting.Models;
namespace DevMeeting.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly ILogger<AccountController> _logger;
        private readonly IUserRepository _userRepository;
        private readonly IRoleRepository _roleRepository;

        public AccountController(UserManager<User> userManager,
            SignInManager<User> signInManager,
            ILogger<AccountController> logger,
            IUserRepository userRepository,
            IRoleRepository roleRepository)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _userRepository = userRepository;
            _roleRepository = roleRepository;
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult> SignIn(SignIn model)
        {
            if (!ModelState.IsValid)
                return BadRequest();
            var user = await _userRepository.GetUserByEmail(model.Email);
            if (user is null)
                return Unauthorized(Responses.SignInFailed);
            var result = await _signInManager.PasswordSignInAsync(user,
                model.Password, model.IsPersistant, true);
            if (result.Succeeded)
                return Ok(Responses.SignInSucceeded);
            else if (result.RequiresTwoFactor)
                return Accepted(Responses.TwoFactorRequired);
            else if (result.IsLockedOut)
            {
                return Unauthorized(Responses.AccountLockedOut);
            }
            else
                return Unauthorized(Responses.SignInFailed);
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult> SignUp(SignUp model)
        {
            if (!ModelState.IsValid)
                return BadRequest();
            if (await _userRepository.UserExistsByEmail(model.Email))
                return Conflict(Responses.UserWithEmailExists);
            var user = new User()
            {
                Email = model.Email,
                CreationDate = DateTime.UtcNow,
                ModifiedDate = DateTime.UtcNow,
                BirthDate = model.BirthDate,
                FirstName = model.FirstName,
                LastName = model.LastName
            };
            var result = await _userManager.CreateAsync(user);
            if (result.Succeeded)
                return CreatedAtAction(nameof(Profile),Responses.AccountCreatedAndEmailConfirmation);
            else
                return StatusCode(StatusCodes.Status422UnprocessableEntity, result.Errors);
        }

        [HttpGet]
        public async Task<ActionResult> Profile()
        {
            return Ok();
        }
        //TODO: Add later
        // [HttpPost]
        // [AllowAnonymous]
        // public async Task<ActionResult> SignInTwoFactor(SignInTwoFactor model)
        // {
        //     var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();
        //     if (user is null)
        //         return Unauthorized(Responses.SignInFailed);
        //     var provider =  await _userManager.GetValidTwoFactorProvidersAsync(user);
        //     var result = _signInManager.TwoFactorSignInAsync();
        // }
    }
}