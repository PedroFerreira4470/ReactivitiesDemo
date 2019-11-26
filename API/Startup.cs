using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Persistence;
using Application.Activities;
using MediatR;
using FluentValidation.AspNetCore;
using API.Middleware;
using Domain;
using Microsoft.AspNetCore.Identity;
using Application.Interfaces;
using Infrastructure.Security;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using AutoMapper;
using Infrastructure.Photos;
using API.SignalR;
using System.Threading.Tasks;
using Application.Profiles;
using System;
using Microsoft.Extensions.Hosting;

namespace API
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }


        public void ConfigureDevelopmentServices(IServiceCollection services) {
            services.AddDbContext<DataContext>(opt =>
            {
                opt.UseLazyLoadingProxies();
                opt.UseSqlite(Configuration.GetConnectionString("DefaultConnection"));
            });
            ConfigureServices(services);
        }

        public void ConfigureProductionServices(IServiceCollection services)
        {
            services.AddDbContext<DataContext>(opt =>
            {
                opt.UseLazyLoadingProxies();
                opt.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"));
            });
            ConfigureServices(services);
        }
        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            services.AddCors(opt => {
                opt.AddPolicy("CorsPolicy", policy =>{
                    policy.AllowAnyHeader()
                          .AllowAnyMethod()
                          .WithExposedHeaders("WWW-Authenticate")
                          .WithOrigins("http://localhost:3000")
                          .AllowCredentials();
                });
            });
            services.AddMediatR(typeof(List.Handler).Assembly);
            services.AddAutoMapper(typeof(List.Handler));
            services.AddSignalR();
            services.AddControllers(opt => {
                //This will give authorization to every api including the login, you have to use allowanonymous///
                var policy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build();
                opt.Filters.Add(new AuthorizeFilter(policy));
            })
                .AddFluentValidation(cfg => cfg.RegisterValidatorsFromAssemblyContaining<Create>());

            var builder = services.AddIdentityCore<AppUser>();
            var identitybuilder = new IdentityBuilder(builder.UserType,builder.Services);
            identitybuilder.AddEntityFrameworkStores<DataContext>();
            identitybuilder.AddSignInManager<SignInManager<AppUser>>();

            services.AddAuthorization(opt => {
                opt.AddPolicy("isActivityHost", policy =>
                {
                    policy.Requirements.Add(new IsHostRequirement());
                });
            });
            services.AddTransient<IAuthorizationHandler, IsHostRequirementHandler>();
            //Give token key to the Authentication
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["TokenKey"] /*secret user*/));
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(opt => {
                    opt.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = key,
                        ValidateAudience = false,
                        ValidateIssuer = false,
                        ValidateLifetime = true,
                        ClockSkew = TimeSpan.Zero,
                    };
                    
                    opt.Events = new JwtBearerEvents
                    {
                        OnMessageReceived = context => {
                            var acessToken = context.Request.Query["access_token"];
                            var path = context.HttpContext.Request.Path;
                            if (!string.IsNullOrEmpty(acessToken) &&
                                (path.StartsWithSegments("/chat")))
                            {
                                context.Token = acessToken;
                            }
                            return Task.CompletedTask;
                        }
                    };
                });

            services.AddScoped<IJwtGenerator, JwsGenerator>();
            services.AddScoped<IUserAcessor, UserAcessor>();
            services.AddScoped<IProfileReader, ProfileReader>();
            services.AddScoped<IPhotoAcessor, PhotoAcessor>();
            services.AddScoped<IFacebookAccessor, FacebookAccessor>();
            services.Configure<CloudinarySettings>(Configuration.GetSection("Cloudinary"));
            services.Configure<FacebookAppSettings>(Configuration.GetSection("Authentication:Facebook"));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseMiddleware<ErrorHandlingMiddleware>();
            if (env.IsDevelopment())
            {
                //app.UseDeveloperExceptionPage();
            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseXContentTypeOptions();
            app.UseReferrerPolicy(opt => opt.NoReferrer());
            app.UseXXssProtection(opt => opt.EnabledWithBlockMode());
            app.UseXfo(opt => opt.Deny());
            app.UseCsp(opt => opt
                .BlockAllMixedContent()
                .StyleSources(s => s.Self().CustomSources("http://cdn.jsdelivr.net", "https://fonts.googleapis.com"))
                .FontSources(s => s.Self().CustomSources("https://fonts.gstatic.com", "data:"))
                .FormActions(s => s.Self())
                .FrameAncestors(s => s.Self())
                .ImageSources(s => s.Self().CustomSources("https://res.cloudinary.com","blob:", "data:"))
                .ScriptSources(s => s.Self().CustomSources(
                "sha256-EWcbgMMrMgeuxsyT4o76Gq/C5zilrLxiq6oTo2KDqus=", "https://connect.facebook.net", "sha256-tsYFq5pUcggQKFXnvmlvUrk8MgTJLL1Gjnqenv201b8="))
            );

            app.UseDefaultFiles();
            app.UseStaticFiles();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseCors("CorsPolicy");
            app.UseEndpoints(endpoints => {
                endpoints.MapControllers();
                endpoints.MapHub<ChatHub>("/chat");
                endpoints.MapFallbackToController("index","Fallback");
            });
            // app.UseSignalR(routes => { routes.MapHub<ChatHub>("/chat"); });
            // //app.UseHttpsRedirection();
            // app.UseMvc(routes => {
            //     routes.MapSpaFallbackRoute(
            //         name: "spa-fallback",
            //         defaults: new { controller = "Fallback", action = "Index" }
            //         );
            // });
        }
    }
}
