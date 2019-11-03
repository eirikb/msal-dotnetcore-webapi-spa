using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.AzureAD.UI;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Logging;
using Microsoft.IdentityModel.Tokens;

namespace api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }
        //Reference Links : https://docs.microsoft.com/en-us/aspnet/core/security/cors?view=aspnetcore-2.2
        readonly string MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {//Error Occurs:  idx20803: unable to obtain configuration from: '[pii is hidden]'.
            //For Error resolution : https://stackoverflow.com/questions/54435551/how-to-fix-invalidoperationexception-idx20803-unable-to-obtain-configuration
            IdentityModelEventSource.ShowPII = true; //Add this line
            services.AddCors(options =>
            {
                options.AddPolicy(MyAllowSpecificOrigins,
                builder =>
                {
                    builder.WithOrigins("http://localhost:1234").AllowAnyHeader();
                });
            });
            

            services.AddAuthentication(AzureADDefaults.BearerAuthenticationScheme)
                .AddAzureADBearer(options => Configuration.Bind("AzureAd", options))
                .AddJwtBearer(options => {
                    options.TokenValidationParameters = new TokenValidationParameters()
                    {
                        ValidateIssuer = false,
                        //ValidateAudience = false
                     
                       ValidAudience = "80ce0b17-ac0b-43f5-add5-cd8c3412b6c9",
                        RequireSignedTokens =false
                    };
                    //options.MetadataAddress
                    //        options.SaveToken = true;
                    //For Localost testing disbaled https
                    //options.Authority = "http://localhost:5000";
                    //options.RequireHttpsMetadata = false;
                    // options.Audience = "api1";
                    options.Audience = "api://80ce0b17-ac0b-43f5-add5-cd8c3412b6c9";
                });
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseAuthentication();
            //app.UseCors(options => options.AllowCredentials().AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin());
            app.UseCors(MyAllowSpecificOrigins);
            //For Localost testing disbaled https
            //app.UseHttpsRedirection();
            app.UseMvc();
        }
    }
}
