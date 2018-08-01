using System.Threading.Tasks;

namespace Built.Grpc
{
    public class TimerMiddleware
    {
        private PipelineDelagate _next;

        public TimerMiddleware(PipelineDelagate next)
        {
            _next = next;
        }

        public async Task Invoke(MiddlewareContext context)
        {
            System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
            stopwatch.Start();
            await _next(context);
            stopwatch.Stop();
            InnerLogger.Log(LoggerLevel.Info, $"TimerMiddleware:{stopwatch.ElapsedMilliseconds}");
        }
    }
}