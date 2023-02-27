using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Models.UI;
using Newtonsoft.Json;
using Web.UI.Models;

namespace Web.UI.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private readonly string baseAPI_Url = System.Configuration.ConfigurationManager.AppSettings["BaseUrl"];
        private readonly string token = System.Web.HttpContext.Current?.Session["Token"]?.ToString();
       

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
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // This doesn't count login failures towards account lockout
            // To enable password failures to trigger account lockout, change to shouldLockout: true
            var loginModel = new UserLogin() { Username = model.Email, Password = model.Password };

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(baseAPI_Url);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var json = JsonConvert.SerializeObject(loginModel);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                // Let the runtime unwrap the task for you!
                var result1 = await client.PostAsync("api/account/token", content);

                var res = result1.Content.ReadAsStringAsync().Result;

                var loginResult = JsonConvert.DeserializeObject<TokenModel>(res);

                switch (loginResult?.Success)
                {
                    case true:
                        Session["LoginStatus"] = model.Email;
                        Session["Token"] = loginResult.AccessToken;
                        Session["Username"] = loginResult.FullName;
                        Session["UserId"] = loginResult.UserId;
                        Session["RoleId"] = loginResult.Roles.FirstOrDefault().RoleId;
                        return RedirectToAction("Index", "Home");
                    default:
                        foreach (var error in loginResult.Errors)
                        {
                            ModelState.AddModelError("", error);
                        }
                        return View(model);
                }
            }
        }

       
        //
        // GET: /Account/Register
        [AllowAnonymous]
        public async Task<ActionResult> Register()
        {
            var model = new RegisterViewModel();
            model.Roles = GetRoles().Result;
            model.Genders = GetGenders().Result;

            return View(model);
        }

        //
        // POST: /Account/Register
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(baseAPI_Url);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    var json = JsonConvert.SerializeObject(model);
                    var content = new StringContent(json, Encoding.UTF8, "application/json");

                    var result1 = await client.PostAsync("api/account/register", content);

                    var res = result1.Content.ReadAsStringAsync().Result;

                    var result = JsonConvert.DeserializeObject<BaseResponse>(res);

                    if (result.Success)
                    {

                        base.ModelState.Clear();
                        base.ViewBag.Accounts = "active";
                        return RedirectToAction("Login", "Account");
                    }
                    AddErrors(result);
                }
            }

            // If we got this far, something failed, redisplay form
            model.Roles = GetRoles().Result;
            model.Genders = GetGenders().Result;
            return View(model);
        }

        private void AddErrors(BaseResponse result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        private async Task<List<RoleModel>> GetRoles()
        {
            var roles = new List<RoleModel>();

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(baseAPI_Url);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var result1 = await Task.Run(() => client.GetAsync("api/account/getroles")).ConfigureAwait(false);

                var res = result1.Content.ReadAsStringAsync().Result;

                var rolesResult = JsonConvert.DeserializeObject<List<RoleModel>>(res);

                roles = rolesResult;
            }

            return roles;
        }

        private async Task<List<GenderModel>> GetGenders()
        {
            var genders = new List<GenderModel>();

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(baseAPI_Url);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var result1 = await Task.Run(() => client.GetAsync("api/employee/getgender")).ConfigureAwait(false);

                var res = result1.Content.ReadAsStringAsync().Result;

                var gendersResult = JsonConvert.DeserializeObject<List<GenderModel>>(res);

                genders = gendersResult;
            }

            return genders;
        }

        //
        // GET: /Account/ForgotPassword
        [AllowAnonymous]
        public ActionResult ForgotPassword()
        {
            return View();
        }


        //
        // GET: /Account/ForgotPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ForgotPasswordConfirmation()
        {
            return View();
        }

        //
        // GET: /Account/ResetPassword
        [AllowAnonymous]
        public ActionResult ResetPassword(string code)
        {
            return code == null ? View("Error") : View();
        }



        //
        // GET: /Account/ResetPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ResetPasswordConfirmation()
        {
            return View();
        }

        //
        // POST: /Account/ExternalLogin
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ExternalLogin(string provider, string returnUrl)
        {
            // Request a redirect to the external login provider
            return new ChallengeResult(provider, Url.Action("ExternalLoginCallback", "Account", new { ReturnUrl = returnUrl }));
        }


        //
        // POST: /Account/LogOff
        [AllowAnonymous]
        [HttpGet]
        public ActionResult LogOff()
        {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            Session["LoginStatus"] = null;
            Session["Token"] = null;
            Session["Username"] = null;
            Session["UserId"] = null;
            Session["RoleId"] = null;
            return RedirectToAction("Index", "Home");
        }

        //
        // GET: /Account/ExternalLoginFailure
        [AllowAnonymous]
        public ActionResult ExternalLoginFailure()
        {
            return View();
        }


        #region Helpers
        // Used for XSRF protection when adding external logins
        private const string XsrfKey = "XsrfId";

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        private async Task<ActionResult> RedirectToLocal(string returnUrl, string email = null)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Home");
        }

        internal class ChallengeResult : HttpUnauthorizedResult
        {
            public ChallengeResult(string provider, string redirectUri)
                : this(provider, redirectUri, null)
            {
            }

            public ChallengeResult(string provider, string redirectUri, string userId)
            {
                LoginProvider = provider;
                RedirectUri = redirectUri;
                UserId = userId;
            }

            public string LoginProvider { get; set; }
            public string RedirectUri { get; set; }
            public string UserId { get; set; }

            public override void ExecuteResult(ControllerContext context)
            {
                var properties = new AuthenticationProperties { RedirectUri = RedirectUri };
                if (UserId != null)
                {
                    properties.Dictionary[XsrfKey] = UserId;
                }
                context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
            }
        }
        #endregion
    }
}