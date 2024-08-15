using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using BusinessLogic;
using DataAccess;
using DataAccess.DTO;
using DataAccess.EFCore;
using DataAccess.EFCore.Repository;
using MCSWebApp.Middleware;
using MCSWebApp.Models;
using MCSWebApp.Workers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using NLog;
using FastReport.Utils;
using FastReport.Data;
using MCSWebApp.Scheduler;
namespace MCSWebApp
{
    public class Startup
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        public IConfiguration Configuration { get; }
        public IWebHostEnvironment WebHostEnvironment { get; }

        public Startup(IConfiguration configuration, IWebHostEnvironment webHostEnvironment)
        {
            Configuration = configuration;
            WebHostEnvironment = webHostEnvironment;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            RegisteredObjects.AddConnection(typeof(PostgresDataConnection));

            services.AddHttpClient();
            //services.AddHostedService<SchedulerService>();
            services.AddSignalR();

            services.AddLogging();
            services.AddSession(options =>
            {
                services.AddDistributedMemoryCache();

                options.IdleTimeout = TimeSpan.FromDays(1);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });
            services.AddControllersWithViews()
                .AddNewtonsoftJson(x => x.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore);
            JsonConvert.DefaultSettings = () => new JsonSerializerSettings
            {
                Formatting = Newtonsoft.Json.Formatting.Indented,
                ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
            };

            services.AddRazorPages().AddRazorRuntimeCompilation();
            services.AddMvc()
                .AddJsonOptions(options => options.JsonSerializerOptions.PropertyNamingPolicy = null);
            services.AddCors();
            services.AddAntiforgery(options =>
            {
                options.Cookie.Name = "X-CSRF-TOKEN-MCS";
                options.FormFieldName = "CSRF-TOKEN-MCS-FORM";
            });

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "MCS",
                    Version = "v1"
                });

                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "JWT Authorization header using the Bearer scheme."
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                          new OpenApiSecurityScheme
                            {
                                Reference = new OpenApiReference
                                {
                                    Type = ReferenceType.SecurityScheme,
                                    Id = "Bearer"
                                }
                            },
                            new string[] {}
                    }
                });

                c.SchemaFilter<SwaggerExcludeSchemaFilter>();
            });
            services.AddSwaggerGenNewtonsoftSupport();

            // configure strongly typed settings objects
            var appSettingsSection = Configuration.GetSection("AppSettings");
            services.Configure<AppSettings>(appSettingsSection);

            // configure jwt authentication
            var appSettings = appSettingsSection.Get<AppSettings>();
            var key = JwtManager.SymmetricKey;
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
                    ValidateAudience = false
                };
            });

            services.AddAuthorization(options =>
            {
                options.AddPolicy("Administrator",
                    authBuilder =>
                    {
                        authBuilder.RequireRole("Administrator");
                    });
            });

            services.Configure<SysAdmin>(Configuration.GetSection("SysAdmin"));

            services.AddDbContext<mcsContext>(options => options
                .UseNpgsql(Configuration.GetConnectionString("MCS"))
                .UseLoggerFactory(MyEFCoreLogger.MyLoggerFactory));

            services.AddScoped<SmtpClient>((serviceProvider) =>
            {
                var config = serviceProvider.GetRequiredService<IConfiguration>();
                logger.Debug($"SMTP Host = {config.GetValue<string>("Email:Smtp:Host")}");
                logger.Debug($"SMTP Port = {config.GetValue<string>("Email:Smtp:Port")}");
                logger.Debug($"SMTP User = {config.GetValue<string>("Email:Smtp:Username")}");
                logger.Debug($"SMTP Ssl = {config.GetValue<string>("Email:Smtp:EnableSsl")}");

                return new SmtpClient()
                {
                    Host = config.GetValue<string>("Email:Smtp:Host"),
                    Port = config.GetValue<int>("Email:Smtp:Port"),
                    Credentials = new NetworkCredential(
                            config.GetValue<string>("Email:Smtp:Username"),
                            config.GetValue<string>("Email:Smtp:Password")
                        ),
                    EnableSsl = config.GetValue<bool>("Email:Smtp:EnableSsl")
                };
            });
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            // Run one-time only on startup
            services.AddHostedService<ApplicationRoleWorker>();
            services.AddHostedService<EmailWorker>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseFastReport();

            app.Use(async (context, next) =>
            {
                logger.Trace(context.Request.Method + " " + context.Request.GetEncodedPathAndQuery());

                var initialBody = context.Request.Body;
                using (var bodyReader = new StreamReader(context.Request.Body))
                {
                    string body = await bodyReader.ReadToEndAsync();
                    logger.Trace(body);

                    context.Request.Body = new MemoryStream(Encoding.UTF8.GetBytes(body));
                    await next.Invoke();
                    context.Request.Body = initialBody;                    
                }
            });

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                //app.UseHsts();
            }
            //app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();            

            app.UseCors(option => option
               .AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader());

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseSession();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHub<ProgressHub>("/progressHub");
                endpoints.MapRazorPages();

                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
                endpoints.MapControllerRoute(
                      name: "areas",
                      pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");
            });

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "MCS Api");
            });
        }
    }
}
