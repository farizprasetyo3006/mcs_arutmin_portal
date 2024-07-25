using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BusinessLogic.Entity;
using MCSWebApp.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using NLog;
using Common;
using DataAccess.EFCore.Repository;
using Microsoft.AspNetCore.Hosting;
using System.Text;
using System.IO;
using Npoi.Mapper;
using Npoi.Mapper.Attributes;
using Microsoft.AspNetCore.Http;

namespace MCSWebApp.Areas.General.Controllers
{
    [Area("General")]
    public class GeneralController : Controller
    {
        public async Task<IActionResult> UploadError()
        {
            if (HttpContext.Session.GetString("errormessage") == null) return null;

            string teks = HttpContext.Session.GetString("errormessage");
            string filename = HttpContext.Session.GetString("filename");
            filename = "Error_" + filename + "_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".txt";

            //HttpContext.Session.Clear();
            HttpContext.Session.Remove("errormessage");
            HttpContext.Session.Remove("filename");

            string FilePath = @"c:\temp\UploadError\";
            if (!Directory.Exists(FilePath)) Directory.CreateDirectory(FilePath);

            FilePath += filename;

            using (StreamWriter sw = System.IO.File.CreateText(FilePath))
            {
                await sw.WriteLineAsync(teks);
            }

            var memory = new MemoryStream();
            using (var stream = new FileStream(FilePath, FileMode.Open))
            {
                await stream.CopyToAsync(memory);
            }
            memory.Position = 0;
            //Throws Generated file to Browser
            try
            {
                return File(memory, "text/plain", filename);
            }
            // Deletes the generated file
            finally
            {
                if (System.IO.File.Exists(FilePath)) System.IO.File.Delete(FilePath);
            }
        }

    }
}
