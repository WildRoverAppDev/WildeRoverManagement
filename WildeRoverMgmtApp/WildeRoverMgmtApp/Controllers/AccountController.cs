using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WildeRoverMgmtApp.Models;

namespace WildeRoverMgmtApp.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IMessageService _messageService;

        public AccountController(UserManager<User> userManager, SignInManager<User> signInManager, IMessageService messageService)
        {
            this._userManager = userManager;
            this._signInManager = signInManager;
            this._messageService = messageService;
        }

        public IActionResult Index()
        {
            return View();
        }

        //Register new user page
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        //Register new user
        [HttpPost]
        public async Task<IActionResult> Register(AccountUserViewModel model)
        {
            //Check if passwords match
            if (model.Password != model.RePassword)
            {
                //Display error
                ModelState.AddModelError(string.Empty, "Password does not match.");
                return View();
            }

            //Create new user using information from view model
            var newUser = new User()
            {
                UserName = model.Email,
                Email = model.Email,
                FirstName = model.FirstName,
                LastName = model.LastName
            };

            //Use User Manager to create new user
            var userCreationResult = await _userManager.CreateAsync(newUser, model.Password);
            if (!userCreationResult.Succeeded)  //user creation failed
            {
                //Display all errors from failure adn display
                foreach (var error in userCreationResult.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }

                return View();
            }

            //Send email confirmation
            var emailConfirmationToken = await _userManager.GenerateEmailConfirmationTokenAsync(newUser);  //create confirmation token
            var tokenVerificationURL = Url.Action("VerifyEmail", "Account", new { id = newUser.Id, token = emailConfirmationToken }, Request.Scheme);  //get url

            //Send email
            await _messageService.Send(model.Email, "Verify your email", $"Click <a href=\"{tokenVerificationURL}\">here</a> to verify your email");

            //Show page telling user to check email
            return Content("Check your email for a verification link");
        }

        //Verify registration email
        //id -given in email link
        //token - given in email link
        public async Task<IActionResult> VerifyEmail(string id, string token)
        {
            //get newly created user
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)  //user not found
            {
                throw new InvalidOperationException();
            }

            //Confirm that token is correct
            var emailConfirmationResult = await _userManager.ConfirmEmailAsync(user, token);
            if (!emailConfirmationResult.Succeeded)  //Error confirming user with token
            {
                //Display error results
                return Content(emailConfirmationResult.Errors.Select(error => error.Description).Aggregate((allErrors, error) => allErrors += ", " + error));
            }

            return RedirectToAction("EmailConfirmed"); //Email successfully verified
        }

        //Show email confirmation page
        public IActionResult EmailConfirmed()
        {
            return View();
        }

        //Show Login page
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        //Post for Login page
        [HttpPost]
        public async Task<IActionResult> Login(AccountLoginViewModel model, bool rememberMe = true)
        {
            //Get user using email
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)  //user not found
            {
                ModelState.AddModelError(string.Empty, "Invalid Login");
                return View();
            }

            //Uer has not confirmed registration email
            if (!user.EmailConfirmed)
            {
                ModelState.AddModelError(string.Empty, "Confirm your email first.");
                return View();
            }

            //Sign in using sign in manager
            var passwordSignInResult = await _signInManager.PasswordSignInAsync(user, model.Password, isPersistent: rememberMe, lockoutOnFailure: false);
            if (!passwordSignInResult.Succeeded)  //sing in unsuccessful
            {
                ModelState.AddModelError(string.Empty, "Invalid Login");
                return View();
            }

            //Sign in successful
            return RedirectToAction("Index", "Home");
        }

        //Display Forgot Password Page
        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        //Post method for Forgot Password Page
        //Sends email to user with reset token
        [HttpPost]
        public async Task<IActionResult> ForgotPassword(string email)
        {
            //Find user by email
            var user = await _userManager.FindByEmailAsync(email);

            //Send the same message regardless of valid or invalid email to prevent phishing
            if (user == null)
            {
                return Content("Check your email for password reset link.");
            }

            //Obtain password reset token
            var passwordResetToken = await _userManager.GeneratePasswordResetTokenAsync(user);

            //Create Url for password reset
            var passwordResetUrl = Url.Action("ResetPassword", "Account", new { id = user.Id, token = passwordResetToken }, Request.Scheme);

            //Send email
            await _messageService.Send(email, "Password reset", $"Click <a href=\"" + passwordResetUrl + "\">here</a> to reset your password");

            //Notify user to check email (for actual users)
            return Content("Check your email for password reset link.");
        }

        //Reset password page
        //Obtained from reset password link in user email
        //id - given in email link
        //token - given in email link
        [HttpGet]
        public IActionResult ResetPassword(string id, string token)
        {
            AccountResetPasswordViewModel model = new AccountResetPasswordViewModel();
            model.Id = id;
            model.Token = token;
            model.Password = string.Empty;
            model.RePassword = string.Empty;

            return View(model);
        }

        //Post method for password reset
        [HttpPost]
        public async Task<IActionResult> ResetPassword(AccountResetPasswordViewModel model)
        {
            //Find User by Id (given in email link)
            var user = await _userManager.FindByIdAsync(model.Id);
            if (user == null)
            {
                throw new InvalidOperationException();
            }

            //Check if passwords entered matches
            if (model.Password != model.RePassword)
            {
                //Display error
                ModelState.AddModelError(string.Empty, "Passwords do not match");
                return View();
            }

            //Use user manager to reset password using information from view model
            var resetPasswordResult = await _userManager.ResetPasswordAsync(user, model.Token, model.Password);
            if (!resetPasswordResult.Succeeded)  //reset unsuccessful
            {
                //Display errors
                foreach (var error in resetPasswordResult.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }

                return View();
            }

            return RedirectToAction("ResetPasswordConfirm", "Account");
        }

        //Display successful password reset
        public IActionResult ResetPasswordConfirm()
        {
            return View();
        }

        //Change password page
        [HttpGet]
        public IActionResult ChangePassword()
        {
            return View();
        }

        //Post for ChangePassword
        [HttpPost]
        public async Task<IActionResult> ChangePassword(string currentPassword, string newPassword, string rePassword)
        {
            //get user by Id
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)  //user not found
            {
                throw new InvalidOperationException();
            }        

            //Check if passwords match
            if (newPassword != rePassword)
            {
                //Display error
                ModelState.AddModelError(string.Empty, "Password does not match.");
                return View();
            }

            //Check if currentPassword is correct
            var passwordCheck = await _userManager.CheckPasswordAsync(currentUser, currentPassword);
            if (!passwordCheck)  //current password is incorrect
            {
                //Display error
                ModelState.AddModelError(string.Empty, "Current Password incorrect.");
                return View();
            }

            //Use user manager to change the password
            var resetPasswordCheck = await _userManager.ChangePasswordAsync(currentUser, currentPassword, newPassword);
            if (!resetPasswordCheck.Succeeded)  //password change unsuccessful
            {
                //Display all errors
                foreach(var error in resetPasswordCheck.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }

                return View();
            }

            return RedirectToAction("Index", "Home");
        }

        //Display user account details
        [HttpGet]
        public async Task<IActionResult> Details()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            return View(currentUser);
        }

        //Edit User account page
        [HttpGet]
        public async Task<IActionResult> Edit()
        {
            //Get current user
            var currentUser = await _userManager.GetUserAsync(User);

            //Populate view model
            AccountUserViewModel model = new AccountUserViewModel();
            model.Email = currentUser.Email;
            model.FirstName = currentUser.FirstName;
            model.LastName = currentUser.LastName;
            model.Password = string.Empty;
            model.RePassword = string.Empty;

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(AccountUserViewModel model)
        {
            //Check if passwords match
            if (model.Password != model.RePassword)
            {
                //Display error
                ModelState.AddModelError("Password", "Passwords do not match.");
                return View(model);
            }

            //Get current user
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)  //user not found
            {
                throw new InvalidOperationException();
            }

            //Check if confirmation password is correct
            var passwordCheck = await _userManager.CheckPasswordAsync(currentUser, model.Password);
            if (!passwordCheck)  //password incorrect
            {
                //Display error
                ModelState.AddModelError("Password", "Incorrect password.");
                return View(model);
            }                

            //Update current user data
            currentUser.Email = model.Email;
            currentUser.UserName = model.Email;
            currentUser.FirstName = model.FirstName;
            currentUser.LastName = model.LastName;

            //Use user manager to update user
            await _userManager.UpdateAsync(currentUser);

            //Redirect to home page
            return RedirectToAction("Index", "Home"); 
        }

        //Get method for Logout
        //Automatically redirect to LogoutPost (safer to have logout as POST)
        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            return await LogoutPost();
        }

        //Logout User
        [HttpPost]
        public async Task<IActionResult> LogoutPost()
        {
            //Logout using sign in manager
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");  //redirect to home page
        }   
    }
}