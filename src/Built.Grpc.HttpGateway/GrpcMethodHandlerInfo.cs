using Grpc.Core;
using System;
using System.Reflection;

namespace Built.Grpc.HttpGateway
{
    /// <summary>
    /// GrpcMethodHandlerInfo
    /// </summary>
    public class GrpcMethodHandlerInfo
    {
        public GrpcMethodHandlerInfo(MethodType methodType, Type requestType, Type responseType, MethodInfo handler)
        {
            m_MethodType = methodType;
            m_RequestType = requestType;
            m_ResponseType = responseType;
            m_Handler = handler;
        }

        public MethodType MethodType
        {
            get { return m_MethodType; }
        }

        private MethodType m_MethodType;

        public Type RequestType
        {
            get { return m_RequestType; }
        }

        private Type m_RequestType;

        public Type ResponseType
        {
            get { return m_ResponseType; }
        }

        private Type m_ResponseType;

        public MethodInfo Handler
        {
            get { return m_Handler; }
        }

        private MethodInfo m_Handler;
    }
}