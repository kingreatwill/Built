using Built.Grpc.HttpGateway.Swagger;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Built.Grpc.HttpGateway
{
    public class SwaggerMiddleware
    {
        private static readonly Task EmptyTask = Task.FromResult(0);

        private readonly RequestDelegate _next;

        //private readonly IReadOnlyList<GrpcMethodHandlerInfo> handlers;
        private readonly SwaggerOptions options;

        public SwaggerMiddleware(RequestDelegate next, SwaggerOptions options)
        {
            this._next = next;
            this.options = options;
        }

        public async Task Invoke(HttpContext httpContext)
        {
            if (httpContext.Request.Path.Value.Equals(options.JsonName))
            {
                var builder = new SwaggerDefinitionBuilder(options, httpContext, GrpcServiceMethodFactory.Handers);
                var bytes = builder.BuildSwaggerJson();
                httpContext.Response.Headers["Content-Type"] = new[] { "application/json" };
                httpContext.Response.StatusCode = 200;
                httpContext.Response.Body.Write(bytes, 0, bytes.Length);
                return;
            }
            else
            {
                if (_next != null) { await _next(httpContext); }
                return;
            }
            //// reference embedded resouces
            //const string prefix = "Built.Grpc.HttpGateway.swagger.ui_3._18._1.";
            ////Type type = MethodBase.GetCurrentMethod().DeclaringType;
            ////string _namespace = type.Namespace;

            //var path = httpContext.Request.Path.Value.Trim('/');
            //if (path == "") path = "index.html";
            //var filePath = prefix + path.Replace("/", ".");
            //var mediaType = GetMediaType(filePath);

            //var myAssembly = typeof(SwaggerMiddleware).GetTypeInfo().Assembly;
            //using (var stream = myAssembly.GetManifestResourceStream(filePath))
            //{
            //    if (options.ResolveCustomResource == null)
            //    {
            //        if (stream == null)
            //        {
            //            // not found, standard request.
            //            return _next(httpContext);
            //        }

            //        httpContext.Response.Headers["Content-Type"] = new[] { mediaType };
            //        httpContext.Response.StatusCode = 200;
            //        var response = httpContext.Response.Body;
            //        stream.CopyTo(response);
            //    }
            //    else
            //    {
            //        byte[] bytes;
            //        if (stream == null)
            //        {
            //            bytes = options.ResolveCustomResource(path, null);
            //        }
            //        else
            //        {
            //            using (var ms = new MemoryStream())
            //            {
            //                stream.CopyTo(ms);
            //                bytes = options.ResolveCustomResource(path, ms.ToArray());
            //            }
            //        }

            //        if (bytes == null)
            //        {
            //            // not found, standard request.
            //            return _next(httpContext);
            //        }

            //        httpContext.Response.Headers["Content-Type"] = new[] { mediaType };
            //        httpContext.Response.StatusCode = 200;
            //        var response = httpContext.Response.Body;
            //        response.Write(bytes, 0, bytes.Length);
            //    }
            //}

            //return EmptyTask;
        }

        private static string GetMediaType(string path)
        {
            var extension = path.Split('.').Last();

            switch (extension)
            {
                case "css":
                    return "text/css";

                case "js":
                    return "text/javascript";

                case "json":
                    return "application/json";

                case "gif":
                    return "image/gif";

                case "png":
                    return "image/png";

                case "eot":
                    return "application/vnd.ms-fontobject";

                case "woff":
                    return "application/font-woff";

                case "woff2":
                    return "application/font-woff2";

                case "otf":
                    return "application/font-sfnt";

                case "ttf":
                    return "application/font-sfnt";

                case "svg":
                    return "image/svg+xml";

                case "ico":
                    return "image/x-icon";

                default:
                    return "text/html";
            }
        }
    }
}