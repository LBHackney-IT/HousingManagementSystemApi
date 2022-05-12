using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;

namespace HousingManagementSystemApi
{
    using System.Collections.Generic;
    using System.Net.Http;
    using Gateways;
    using Hackney.Shared.Asset.Domain;
    using Hackney.Shared.Tenure.Domain;
    using HousingRepairsOnline.Authentication.DependencyInjection;
    using UseCases;

    public class Startup
    {
        private const string HousingManagementSystemApiIssuerId = "Housing Management System Api";
        public static IEnumerable<AssetType> EligibleAssetTypes = new[]
        {
            AssetType.Flat, AssetType.House, AssetType.Dwelling, AssetType.StudioFlat, AssetType.SelfContainedBedsit
        };

        public static readonly IEnumerable<TenureType> EligibleTenureTypes = new List<TenureType>
        {
            TenureTypes.Introductory
        };

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


            services.AddTransient<IRetrieveAddressesUseCase, RetrieveAddressesUseCase>(s =>
            {
                var tenureGateway = s.GetService<ITenureGateway>();
                var assetGateway = s.GetService<IAssetGateway>();
                var addressesGateway = s.GetService<IAddressesGateway>();
                var eligibleAssets = EligibleAssetTypes;
                var eligibleTenureTypes = EligibleTenureTypes;
                return new RetrieveAddressesUseCase(addressesGateway, assetGateway, eligibleAssets, tenureGateway, eligibleTenureTypes);
            });

            AddHttpClients(services);

            services.AddTransient<ITenureGateway, TenureGateway>();
            services.AddTransient<IAssetGateway, AssetGateway>();
            services.AddTransient<IAddressesGateway, PropertiesGateway>();
            // services.AddTransient<IAddressesGateway, HousingSearchGateway>();

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
            AddHttpClient(services, HttpClientNames.HousingSearch, "HOUSING_SEARCH_API_URI", "HOUSING_SEARCH_API_KEY");
            AddHttpClient(services, HttpClientNames.Properties, "PROPERTIES_API_URI", "PROPERTIES_API_KEY");
            AddHttpClient(services, HttpClientNames.Asset, "HOUSING_ASSET_API_URI", "HOUSING_ASSET_API_KEY");
            AddHttpClient(services, HttpClientNames.TenureInformation, "TENURE_INFORMATION_API_URI", "TENURE_INFORMATION_API_KEY");
        }

        private static void AddHttpClient(IServiceCollection services, string clientName, string apiUriEnvVarName, string apiKey)
        {
            var uri = new Uri(GetEnvironmentVariable(apiUriEnvVarName));
            var key = GetEnvironmentVariable(apiKey);
            AddClient(services, clientName, uri, key);
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
