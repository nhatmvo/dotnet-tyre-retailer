using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using store_management.Domain;
using store_management.Infrastructure;

namespace store_management
{
    public class Startup
    {
        public const string DEFAULT_CONNECTION_STRING = "Server=localhost;Port=32769;Database=TIRE_STORE_MANAGEMENT;User=root;Password=nhat1997;";
        public const string DEFAULT_DATABASE_PROVIDER = "mysql";

        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMediatR(Assembly.GetExecutingAssembly());

            var connectionString = Configuration.GetValue<string>("") ?? DEFAULT_CONNECTION_STRING;
            var databaseProvider = Configuration.GetValue<string>("") ?? DEFAULT_DATABASE_PROVIDER;

            services.AddDbContext<StoreContext>(options =>
            {
                if (databaseProvider.Trim().ToLower().Equals("mysql"))
                {
                    options.UseMySql(connectionString);
                }
                    
            });
            services.AddControllers();
            services.AddSwaggerGen(c => {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });
                // swagger use class name as SchemaId => specify fullname (including namesapce to generate unique SchemaId)
                c.CustomSchemaIds(i => i.FullName);
            });

            services.AddScoped<ICurrentUserAccessor, CurrentUserAccessor>();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseSwagger();

            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
            });

            app.UseHttpsRedirection();

            app.UseRouting();
            
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
