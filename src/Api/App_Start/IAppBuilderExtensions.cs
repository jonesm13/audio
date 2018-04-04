﻿namespace Api
{
    using Hangfire;
    using IoC;
    using MediatR;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Serialization;
    using Owin;
    using SimpleInjector;
    using SimpleInjector.Integration.WebApi;
    using SimpleInjector.Lifestyles;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http.Formatting;
    using System.Net.Http.Headers;
    using System.Reflection;
    using System.Web.Http;
    using ApiController = Infrastructure.WebApi.ApiController;
    using GlobalConfiguration = Hangfire.GlobalConfiguration;

    // ReSharper disable once InconsistentNaming
    public static class IAppBuilderExtensions
    {
        public static IAppBuilder UseHangFire(this IAppBuilder app)
        {
            GlobalConfiguration.Configuration.UseSqlServerStorage("audio-db");

            app.UseHangfireDashboard();
            app.UseHangfireServer();

            return app;
        }

        public static IAppBuilder UseWebApi(this IAppBuilder app)
        {
            HttpConfiguration config = new HttpConfiguration();

            Container container = ContainerFactory.Create(
                new AsyncScopedLifestyle(),
                Assemblies.ToList(),
                c =>
                {
                    c.RegisterWebApiControllers(config);

                    c.RegisterInitializer<ApiController>(apiController =>
                    {
                        apiController.Mediator = c.GetInstance<IMediator>();
                    });
                });

            config.DependencyResolver = new SimpleInjectorWebApiDependencyResolver(container);

            config.MapHttpAttributeRoutes();

            // formatters
            config.Formatters.Clear();
            JsonMediaTypeFormatter jsonFormatter = new JsonMediaTypeFormatter();
            jsonFormatter.SupportedMediaTypes.Clear();
            jsonFormatter.SupportedMediaTypes.Add(new MediaTypeHeaderValue("application/json"));
            config.Formatters.Add(jsonFormatter);

            // set JSON serialiser used by WebApi to use our desired serialisation settings
            config.Formatters.JsonFormatter.SerializerSettings = new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                DateFormatHandling = DateFormatHandling.IsoDateFormat,
                DateParseHandling = DateParseHandling.DateTime,
                DateTimeZoneHandling = DateTimeZoneHandling.Utc,
                NullValueHandling = NullValueHandling.Ignore
            };

            app.UseWebApi(config);

            return app;
        }

        static IEnumerable<Assembly> Assemblies
        {
            get
            {
                yield return Assembly.Load("Domain");
            }
        }
    }
}
