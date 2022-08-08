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
using Online_Taxi.Hubs;
using _3_Infrastructure.Repositories.Passenger;
using _3_Infrastructure.Repositories.Driver;

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
            services.AddCors(o => o.AddPolicy("MyPolicy", builder =>
            {
                builder.AllowAnyOrigin()
                       .AllowAnyMethod()
                       .AllowAnyHeader();

            }));
            services.AddMediatR(typeof(Startup));
            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Online_Taxi", Version = "v1" });
            });
            
            //var assembly = Assembly.GetExecutingAssembly();
            //services.AddMediatR(assembly);
            //services.AddMediatR(AppDomain.CurrentDomain.GetAssemblies());
            services.AddApplication();
            services.AddMediatR(typeof(RegisterRequestHandler).GetTypeInfo().Assembly);
            services.AddScoped(typeof(IUserRepository), typeof(UserRepository));
            services.AddScoped(typeof(ILocationRepository), typeof(LocationRepository));
            services.AddScoped(typeof(ITripReqRepository), typeof(TripReqRepository));
            services.AddScoped(typeof(IPassengerRepository), typeof(PassengerRepository));
            services.AddScoped(typeof(IDriverRepository), typeof(DriverRepository));
            services.AddTransient<IUnitOfWork, UnitOfWork>();
            services.AddHttpContextAccessor();
            services.AddTransient<IHttpContextAccessor, HttpContextAccessor>();


            services.AddScoped<IDbConnection>((sp) => new SqlConnection(Configuration["ConnectionStrings:DefaultConnection"]));

            services.AddSingleton<IDictionary<string, UserConnection>>(opts => new Dictionary<string, UserConnection>());

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

            app.UseHttpsRedirection();

            app.UseRouting();
            app.UseCors("MyPolicy");
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
