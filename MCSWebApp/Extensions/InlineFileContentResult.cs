using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Threading.Tasks;

namespace WebApp.Extensions
{
    public class InlineFileContentResult : FileContentResult
    {
        public InlineFileContentResult(byte[] fileContents, string contentType)
            : base(fileContents, contentType)
        {
        }

        public override Task ExecuteResultAsync(ActionContext context)
        {
            var contentDispositionHeader = new ContentDisposition
            {
                FileName = FileDownloadName,
                Inline = true,
            };
            context.HttpContext.Response.Headers.Add("Content-Disposition", contentDispositionHeader.ToString());
            FileDownloadName = null;
            return base.ExecuteResultAsync(context);
        }
    }
}
