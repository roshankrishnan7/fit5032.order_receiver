using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using fit5032.order_receiver.ConfigOptions;
using fit5032.order_receiver.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace fit5032.order_receiver
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
            services.AddControllersWithViews();

            var dbConnectionString = Configuration.GetConnectionString("OrderingDB");
            services.AddDbContext<OrderingDBContext>(options =>
            {
                var builder = new SqlConnectionStringBuilder(dbConnectionString);
                var dbConnection = new SqlConnection(builder.ConnectionString);

                options.UseSqlServer(dbConnection);
            });
            // Read ServiceBus section from appsettings and binds to ListenerServiceBusConfig model
            // This helps us to easily group related configuration, which makes it easier for a developer to understand
            var serviceBusConfig = Configuration.GetSection("ServiceBus");
            services.Configure<ListenerServiceBusConfig>(serviceBusConfig);

            var serviceBusConnectionString = serviceBusConfig.Get<ListenerServiceBusConfig>().ConnectionString;

            // Registers ServiceBusClient using the provided connection string
            // Once ServiceBusClient is registered, we can use Dependency Injection to use instance of ServiceBusClient
            // in other services/controllers
            services.AddAzureClients(builder =>
            {
                builder.AddServiceBusClient(serviceBusConnectionString);
            });

            services.AddTransient<IServiceBusMessageProcessorService, ServiceBusMessageProcessorService>();

            services.AddHostedService<ServiceBusWorkerService>();
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
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
