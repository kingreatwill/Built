using Grpc.Core;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;

namespace Built.Grpc.HttpGateway
{
    /// <summary>
    /// GrpcReflection
    /// </summary>
    public static class GrpcReflection
    {
        public static IEnumerable<GrpcMethodHandlerInfo> EnumerateServiceMethods(string serviceName, Type serviceImplType, IGrpcMarshallerFactory marshallerFactory)
        {
            foreach (MethodInfo method in serviceImplType.GetMethods(BindingFlags.Public | BindingFlags.Instance))
            {
                if (IsIgnore(method)) { continue; }

                if (!TryGetServiceMethodInfo(method, out MethodType methodType, out Type requestType, out Type responseType)) { continue; }

                yield return new GrpcMethodHandlerInfo(serviceName, methodType, requestType, responseType, method, marshallerFactory);
            }
        }

        private static bool IsIgnore(MethodInfo methodImpl)
        {
            GrpcIgnoreAttribute attr = methodImpl.GetCustomAttribute<GrpcIgnoreAttribute>(false);

            return (attr != null);
        }

        private static bool TryGetServiceMethodInfo(MethodInfo methodImpl, out MethodType methodType, out Type requestType, out Type responseType)
        {
            if (methodImpl.IsPublic && !methodImpl.IsStatic)
            {
                if (IsUnaryMathod(methodImpl, out requestType, out responseType))
                {
                    methodType = MethodType.Unary;
                    return true;
                }
                else if (IsClientStreamingMathod(methodImpl, out requestType, out responseType))
                {
                    methodType = MethodType.ClientStreaming;
                    return true;
                }
                else if (IsServerStreamingMathod(methodImpl, out requestType, out responseType))
                {
                    methodType = MethodType.ServerStreaming;
                    return true;
                }
                else if (IsDuplexStreamingMathod(methodImpl, out requestType, out responseType))
                {
                    methodType = MethodType.DuplexStreaming;
                    return true;
                }
            }

            methodType = default(MethodType);
            responseType = null;
            requestType = null;

            return false;
        }

        private static bool IsUnaryMathod(MethodInfo methodImpl, out Type requestType, out Type responseType)
        {
            //Task<TResponse> Method(TRequest request, ServerCallContext context)

            requestType = null;
            responseType = null;

            if (!methodImpl.ReturnType.IsGenericType) { return false; }
            if (methodImpl.ReturnType.GetGenericTypeDefinition() != typeof(Task<>)) { return false; }

            ParameterInfo[] parameters = methodImpl.GetParameters();

            if (parameters.Length != 2) { return false; }

            if (IsRequestStreamType(parameters[0].ParameterType)) { return false; }
            if (parameters[1].ParameterType != typeof(ServerCallContext)) { return false; }

            requestType = parameters[0].ParameterType;
            responseType = methodImpl.ReturnType.GetGenericArguments()[0];

            return true;
        }

        private static bool IsServerStreamingMathod(MethodInfo methodImpl, out Type requestType, out Type responseType)
        {
            //Task Method(TRequest request, IServerStreamWriter<TResponse> responseStream, ServerCallContext context)

            requestType = null;
            responseType = null;

            if (methodImpl.ReturnType != typeof(Task)) { return false; }

            ParameterInfo[] parameters = methodImpl.GetParameters();

            if (parameters.Length != 3) { return false; }

            if (IsRequestStreamType(parameters[0].ParameterType)) { return false; }
            if (!IsResponseStreamType(parameters[1].ParameterType)) { return false; }
            if (parameters[2].ParameterType != typeof(ServerCallContext)) { return false; }

            requestType = parameters[0].ParameterType;
            responseType = parameters[1].ParameterType.GetGenericArguments()[0];

            return true;
        }

        private static bool IsClientStreamingMathod(MethodInfo methodImpl, out Type requestType, out Type responseType)
        {
            //Task<TResponse> Method(IAsyncStreamReader<TRequest> requestStream, ServerCallContext context)

            requestType = null;
            responseType = null;

            if (!methodImpl.ReturnType.IsGenericType) { return false; }
            if (methodImpl.ReturnType.GetGenericTypeDefinition() != typeof(Task<>)) { return false; }

            ParameterInfo[] parameters = methodImpl.GetParameters();

            if (parameters.Length != 2) { return false; }

            if (!IsRequestStreamType(parameters[0].ParameterType)) { return false; }
            if (parameters[1].ParameterType != typeof(ServerCallContext)) { return false; }

            requestType = parameters[0].ParameterType.GetGenericArguments()[0];
            responseType = methodImpl.ReturnType.GetGenericArguments()[0];

            return true;
        }

        private static bool IsDuplexStreamingMathod(MethodInfo methodImpl, out Type requestType, out Type responseType)
        {
            //Task Method(IAsyncStreamReader<TRequest> requestStream, IServerStreamWriter<TResponse> responseStream, ServerCallContext context)

            requestType = null;
            responseType = null;

            if (methodImpl.ReturnType != typeof(Task)) { return false; }

            ParameterInfo[] parameters = methodImpl.GetParameters();

            if (parameters.Length != 3) { return false; }

            if (!IsRequestStreamType(parameters[0].ParameterType)) { return false; }
            if (!IsResponseStreamType(parameters[1].ParameterType)) { return false; }
            if (parameters[2].ParameterType != typeof(ServerCallContext)) { return false; }

            requestType = parameters[0].ParameterType.GetGenericArguments()[0];
            responseType = parameters[1].ParameterType.GetGenericArguments()[0];

            return true;
        }

        private static bool IsRequestStreamType(Type t)
        {
            return (t.IsGenericType && t.GetGenericTypeDefinition() == typeof(IAsyncStreamReader<>));
        }

        private static bool IsResponseStreamType(Type t)
        {
            return (t.IsGenericType && t.GetGenericTypeDefinition() == typeof(IServerStreamWriter<>));
        }

        public static IMethod CreateMethod(string serviceName, GrpcMethodHandlerInfo handler, IGrpcMarshallerFactory marshallerFactory)
        {
            MethodInfo m = typeof(GrpcReflection).GetMethod("CreateMethodCore", BindingFlags.Static | BindingFlags.NonPublic);

            return (IMethod)m.MakeGenericMethod(new Type[] { handler.RequestType, handler.ResponseType }).Invoke(null, new object[] { serviceName, handler, marshallerFactory });
        }

        private static Method<TRequest, TResponse> CreateMethodCore<TRequest, TResponse>(string serviceName, GrpcMethodHandlerInfo handler, IGrpcMarshallerFactory marshallerFactory)
        {
            return new Method<TRequest, TResponse>(handler.MethodType, serviceName, handler.Handler.Name, marshallerFactory.GetMarshaller<TRequest>(), marshallerFactory.GetMarshaller<TResponse>());
        }
    }
}