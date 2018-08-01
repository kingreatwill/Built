using Grpc.Core;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;

namespace Built.Grpc
{
    public class TimeoutMiddlewareOptions : IOptions<TimeoutMiddlewareOptions>
    {
        public TimeoutMiddlewareOptions()
        {
            TimoutMilliseconds = 60000;
        }

        //  public TimeoutMiddlewareOptions Value => throw new System.NotImplementedException();
        public int TimoutMilliseconds { get; set; }

        TimeoutMiddlewareOptions IOptions<TimeoutMiddlewareOptions>.Value
        {
            get
            {
                return this;
            }
        }
    }

    public class TimeoutMiddleware
    {
        private PipelineDelagate _next;
        private const string TIMEOUT_KEY = "grpc-timeout";
        private TimeoutMiddlewareOptions _options = new TimeoutMiddlewareOptions();

        public TimeoutMiddleware(PipelineDelagate next)
        {
            _next = next;
        }

        public TimeoutMiddleware(PipelineDelagate next, TimeoutMiddlewareOptions options)
        {
            _next = next;
            _options = options;
        }

        public async Task Invoke(MiddlewareContext context)
        {
            if (context.Options.Headers == null)
                context.Options = context.Options.WithHeaders(new Metadata());

            if (!context.Options.Deadline.HasValue)
            {
                context.Options.Headers.Add(TIMEOUT_KEY, $"{_options.TimoutMilliseconds}m");
            }
            await _next(context);
            InnerLogger.Log(LoggerLevel.Info, $"{context.Method.FullName} :End");
        }
    }
}