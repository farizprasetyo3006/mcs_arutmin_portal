using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BusinessLogic.Entity;
using MCSWebApp.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using NLog;

namespace MCSWebApp.Areas.Material.Controllers
{
    [Area("Material")]
    public class ProductSpecificationController : BaseController
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        public ProductSpecificationController(IConfiguration Configuration)
            : base(Configuration)
        {
        }

        public IActionResult Index()
        {
            ViewBag.WebAppName = WebAppName;
            ViewBag.AreaBreadcrumb = "Material";
            ViewBag.Breadcrumb = "Product Specification";

            return View();
        }

        public async Task<IActionResult> Detail(string Id)
        {
            ViewBag.WebAppName = WebAppName;
            ViewBag.AreaBreadcrumb = "Material";
            ViewBag.Breadcrumb = "Product Specification";

            try
            {
                if (!string.IsNullOrEmpty(Id))
                {
                    var svc = new BusinessLogic.Entity.ProductSpecification(CurrentUserContext);
                    var record = await svc.GetByIdAsync(Id);
                    if (record != null)
                    {
                        ViewBag.Id = Id;
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex.ToString());
            }

            return View();
        }
    }
}
