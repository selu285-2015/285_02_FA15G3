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



namespace FuNDs.Controllers
{
    public class FundRaisersController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

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
        public ActionResult Create([Bind(Include = "FundRaisersId,FirstName,LastName,Email,Password,ConfirmPassword,verificationToken,verified")] FundRaisers FundRaisers)
        {

            if (ModelState.IsValid)
            {

                var hashedPassword = Crypto.HashPassword(FundRaisers.Password);
                FundRaisers.Password = hashedPassword;
                FundRaisers.ConfirmPassword = hashedPassword;
                db.FundRaisers.Add(FundRaisers);
                FundRaisers.verified = false;
                FundRaisers.verificationToken = Guid.NewGuid().ToString();

                db.SaveChanges();
                //  
              this.confirmationEmailSend(FundRaisers.Email, FundRaisers.verificationToken);
                return RedirectToAction("checkEmail");

                //   HttpContext.Session.Add("fundRaiser", FundRaisers);


            }
            else {


                return View("FundRaisers");
            }
        }

        public ActionResult CheckEmail() {


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

                if (!doesUserExist.verified )
                {
                    ModelState.AddModelError("doesUserExist", "Email Not verified. Please check your email confirmation");
                    return View("SignInFailure");
                }

                // var dbPassword = db.FundRaisers.Where(fundOb => fundOb.Email == fundRaisers.Email1).FirstOrDefault().Password;
                bool a = Crypto.VerifyHashedPassword(doesUserExist.Password, userTryingToLogin.Password);
                if (a == true)
                {

                    // creating authetication ticket
                    FormsAuthentication.SetAuthCookie(userTryingToLogin.Email, false);
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



        //    public void confirmationEmailSend(String emailAddress, String verificationToken)
        //    {
        //        string verifyUrl = System.Web.HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority) + "/FundRaisers/verify?id="
        //            + verificationToken;
        //        //  smtp.UseDefaultCredentials = True

        //        var message = new System.Net.Mail.MailMessage("Convergent.origin@gmail.com", emailAddress)
        //        {

        //            Subject = " Welcome to fundraising project!!! Please verify your Crux account: ",
        //            Body = "Team => The Usual Suspects Welcomes you!!! To complete the registration, Please click the following link for verification purpose " + verifyUrl,

        //        };

        //        var client = new SmtpClient();
        //        client.EnableSsl = true;
        //        client.UseDefaultCredentials = false;

        //        client.Send(message);

        //    }
        //}

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
        //    catch (Exception e)
        //    {
        //        Console.WriteLine("Email sending failed: " + e.Message);
        //    }

        //    Console.WriteLine("Email has been sent to " + email);
        //    Console.ReadKey();
        //}
        //


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
    }
}

