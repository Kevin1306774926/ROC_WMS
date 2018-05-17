using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ROC.Data;
using ROC.Models;
using ROC.Comm;
namespace ROC_WMS.Controllers
{
    public class UserController : BaseController
    {
        // GET: Users
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult List()
        {
            this.Rows = db.Users.AsNoTracking().OrderBy(t => t.Id).SelectPage(PageIndex, PageSize, out PageTotal).ToList();
            return DataGridResult();
        }
        // GET: Users/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: Users/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Users/Create
        [HttpPost]
        public ActionResult Create(User mode)
        {
            try
            {
                // TODO: Add insert logic here                
                db.Users.Add(mode);
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                this.Success = false;
                this.Message = ex.Message;
            }
            return EasyUIResult();
        }

        // GET: Users/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Users/Edit/5
        [HttpPost]
        public ActionResult Edit(User model)
        {
            try
            {
                // TODO: Add update logic here
                if (model != null)
                {
                    //var m = db.Users.Find(model.Id);                    
                    //m.Name = model.Name;                    

                    db.Users.Attach(model);
                    db.Entry(model).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                }                
            }
            catch(Exception ex)
            {
                this.Success = false;
                this.Message = ex.Message;
            }
            return EasyUIResult();
        }

        // GET: Users/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Users/Delete/5
        [HttpPost]
        public ActionResult Delete(User model)
        {
            try
            {
                // TODO: Add delete logic here

                if(model!=null)
                {
                    db.Users.Attach(model);
                    db.Users.Remove(model);
                    db.SaveChanges();
                }
            }
            catch(Exception ex)
            {
                this.Success = false;
                this.Message = ex.Message;
            }
            return EasyUIResult();
        }
    }
}
