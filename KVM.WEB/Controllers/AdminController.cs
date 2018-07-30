using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KVM.LiteDB.DAL.Admin;
using KVM.LiteDB.DAL.Project;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace KVM.WEB.Controllers
{
  public class AdminController : Controller
  {
    private IAdmin _col;
    public IProject _projectCol;
    private string rootPath;
    public AdminController([FromServices]IHostingEnvironment env, IAdmin col, IProject pCol)
    {
      _col = col;
      _projectCol = pCol;
      rootPath = env.WebRootPath;
    }
    // GET: /<controller>/
    [HttpGet("admin")]
    public IActionResult Index()
    {
      return Json(_col.GetAll().Count());
    }

    [HttpPost("admin/new")]
    public IActionResult New(entity.Admin admin)
    {
      admin.Id = Guid.NewGuid().ToString();
      return Json(_col.Insert(admin));
    }

    [HttpPost("admin/login")]
    public IActionResult Login(entity.Admin admin)
    {
      return Json(_col.Login(admin));
    }
    [HttpPost("user/login/{id}")]
    public IActionResult UserLogin(entity.LoginData data, string id)
    {
      return Json(_projectCol.UserLogin(data, id));
    }
  }
}
