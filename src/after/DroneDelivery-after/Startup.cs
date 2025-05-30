﻿using DroneDelivery.Common.Services;
using DroneDelivery_after.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;

namespace DroneDelivery_after
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services
                .AddHttpClient()
                .AddTransient<IDeliveryRepository, DeliveryRepository>()
                .AddTransient<IDroneScheduler, DroneScheduler>()
                .AddTransient<IPackageProcessor, PackageServiceCaller>()
                .AddTransient<IRequestProcessor, RequestProcessor>();

            services
                .AddHttpClient<IPackageProcessor, PackageServiceCaller>(c =>
                {
                    c.BaseAddress = new System.Uri(Configuration["PackageServiceUri"]);
                });

            PackageServiceCaller.FunctionCode = Configuration["PackageServiceFunctionCode"];

            services.AddControllersWithViews();

            // Enable swagger doc
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "DroneDelivery-after API", Version = "v1" });
            });

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger();

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.), 
            // specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "DroneDelivery-after (Microservice) API v1");
            });

            app.UseHttpsRedirection();

            // Add routing middleware before endpoints
            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
