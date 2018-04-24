namespace Api
{
    using System;
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
    using System.Web.Http.ExceptionHandling;
    using log4net.Config;
    using ApiController = Infrastructure.WebApi.ApiController;
    using GlobalConfiguration = Hangfire.GlobalConfiguration;

    // ReSharper disable once InconsistentNaming
    public static class IAppBuilderExtensions
    {
        public static IAppBuilder UseLog4Net(this IAppBuilder app)
        {
            XmlConfigurator.Configure();

            return app;
        }

        public static IAppBuilder UseHangFire(this IAppBuilder app)
        {
            GlobalConfiguration.Configuration.UseSqlServerStorage("audio-db");

            GlobalConfiguration.Configuration.UseActivator(
                new SimpleInjectorJobActivator(
                    BackgroundJobContainerFactory.Create()));

            app.UseHangfireDashboard();
            app.UseHangfireServer();

            return app;
        }

        public static IAppBuilder UseWebApi(this IAppBuilder app)
        {
            HttpConfiguration config = new HttpConfiguration();

            Container container = ContainerFactory.Create(
                new AsyncScopedLifestyle(),
                DomainAssembly.ToList(),
                c =>
                {
                    c.RegisterWebApiControllers(config);

                    c.RegisterInitializer<ApiController>(apiController =>
                    {
                        apiController.Mediator = c.GetInstance<IMediator>();
                    });

                    IEnumerable<Type> notificationHandlers = c.GetTypesToRegister(
                        typeof(INotificationHandler<>),
                        ProcessAssembly,
                        new TypesToRegisterOptions
                        {
                            IncludeGenericTypeDefinitions = true
                        });

                    c.RegisterCollection(
                        typeof(INotificationHandler<>),
                        notificationHandlers);
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

            config.Services.Replace(typeof(IExceptionHandler), new CustomExceptionHandler());

            app.UseWebApi(config);

            return app;
        }

        static IEnumerable<Assembly> DomainAssembly
        {
            get
            {
                yield return Assembly.Load("Domain");
            }
        }

        static IEnumerable<Assembly> ProcessAssembly
        {
            get
            {
                yield return Assembly.GetExecutingAssembly();
            }
        }
    }

    public class SimpleInjectorJobActivator : JobActivator
    {
        readonly Container container;

        readonly Lifestyle lifestyle;

        public SimpleInjectorJobActivator(Container container)
        {
            if (container == null)
            {
                throw new ArgumentNullException(nameof(container));
            }

            this.container = container;
        }

        public SimpleInjectorJobActivator(Container container, Lifestyle lifestyle)
        {
            if (container == null)
            {
                throw new ArgumentNullException(nameof(container));
            }

            if (lifestyle == null)
            {
                throw new ArgumentNullException(nameof(lifestyle));
            }

            this.container = container;
            this.lifestyle = lifestyle;
        }

        public override object ActivateJob(Type jobType)
        {
            return container.GetInstance(jobType);
        }

        public override JobActivatorScope BeginScope(JobActivatorContext context)
        {
            if (lifestyle == null || lifestyle != Lifestyle.Scoped)
            {
                return new SimpleInjectorScope(container, AsyncScopedLifestyle.BeginScope(container));
            }

            return new SimpleInjectorScope(container, Lifestyle.Scoped.GetCurrentScope(container));
        }
    }

    class SimpleInjectorScope : JobActivatorScope
    {
        readonly Container container;
        readonly Scope scope;

        public SimpleInjectorScope(Container container, Scope scope)
        {
            this.container = container;
            this.scope = scope;
        }

        public override object Resolve(Type type)
        {
            return container.GetInstance(type);
        }

        public override void DisposeScope()
        {
            scope?.Dispose();
        }
    }
}
