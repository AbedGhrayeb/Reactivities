using API.Middleware;
using Application.Activities;
using Application.Interfaces;
using Application.MappingProfile;
using AutoMapper;
using Domain;
using Infrastructure.Photos;
using Infrastructure.Security;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Persistence;
using System.Text;

namespace API
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
            //add dbcontext
            services.AddDbContext<DataContext>(opt=>{
                opt.UseSqlite(Configuration.GetConnectionString("DefaultConnection"));
               opt.UseLazyLoadingProxies();
            });
            //Add Cors
            services.AddCors(options=>{
                options.AddPolicy("CorsPolicy",policy=>{
                    policy.AllowAnyHeader()
                    .AllowAnyMethod()
                    .WithOrigins("http://localhost:3000");
                });
            });
            //add mediarR
            services.AddMediatR(typeof(List.Handler).Assembly);
            //add automapper config
            var mappingConfig = new MapperConfiguration(map =>
                 map.AddProfile(new MappingProfile()));
            services.AddSingleton(mappingConfig.CreateMapper());
            //services.AddAutoMapper(typeof(List.Handler));

            //add Ideintity
            var builder = services.AddDefaultIdentity<AppUser>(opt=> 
            {
                opt.Password.RequiredLength = 6;
                opt.Password.RequireLowercase = false;
                opt.Password.RequireUppercase = false;
                opt.Password.RequireNonAlphanumeric = false;
                
            });
            var IdentityBuilder = new IdentityBuilder(builder.UserType,builder.Services);
            IdentityBuilder.AddEntityFrameworkStores<DataContext>();
            IdentityBuilder.AddSignInManager<SignInManager<AppUser>>();

            services.Configure<CloudinarySettings>(Configuration.GetSection("Cloudinary"));
            services.AddScoped<IUserAccessor, UserAccessor>();
            services.AddScoped<IPhotoAccessor,PhotoAccessor>();
            //Token
            services.AddScoped<IJwtJenerator, JwtJenerator>();

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("7592d6c3-fe20-488b-8761-1f5fe317a96c"));
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = key,
                        ValidateAudience = false,
                        ValidateIssuer = false
                    };
                });


            services.AddControllers(options=>
            {
                var policy = new AuthorizationPolicyBuilder()
                .RequireAuthenticatedUser().Build();
                options.Filters.Add(new AuthorizeFilter(policy));
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseMiddleware<ErrorHandlingMeddleware>();
            //if (env.IsDevelopment())
            //{
            //    //app.UseDeveloperExceptionPage();
            //}

            //app.UseHttpsRedirection();

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseCors("CorsPolicy");
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
