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
        public GrpcMethodHandlerInfo(string serviceName, MethodType methodType, Type requestType, Type responseType, MethodInfo handler, IGrpcMarshallerFactory marshallerFactory)
        {
            m_MethodType = methodType;
            m_RequestType = requestType;
            m_ResponseType = responseType;
            m_Handler = handler;
            m_ServiceName = serviceName;
            m_Method = GrpcReflection.CreateMethod(serviceName, this, marshallerFactory);
            // var srvMethod = new GrpcServiceMethod(method, requestType, responseType);
        }

        /// <summary>
        ///
        /// </summary>
        public IMethod Method
        {
            get { return m_Method; }
        }

        private IMethod m_Method;

        public string ServiceName
        {
            get { return m_ServiceName; }
        }

        private string m_ServiceName;

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

        internal Type GetJsonRequestType()
        {
            if (m_RequestType == null) { return null; }

            switch (Method.Type)
            {
                case MethodType.ClientStreaming:
                case MethodType.DuplexStreaming:
                    return m_RequestType.MakeArrayType();

                default:
                    return m_RequestType;
            }
        }

        public MethodInfo Handler
        {
            get { return m_Handler; }
        }

        private MethodInfo m_Handler;

        public string GetHashString()
        {
            return this.Method.FullName;
        }
    }
}