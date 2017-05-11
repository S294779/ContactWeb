using System;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;

namespace ContactWeb.Controllers
{
    public class ContactsController : Controller
    {
        
        private ContactInfoEntities2 db = new ContactInfoEntities2();
        [Authorize]
        // GET: Contacts
        public ActionResult Index()
        {// checking user is authorized or not 
            if (CheckUserAuthorize() == true)    
            {
                // checking user is admin or not
                if (CheckIfAdmin() == true)    
                {
                    var contacts = db.Contacts.Include(t => t.AspNetUser);
                    return View(contacts);
                }
                else
                {
                    var currentuser = User.Identity.GetUserId();
                    var contact = db.Contacts.Where(t => t.UserId == currentuser);
                    return View(contact.ToList());
                }
            }
            else
            {
                return RedirectToAction("Login", "Account");
            }
        }
        // GET: Contacts/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Contact contact = db.Contacts.Find(id);
            if (contact == null)
            {
                return HttpNotFound();
            }
            return View(contact);
        }

        // GET: Contacts/Create
        [Authorize]
        public ActionResult Create()
        {
            ViewBag.UserId = new SelectList(db.AspNetUsers, "Id", "Email");
            return View();
        }

        // POST: Contacts/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,UserId,FirstName,LastName,Email,PhonePrimary,PhoneSecondary,Birthday,StreetAddress1,StreetAddress2,City,State,Zip")] Contact contact)
        {
            contact.UserId = User.Identity.GetUserId();
            if (ModelState.IsValid)
            {
                db.Contacts.Add(contact);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.UserId = new SelectList(db.AspNetUsers, "Id", "Email", contact.UserId);
            return View(contact);
        }

        // GET: Contacts/Edit/5
        [Authorize]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Contact contact = db.Contacts.Find(id);
            if (contact == null || !EnsureIsUserContact(contact))
            {
                return HttpNotFound();
            }
            ViewBag.UserId = new SelectList(db.AspNetUsers, "Id", "Email", contact.UserId);
            return View(contact);
        }

        // POST: Contacts/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public ActionResult Edit([Bind(Include = "Id,UserId,FirstName,LastName,Email,PhonePrimary,PhoneSecondary,Birthday,StreetAddress1,StreetAddress2,City,State,Zip")] Contact contact)
        {
            contact.UserId = User.Identity.GetUserId();
            if (ModelState.IsValid)
            {
                db.Entry(contact).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.UserId = new SelectList(db.AspNetUsers, "Id", "Email", contact.UserId);
            return View(contact);
        }

        // GET: Contacts/Delete/5
        [Authorize]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Contact contact = db.Contacts.Find(id);
            if (contact == null)
            {
                return HttpNotFound();
            }
            return View(contact);
        }

        // POST: Contacts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Contact contact = db.Contacts.Find(id);
            db.Contacts.Remove(contact);
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

        public Boolean CheckUserAuthorize()
        {
            var strUserID = User.Identity.GetUserId();
            var strUserName = User.Identity.GetUserName();
            if (strUserID != null && strUserName != "")
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public Boolean CheckIfAdmin()
        {
            if (User.Identity.GetUserName() == "admin@gmail.com")
            {
                return true;
            }
            else
            {
                return false;
            }

        }
        private bool EnsureIsUserContact(Contact contact)
        {
            return contact.UserId == User.Identity.GetUserId();
        }
    }
}
