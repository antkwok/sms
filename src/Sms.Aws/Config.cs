﻿using System;
using System.Configuration;

namespace Sms.Aws
{
    internal static class Config
    {
        public static AppSettingItem Require(string key)
        {
            var value = ConfigurationManager.AppSettings[key];
            if (value == null) throw new ArgumentNullException("AppSetting:  \"" + key + "\" is missing!");
            return new AppSettingItem(key, value);
        }

        public static AppSettingItem Setting(string key, string defaultValue)
        {
            var value = ConfigurationManager.AppSettings[key] ?? defaultValue;
            return new AppSettingItem(key, value);
        }

		public const string ContentType  = "body-content-type";
		public const string ContentTypeBase64EncodedSmsMessageContent = "Base64EncodedSmsMessageContent";
        public const string ToAttributename ="to";
        public const string HeaderKeysAttributename ="headerskey";
        public const string HeaderValuesAttributename ="headervalues";

        public class AppSettingItem
        {
            private readonly string key;
            private string value;

            public AppSettingItem(string key, string value)
            {
                if (key == null) throw new ArgumentNullException("key");
                this.key = key;
                this.value = value;
            }

            public string Value
            {
                get { return value; }
            }
        }
    }
}