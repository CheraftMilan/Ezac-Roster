using Ezac.Roster.Domain.Entities;
using Ezac.Roster.Domain.Interfaces.Repositories;
using Ezac.Roster.Infrastructure.Data;
using Ezac.Roster.Domain.Interfaces.Services;
using Ezac.Roster.Domain.Services;
using Ezac.Roster.Infrastructure.Interfaces;
using Ezac.Roster.Infrastructure.Repositories;
using Ezac.Roster.Web.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MudBlazor.Services;
using Ezac.Roster.Web.Components.Management;
using Microsoft.AspNetCore.Components;

namespace Ezac.Roster.Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddDbContext<EzacDbContext>
               (options => options
               .UseSqlServer(builder.Configuration.GetConnectionString("DefaultDb")));
            // Add services to the container.
            builder.Services.AddMudServices();
            builder.Services.AddRazorComponents()
                .AddInteractiveServerComponents();
            builder.Services.AddScoped<ICalendarRepository, CalendarRepository>();
            builder.Services.AddScoped<IDayRepository, DayRepository>();
            builder.Services.AddScoped<IPartOfDayRepository, PartOfDayRepository>();
            builder.Services.AddScoped<IMemberRepository, MemberRepository>();
            builder.Services.AddScoped<IPreferenceRepository, PreferenceRepository>();
            builder.Services.AddScoped<IServiceRepository, ServiceRepository>();
            builder.Services.AddScoped<IPlannedServiceRepository, PlannedServiceRepository>();
            
            builder.Services.AddScoped<IDayRepository, DayRepository>();
            builder.Services.AddScoped<IServiceRepository, ServiceRepository>();
            builder.Services.AddScoped<IQualificationRepository, QualificationRepository>();

            //Services
            builder.Services.AddScoped<ICalendarService, CalendarService>();
            builder.Services.AddScoped<IServiceService, ServiceService>();
            builder.Services.AddScoped<IDayService, DayService>();
            builder.Services.AddScoped<IPartOfDayService, PartOfDayService>();
            builder.Services.AddScoped<IPreferenceService, PreferenceService>();
            builder.Services.AddScoped<IServiceService, ServiceService>();
            builder.Services.AddScoped<IPlannedServiceService, PlannedServices>();
            builder.Services.AddScoped<IMemberService, MemberService>();
            builder.Services.AddScoped<IGenerateService, GeneratorService>();
            builder.Services.AddScoped<IEmailService, EmailService>();
            // Add services for Razor Pages and components
            builder.Services.AddRazorPages();
            builder.Services.AddServerSideBlazor();
       
            builder.Services.AddRazorComponents()
                .AddInteractiveServerComponents();
            builder.Services.AddScoped<IQualificationService, QualificationService>();
            builder.Services.AddScoped<IExcelService, ExcelService>();
            builder.Services.AddCascadingAuthenticationState();
            builder.Services.AddIdentity<AdminUser, IdentityRole>(options =>
            {
                options.SignIn.RequireConfirmedEmail = false;
                options.User.RequireUniqueEmail = false;
                options.Password.RequiredUniqueChars = 0;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireUppercase = false;
                options.Password.RequiredLength = 3;
            }).AddEntityFrameworkStores<EzacDbContext>()
              .AddDefaultTokenProviders();

           
            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseHttpsRedirection();
            app.Use(async (context, next) =>
            {
                var member = new Member();
                var user = context.User;
                var path = context.Request.Path;
                if (!user.Identity.IsAuthenticated && path.Value != "/" && path.Value != "/login" && (path.StartsWithSegments("/memberInput/") == true))
                {
                    if (member.Preferences is null)
                    {
                        context.Response.Redirect("/");
                        return;
                    }
                    context.Response.Redirect("/");
                    return;
                }
                await next();
            });
            app.UseStaticFiles();
            app.UseAntiforgery();
            app.MapRazorComponents<App>()
                .AddInteractiveServerRenderMode();
            
            app.Run();
        }
    }
}
