using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using DotNetOpenAuth.AspNet;
using Microsoft.Web.WebPages.OAuth;
using WebMatrix.WebData;
using DPTnew.Filters;
using DPTnew.Models;
using System.Net.Mail;
using System.Net;

namespace DPTnew.Controllers
{

    [Authorize]
    // [InitializeSimpleMembership]
    public class AccountController : Controller
    {

        //
        // GET: /Account/Login

        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        //
        // POST: /Account/Login

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginModel model, string returnUrl)
        {

            using (var db = new DptContext())
            {
                bool exists = db.Contacts.Any(u => u.Email == model.UserName);

                if (ModelState.IsValid && exists)
                {
                    var user = db.Contacts.Single(u => u.Email == model.UserName);

                    if (user.Company.Blocked == 1)
                    {
                        ModelState.AddModelError("", "This account has been blocked");
                        return View(model);
                    }

                    if (user.Status == 0 /*|| (user.Company.AccountStatus != "03 - Active Customer" && user.Company.AccountStatus != "03 - Premium Customer" && user.Company.AccountStatus != "06 - Partner")*/)
                    {
                        ModelState.AddModelError("", "This account has been disabled");
                        return View(model);
                    }

                    if (WebSecurity.Login(model.UserName, model.Password, persistCookie: model.RememberMe))
                    {
                        Session["MyMenu"] = null;
                        return RedirectToLocal(returnUrl);
                    }


                    /* I check if an old password exists if a new pssw doesn't exist*/
                    if (model.Password == user.OriginalPassword && !WebSecurity.IsConfirmed(model.UserName))
                    {
                        /*I set the new password equals to the old one*/
                        WebSecurity.CreateAccount(model.UserName, user.OriginalPassword, false);
                        if (WebSecurity.Login(model.UserName, model.Password, persistCookie: model.RememberMe))
                        {
                            Session["MyMenu"] = null;
                            return RedirectToLocal(returnUrl);
                        }
                    }
                }
            }
            // If we got this far, something failed, redisplay form
            ModelState.AddModelError("", "The user name or password provided is incorrect.");
            return View(model);
        }

        //View to display user information at the top 
        [ChildActionOnly]
        public PartialViewResult GetUserInfo()
        {
            using (var context = new DptContext())
            {
                if (User.Identity.IsAuthenticated)
                {
                    var id = WebSecurity.CurrentUserId;
                    var user = context.Contacts.SingleOrDefault(u => u.UserId == id);
                    ViewBag.Company = context.Companies.Single(x => x.AccountNumber == user.AccountNumber).AccountName;
                    return PartialView(user);
                }
                return PartialView();
            }
        }


