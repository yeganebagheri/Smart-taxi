using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using MediatR;
using Infrastructure.Repositories.User;
using Infrastructure;
using Application.Auth;
using Microsoft.AspNetCore.Http;
using static Application.Auth.RegisterRequest;
using Application;
using _3_Infrastructure.Repositories.Trip;
//using Online_Taxi.Hubs;
using _3_Infrastructure.Repositories.Passenger;
using _3_Infrastructure.Repositories.Driver;
using _3_Infrastructure.Repositories.Driver.Driver_req;
using _3_Infrastructure.Repositories.Pre_Trip;
using static _3_Infrastructure.Repositories.Pre_Trip.IPreTripRepository;
using Application.Hubs;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNetCore.SignalR;
using Application.Services;
using Newtonsoft.Json.Converters;
using Microsoft.AspNetCore.Authentication.JwtBearer;
//using Api.Middlewares;

namespace Online_Taxi
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
            //services.AddCors(o => o.AddPolicy("MyPolicy", builder =>
            //{
            //    builder/*.AllowAnyOrigin()*/
            //           .AllowAnyMethod()
            //           .AllowAnyHeader();

            //}));

            //services.AddAuthentication()
            //    .AddJwtBearer(options =>
            //    {
            //        options.Events = new JwtBearerEvents
            //        {
            //            OnMessageReceived  = context =>
            //            {
            //                var accesstoken = context.Request.Query["access_token"];
            //                if (string.IsNullOrEmpty(accesstoken) == false)
            //                {
            //                    context.Token = accesstoken;
            //                }
            //                return Task.CompletedTask;
            //            }
            //        };
            //    });
            services.AddSignalR(options =>
            {
                options.EnableDetailedErrors = true;
            });

            services.AddCors(options => options.AddPolicy("MyPolicy",
            builder =>
            {
                builder.AllowAnyHeader()
                       .AllowAnyMethod()
                       .SetIsOriginAllowed((host) => true)
                       .AllowCredentials();
            }));
            services.AddMediatR(typeof(Startup));
            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Online_Taxi", Version = "v1" });
            });


            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = "localhost:6000";
            });

            //var assembly = Assembly.GetExecutingAssembly();
            //services.AddMediatR(assembly);
            //services.AddMediatR(AppDomain.CurrentDomain.GetAssemblies());
            //services.AddSignalR();
            //services.AddSignalR()
            //.AddJsonProtocol(options =>
            //{
            //    options.PayloadSerializerSettings.Converters.Add(new StringEnumConverter(true));
            //});
            services.AddApplication();
            services.AddMediatR(typeof(RegisterRequestHandler).GetTypeInfo().Assembly);
            services.AddScoped(typeof(IUserRepository), typeof(UserRepository));
            services.AddScoped(typeof(ILocationRepository), typeof(LocationRepository));
            services.AddScoped(typeof(ITripReqRepository), typeof(TripReqRepository));
            services.AddScoped(typeof(IPassengerRepository), typeof(PassengerRepository));
            services.AddScoped(typeof(IDriverRepository), typeof(DriverRepository));
            services.AddScoped(typeof(IDriverReqRepository), typeof(DriverReqRepository));
            services.AddScoped(typeof(ISubPreTripRepository), typeof(SubPreTripRepository)); 
            services.AddScoped(typeof(IPreTripRepository), typeof(PreTripRepository));
            services.AddScoped(typeof(IRedisServices), typeof(RedisServices));
            //services.AddScoped(typeof(IHubService), typeof(TripListHub));
            services.AddTransient<IUnitOfWork, UnitOfWork>();
            services.AddHttpContextAccessor();
            services.AddTransient<IHttpContextAccessor, HttpContextAccessor>();
            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = "localhost";
            });

            //services.AddSignalR().AddRedis(options =>
            //{
            //    options.Configuration.ClientName
            //});

            services.AddScoped<IDbConnection>((sp) => new SqlConnection(Configuration["ConnectionStrings:DefaultConnection"]));

            
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Online_Taxi v1"));
            }
            app.UseCors("MyPolicy");
            //app.UseSignalR(routes =>
            //{
            //    routes.MapHub<General>("/hubs/general");
            //});
            app.UseHttpsRedirection();
            //app.UseMiddleware<LoggerMiddleware>();
            app.UseRouting();
            
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHub<TripListHub>("/TripList");
            });
        }
    }
}
