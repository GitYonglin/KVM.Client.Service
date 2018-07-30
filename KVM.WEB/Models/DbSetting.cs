using KVM.LiteDB;
using LitJson;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using KVM.WEB;

namespace KVM.WEB.Models
{
    public class DbSetting
    {
        public string NowDb { get; set; }
        public List<string> DbList { get; set; }
    }
}
