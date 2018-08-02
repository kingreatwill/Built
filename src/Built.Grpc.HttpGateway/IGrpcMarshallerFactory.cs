using System;
using System.Collections.Generic;
using System.Text;

using Grpc.Core;

namespace Built.Grpc.HttpGateway
{
    /// <summary>
    /// IGrpcMarshallerFactory
    /// </summary>
    public interface IGrpcMarshallerFactory
    {
        /// <summary>
        /// GetMarshaller
        /// </summary>
        Marshaller<T> GetMarshaller<T>();
    }
}