﻿using System;
using Sms.Messaging;

namespace Sms.Msmq
{
    public class MsmqFactory : IMessagingFactory
    {
        public const string ProviderName = "msmq";

        public string Name { get { return ProviderName; } }

        public IMessageSender<SmsMessage> Sender(string queueName)
        {
            return new MsmqMessageSender(queueName);
        }

        public IReciever<SmsMessage> Reciever(string queueName)
        {
            return new MsmqMessageReceiver(queueName);
        }
    }
}