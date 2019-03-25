﻿using System;
using System.Collections.Generic;
using System.Linq;
using Template.Models.DomainModels;

namespace Template.Infrastructure.Configuration.Models
{
    public class ApplicationConfiguration
    {
        #region Instance Fields

        public readonly List<ConfigurationEntity> Items;

        #endregion

        #region Properties

        public bool Session_Logging_Is_Enabled
        {
            get
            {
                var item = GetItem(ConfigurationKeys.Session_Logging_Is_Enabled);
                return item.Boolean_Value.Value;
            }
        }

        public bool Home_Promo_Banner_Is_Enabled
        {
            get
            {
                var item = GetItem(ConfigurationKeys.Home_Promo_Banner_Is_Enabled);
                return item.Boolean_Value.Value;
            }
        }

        #endregion

        #region Constructors

        public ApplicationConfiguration(List<ConfigurationEntity> items)
        {
            Items = items;
        }

        #endregion

        #region Private Methods

        private ConfigurationEntity GetItem(string key)
        {
            var configItem = Items.FirstOrDefault(c => c.Key == key);
            if (configItem == null)
            {
                throw new Exception($"Configuration item with key '{key}' could not be found");
            }
            return configItem;
        }

        #endregion
    }
}