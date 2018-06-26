using IdentityServer4.Quickstart.UI;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NHibernate.Cfg;
using NHibernate.Dialect;
using System.Data;
using System.Linq;
using AutoMapper;
using IdentityServer4.NHibernate.Automapper;
using IdentityServer4.NHibernate.Entities;
using IdentityServer4.NHibernate.Stores;
using Microsoft.EntityFrameworkCore.Query.Internal;

namespace Host
{
    public class Startup
    {
        public IConfiguration Configuration { get; }
        public IHostingEnvironment HostingEnvironment { get; }

        public Startup(IConfiguration configuration, IHostingEnvironment env)
        {
            Configuration = configuration;
            HostingEnvironment = env;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services
                .AddIdentityServer()
                .AddDeveloperSigningCredential()
                .AddConfigurationStore(options =>
                {
                    // options.Schema = "identity_configuration";
                    options.SetupDbIntegration = db =>
                    {
                        db.Dialect<PostgreSQL83Dialect>();
                        db.ConnectionString = Configuration.GetConnectionString("Default");
                        db.BatchSize = 50;
                        db.KeywordsAutoImport = Hbm2DDLKeyWords.Keywords;
                        db.IsolationLevel = IsolationLevel.ReadCommitted;
                    };
                })
                .AddOperationalStore(options =>
                {
                    // options.Schema = "identity_operation";
                    options.SetupDbIntegration = db =>
                    {
                        db.Dialect<PostgreSQL83Dialect>();
                        db.KeywordsAutoImport = Hbm2DDLKeyWords.Keywords;
                        db.ConnectionString = Configuration.GetConnectionString("Default");
                        db.BatchSize = 50;
                        db.IsolationLevel = IsolationLevel.ReadCommitted;
                    };
                })
                .AddTestUsers(TestUsers.Users)
                ;
            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            Populate(app);
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app
                .UseIdentityServer()
                .UseStaticFiles()
                .UseMvcWithDefaultRoute();
        }

        private void Populate(IApplicationBuilder app)
        {
            using(var scope = app.ApplicationServices.CreateScope())
            {
                var mapper = new MapperConfiguration(cfg =>
                {
                    cfg.AddProfile<ClientProfile>();
                    cfg.AddProfile<ResourceProfile>();
                }).CreateMapper();

                var provider = scope.ServiceProvider.GetService<ConfigurationSessionProvider>();
                if (!provider.Session.Query<Client>().Any())
                {
                    foreach (var client in Config.GetClients().Select(x => mapper.Map<Client>(x)))
                    {
                        provider.Session.Save(client);
                    }

                    foreach (var identityResouce in Config.GetIdentityResources()
                        .Select(x => mapper.Map<IdentityResource>(x)))
                    {
                        provider.Session.Save(identityResouce);
                    }

                    foreach (var apiResource in Config.GetApiResources().Select(x => mapper.Map<ApiResource>(x)))
                    {
                        provider.Session.Save(apiResource);
                    }
                }

            }
        }
    }
}