using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using FuNDs.Models;
using System.Web.Helpers;
using System.IO;
using System.Web.Script.Serialization;
using System.Web.UI;
using System.Web.Security;
using System.Net.Mail;
using System.Configuration;
using System.Threading.Tasks;
using Facebook;
//using TweetSharp;
using Microsoft.AspNet.Identity;

using System.Web.UI.WebControls;




namespace FuNDs.Controllers
{
    public class FundRaisersController : Controller
    {
        private FundRaisersDbContext db = new FundRaisersDbContext();

        // GET: FundRaisers
        public ActionResult Index()
        {
            return View(db.FundRaisers.ToList());
        }

        // GET: FundRaisers/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            FundRaisers fundRaisers = db.FundRaisers.Find(id);
            if (fundRaisers == null)
            {
                return HttpNotFound();
            }
            return View(fundRaisers);
        }

        // GET: FundRaisers/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: FundRaisers/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "FundRaisersId,FirstName,LastName,Email,Password,ConfirmPassword,verificationToken,verified")] FundRaisers fundRaisers)
        {

            if (ModelState.IsValid)
            {

                var hashedPassword = Crypto.HashPassword(fundRaisers.Password);
                fundRaisers.Password = hashedPassword;
                fundRaisers.ConfirmPassword = hashedPassword;
                db.FundRaisers.Add(fundRaisers);
                fundRaisers.verified = false;
                fundRaisers.verificationToken = Guid.NewGuid().ToString();

                db.SaveChanges();
                HttpContext.Session.Add("fundRaiser", fundRaisers);
                //  
                this.confirmationEmailSend(fundRaisers.Email, fundRaisers.verificationToken);
                return RedirectToAction("checkEmail");

                //   HttpContext.Session.Add("fundRaiser", FundRaisers);


            }
            else
            {


                return View("fundRaisers");
            }
        }

        public ActionResult CheckEmail()
        {


            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public ActionResult SignIn()
        {
            return View();
        }

        // POST: FundRaisers/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        /*
        made slight changes here. This time using AccountLogInViewModel so that email of sign uping and siging do not concide.
        remember we are using the email just one time.
    */
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SignIn([Bind(Include = "Email,Password")] AccountLoginViewModel userTryingToLogin)
        {
            if (userTryingToLogin != null & ModelState.IsValid)
            {

                FundRaisers doesUserExist = db.FundRaisers.FirstOrDefault(s => s.Email.Equals(userTryingToLogin.Email));

                if (!doesUserExist.verified)
                {
                    ModelState.AddModelError("doesUserExist", "Email Not verified. Please check your email confirmation");
                    return View("SignInFailure");
                }

                bool a = Crypto.VerifyHashedPassword(doesUserExist.Password, userTryingToLogin.Password);
                if (a == true)
                {

                    // creating authetication ticket
                    FormsAuthentication.SetAuthCookie(userTryingToLogin.Email, false);
                    Session["userId"] = doesUserExist.FundRaisersId;
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    @ViewBag.Message = "Error.Ivalid login.";
                    return RedirectToAction("SignInFailure", "FundRaisers");
                }
            }
            ModelState.AddModelError("UserDoesNotExist", "Username or Password is Incorrect! Please try Again!!");
            return View(userTryingToLogin);

        }



        public ActionResult SignInFailure()
        {
            return View();
        }

        // myprofile action method 

        public ActionResult MyProfile()
        {
          // return RedirectToAction("", "Home")
            return View();
        }

        public ActionResult UploadImage()
        {

            return View();
        }
        public ActionResult doesEmailExist()
        {
            return View();
        }

        //   Check if the email already 
        // login with unique email address
        [HttpPost]
        public virtual JsonResult doesEmailExist(string Email)
        {
            bool CheckEmail = false;
            //Check in database that particular email address exist or not
            CheckEmail = db.FundRaisers.Any(s => s.Email == Email);
            if (CheckEmail)    // if already exists
            {
                return Json("Email Address Already Regesiterd! Try using another one ", JsonRequestBehavior.AllowGet);
            }
            else
            {   // if not
                return Json(true, JsonRequestBehavior.AllowGet);
            }

        }

        //



        // need to understand
        public void confirmationEmailSend(string email, string verificationToken)
        {

            string verifyUrl = System.Web.HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority) + "/FundRaisers/verify?ID="
                + verificationToken;
            //try
            {
                MailMessage mail = new MailMessage();
                mail.To.Add(email);
                mail.From = new MailAddress("Convergent.origin@gmail.com");
                mail.Subject = "Welcome to Convergent!";
                mail.Body = "Hello there! Thank you for your interest in convergent. Please click on the link below to verify your account.  " + verifyUrl;
                mail.IsBodyHtml = true;
                SmtpClient smtp = new SmtpClient();
                smtp.Host = "smtp.gmail.com";
                smtp.Port = 587;
                smtp.UseDefaultCredentials = false;
                smtp.Credentials = new System.Net.NetworkCredential
                ("Convergent.origin@gmail.com", "fall2015cmps285");// Enter senders User name and password
                smtp.EnableSsl = true;
                Console.WriteLine("Sending email .... ");
                smtp.Send(mail);

            }
        }

