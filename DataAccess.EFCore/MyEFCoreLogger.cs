using Microsoft.EntityFrameworkCore.Diagnostics;
using NLog;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;
using Newtonsoft.Json;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;

namespace DataAccess.EFCore
{
    public class MyEFCoreLogger
    {
        public static readonly LoggerFactory MyLoggerFactory
            = new LoggerFactory(new[] { new NLogLoggerProvider() });
    }
}
