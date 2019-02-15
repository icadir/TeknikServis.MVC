using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AutoMapper;
using TeknikServis.BLL.Repository;
using TeknikServis.Entity.ViewModels.ArizaViewModels;

namespace TeknikServis.Web.UI.Controllers
{
    public class OperatorController : Controller
    {
        // GET: Operator
        public ActionResult Index()
        {
            // TODO mapperları yapalım.
            var data = new ArizaKayitRepo().GetAll(x => x.OperatorKabul == false).Select(x=>Mapper.Map<ArizaViewModel>(x)).ToList();
         
            return View(data);
        }
    }
}