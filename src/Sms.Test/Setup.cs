﻿using DotNetConfigHelper;
using NUnit.Framework;
using Sms.Messaging;
using Sms.Msmq;
using Sms.Services;

namespace Sms.Test
{
	[SetUpFixture]
    public class Setup
    {
		[SetUp]
		public void register()
		{
			AppSettingsReplacer.Install(DotNetConfigHelper.ConfigProvider.CreateAndSetDefault());
			Defaults.SerializerFactory.Register(new JsonSerializer());
			Defaults.MessagingFactories.Add(new MsmqFactory());
		}
    }
}
