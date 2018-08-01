using System.Threading.Tasks;

namespace FM.ConsulInterop
{
    public class LoggingMiddleware
    {
        private PipelineDelagate _next;

        public LoggingMiddleware(PipelineDelagate next)
        {
            _next = next;
        }

        public async Task Invoke(MiddlewareContext context)
        {
            InnerLogger.Log(LoggerLevel.Info, $"{context.Method.FullName} :Before");
            await _next(context);
            InnerLogger.Log(LoggerLevel.Info, $"{context.Method.FullName} :End");
        }
    }
}