﻿using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Template.Common.Notifications;
using Template.Models.ServiceModels;
using Template.Services.Contracts;

namespace Template.MVC.Controllers
{
    public class BaseController : Controller
    {
        #region Instance Fields

        #endregion

        #region Constructors


        #endregion

        #region Public Methods

        public IActionResult RedirectToHome()
        {
            return RedirectToHome(string.Empty);
        }

        public IActionResult RedirectToLogin()
        {
            return RedirectToAction(nameof(AccountController.Login), "Account");
        }

        public IActionResult RedirectToHome(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction(nameof(HomeController.Index), "Home");
        }

        /// <summary>
        ///  adds notifications to the form validation summary
        /// </summary>
        /// <param name="response"></param>
        public void AddFormErrors(IServiceResponse response)
        {
            foreach (var notification in response.Notifications.Where(n => n.Type == NotificationTypeEnum.Error))
            {
                ModelState.AddModelError(string.Empty, notification.Message);
            }
        }

        /// <summary>
        /// adds notifications to the next page redirect
        /// </summary>
        /// <param name="response"></param>
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

        /// <summary>
        /// adds notifications to the page due to a isolated form post (multiple forms present)
        /// </summary>
        /// <param name="model"></param>
        public void AddNotifications(IValidatableObject model)
        {
            var notifications = new List<Notification>();
            var rawExistingNotifications = TempData["Notifications"] as string;

            if (!string.IsNullOrEmpty(rawExistingNotifications))
            {
                var existingNotifications = JsonConvert.DeserializeObject<List<Notification>>(rawExistingNotifications);
                notifications.AddRange(existingNotifications);
            }

            var validationResults = new List<ValidationResult>();
            Validator.TryValidateObject(model, new ValidationContext(model, null, null), validationResults, false);

            foreach (var validationResult in validationResults)
            {
                notifications.Add(new Notification() {
                    Message = validationResult.ErrorMessage,
                    Type = NotificationTypeEnum.Error
                });
            }
            TempData["Notifications"] = JsonConvert.SerializeObject(notifications);
        }

        #endregion
    }
}
