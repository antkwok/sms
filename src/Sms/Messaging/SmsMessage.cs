﻿using System;
using System.Collections.Generic;

namespace Sms.Messaging
{
    public class SmsMessage
    {
        private IDictionary<string, string> headers;
 
        public SmsMessage(string toAddress, string body, IDictionary<string, string> headers = null)
        {
            if (toAddress == null) throw new ArgumentNullException("toAddress");
            Id = Guid.NewGuid().ToString();
            ToAddress = toAddress;
            Body = body;
            this.headers = headers ?? new Dictionary<string, string>();
        }

        public string Id { get; private set; }

        public IDictionary<string,string> Headers { get { return headers;} }

        public string ToAddress { get; set; }
        public string Body { get; private set; }

    }
}