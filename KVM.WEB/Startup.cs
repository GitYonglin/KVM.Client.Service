using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using KVM.LiteDB;
using KVM.LiteDB.DAL;
using KVM.LiteDB.DAL.Admin;
using KVM.LiteDB.DAL.Component;
using KVM.LiteDB.DAL.Component.Hole;
using KVM.LiteDB.DAL.Device;
using KVM.LiteDB.DAL.Project;
using KVM.LiteDB.DAL.Project.Operator;
using KVM.LiteDB.DAL.Project.SteelStrand;
using KVM.LiteDB.DAL.Project.Supervision;
using KVM.LiteDB.DAL.Task;
using KVM.LiteDB.DAL.Task.HoleGroup;
using KVM.WEB.Hubs;
using KVM.WEB.Models;
using LitJson;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace KVM.WEB
{
  public class Startup
  {
    private string rootPath;
    public Startup(IConfiguration configuration, [FromServices]IHostingEnvironment env)
    {
      Configuration = configuration;
      rootPath = env.WebRootPath;
    }

    public IConfiguration Configuration { get; }

    // This method gets called by the runtime. Use this method to add services to the container.
    public void ConfigureServices(IServiceCollection services)
    {
      services.Configure<CookiePolicyOptions>(options =>
      {
        // This lambda determines whether user consent for non-essential cookies is needed for a given request.
        options.CheckConsentNeeded = context => true;
        options.MinimumSameSitePolicy = SameSiteMode.None;
      });
      services.Configure<DbString>(options =>
      {
        //DbSetting skillArray = JsonMapper.ToObject<DbSetting>(File.ReadAllText($@"{rootPath}\test.txt"));//使用这种方法，泛型类的字段，属性必须为public，而且字段名，顺序必须与json文件中对应
        options.ConnectionString = $@"{rootPath}\data\{Configuration.GetSection("LiteDB:ConnectionString").Value}";
      });
      services.AddScoped<IAdmin, RAdmin>();
      services.AddScoped<IProject, RProject>();
      services.AddScoped<IOperator, ROperator>();
      services.AddScoped<ISupervision, RSupervision>();
      services.AddScoped<ISteelStrand, RSteelStrand>();
      services.AddScoped<IComponent, RComponent>();
      services.AddScoped<IHole, RHole>();
      services.AddScoped<IDevice, RDevice>();
      services.AddScoped<ITask, RTask>();
      services.AddScoped<IHoleGroup, RHoleGroup>();

      services.AddCors();

      services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
      services.AddSignalR();
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IHostingEnvironment env, IServiceProvider serviceProvider)
    {
      if (env.IsDevelopment())
      {
        app.UseDeveloperExceptionPage();
      }
      else
      {
        app.UseExceptionHandler("/Home/Error");
      }

      app.UseCors(builder => builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader().AllowCredentials());


      app.UseStaticFiles();
      app.UseCookiePolicy();

      app.UseSignalR(s => s.MapHub<PLCHub>("/PLC"));

      app.UseMvc(routes =>
      {
        routes.MapRoute(
                  name: "default",
                  template: "{controller=Home}/{action=Index}/{id?}");
      });
    }
  }

}