        [AllowAnonymous]
        public ActionResult Verify(string ID)
        {
            if (string.IsNullOrEmpty(ID) || (!System.Text.RegularExpressions.Regex.IsMatch(ID,
                           @"[0-9a-f]{8}\-([0-9a-f]{4}\-){3}[0-9a-f]{12}")))
            {
                TempData["tempMessage"] =
                        "The user account is not valid. Please try clicking the link in your email again.";
                return RedirectToAction("Login");
            }

            else
            {
                var fundRaiser = db.FundRaisers.FirstOrDefault(m => m.verificationToken == new Guid(ID).ToString());
                if (fundRaiser != null)
                {
                    if (!fundRaiser
                        .verified)
                    {
                        fundRaiser.verified = true;
                        db.Entry(fundRaiser).State = EntityState.Modified;
                        db.SaveChanges();
                        return RedirectToAction("SignIn", "FundRaisers");
                    }
                    else
                    {
                        return RedirectToAction("SignIn");
                    }
                }
                return View();
            }
        }
        

        // GET: FundRaisers/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            FundRaisers fundRaiser = db.FundRaisers.Find(id);
            if (fundRaiser == null)
            {
                return HttpNotFound();
            }
            return View(fundRaiser);
        }

        // POST: FundRaisers/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "FundRaiserId,FirstName,LastName,Email,Password,ConfirmPassword")] FundRaisers fundRaiser)
        {
            if (ModelState.IsValid)
            {
                db.Entry(fundRaiser).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(fundRaiser);
        }

        // GET: FundRaisers/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            FundRaisers fundRaiser = db.FundRaisers.Find(id);
            if (fundRaiser == null)
            {
                return HttpNotFound();
            }
            return View(fundRaiser);
        }

        // POST: FundRaisers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            FundRaisers fundRaiser = db.FundRaisers.Find(id);
            db.FundRaisers.Remove(fundRaiser);
            db.SaveChanges();
            return RedirectToAction("Index");
        }


       
        public ActionResult ChangePassword(string emailAddress)
        {
            var user = db.FundRaisers.FirstOrDefault(u => u.Email == emailAddress);
            return View(user);
        }

        [HttpPost]
        public ActionResult ChangePassword(Models.RegisterViewModel updatedUser)
        {
            // Get the current user's emailAddress to find the current user in the database.
            string emailAddress = User.Identity.GetUserName();
            FundRaisers currentUser = db.FundRaisers.FirstOrDefault(u => u.Email == emailAddress);

            // Only change the user's password in the database if the password field is not left blank.
            if (updatedUser.Password != "")
            {
                string newPassword = Crypto.HashPassword(updatedUser.Password);
                currentUser.Password = newPassword;
                currentUser.ConfirmPassword = newPassword;
            }
            db.Entry(currentUser).State = EntityState.Modified;
            db.SaveChanges();

            return RedirectToAction("Index", "Home");

        }

        public ActionResult UserSettings(string emailAddress)
        {
            var user = db.FundRaisers.FirstOrDefault(u => u.Email == emailAddress);

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UserSettings(Models.RegisterViewModel updatedUser)
        {
            // Get the current user's emailAddress to find the current user in the database.
            string emailAddress = User.Identity.GetUserName();
            var currentUser = db.FundRaisers.FirstOrDefault(u => u.Email == emailAddress);

            // Update the currentUser's respective fields.
            currentUser.FirstName = updatedUser.FirstName;
            currentUser.LastName = updatedUser.LastName;

            db.Entry(currentUser).State = EntityState.Modified;
            db.SaveChanges();

            // Go back to /Users/ProfilePage
            return RedirectToAction("Index", "Home");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }


        public ActionResult Logout()
        {
            // This is the predefined SignOut method in FormAuthentication :p
            FormsAuthentication.SignOut();

            return RedirectToAction("Index", "Home");

        }
        //FACEBOOK LOGIN
        //FACEBOOK LOGIN
        private Uri RedirectUri
        {
            get
            {
                var uriBuilder = new UriBuilder(Request.Url);
                uriBuilder.Query = null;
                uriBuilder.Fragment = null;
                uriBuilder.Path = Url.Action("FacebookCallback");
                return uriBuilder.Uri;
            }
        }

     

        public ActionResult Facebook()
        {
            var fb = new FacebookClient();
            var loginUrl = fb.GetLoginUrl(new
            {
                client_id = "618375218302469",
                client_secret = "7af3f4e7d7bd2089a1e9f71dfc659584",
                redirect_uri = RedirectUri.AbsoluteUri,
                response_type = "code",
                scope = "email" // Add other permissions as needed
            });

            return Redirect(loginUrl.AbsoluteUri);
        }

        public ActionResult FacebookCallback(string code)
        {
            var fb = new FacebookClient();
            dynamic result = fb.Post("oauth/access_token", new
            {
                client_id = "618375218302469",
                client_secret = "7af3f4e7d7bd2089a1e9f71dfc659584",
                redirect_uri = RedirectUri.AbsoluteUri,
                code = code
            });

            var accessToken = result.access_token;

            // Store the access token in the session
            Session["AccessToken"] = accessToken;

            // update the facebook client with the access token so 
            // we can make requests on behalf of the user
            fb.AccessToken = accessToken;

            // Get the user's information
            dynamic me = fb.Get("me?fields=first_name,last_name,id,email");
            string email = me.email;

            // Set the auth cookie
            FormsAuthentication.SetAuthCookie(email, false);

            return RedirectToAction("Index", "Home");
        }
        //LOGIN WITH TWITTER
        //LOGIN WITH TWITTER
        //public ActionResult Twitter()
        //{
        //    // Step 1 - Retrieve an OAuth Request Token
        //    TwitterService service = new TwitterService("VDmlMeOXpRbdDK5iXyCcHAGY3", "S7ngKhtW8uskHGfIgNBg9qKdY6fRzgWHwYZ0Lvct4ICezU1lhx");

        //    var url = Url.Action("TwitterCallback", "FundRaisers", null, "http");
        //    // This is the registered callback URL
        //    OAuthRequestToken requestToken = service.GetRequestToken(url);

        //    // Step 2 - Redirect to the OAuth Authorization URL
        //    Uri uri = service.GetAuthorizationUri(requestToken);
        //    return new RedirectResult(uri.ToString(), false /*permanent*/);
        //}
        //// This URL is registered as the application's callback at http://dev.twitter.com
        //public ActionResult TwitterCallback(string oauth_token, string oauth_verifier)
        //{
        //    var requestToken = new OAuthRequestToken { Token = oauth_token };

        //    // Step 3 - Exchange the Request Token for an Access Token
        //    TwitterService service = new TwitterService("VDmlMeOXpRbdDK5iXyCcHAGY3", "S7ngKhtW8uskHGfIgNBg9qKdY6fRzgWHwYZ0Lvct4ICezU1lhx");
        //    OAuthAccessToken accessToken = service.GetAccessToken(requestToken, oauth_verifier);

        //    // Step 4 - User authenticates using the Access Token
        //    service.AuthenticateWith(accessToken.Token, accessToken.TokenSecret);
        //    TwitterUser user = service.VerifyCredentials(new VerifyCredentialsOptions());

        //    FormsAuthentication.SetAuthCookie(user.ScreenName, false);

        //    return RedirectToAction("Index", "Home");
        //}


        public ActionResult ChangePicture()
        {
            return View();
        }
        [HttpPost]
        public ActionResult ChangePicture(HttpPostedFileBase file, string id)
        {
            string emailAddress = User.Identity.GetUserName();
            var user = db.FundRaisers.FirstOrDefault(u => u.Email == emailAddress);
            
            if (file != null && file.ContentLength > 0 && user != null)
            {
                var fileName = Path.GetFileName(file.FileName);
                fileName = fileName.Replace("(", "me");
                fileName = fileName.Replace(")", "me");
                fileName = fileName.Replace(" ", "");

                var path = Path.Combine(Server.MapPath("~/Images"), fileName);
                file.SaveAs(path);
                user.Image = "/Images/" + fileName;
                db.Entry(user).State = EntityState.Modified;
                db.SaveChanges();
               // return View((object)path);
            }
            return RedirectToAction("Index","Home");

          //  return RedirectToAction(");
        }
        

    public ActionResult ForgotPassword() {

            return View();

        }

        // POST: /Account/ForgotPassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        // public ActionResult ForgotPassword(Models.ForgotPasswordViewModel model)
        public ActionResult ForgotPassword(Models.ResetPasswordViewModel model)
        {
            string emailAddress = model.Email;
            
            var user = db.FundRaisers.FirstOrDefault(s => s.Email.Equals(emailAddress));

            string callbackUrl = System.Web.HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority) + "/FundRaisers/verify1?ID="
           + user.verificationToken;
            MailMessage mail = new MailMessage();
            mail.To.Add(model.Email);
            mail.From = new MailAddress("Convergent.origin@gmail.com");
            mail.Subject = "Welcome to Convergent!";
            mail.Body = "Hello there! Thank you for your interest in convergent. Please click on the link below to verify your account <a href =\"" + callbackUrl + "\">here </a>";
            mail.IsBodyHtml = true;
            SmtpClient smtp = new SmtpClient();
            smtp.Host = "smtp.gmail.com";
            smtp.Port = 587;
            smtp.UseDefaultCredentials = false;
            smtp.Credentials = new System.Net.NetworkCredential
            ("Convergent.origin@gmail.com", "fall2015cmps285");// Enter senders User name and password
            smtp.EnableSsl = true;
            Console.WriteLine("Sending email .... ");
            smtp.Send(mail);



            return RedirectToAction("Index", "Home");
        }


        [AllowAnonymous]
        public ActionResult Verify1(string ID)
        {
            if (string.IsNullOrEmpty(ID) || (!System.Text.RegularExpressions.Regex.IsMatch(ID,
                           @"[0-9a-f]{8}\-([0-9a-f]{4}\-){3}[0-9a-f]{12}")))
            {
                TempData["tempMessage"] =
                        "The user account is not valid. Please try clicking the link in your email again.";
                return RedirectToAction("Login");
            }

            else
            {
                var fundRaiser = db.FundRaisers.FirstOrDefault(m => m.verificationToken == new Guid(ID).ToString());

                string n = new Guid(ID).ToString();
                if (fundRaiser != null)
                {
                    return RedirectToAction("ResetPassword", "FundRaisers");
                    // return View("ResetPassword","FundRaisers");
                }

                else
                {
                    return RedirectToAction("Index", "Home");
                }

            }
        }

        public ActionResult ResetPassword()
        {
            return View();
        }

        [HttpPost]
        public ActionResult ResetPassword(Models.ResetPasswordViewModel model)
        {
            var user = db.FundRaisers.FirstOrDefault(m => m.Email.Equals(model.Email));

            if (user.Password != "")
            {
                string newPassword = Crypto.HashPassword(model.Password);
                user.Password = newPassword;
                user.ConfirmPassword = newPassword;
            
            db.Entry(user).State = EntityState.Modified;
            db.SaveChanges();}

            return RedirectToAction("Index", "Home");
        }
        }
    }


        
    
