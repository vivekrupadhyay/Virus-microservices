using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Logging;
using Microsoft.IdentityModel.Tokens;
using Middleware;
using Newtonsoft.Json.Serialization;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using Ocelot.Provider.Consul;
using Org.BouncyCastle.Asn1.Tsp;
using System;
using System.Text;

namespace BackendGateway
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        private readonly IWebHostEnvironment webHostEnvironment;
        public Startup(IConfiguration configuration, IWebHostEnvironment environment)
        {
            Configuration = configuration;
            webHostEnvironment = environment;
        }

        public IConfiguration Configuration { get; }
        public void ConfigureServices(IServiceCollection services)
        {
            IdentityModelEventSource.ShowPII = true;

            services.AddOcelot(Configuration);//.AddConsul().AddConfigStoredInConsul();//
            services.AddControllers().AddNewtonsoftJson(setupAction => { setupAction.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver(); })
            .AddXmlDataContractSerializerFormatters();


            //services.AddAuthentication(TokenHelper.AuthenticationOptions())
            //    .AddJwtBearer(TokenHelper.JwtBearerOptions(webHostEnvironment.IsDevelopment()));


            var jwtSection = Configuration.GetSection("jwt");
            var jwtOptions = jwtSection.Get<JwtOptions>();
            var key = Encoding.UTF8.GetBytes(jwtOptions.Secret);

            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
           .AddJwtBearer(x =>
           {
               x.RequireHttpsMetadata = false;
               x.SaveToken = true;
               x.TokenValidationParameters = new TokenValidationParameters
               {
                   ValidateIssuerSigningKey = true,
                   IssuerSigningKey = new SymmetricSecurityKey(key),
                   ValidateIssuer = false,
                   ValidateAudience = false,
                   // ValidIssuer = "localhost",
                   // ValidAudience = "localhost"

               };
           });

            services.AddAuthorization(options =>
            {

                options.AddPolicy("User",
                    authBuilder =>
                    {
                        authBuilder.RequireRole("Users");
                    });

            });
            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy",
                //options.AddDefaultPolicy(
                    builder =>
                    {
                        builder.AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader()
                .SetPreflightMaxAge(TimeSpan.FromMinutes(10));
                    }
                  );
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
        public async void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseHealthChecks("/virus", new HealthCheckOptions()
            {
                Predicate = _ => true,
                ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
            });
            app.UseHealthChecksUI();
            app.UseMiddleware<RequestResponseLoggingMiddleware>();

            app.UseCors("CorsPolicy");

            app.UseAuthentication();
            app.UseAuthorization();
            //app.Run(async (context) =>
            //{
            //    await context.Response.WriteAsync("Hello World!");
            //});
            app.UseRouting();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
            await app.UseOcelot();
        }
    }
}
