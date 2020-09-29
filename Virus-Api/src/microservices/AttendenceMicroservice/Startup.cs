using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AttendenceMicroservice.Repository;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Middleware;
using MongoDB.Driver;
using System.Text.Json;
using MongoDB.Bson;

namespace AttendenceMicroservice
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
            services.AddControllers().AddNewtonsoftJson(options =>
            options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore);

            services.AddMongoDb(Configuration);
            services.AddSingleton<IAttendenceRepository>(sp => new AttendenceRepository(sp.GetService<IMongoDatabase>()));
            services.AddJwt(Configuration);
            services.AddTransient<IEncryptor, Encryptor>();
            services.AddTransient<IJwtBuilder, JwtBuilder>();
            services.AddTransient<TokenManagerMiddleware>();
            services.AddDistributedMemoryCache();
            services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Attendence", Version = "v1" });
            });
            services.AddHealthChecks()
                .AddMongoDb(
                mongodbConnectionString: Configuration.GetSection("mongo").Get<MongoOptions>().ConnectionString,
                name: "mongo",
                failureStatus: HealthStatus.Unhealthy
                );
            services.AddHealthChecksUI().AddInMemoryStorage();


        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMiddleware<TokenManagerMiddleware>();
            app.UseHealthChecks("/virus", new HealthCheckOptions()
            {
                Predicate = _ => true,
                ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
            });
            app.UseHealthChecksUI();

            app.UseSwagger();

            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Attendence V1");
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
