using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KVM.LiteDB.DAL.Project;
using KVM.WEB.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace KVM.WEB.Controllers
{
  [Route("[controller]")]
  public class ProjectController : Controller
  {
    public IProject _col;
    private string rootPath;
    public ProjectController([FromServices]IHostingEnvironment env, IProject col)
    {
      _col = col;
      rootPath = env.WebRootPath;
    }

    // GET: /<controller>/
    public IActionResult Index()
    {
      return Json(_col.MenuData());
    }

    [HttpGet("{id}")]
    public JsonResult One(string id)
    {
      return Json(_col.GetOne(id));
    }


    [HttpPost]
    public IActionResult Post(entity.Project data)
    {
      data.Id = Guid.NewGuid().ToString();
      return Json(_col.Insert(data));
    }

    [HttpPut("{id}")]
    public IActionResult Put(string id, entity.Project data)
    { 
      return Json(_col.UpData(id, data));
    }

    [HttpDelete("{id}")]
    public IActionResult Delete(string id)
    {
      FileOperation.DeleteDir($@"{rootPath}/data/project/operator/{id}");
      return Json(_col.Delete(id));
    }
  }
}
