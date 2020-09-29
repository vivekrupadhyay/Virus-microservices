using HealthChecks.UI.Client;
using IdentityMicroservice.Repository;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Middleware;
using MongoDB.Driver;
using Newtonsoft.Json.Serialization;
using System;
using System.Text;

namespace IdentityMicroservice
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
            // services.AddControllers();
            services.AddControllers().AddNewtonsoftJson(setupAction => { setupAction.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver(); })
            .AddXmlDataContractSerializerFormatters();
            services.AddMongoDb(Configuration);
            services.AddSingleton<IUserRepository>(sp => new UserRepository(sp.GetService<IMongoDatabase>()));
            services.AddJwt(Configuration);
            services.AddTransient<IEncryptor, Encryptor>();
            services.AddTransient<IJwtBuilder, JwtBuilder>();
            services.AddTransient<TokenManagerMiddleware>();
            services.AddDistributedMemoryCache();
            services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Identity", Version = "v1" });
            });
            services.AddHealthChecks()
                .AddMongoDb(
                mongodbConnectionString: Configuration.GetSection("mongo").Get<MongoOptions>().ConnectionString,
                name: "mongo",
                failureStatus: HealthStatus.Unhealthy
                );
            services.AddHealthChecksUI().AddInMemoryStorage();



            //services.AddAuthorization(options =>
            //{
            //    options.AddPolicy("User",
            //        authBuilder =>
            //        {
            //            authBuilder.RequireRole("User");
            //        });
            //});

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
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Identity V1");
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
