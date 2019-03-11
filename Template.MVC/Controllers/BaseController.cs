using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using Template.Common.Notifications;
using Template.Models.ServiceModels;

namespace Template.MVC.Controllers
{
    public class BaseController : Controller
    {
        public IActionResult RedirectToHome(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction(nameof(HomeController.Index), "Home");
            }
        }

        public void AddFormErrors(IServiceResponse response)
        {
            foreach (var notification in response.Notifications.Where(n => n.Type == NotificationTypeEnum.Error))
            {
                ModelState.AddModelError(string.Empty, notification.Message);
            }
        }

        public void AddNotifications(IServiceResponse response)
        {
            var notifications = new List<Notification>();
            var rawExistingNotifications = TempData["Notifications"] as string;

            if (!string.IsNullOrEmpty(rawExistingNotifications))
            {
                var existingNotifications = JsonConvert.DeserializeObject<List<Notification>>(rawExistingNotifications);
                notifications.AddRange(existingNotifications);
            }

            notifications.AddRange(response.Notifications);
            TempData["Notifications"] = JsonConvert.SerializeObject(notifications);
        }
    }
}
