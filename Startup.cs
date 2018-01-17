using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using IdentityServer.Models;
using IdentityServer4.AccessTokenValidation;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Mappers;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace IdentityServer
{
    public class Startup
    {
        public IConfiguration Configuration { get; }
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        public void ConfigureServices(IServiceCollection services)
        {
            //Preciso estudar melhor sobre o método
            //Só é possível fazer injeção de dependencia inserindo este método, sem ele a classe UserController
            services.AddIdentity<ApplicationUser, ApplicationRole>(options =>
                {
                    //options.Cookies.ApplicationCookie.AutomaticChallenge = false;
                    //Requer que cada Usuário tenha um e-mail exclusivo
                    // options.User.RequireUniqueEmail = false;
                })
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();//Adiciona funcionalidade padrao tipo reset password e outras caracteristicas

            //Define uma conexão com o banco de dados - Utiliza Postgres - Para o método UseNpgsql() foi utilizado a Biblioteca Npgsql.EntityFrameworkCore.PostgreSQL
            //As configurações estão dentro do arquivo appsettings.json com o objeto DefaultConnection
            services.AddDbContext<ApplicationDbContext>(opt =>
                opt.UseNpgsql(Configuration.GetConnectionString("DefaultConnection")));

            //services.AddMvcCore()
            //          .AddAuthorization()
            //        .AddJsonFormatters();

            //Define o esquema de autenticação.
            //Define qual servidor de Identidade a API pode aceitar TOKEN
            //As configurações estão dentro do arquivo appsettings.json com o objeto IdentityServerAuthority
             /* */
            services.AddAuthentication(option =>
                {
                    //option.DefaultScheme = IdentityServerAuthenticationDefaults.AuthenticationScheme;
                   // option.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                  //  option.DefaultAuthenticateScheme =CookieAuthenticationDefaults.AuthenticationScheme;
                    //option.DefaultSignInScheme = JwtBearerDefaults.AuthenticationScheme;
                    //option.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                })
               .AddIdentityServerAuthentication(options =>
                {
                    
                    options.SaveToken = true;
                    Configuration.Bind("IdentityServerAuthority", options);
                });


            // services.AddTransient<IEmailSender, EmailSender>();

            //Adiciona um endereço de origem (Nossa Aplicação Angular) para fazer requisições nesta API
            services.AddCors(options =>
            {
                // this defines a CORS policy called "AllowSpecificOrigin"
                options.AddPolicy("AllowSpecificOrigin", policy =>
                {
                    policy.WithOrigins("http://localhost:4200")
                        .AllowAnyHeader()
                        .AllowAnyMethod();
                });
            });

services.AddMvc(options =>
        {
            // The default policy is to make sure that both authentication schemes - Cookie and Jwt - are challenged
            var defaultPolicy = new AuthorizationPolicyBuilder(IdentityConstants.ApplicationScheme, "Bearer")
                .RequireAssertion(c => true) // A requirement is mandatory
                .Build();
            options.Filters.Add(new AuthorizeFilter(defaultPolicy));
        });
// services.AddMvc();
            // services.AddMvc(options => { var defaultPolicy = 
            // new AuthorizationPolicyBuilder( new[] { JwtBearerDefaults.AuthenticationScheme, IdentityConstants.ApplicationScheme })
            // .RequireAuthenticatedUser().Build(); options.Filters.Add(new AuthorizeFilter(defaultPolicy));});
            
            services.AddIdentityServer(option =>
                {
                    // options.Events.RaiseErrorEvents = true;
                    // options.Events.RaiseInformationEvents = true;
                    // options.Events.RaiseFailureEvents = true;
                    // options.Events.RaiseSuccessEvents = true;
                })
            .AddDeveloperSigningCredential() //Deve ser substituido
            .AddInMemoryApiResources(Config.GetApiResources())
            .AddInMemoryIdentityResources(Config.GetIdentityResources())
            .AddInMemoryClients(Config.GetClients())
            //.AddTestUsers(Config.GetUsers())
            .AddAspNetIdentity<ApplicationUser>()
             .AddJwtBearerClientAuthentication();
            
        }


        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
      if (env.IsDevelopment())
    {
        app.UseDeveloperExceptionPage();
        app.UseBrowserLink();
        app.UseDatabaseErrorPage();
    }
    else
    {
        app.UseExceptionHandler("/Home/Error");
    }
            app.UseCors("AllowSpecificOrigin");
            //adiciona o middleware IdentityServer ao pipeline HTTP:
            //permite que IdentityServer comece a interceptar rotas e processar pedidos
            app.UseIdentityServer();
            app.UseStaticFiles();
            app.UseMvc(routes =>
{
    routes.MapRoute(
        name: "default",
        template: "{controller=Home}/{action=Index}/{id?}");
});
        }
    }
}
