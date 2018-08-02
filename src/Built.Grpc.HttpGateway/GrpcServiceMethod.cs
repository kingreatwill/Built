using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Grpc.Core;

namespace Built.Grpc.HttpGateway
{
    /// <summary>
    /// GrpcServiceMethod
    /// </summary>
    public class GrpcServiceMethod
    {
        public GrpcServiceMethod(IMethod method, Type requestType, Type responseType)
        {
            m_Method = method;
            m_RequestType = requestType;
            m_ResponseType = responseType;
        }

        /// <summary>
        ///
        /// </summary>
        public IMethod Method
        {
            get { return m_Method; }
        }

        private IMethod m_Method;

        /// <summary>
        ///
        /// </summary>
        public Type RequestType
        {
            get { return m_RequestType; }
        }

        private Type m_RequestType;

        /// <summary>
        ///
        /// </summary>
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

        public string GetHashString()
        {
            return this.Method.FullName;
        }
    }
}