using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Grpc.Core;

namespace Built.Grpc
{
    public class GrpcPool
    {
        private readonly IEnumerable<string> _scopes;

        // TODO: See if we could use ConcurrentDictionary instead of locking. I suspect the issue would be making an atomic
        // "clear and fetch values" for shutdown.
        private readonly Dictionary<ServiceEndpoint, Channel> _channels = new Dictionary<ServiceEndpoint, Channel>();

        private readonly object _lock = new object();

        /// <summary>
        /// Creates a channel pool which will apply the specified scopes to the default application credentials
        /// if they require any.
        /// </summary>
        /// <param name="scopes">The scopes to apply. Must not be null, and must not contain null references. May be empty.</param>
        public GrpcPool(IEnumerable<string> scopes)
        {
            // Always take a copy of the provided scopes, then check the copy doesn't contain any nulls.
            _scopes = GaxPreconditions.CheckNotNull(scopes, nameof(scopes)).ToList();
            GaxPreconditions.CheckArgument(!_scopes.Any(x => x == null), nameof(scopes), "Scopes must not contain any null references");
            // In theory, we don't actually need to store the scopes as field in this class. We could capture a local variable here.
            // However, it won't be any more efficient, and having the scopes easily available when debugging could be handy.
        }

        /// <summary>
        /// Shuts down all the currently-allocated channels asynchronously. This does not prevent the channel
        /// pool from being used later on, but the currently-allocated channels will not be reused.
        /// </summary>
        /// <returns></returns>
        public Task ShutdownChannelsAsync()
        {
            try
            {
                List<Channel> channelsToShutdown;
                lock (_lock)
                {
                    channelsToShutdown = _channels.Values.ToList();
                    _channels.Clear();
                }
                var shutdownTasks = channelsToShutdown.Select(c => c.ShutdownAsync());
                return Task.WhenAll(shutdownTasks);
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Returns a channel from this pool, creating a new one if there is no channel
        /// already associated with <paramref name="endpoint"/>.
        /// </summary>
        /// <param name="endpoint">The endpoint to connect to. Must not be null.</param>
        /// <returns>A channel for the specified endpoint.</returns>
        public Channel GetChannel(ServiceEndpoint endpoint)
        {
            try
            {
                GaxPreconditions.CheckNotNull(endpoint, nameof(endpoint));
                return GetChannelFromDict(endpoint, ChannelCredentials.Insecure);
            }
            catch (AggregateException e)
            {
                // Unwrap the first exception, a bit like await would.
                // It's very unlikely that we'd ever see an AggregateException without an inner exceptions,
                // but let's handle it relatively gracefully.
                throw e.InnerExceptions.FirstOrDefault() ?? e;
            }
        }

        public Channel GetChannel(ServiceEndpoint endpoint, ChannelCredentials credentials)
        {
            try
            {
                GaxPreconditions.CheckNotNull(endpoint, nameof(endpoint));
                return GetChannelFromDict(endpoint, credentials);
            }
            catch (AggregateException e)
            {
                // Unwrap the first exception, a bit like await would.
                // It's very unlikely that we'd ever see an AggregateException without an inner exceptions,
                // but let's handle it relatively gracefully.
                throw e.InnerExceptions.FirstOrDefault() ?? e;
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="endpoint"></param>
        /// <param name="credentials"></param>
        /// <returns></returns>
        public async Task<Channel> GetChannelAsync(ServiceEndpoint endpoint, ChannelCredentials credentials)
        {
            try
            {
                return await Task.Run(() =>
                {
                    GaxPreconditions.CheckNotNull(endpoint, nameof(endpoint));
                    GaxPreconditions.CheckNotNull(credentials, nameof(credentials));
                    return GetChannelFromDict(endpoint, credentials);
                });
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<Channel> GetChannelAsync(ServiceEndpoint endpoint)
        {
            try
            {
                return await Task.Run(() =>
                {
                    GaxPreconditions.CheckNotNull(endpoint, nameof(endpoint));
                    return GetChannelFromDict(endpoint, ChannelCredentials.Insecure);
                });
            }
            catch (Exception)
            {
                throw;
            }
        }

        private Channel GetChannelFromDict(ServiceEndpoint endpoint, ChannelCredentials credentials)
        {
            try
            {
                lock (_lock)
                {
                    Channel channel;
                    if (!_channels.TryGetValue(endpoint, out channel))
                    {
                        channel = new Channel(endpoint.Host, endpoint.Port, credentials);
                        _channels[endpoint] = channel;
                    }

                    return channel;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}