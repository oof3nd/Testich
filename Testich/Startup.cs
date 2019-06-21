using JavaScriptEngineSwitcher.ChakraCore;
using JavaScriptEngineSwitcher.Extensions.MsDependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SpaServices.Webpack;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using React.AspNet;
using reCAPTCHA.AspNetCore;
using System;
using System.Threading.Tasks;
using Testich.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity.UI.Services;
using Testich.Services;

namespace Testich
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


            services.AddAuthentication(options =>
            {

                options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;

            }).AddYandex(options =>
            {
                options.ClientId = "f58325ac297b43779d510abe9425e901";
                options.ClientSecret = "ca5f98eb77aa42b6b53f7ee648db4e72";
                options.SaveTokens = true;
            }).AddGoogle(options =>
            {
                options.ClientId = "960482467956-oe5i0ee5a38eoic9suuipokos1tf926q.apps.googleusercontent.com";
                options.ClientSecret = "UmRZlpey4nLJua0LlKx6C4B3";
            });

            services.AddTransient<IEmailSender, EmailSender>();

            services.AddTransient<IRecaptchaService, RecaptchaService>();

            if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Production")
                services.AddDbContext<UserContext>(options =>
                        options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));
            else
                services.AddDbContext<UserContext>(options => 
                options.UseSqlServer(Configuration.GetConnectionString("UserDatabase")));

            

            services.AddMvc().AddJsonOptions(options =>
            {
                options.SerializerSettings.ContractResolver = new DefaultContractResolver();
                options.SerializerSettings.Formatting = Formatting.Indented;
                options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Serialize;
                options.SerializerSettings.PreserveReferencesHandling = PreserveReferencesHandling.Objects;
            });

            JsonConvert.DefaultSettings = () => new JsonSerializerSettings
            {
                Formatting = Formatting.Indented,
                ReferenceLoopHandling = ReferenceLoopHandling.Serialize,
                PreserveReferencesHandling = PreserveReferencesHandling.Objects
            };

            services.AddIdentity<User, IdentityRole>()
                .AddEntityFrameworkStores<UserContext>().AddErrorDescriber<RussianIdentityErrorDescriber>().AddDefaultTokenProviders(); 

            services.Configure<CookiePolicyOptions>(options =>
            {
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

             

            services.AddDistributedMemoryCache();

            services.AddSession(options =>
            {

                options.IdleTimeout = TimeSpan.FromSeconds(10);
                options.Cookie.HttpOnly = true;

            });

            services.AddJsEngineSwitcher(options => options.DefaultEngineName = ChakraCoreJsEngine.EngineName)
              .AddChakraCore();
         
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddReact();
            
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2).AddSessionStateTempDataProvider();


            var recaptcha = Configuration.GetSection("RecaptchaSettings");
            if (!recaptcha.Exists())
                throw new ArgumentException("Missing RecaptchaSettings in configuration.");
            services.Configure<RecaptchaSettings>(Configuration.GetSection("RecaptchaSettings"));

            services.BuildServiceProvider();

           // services.BuildServiceProvider().GetService<UserContext>().Database.Migrate();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IServiceProvider serviceProvider)
        {
            if (env.IsDevelopment())
            {

                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage(); // Загуглить
              //  app.UseWebpackDevMiddleware(new Microsoft.AspNetCore.SpaServices.Webpack.WebpackDevMiddlewareOptions { HotModuleReplacement = true });

            }
            else
            {

               // app.UseDeveloperExceptionPage();
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts(hsts => hsts.MaxAge(365).IncludeSubdomains().Preload());
          
            }

            app.UseReact(config => {
                config
                   .SetReuseJavaScriptEngines(true)
                   .SetLoadBabel(false)
                   .SetLoadReact(false)
                   .AddScriptWithoutTransform("~/dist/app.js");
            });


            app.UseXContentTypeOptions();
            app.UseReferrerPolicy(opts => opts.NoReferrer());

            /*  app.UseCsp(opts => opts
                                  .BlockAllMixedContent()
                                  .StyleSources(s => s.Self())
                                  .StyleSources(s => s.UnsafeInline())
                                  .FontSources(s => s.Self())
                                  .FormActions(s => s.Self())
                                  .FrameAncestors(s => s.Self())
                                  .ImageSources(s => s.Self())
                                  .ScriptSources(s => s.Self())
                              );*/
            // app.UseHttpsRedirection();

            app.UseStaticFiles();

            app.UseXXssProtection(options => options.EnabledWithBlockMode());
            app.UseXfo(options => options.SameOrigin());
            app.UseRedirectValidation(options =>
                options.AllowedDestinations("http://www.nwebsec.com/", "https://www.google.com/accounts/", "https://oauth.yandex.ru/", "https://accounts.google.com")
               .AllowSameHostRedirectsToHttps());

            app.UseAuthentication();

            app.UseCookiePolicy();

            app.UseStatusCodePages();
        
            app.UseSession();

            app.UseMvc(routes =>
            {
                routes.MapRoute("/Tests/Testing/", "{path?}", new { action = "Testing" });

                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });


            CreateRoles(serviceProvider).Wait();

        }

        private static async Task CreateRoles(IServiceProvider serviceProvider)
        {
            //initializing custom roles   
            var RoleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var UserManager = serviceProvider.GetRequiredService<UserManager<User>>();
            string[] roleNames = { "Admin", "PR", "HR" };
            IdentityResult roleResult;

            foreach (var roleName in roleNames)
            {
                var roleExist = await RoleManager.RoleExistsAsync(roleName);
                if (!roleExist)
                {
                    //create the roles and seed them to the database: Question 1  
                    roleResult = await RoleManager.CreateAsync(new IdentityRole(roleName));
                }
            }

            User user = await UserManager.FindByEmailAsync("jignesh@gmail.com");

            if (user == null)
            {
                user = new User()
                {
                    UserName = "jignesh@gmail.com",
                    Email = "jignesh@gmail.com",
                };
                await UserManager.CreateAsync(user, "Test@123");
            }
            await UserManager.AddToRoleAsync(user, "Admin");


            User user1 = await UserManager.FindByEmailAsync("tejas@gmail.com");

            if (user1 == null)
            {
                user1 = new User()
                {
                    UserName = "tejas@gmail.com",
                    Email = "tejas@gmail.com",
                };
                await UserManager.CreateAsync(user1, "Test@123");
            }
            await UserManager.AddToRoleAsync(user1, "PR");

            User user2 = await UserManager.FindByEmailAsync("rakesh@gmail.com");

            if (user2 == null)
            {
                user2 = new User()
                {
                    UserName = "rakesh@gmail.com",
                    Email = "rakesh@gmail.com",
                };
                await UserManager.CreateAsync(user2, "Test@123");
            }
            await UserManager.AddToRoleAsync(user2, "HR");

        }
    }
}
