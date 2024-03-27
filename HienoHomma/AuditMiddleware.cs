using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using log4net;

namespace HienoHomma
{
    public class AuditMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILog _auditLogger;

        public AuditMiddleware(RequestDelegate next, ILog auditLogger)
        {
            log4net.Util.LogLog.InternalDebugging = true;
            _next = next;
            _auditLogger = auditLogger;
        }

        public async Task Invoke(HttpContext context)
        {
            log4net.Util.LogLog.InternalDebugging = true;
            // Log every request to AuditFileAppender
            _auditLogger.Info($"Request to {context.Request.Path} from {context.Connection.RemoteIpAddress}");

            await _next(context);
        }
    }

}
