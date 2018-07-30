using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KVM.LiteDB.DAL.Component.Hole;
using KVM.WEB.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace KVM.WEB.Controllers.component
{
  [Route("[controller]")]
  public class HoleController : Controller
  {
    public IHole _col;
    private string rootPath;
    public HoleController([FromServices]IHostingEnvironment env, IHole col)
    {
      rootPath = env.WebRootPath;
      _col = col;
    }
    // GET: /<controller>/
    public IActionResult Index()
    {
      return Json(_col.GetAll());
    }

    [HttpPost]
    public IActionResult Post(entity.Hole data)
    {
      data.Id = Guid.NewGuid().ToString();
      data.ImgUrl = FileOperation.FileImg(data.ImgFile, data.Id, rootPath, Path(data.ParentId));
      return Json(_col.Insert(data));
    }

    [HttpPut("{id}")]
    public IActionResult Put(string id, entity.Hole data)
    {
      data.ImgUrl = FileOperation.FileImg(data.ImgFile, data.Id, rootPath, Path(data.ParentId));
      return Json(_col.UpData(id, data));
    }

    [HttpDelete("{id}")]
    public IActionResult Delete(string id)
    {
      var delete = _col.GetOne(id);
      FileOperation.DeleteFile(rootPath, delete.ImgUrl);
      return Json(_col.Delete(delete));
    }

    private string Path(string path)
    {
      return $@"data/component/hole/{path}";
    }
  }
}
