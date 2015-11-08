using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using FuNDs.Models;
using System.Net.Mail;
using System.Web.Helpers;

namespace FuNDs.Controllers
{
    public class PasswordResetsController : Controller
    {
        private FundRaisersDbContext db = new FundRaisersDbContext();

        // GET: PasswordResets
        public ActionResult Index()
        {
            return View(db.PasswordResets.ToList());
        }

        // GET: PasswordResets/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PasswordReset passwordReset = db.PasswordResets.Find(id);
            if (passwordReset == null)
            {
                return HttpNotFound();
            }
            return View(passwordReset);
        }

        // GET: PasswordResets/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: PasswordResets/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "PasswordResetID,Email,Password,ConfirmPassword,Code")] PasswordReset passwordReset)
        {
            if (ModelState.IsValid)
            {
                db.PasswordResets.Add(passwordReset);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(passwordReset);
        }

        // GET: PasswordResets/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PasswordReset passwordReset = db.PasswordResets.Find(id);
            if (passwordReset == null)
            {
                return HttpNotFound();
            }
            return View(passwordReset);
        }

        // POST: PasswordResets/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "PasswordResetID,Email,Password,ConfirmPassword,Code")] PasswordReset passwordReset)
        {
            if (ModelState.IsValid)
            {
                db.Entry(passwordReset).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(passwordReset);
        }

        // GET: PasswordResets/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PasswordReset passwordReset = db.PasswordResets.Find(id);
            if (passwordReset == null)
            {
                return HttpNotFound();
            }
            return View(passwordReset);
        }

        // POST: PasswordResets/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            PasswordReset passwordReset = db.PasswordResets.Find(id);
            db.PasswordResets.Remove(passwordReset);
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



        public ActionResult ForgotPassword()
        {

            return View();

        }

        // POST: /Account/ForgotPassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ForgotPassword(PasswordReset model)


        //   public ActionResult ForgotPassword(Models.ForgotPasswordViewModel model)
        {
            // if (ModelState.IsValid)
            // {
            // Session["products"] = products;
            // Session["rp"] = false;
           // PasswordReset model = new PasswordReset();
             
            string emailAddress = model.Email;
            // Session["email"] = model.Email;
            db.PasswordResets.Add(model);
            
           // return RedirectToAction("Index");

            var user = db.FundRaisers.FirstOrDefault(s => s.Email.Equals(emailAddress));

            //  var user = await UserManager.FindByNameAsync(model.Email);
            //  if (user == null || !(await UserManager.IsEmailConfirmedAsync(user.Id)))
            if (user == null || (!user.verified))
            {
                // Don't reveal that the user does not exist or is not confirmed
                return View("ResetPassword", "FundRaisers");
            }
            string code = Guid.NewGuid().ToString();
            //  string code = await db.GeneratePasswordResetTokenAsync(user.Id);
            var callbackUrl = Url.Action("ResetPassword", "PasswordResets", new { userId = user.FundRaisersId, code = code }, protocol: Request.Url.Scheme);
            
            //await UserManager.SendEmailAsync(user.Id, "Reset Password", "Please reset your password by clicking <a href=\"" + callbackUrl + "\">here</a>");
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

         //   db.SaveChanges();

            return RedirectToAction("Index", "Home");
        }

        // If we got this far, something failed, redisplay form
        //return View(model);



        public ActionResult ResetPassword()
        {
            return View();
        }

        [HttpPost]
        public ActionResult ResetPassword(PasswordReset model)
        {
            // Get the current user's emailAddress to find the current user in the database.
            //  string emailAddress = User.Identity.GetUserName();
            // string x = Session["forgotPasswordemailAddress"];
             // FundRaisers currentUser = db.FundRaisers.FirstOrDefault(u => u.Email.Equals(model.Email));

          ////  Only change the user's password in the database if the password field is not left blank.
          //  if (model.Password != "")
          //  {
          //      string newPassword = Crypto.HashPassword(currentUser.Password);
          //      currentUser.Password = newPassword;
          //      currentUser.ConfirmPassword = newPassword;
          //  }
          //  db.Entry(currentUser).State = EntityState.Modified;
          //  db.SaveChanges();


            return RedirectToAction("Create", "PasswordResets");

        }
    }
}
    

