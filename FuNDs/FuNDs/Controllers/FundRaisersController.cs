using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using FuNDs.Models;

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
            FundRaisers fundRaiser = db.FundRaisers.Find(id);
            if (fundRaiser == null)
            {
                return HttpNotFound();
            }
            return View(fundRaiser);
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
        public ActionResult Create([Bind(Include = "FundRaiserId,FirstName,LastName,Email,Password,ConfirmPassword")] FundRaisers FundRaisers)
        {
            if (ModelState.IsValid)
            {
                db.FundRaisers.Add(FundRaisers);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(FundRaisers);
        }

        // Sign In 
        public ActionResult SignIn()
        {
            return View();
        }

        // POST: FundRaisers/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "FundRaiserId,FirstName,LastName,Email,Password,ConfirmPassword")] FundRaisers FundRaisers)
        {
            if (ModelState.IsValid)
            {
                db.FundRaisers.Add(FundRaisers);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(FundRaisers);
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
    }
}
