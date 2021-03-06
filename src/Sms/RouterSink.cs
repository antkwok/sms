﻿using System;
using System.Collections.Generic;
using System.Linq;
using Sms.Messaging;
using Sms.Router;
using Sms.Routing;
using Sms.Services;

namespace Sms
{
	public class RouterSink : IDisposable, IMessageSink
	{
		private readonly IMessageSink router;
		private readonly IServiceDefinitionRegistry registry;
		private readonly ISerializerFactory serializerFactory;

		public RouterSink(IMessageSink router = null, IServiceDefinitionRegistry registry = null, ISerializerFactory serializerFactory = null)
		{
			this.router = router ?? SmsFactory.Sender(RouterSettings.ProviderName, RouterSettings.SendQueueName);
			this.registry = registry ?? new ServiceDefinitionRegistry();
			this.serializerFactory = serializerFactory ?? SerializerFactory.CreateEmpty();
		}

		public void Send<T>(T request) where T : class, new()
		{
			this.Send(CreateMessage(request));
		}

		public void ConfigureEndpoint<T>(string providerName, string queueName, string version) where T : class, new()
		{
			ConfigureEndpoint(typeof(T), providerName, queueName, version);
		}

		public void ConfigureEndpoint(Type type, string providerName, string queueName, string version)
		{
			var config = registry.Get(type);
			Send(new SmsMessage(RouterService.ConfigureServiceEndpointAddress, null,
				new Dictionary<string, string>(){
					{"MessageType", config.RequestTypeName},
					{"ProviderName", providerName},
					{"Version", version},
					{"QueueIdentifier", queueName},
				}));
		}

		public void ConfigureEndpoint(string serviceName, string providerName, string queueName, string version)
		{
			Send(new SmsMessage(RouterService.ConfigureServiceEndpointAddress, null,
				new Dictionary<string, string>(){
					{"MessageType", serviceName},
					{"ProviderName", providerName},
					{"Version", version},
					{"QueueIdentifier", queueName},
				}));
		}


		public void ClearConfiguration(string queueIdentier, string exceptVersion)
		{
			Send(new SmsMessage(RouterService.ConfigureServiceClearAll, null,
						new Dictionary<string, string>(){
					{"Version", exceptVersion},
					{"QueueIdentifier", queueIdentier}
				}));
		}

		public void ConfigureMapping<FROM, TO>(string version)
		{
			var from = registry.Get<FROM>();
			var to = registry.Get<TO>();
			this.ConfigureMapping(from.RequestTypeName, to.RequestTypeName, version);
		}

		public void ConfigureMapping(Type fromType, Type toType, string version)
		{
			var from = registry.Get(fromType);
			var to = registry.Get(toType);
			this.ConfigureMapping(from.RequestTypeName, to.RequestTypeName, version);
		}

		public void ConfigureMapping(string fromType, string toType, string version)
		{
			Send(new SmsMessage(RouterService.ConfigureServiceMappingAddress, null,
				new Dictionary<string, string>(){
					{"FromType", fromType},
					{"Version", version},
					{"ToType", toType}
				}));
		}

		public string ProviderName { get { return router.ProviderName; } }

		public string QueueName { get { return router.QueueName; } }

		public void Send(SmsMessage request)
		{
			request.Headers[RouterSettings.ServiceNameHeaderKey] = request.ToAddress;
			router.Send(request);
		}

		public void Send(string toService, string content)
		{
			Send(new SmsMessage(toService, content));
		}

		public SmsMessage CreateMessage<T>(T request) where T : class, new()
		{
			var config = registry.Get<T>();
			var serializer = serializerFactory.Get(config.Serializer);
			var headers = config.Headers == null ? new Dictionary<string, string>() : config.Headers.ToDictionary(x => x.Key, x => x.Value);
			headers[RouterSettings.ServiceNameHeaderKey] = config.RequestTypeName;
			return new SmsMessage(config.RequestTypeName, serializer.Serialize(request), headers);
		}

		public void Dispose()
		{
			router.Dispose();
		}

		private static RouterSink defaultSink;

		/// <summary>
		///  Not thread safe, make sure you set the default value thread-safely...
		/// </summary>
		public static RouterSink Default
		{
			get { return defaultSink ?? DefaultRouter.Item; }
			set { defaultSink = value; }
		}


		//        public SmsMessage CreateMessage<T>(T request) where T : class, new()
		//        {
		//            var config = registry.Get<T>();

		//            var serializer = serializerFactory.Get(config.Serializer);

		//            var headers = config.Headers == null ? null : config.Headers.ToDictionary(x => x.Key, x => x.Value);

		//            return new SmsMessage(config.ServiceName, serializer.Serialize(request), headers);
		//        }

		class DefaultRouter
		{
			// Explicit static constructor to tell C# compiler not to mark type as beforefieldinit
			static DefaultRouter() { }
			internal static readonly RouterSink Item = new RouterSink(SmsFactory.Sender(RouterSettings.ProviderName, RouterSettings.SendQueueName),
				Defaults.ServiceDefinitionRegistry, Defaults.SerializerFactory);
		}
	}
}
