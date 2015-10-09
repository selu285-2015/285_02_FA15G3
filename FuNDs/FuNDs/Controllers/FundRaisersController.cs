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
        public ActionResult Create([Bind(Include = "FundRaisersId,FirstName,LastName,Email,Password,ConfirmPassword")] FundRaisers FundRaisers)
        {

            if (ModelState.IsValid)
            {

                //  CheckEmailAddressAlreadyExists(FundRaisers.Email);
                //var salt = Crypto.GenerateSalt();
                //var saltedPassword = FundRaisers.Password + salt;
                var hashedPassword = Crypto.HashPassword(FundRaisers.Password);
                FundRaisers.Password = hashedPassword;
                FundRaisers.ConfirmPassword = hashedPassword;

                db.FundRaisers.Add(FundRaisers);
                db.SaveChanges();
                return RedirectToAction("Index", "Home");
            }
            return View(FundRaisers);
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
        public ActionResult SignIn([Bind(Include = "Email1,Password")] AccountLoginViewModel userTryingToLogin)
        {
            try
            {
                FundRaisers doesUserExist = db.FundRaisers.FirstOrDefault(s => s.Email.Equals(userTryingToLogin.Email1));

                // var dbPassword = db.FundRaisers.Where(fundOb => fundOb.Email == fundRaisers.Email1).FirstOrDefault().Password;
                bool a = Crypto.VerifyHashedPassword(doesUserExist.Password, userTryingToLogin.Password);
                if (a == true)
                {

                    // creating authetication ticket
                    FormsAuthentication.SetAuthCookie(userTryingToLogin.Email1, false);
                    return RedirectToAction("Index", "Home");
                }
                else
                {

                    @ViewBag.Message = "Error.Ivalid login.";
                    return RedirectToAction("SignInFailure", "FundRaisers");
                }
            }
            catch (Exception e)
            {
                //@ViewBag.Message = "Error.Ivalid login.";
                return RedirectToAction("SignInFailure", "FundRaisers");

            }
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