        //
        // POST: /Account/LogOff

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            WebSecurity.Logout();
            Session["MyMenu"] = null;
            return RedirectToAction("Login", "Account");
        }

        // GET: Account/LostPassword
        [AllowAnonymous]
        public ActionResult LostPassword()
        {
            return View();
        }

        // GET: /Account/ResetPassword
        [AllowAnonymous]
        public ActionResult ResetPassword(string rt)
        {
            ResetPasswordModel model = new ResetPasswordModel();
            model.ReturnToken = rt;
            return View(model);
        }

        // POST: Account/LostPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult LostPassword(LostPasswordModel model)
        {
            MembershipUser user;

            using (var context = new DptContext())
            {
                bool exists = context.Contacts.Any(u => u.Email == model.Email);

                //if user exists an model is valid
                if (ModelState.IsValid && exists)
                {
                    var foundUser = context.Contacts.Single(u => u.Email == model.Email);

                    //the account is blocked?
                    if (foundUser.Company.Blocked == 1)
                    {
                        ModelState.AddModelError("", "This account has been blocked");
                        return View(model);
                    }

                    //the account is disabled?
                    if (foundUser.Status == 0 /*|| (foundUser.Company.AccountStatus != "03 - Active Customer" && foundUser.Company.AccountStatus != "03 - Premium Customer" && foundUser.Company.AccountStatus != "06 - Partner")*/)
                    {
                        ModelState.AddModelError("", "This account has been disabled");
                        return View(model);
                    }
                    user = Membership.GetUser(foundUser.Email.ToString());
                    //I check if the user has already a password associated, otherwise I create a temporary one
                    bool existuser = WebSecurity.IsConfirmed(model.Email);
                    if (!existuser)
                    {
                        WebSecurity.CreateAccount(model.Email, "passwordtemp", false);
                    }


                    // Generate password token that will be used in the email link to authenticate user
                    var token = WebSecurity.GeneratePasswordResetToken(user.UserName);
                    // Generate the html link sent via email
                    string resetLink = Url.Action("ResetPassword", "Account", new { rt = token }, "https");

                    // Email stuff
                    string subject = "Reset your password";
                    string body = "Click this link to reset your password: " + resetLink;

                    MailMessage message = new MailMessage();
                    message.To.Add(new MailAddress(model.Email));
                    message.Subject = subject;
                    message.Body = body;
                    SmtpClient client = new SmtpClient();
                    client.Credentials = new NetworkCredential(System.Configuration.ConfigurationManager.AppSettings["hostusername"],
                    System.Configuration.ConfigurationManager.AppSettings["hostpassword"]);
                    client.Host = System.Configuration.ConfigurationManager.AppSettings["host"];
                    client.Port = 587;
                    client.EnableSsl = true;

                    // Attempt to send the email
                    try
                    {
                        client.Send(message);
                    }
                    catch (Exception e)
                    {
                        ModelState.AddModelError("", "Issue sending email: " + e.Message);
                    }

                    ViewBag.title = DPTnew.Localization.Resource.TitleMsg;
                    ViewBag.message1 = DPTnew.Localization.Resource.Message1;
                    ViewBag.message2 = DPTnew.Localization.Resource.Message2;

                    return View("Result");
                }
            }


            ModelState.AddModelError("", DPTnew.Localization.Resource.CheckMailMsg);
            return View(model);
        }

        // POST: /Account/ResetPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ResetPassword(ResetPasswordModel model)
        {
            if (ModelState.IsValid)
            {
                bool resetResponse = WebSecurity.ResetPassword(model.ReturnToken, model.Password);
                if (resetResponse)
                {
                    ViewBag.title = "Password Successfully Changed";
                    ViewBag.message1 = "You have correctly reset your password";
                    return View("Result");
                }
                else
                {
                    ViewBag.Message = "Something went horribly wrong!";
                }
            }
            return View(model);
        }

        /*
        //
        // GET: /Account/Register
        [AllowAnonymous]
        public ActionResult Register()
        {
            return View();
        }

        //
        // POST: /Account/Register

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Register(RegisterModel model)
        {
            if (ModelState.IsValid)
            {
                // Attempt to register the user
                try
                {
                    WebSecurity.CreateUserAndAccount(model.UserName, model.Password,
                        new { FirstName = model.FirstName, LastName = model.LastName, 
                             Language = model.Language, Status = model.Status, EmailStatus = model.EmailStatus, AccountNumber = model.AccountNumber});
                    WebSecurity.Login(model.UserName, model.Password);
                    return RedirectToAction("Index", "Home");
                }
                catch (MembershipCreateUserException e)
                {
                    ModelState.AddModelError("", ErrorCodeToString(e.StatusCode));
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        } 

        //
        // POST: /Account/Disassociate

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Disassociate(string provider, string providerUserId)
        {
            string ownerAccount = OAuthWebSecurity.GetUserName(provider, providerUserId);
            ManageMessageId? message = null;

            // Only disassociate the account if the currently logged in user is the owner
            if (ownerAccount == User.Identity.Name)
            {
                // Use a transaction to prevent the user from deleting their last login credential
                using (var scope = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.Serializable }))
                {
                    bool hasLocalAccount = OAuthWebSecurity.HasLocalAccount(WebSecurity.GetUserId(User.Identity.Name));
                    if (hasLocalAccount || OAuthWebSecurity.GetAccountsFromUserName(User.Identity.Name).Count > 1)
                    {
                        OAuthWebSecurity.DeleteAccount(provider, providerUserId);
                        scope.Complete();
                        message = ManageMessageId.RemoveLoginSuccess;
                    }
                }
            }

            return RedirectToAction("Manage", new { Message = message });
        }

         */


        //
        // GET: /Account/Manage

        public ActionResult Manage(ManageMessageId? message)
        {
            using (var context = new DptContext())
            {
                if (User.Identity.IsAuthenticated)
                {
                    var id = WebSecurity.CurrentUserId;
                    var user = context.Contacts.SingleOrDefault(u => u.UserId == id);
                    ViewBag.User = user.FirstName + " " + user.LastName;

                }
            }
            ViewBag.StatusMessage =
                message == ManageMessageId.ChangePasswordSuccess ? "Your password has been changed."
                : message == ManageMessageId.SetPasswordSuccess ? "Your password has been set."
                : message == ManageMessageId.RemoveLoginSuccess ? "The external login was removed."
                : "";
            ViewBag.HasLocalPassword = OAuthWebSecurity.HasLocalAccount(WebSecurity.GetUserId(User.Identity.Name));
            ViewBag.ReturnUrl = Url.Action("Manage");
            return View();
        }

        //
        // POST: /Account/Manage

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Manage(LocalPasswordModel model)
        {
            bool hasLocalAccount = OAuthWebSecurity.HasLocalAccount(WebSecurity.GetUserId(User.Identity.Name));
            ViewBag.HasLocalPassword = hasLocalAccount;
            ViewBag.ReturnUrl = Url.Action("Manage");
            if (hasLocalAccount)
            {
                if (ModelState.IsValid)
                {
                    // ChangePassword will throw an exception rather than return false in certain failure scenarios.
                    bool changePasswordSucceeded;
                    try
                    {
                        changePasswordSucceeded = WebSecurity.ChangePassword(User.Identity.Name, model.OldPassword, model.NewPassword);
                    }
                    catch (Exception)
                    {
                        changePasswordSucceeded = false;
                    }

                    if (changePasswordSucceeded)
                    {
                        return RedirectToAction("Manage", new { Message = ManageMessageId.ChangePasswordSuccess });
                    }
                    else
                    {
                        ModelState.AddModelError("", "The current password is incorrect or the new password is invalid.");
                    }
                }
            }
            else
            {
                // User does not have a local password so remove any validation errors caused by a missing
                // OldPassword field
                ModelState state = ModelState["OldPassword"];
                if (state != null)
                {
                    state.Errors.Clear();
                }

                if (ModelState.IsValid)
                {
                    try
                    {
                        WebSecurity.CreateAccount(User.Identity.Name, model.NewPassword);
                        return RedirectToAction("Manage", new { Message = ManageMessageId.SetPasswordSuccess });
                    }
                    catch (Exception)
                    {
                        ModelState.AddModelError("", String.Format("Unable to create local account. An account with the name \"{0}\" may already exist.", User.Identity.Name));
                    }
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // POST: /Account/ExternalLogin

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ExternalLogin(string provider, string returnUrl)
        {
            return new ExternalLoginResult(provider, Url.Action("ExternalLoginCallback", new { ReturnUrl = returnUrl }));
        }

        //
        // GET: /Account/ExternalLoginCallback

        [AllowAnonymous]
        public ActionResult ExternalLoginCallback(string returnUrl)
        {
            AuthenticationResult result = OAuthWebSecurity.VerifyAuthentication(Url.Action("ExternalLoginCallback", new { ReturnUrl = returnUrl }));
            if (!result.IsSuccessful)
            {
                return RedirectToAction("ExternalLoginFailure");
            }

            if (OAuthWebSecurity.Login(result.Provider, result.ProviderUserId, createPersistentCookie: false))
            {
                return RedirectToLocal(returnUrl);
            }

            if (User.Identity.IsAuthenticated)
            {
                // If the current user is logged in add the new account
                OAuthWebSecurity.CreateOrUpdateAccount(result.Provider, result.ProviderUserId, User.Identity.Name);
                return RedirectToLocal(returnUrl);
            }
            else
            {
                // User is new, ask for their desired membership name
                string loginData = OAuthWebSecurity.SerializeProviderUserId(result.Provider, result.ProviderUserId);
                ViewBag.ProviderDisplayName = OAuthWebSecurity.GetOAuthClientData(result.Provider).DisplayName;
                ViewBag.ReturnUrl = returnUrl;
                return View("ExternalLoginConfirmation", new RegisterExternalLoginModel { UserName = result.UserName, ExternalLoginData = loginData });
            }
        }


        //
        // POST: /Account/ExternalLoginConfirmation

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ExternalLoginConfirmation(RegisterExternalLoginModel model, string returnUrl)
        {
            string provider = null;
            string providerUserId = null;

            if (User.Identity.IsAuthenticated || !OAuthWebSecurity.TryDeserializeProviderUserId(model.ExternalLoginData, out provider, out providerUserId))
            {
                return RedirectToAction("Manage");
            }

            if (ModelState.IsValid)
            {
                // Insert a new user into the database
                using (DptContext db = new DptContext())
                {
                    Contact user = db.Contacts.FirstOrDefault(u => u.Email.ToLower() == model.UserName.ToLower());
                    // Check if user already exists
                    if (user == null)
                    {
                        // Insert name into the profile table
                        db.Contacts.Add(new Contact { Email = model.UserName });
                        db.SaveChanges();

                        OAuthWebSecurity.CreateOrUpdateAccount(provider, providerUserId, model.UserName);
                        OAuthWebSecurity.Login(provider, providerUserId, createPersistentCookie: false);

                        return RedirectToLocal(returnUrl);
                    }
                    else
                    {
                        ModelState.AddModelError("UserName", "User name already exists. Please enter a different user name.");
                    }
                }
            }

            ViewBag.ProviderDisplayName = OAuthWebSecurity.GetOAuthClientData(provider).DisplayName;
            ViewBag.ReturnUrl = returnUrl;
            return View(model);
        }

        //
        // GET: /Account/ExternalLoginFailure

        [AllowAnonymous]
        public ActionResult ExternalLoginFailure()
        {
            return View();
        }

        [AllowAnonymous]
        [ChildActionOnly]
        public ActionResult ExternalLoginsList(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return PartialView("_ExternalLoginsListPartial", OAuthWebSecurity.RegisteredClientData);
        }

        [ChildActionOnly]
        public ActionResult RemoveExternalLogins()
        {
            ICollection<OAuthAccount> accounts = OAuthWebSecurity.GetAccountsFromUserName(User.Identity.Name);
            List<ExternalLogin> externalLogins = new List<ExternalLogin>();
            foreach (OAuthAccount account in accounts)
            {
                AuthenticationClientData clientData = OAuthWebSecurity.GetOAuthClientData(account.Provider);

                externalLogins.Add(new ExternalLogin
                {
                    Provider = account.Provider,
                    ProviderDisplayName = clientData.DisplayName,
                    ProviderUserId = account.ProviderUserId,
                });
            }

            ViewBag.ShowRemoveButton = externalLogins.Count > 1 || OAuthWebSecurity.HasLocalAccount(WebSecurity.GetUserId(User.Identity.Name));
            return PartialView("_RemoveExternalLoginsPartial", externalLogins);
        }

        #region Helpers
        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        public enum ManageMessageId
        {
            ChangePasswordSuccess,
            SetPasswordSuccess,
            RemoveLoginSuccess,
        }

        internal class ExternalLoginResult : ActionResult
        {
            public ExternalLoginResult(string provider, string returnUrl)
            {
                Provider = provider;
                ReturnUrl = returnUrl;
            }

            public string Provider { get; private set; }
            public string ReturnUrl { get; private set; }

            public override void ExecuteResult(ControllerContext context)
            {
                OAuthWebSecurity.RequestAuthentication(Provider, ReturnUrl);
            }
        }

        private static string ErrorCodeToString(MembershipCreateStatus createStatus)
        {
            // See http://go.microsoft.com/fwlink/?LinkID=177550 for
            // a full list of status codes.
            switch (createStatus)
            {
                case MembershipCreateStatus.DuplicateUserName:
                    return "User name already exists. Please enter a different user name.";

                case MembershipCreateStatus.DuplicateEmail:
                    return "A user name for that e-mail address already exists. Please enter a different e-mail address.";

                case MembershipCreateStatus.InvalidPassword:
                    return "The password provided is invalid. Please enter a valid password value.";

                case MembershipCreateStatus.InvalidEmail:
                    return "The e-mail address provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidAnswer:
                    return "The password retrieval answer provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidQuestion:
                    return "The password retrieval question provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidUserName:
                    return "The user name provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.ProviderError:
                    return "The authentication provider returned an error. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

                case MembershipCreateStatus.UserRejected:
                    return "The user creation request has been canceled. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

                default:
                    return "An unknown error occurred. Please verify your entry and try again. If the problem persists, please contact your system administrator.";
            }
        }
        #endregion
    }
}
