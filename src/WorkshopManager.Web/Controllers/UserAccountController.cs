using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BrockAllen.MembershipReboot;
using BrockAllen.MembershipReboot.Hierarchical;


namespace WorkshopManager.Web.Controllers
{
    public class UserAccountController : Controller
    {
        private readonly AuthenticationService<HierarchicalUserAccount> _authService;
        private readonly UserAccountService<HierarchicalUserAccount> _userAccountService;

        public UserAccountController(AuthenticationService<HierarchicalUserAccount> authService)
        {
            _authService = authService;
            _userAccountService = authService.UserAccountService;
        }

        // GET: UserAccount
        [Authorize]
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Login()
        {
            return View(new LoginInputModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginInputModel model)
        {
            if (ModelState.IsValid)
            {
                HierarchicalUserAccount account;
                if (_userAccountService.AuthenticateWithUsernameOrEmail(model.Username, model.Password, out account))
                {
                    _authService.SignIn(account, model.RememberMe);

                    //if (account.RequiresTwoFactorAuthCodeToSignIn())
                    //{
                    //    return RedirectToAction("TwoFactorAuthCodeLogin");
                    //}
                    //if (account.RequiresTwoFactorCertificateToSignIn())
                    //{
                    //    return RedirectToAction("CertificateLogin");
                    //}

                    if (_userAccountService.IsPasswordExpired(account))
                    {
                        return RedirectToAction("Index", "ChangePassword");
                    }

                    if (Url.IsLocalUrl(model.ReturnUrl))
                    {
                        return Redirect(model.ReturnUrl);
                    }

                    return RedirectToAction("Index");
                }
                else
                {
                    ModelState.AddModelError("", "Invalid Username or Password");
                }
            }

            return View(model);
        }
    }

    public class LoginInputModel
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public bool RememberMe { get; set; }
        public string ReturnUrl { get; set; }
    }
}