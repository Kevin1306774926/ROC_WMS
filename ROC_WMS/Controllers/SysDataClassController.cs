using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ROC.Models;
using ROC.Data;
using System.Net;

namespace ROC_WMS.Controllers
{
    public class SysDataClassController : BaseController
    {
        SysDataClassService service = new SysDataClassService();
        // GET: Sys_DataClass
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult List(string id)
        {
            string typeName = id;
            var list = service.GetClassList(typeName);
            if (list.Count == 0)
            {
                service.AddClass("Menu", "菜单分类", "0000");
                list = service.GetClassList(typeName);
            }
            return Json(list, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Create(string id)
        {
            var model = new SysDataClass();
            model.ParentCode = id;
            return View(model);
        }

        [HttpPost]
        public ActionResult Create(SysDataClass model)
        {
            service.AddClass("Menu", model.Name, model.ParentCode);
            return RedirectToAction("Close");
        }

        public ActionResult Edit(string id)
        {
            if (Guid.TryParse(id, out Guid dataClassId))
            {
                var model = service.Get(dataClassId);
                if (model != null)
                {
                    return View(model);
                }
            }
            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        }

        [HttpPost]
        public ActionResult Delete(string id)
        {
            if (Guid.TryParse(id, out Guid dataClassId))
            {
                service.Delete(dataClassId);
                this.Success = true;
                this.Message = "删除成功";
                return EasyUIResult();
            }
            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        }
    }
}