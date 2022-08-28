using System;
using System.IO;
using System.Linq;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using CoreCommon.Application.APIBase.Base;
using CoreCommon.Application.WebAPIBase.Components;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace CoreCommon.Application.WebAPIBase.Base
{
    /// <summary>
    /// <inheritdoc />.
    /// </summary>
    public abstract class StartupWebBase : StartupBase
    {
        public string[] Origins { get; set; } = new string[]
        {
            "http://localhost:8080",
            "http://localhost:3001",
            "http://localhost:3002",
            "http://localhost:49300",
        };

        /// <summary>
        /// Configure application.
        /// </summary>
        /// <param name="app"></param>
        /// <param name="env"></param>
        public virtual void Configure(IApplicationBuilder app, IHostEnvironment env)
        {
            AutofacContainer = app.ApplicationServices.GetAutofacRoot();

            if (!env.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");

                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            // app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();
            app.UseCors();

            app.UseAuthentication();
            app.UseAuthorization();
            app.UseCookiePolicy(new CookiePolicyOptions
            {
                MinimumSameSitePolicy = SameSiteMode.None,
                HttpOnly = Microsoft.AspNetCore.CookiePolicy.HttpOnlyPolicy.None,
            });

            app.UseMiddleware<AppExceptionMiddleware>();

            app.UseFileServer(new FileServerOptions
            {
                // FileProvider = new Microsoft.Extensions.FileProviders.PhysicalFileProvider(@"\\api"),
                // RequestPath = new PathString("/api"),
                EnableDirectoryBrowsing = false,
            });

            app.UseMiddleware<HttpHeadersMiddleware>();

            app.UseSwagger(c =>
            {
                c.RouteTemplate = "swg/{documentname}/swagger.json";
            });

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.),
            // specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint($"./v-1/swagger.json", "API V1");
                c.RoutePrefix = "swg";
            });

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();

                endpoints.MapGet("/", async context =>
                {
                    await context.Response.WriteAsync($"{Configuration["ProjectName"]} project is running. v{Configuration["Version"]}");
                });
            });
        }

        /// <summary>
        /// Add services.
        /// </summary>
        /// <param name="services"></param>
        public override void ConfigureServices(IServiceCollection services)
        {
            services.AddHttpContextAccessor();
            base.ConfigureServices(services);

            services.AddCors(o =>
            {
                o.AddDefaultPolicy(builder =>
                {
                    builder.WithOrigins(Origins)
                           .AllowAnyMethod()
                           .AllowAnyHeader()
                           .AllowCredentials();
                });
            });

            var mvcBuilder = services.AddControllers().AddControllersAsServices();
            mvcBuilder.AddApplicationPart(GetType().Assembly).AddControllersAsServices();

            // register api modules routes
            var apiModuleFiles = Directory.GetFiles(AppContext.BaseDirectory, "*.ApiBase.dll").ToList();
            foreach (var apiFilePath in apiModuleFiles)
            {
                mvcBuilder.AddApplicationPart(System.Reflection.Assembly.LoadFrom(apiFilePath)).AddControllersAsServices();
            }

            services.AddAuthorization(options =>
            {
                // options.DefaultPolicy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build();
            });

            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(options =>
            {
                // options.Cookie.HttpOnly = true;
            });

            // services.AddHttpsRedirection(options =>
            // {
            //    options.RedirectStatusCode = Environment.IsDevelopment() ? StatusCodes.Status307TemporaryRedirect : StatusCodes.Status308PermanentRedirect;
            //    //options.HttpsPort = Environment.IsDevelopment() ? 44308 : 443;
            // });

            // Register the Swagger generator, defining 1 or more Swagger documents
            services.AddSwaggerGen(c =>
            {
                c.AddServer(new Microsoft.OpenApi.Models.OpenApiServer { Url = $"/{Configuration["SettingName"]}/{Configuration["ProjectName"]}" });
                c.AddServer(new Microsoft.OpenApi.Models.OpenApiServer { Url = "/" });
                c.SwaggerDoc("v-1", new Microsoft.OpenApi.Models.OpenApiInfo { Title = Configuration["ProjectName"] + " API", Version = "v-1" });
                c.ResolveConflictingActions(apiDescriptions => apiDescriptions.First());
                c.CustomSchemaIds(type => type.ToString());
            });

            services.Configure<Microsoft.AspNetCore.Http.Features.FormOptions>(options =>
            {
                options.ValueCountLimit = int.MaxValue;
                options.ValueLengthLimit = 1024 * 1024 * 100; // 100MB max len form data
            });

            services.AddMvc(options =>
            {
                options.Conventions.Add(new ModelBindingConvention());
            }).AddNewtonsoftJson(o =>
            {
                o.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                o.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                o.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
#if DEBUG
                o.SerializerSettings.Formatting = Formatting.Indented;
#else
                o.SerializerSettings.Formatting = Formatting.None;
#endif
            });
        }

        public override void ConfigureContainer(ContainerBuilder builder)
        {
            base.ConfigureContainer(builder);
        }
    }
}
