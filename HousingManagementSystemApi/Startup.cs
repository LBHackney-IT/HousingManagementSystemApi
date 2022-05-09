using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;

namespace HousingManagementSystemApi
{
    using System.Net.Http;
    using Gateways;
    using HousingRepairsOnline.Authentication.DependencyInjection;
    using UseCases;

    public class Startup
    {
        private const string HousingManagementSystemApiIssuerId = "Housing Management System Api";

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddHousingRepairsOnlineAuthentication(HousingManagementSystemApiIssuerId);

            services.AddControllers();
            services.AddTransient<IRetrieveAddressesUseCase, RetrieveAddressesUseCase>();

            AddHttpClients(services);

            services.AddTransient<IAddressesGateway, HousingSearchGateway>();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "HousingManagementSystemApi", Version = "v1" });
            });

            services.AddHealthChecks();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "HousingManagementSystemApi v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            // app.UseSentryTracing();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHealthChecks("/health");
                endpoints.MapControllers().RequireAuthorization();
            });
        }

        private static string GetEnvironmentVariable(string name) =>
            Environment.GetEnvironmentVariable(name) ??
            throw new InvalidOperationException($"Incorrect configuration: '{name}' environment variable must be set");

        private static void AddHttpClients(IServiceCollection services)
        {
            var uri = new Uri(GetEnvironmentVariable("HOUSING_SEARCH_API_URI"));
            var key = GetEnvironmentVariable("HOUSING_SEARCH_API_KEY");

            AddClient(services, HttpClientNames.HousingSearch, uri, key);
        }

        private static void AddClient(IServiceCollection services, string clientName, Uri uri, string key)
        {
            services.AddHttpClient(clientName, c =>
            {
                c.BaseAddress = uri;
                c.DefaultRequestHeaders.Add("Authorization", key);
            });
        }
    }
}
