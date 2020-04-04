using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace CoreCommon.Application.API.Base
{
    public abstract class StartupWebBase : CoreCommon.Business.Service.Base.StartupBase
    {
        public virtual void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            AutofacContainer = app.ApplicationServices.GetAutofacRoot();
        }

        public void AddControllersAndModules(IServiceCollection services)
        {
            var mvcBuilder = services.AddControllers().AddControllersAsServices();
            mvcBuilder.AddApplicationPart(GetType().Assembly).AddControllersAsServices();
            // register api modules routes
            var apiModuleFiles = Directory.GetFiles(AppContext.BaseDirectory, "Module*Api.dll").ToList();
            foreach (var apiFilePath in apiModuleFiles)
            {
                mvcBuilder.AddApplicationPart(System.Reflection.Assembly.LoadFrom(apiFilePath)).AddControllersAsServices();
            }
        }
    }
}
