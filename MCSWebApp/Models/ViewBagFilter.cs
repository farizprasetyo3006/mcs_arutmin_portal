using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace MCSWebApp.Models
{
    public class ViewBagFilter : IActionFilter
    {
        private const string Enabled = "Enabled";
        private static readonly string Disabled = string.Empty;

        public void OnActionExecuting(ActionExecutingContext context)
        {
            if (context.Controller is Controller controller)
            {
                // API Toggle Features
                controller.ViewBag.AppSidebar = Enabled;
                controller.ViewBag.AppHeader = Enabled;
                controller.ViewBag.AppLayoutShortcut = Enabled;
                controller.ViewBag.AppFooter = Enabled;
                controller.ViewBag.ShortcutMenu = Enabled;
                controller.ViewBag.ChatInterface = Enabled;
                controller.ViewBag.LayoutSettings = Enabled;

                // API Default Settings
                controller.ViewBag.App = "Igreja";
                controller.ViewBag.AppName = "Igreja";
                controller.ViewBag.AppFlavor = "ASP.NET Core 3.1";
                controller.ViewBag.AppFlavorSubscript = "Full";
                controller.ViewBag.User = "System Administrator";
                controller.ViewBag.Email = "masdono@gmail.com";
                controller.ViewBag.Twitter = "";
                controller.ViewBag.Avatar = "avatar-lg.jpg";
                controller.ViewBag.Version = "4.0.2";
                controller.ViewBag.Bs4v = "4.3";
                controller.ViewBag.Logo = "logo.png";
                controller.ViewBag.LogoM = "logo.png";
                controller.ViewBag.Copyright = "2020 © Igreja";
                controller.ViewBag.CopyrightInverse = "2020 © Igreja";
            }
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
        }
    }
}