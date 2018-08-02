using System;
using System.Collections.Generic;
using System.Text;

namespace Built.Grpc.HttpGateway
{
    /// <summary>
    /// IGrpcSerializerFactory
    /// </summary>
    public interface IGrpcSerializerFactory
    {
        /// <summary>
        /// GetSerializer
        /// </summary>
        Func<T, byte[]> GetSerializer<T>();

        /// <summary>
        /// GetDeserializer
        /// </summary>
        Func<byte[], T> GetDeserializer<T>();
    }
}