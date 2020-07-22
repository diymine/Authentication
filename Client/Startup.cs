using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Client
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddAuthentication(config =>
            {
                config.DefaultAuthenticateScheme = "ClientCookie";
                config.DefaultSignInScheme = "ClientCookie";
                config.DefaultChallengeScheme = "OurServer";
            }).AddCookie("ClientCookie").AddOAuth("OurServer", config =>
             {
                 config.ClientId = "client_id";
                 config.ClientSecret = "client_secret";
                 config.CallbackPath = "/oauth/callbak";
                 config.AuthorizationEndpoint = "https://localhost:44330/oauth/authorize";
                 config.TokenEndpoint = "https://localhost:44330/oauth/token";
                 config.SaveTokens = true;

                 config.Events = new OAuthEvents()
                 {
                     OnCreatingTicket = context =>
                     {
                         var accessToken = context.AccessToken;
                         var base64playLoad = accessToken.Split('.')[1];
                         var bytes = Convert.FromBase64String(base64playLoad);
                         var jsonPlayLoad = Encoding.UTF8.GetString(bytes);
                         var claims = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, string>>(jsonPlayLoad);

                         foreach(var claim in claims)
                         {
                             context.Identity.AddClaim(new Claim(claim.Key, claim.Value));
                         }
                         return Task.CompletedTask;
                     }
                 };

             });
            services.AddControllersWithViews();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapDefaultControllerRoute();
            });
        }
    }
}
