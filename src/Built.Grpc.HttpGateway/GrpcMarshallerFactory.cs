using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

using Grpc.Core;

namespace Built.Grpc.HttpGateway
{
    /// <summary>
    /// GrpcMarshallerFactory
    /// </summary>
    public class GrpcMarshallerFactory : IGrpcMarshallerFactory
    {
        #region ctor

        /// <summary>
        /// GrpcMarshallerFactory
        /// </summary>
        public GrpcMarshallerFactory(IGrpcSerializerFactory serializerFactory)
        {
            m_SerializerFactory = serializerFactory;
        }

        #endregion ctor

        #region fields

        /// <summary>
        /// DefaultInstance
        /// </summary>
        public static readonly GrpcMarshallerFactory DefaultInstance = new GrpcMarshallerFactory(null);

        /// <summary>
        /// SerializerFactory
        /// </summary>
        public IGrpcSerializerFactory SerializerFactory
        {
            get { return m_SerializerFactory; }
        }

        private IGrpcSerializerFactory m_SerializerFactory;

        #endregion fields

        #region 实现

        /// <summary>
        /// GetMarshaller
        /// </summary>
        public Marshaller<T> GetMarshaller<T>()
        {
            if (m_TypeMarshallers.TryGetValue(typeof(T), out object obj))
            {
                return (Marshaller<T>)obj;
            }

            if (TryCreateCustomMarshaller<T>(out Marshaller<T> marshaller))
            {
                m_TypeMarshallers.Add(typeof(T), marshaller);
                return marshaller;
            }

            Type t = typeof(Google.Protobuf.IMessage<>).MakeGenericType(new Type[] { typeof(T) });

            if (t.IsAssignableFrom(typeof(T)) && HasDefaultConstructor(typeof(T)))
            {
                obj = typeof(GrpcMarshallerFactory).GetMethod("CreateDefaultMarshaller", BindingFlags.Static | BindingFlags.NonPublic)
                    .MakeGenericMethod(new Type[] { typeof(T) })
                    .Invoke(null, new object[] { });

                m_TypeMarshallers.Add(typeof(T), obj);

                return (Marshaller<T>)obj;
            }

            return null;
        }

        /// <summary>
        /// TryCreateCustomMarshaller
        /// </summary>
        private bool TryCreateCustomMarshaller<T>(out Marshaller<T> marshaller)
        {
            if (m_SerializerFactory == null)
            {
                marshaller = null;
                return false;
            }

            Func<T, byte[]> s = m_SerializerFactory.GetSerializer<T>();
            Func<byte[], T> d = m_SerializerFactory.GetDeserializer<T>();

            if (s == null || d == null)
            {
                marshaller = null;
                return false;
            }

            marshaller = new Marshaller<T>(s, d);
            return true;
        }

        private Dictionary<Type, object> m_TypeMarshallers = new Dictionary<Type, object>();

        private bool HasDefaultConstructor(Type t)
        {
            ConstructorInfo ctor = t.GetConstructor(new Type[] { });

            return (t != null);
        }

        private static Marshaller<T> CreateDefaultMarshaller<T>() where T : Google.Protobuf.IMessage<T>, new()
        {
            Func<T, byte[]> s = delegate (T arg)
            {
                if (arg == null) { return null; }
                return Google.Protobuf.MessageExtensions.ToByteArray((Google.Protobuf.IMessage)arg);
            };

            Func<byte[], T> d = delegate (byte[] data)
            {
                if (data == null) { return default(T); }
                return new global::Google.Protobuf.MessageParser<T>(delegate () { return new T(); }).ParseFrom(data);
            };

            return Marshallers.Create<T>(s, d);
        }

        #endregion 实现
    }
}