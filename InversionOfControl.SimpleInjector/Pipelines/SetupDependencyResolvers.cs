﻿using System.Web.Http;
using Common.Web.IoC;
using SimpleInjector;
using SimpleInjector.Integration.Web.Mvc;
using SimpleInjector.Integration.WebApi;
using Sitecore.Pipelines;
using IMVCDependencyResolver = System.Web.Mvc.IDependencyResolver;
using IWebAPIDependencyResolver = System.Web.Http.Dependencies.IDependencyResolver;

namespace InversionOfControl.SimpleInjector.Pipelines
{
    public class SetupDependencyResolvers
    {
        public void Process(PipelineArgs args)
        {
            Container container = this.BuildContainerAndRegisterTypes();

            // create the chained resolvers using our own resolvers then falling back to whatever was previously set.
            // MVC
            IMVCDependencyResolver chainedMVCResolver = new ChainedMvcResolver(new SimpleInjectorDependencyResolver(container),
                                                                               System.Web.Mvc.DependencyResolver.Current);
            System.Web.Mvc.DependencyResolver.SetResolver(chainedMVCResolver);

            //WebAPI
            IWebAPIDependencyResolver chainedWebAPIResolver = new ChainedWebApiResolver(new SimpleInjectorWebApiDependencyResolver(container),
                                                                                GlobalConfiguration.Configuration.DependencyResolver);
            System.Web.Http.GlobalConfiguration.Configuration.DependencyResolver = chainedWebAPIResolver;
        }

        private Container BuildContainerAndRegisterTypes()
        {
            var container = new Container();

            container.Register<CustomController.Interfaces.ILogger, CustomController.Implementations.Log4NetLogger>();

            // Simple Injector doesn't require us to register our controllers, so nothing extra is needed here.

            return container;
        }
    }
}