using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Sabio.Web.Controllers
{
    [Authorize]
    [RoutePrefix("admin/schedule")]
    public class AdminJobScheduleController : BaseController
    {
       [Route()]
       [Route("index")]
        public ActionResult Index()
        {
            return View("IndexNg");
        }
    }
}