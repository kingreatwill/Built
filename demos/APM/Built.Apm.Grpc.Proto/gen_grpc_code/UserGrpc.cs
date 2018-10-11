// <auto-generated>
//     Generated by the protocol buffer compiler.  DO NOT EDIT!
//     source: user.proto
// </auto-generated>
#pragma warning disable 1591
#region Designer generated code

using System;
using System.Threading;
using System.Threading.Tasks;
using grpc = global::Grpc.Core;

namespace User {
  /// <summary>
  /// The service definition.
  /// </summary>
  public static partial class UserService
  {
    static readonly string __ServiceName = "UserDemo.UserService";

    static readonly grpc::Marshaller<global::User.UserGetRequest> __Marshaller_UserGetRequest = grpc::Marshallers.Create((arg) => global::Google.Protobuf.MessageExtensions.ToByteArray(arg), global::User.UserGetRequest.Parser.ParseFrom);
    static readonly grpc::Marshaller<global::User.UserGetResponse> __Marshaller_UserGetResponse = grpc::Marshallers.Create((arg) => global::Google.Protobuf.MessageExtensions.ToByteArray(arg), global::User.UserGetResponse.Parser.ParseFrom);

    static readonly grpc::Method<global::User.UserGetRequest, global::User.UserGetResponse> __Method_Get = new grpc::Method<global::User.UserGetRequest, global::User.UserGetResponse>(
        grpc::MethodType.Unary,
        __ServiceName,
        "Get",
        __Marshaller_UserGetRequest,
        __Marshaller_UserGetResponse);

    static readonly grpc::Method<global::User.UserGetResponse, global::User.UserGetResponse> __Method_Update = new grpc::Method<global::User.UserGetResponse, global::User.UserGetResponse>(
        grpc::MethodType.Unary,
        __ServiceName,
        "Update",
        __Marshaller_UserGetResponse,
        __Marshaller_UserGetResponse);

    /// <summary>Service descriptor</summary>
    public static global::Google.Protobuf.Reflection.ServiceDescriptor Descriptor
    {
      get { return global::User.UserReflection.Descriptor.Services[0]; }
    }

    /// <summary>Base class for server-side implementations of UserService</summary>
    public abstract partial class UserServiceBase
    {
      /// <summary>
      /// Sends a greeting
      /// </summary>
      /// <param name="request">The request received from the client.</param>
      /// <param name="context">The context of the server-side call handler being invoked.</param>
      /// <returns>The response to send back to the client (wrapped by a task).</returns>
      public virtual global::System.Threading.Tasks.Task<global::User.UserGetResponse> Get(global::User.UserGetRequest request, grpc::ServerCallContext context)
      {
        throw new grpc::RpcException(new grpc::Status(grpc::StatusCode.Unimplemented, ""));
      }

      public virtual global::System.Threading.Tasks.Task<global::User.UserGetResponse> Update(global::User.UserGetResponse request, grpc::ServerCallContext context)
      {
        throw new grpc::RpcException(new grpc::Status(grpc::StatusCode.Unimplemented, ""));
      }

    }

    /// <summary>Client for UserService</summary>
    public partial class UserServiceClient : grpc::ClientBase<UserServiceClient>
    {
      /// <summary>Creates a new client for UserService</summary>
      /// <param name="channel">The channel to use to make remote calls.</param>
      public UserServiceClient(grpc::Channel channel) : base(channel)
      {
      }
      /// <summary>Creates a new client for UserService that uses a custom <c>CallInvoker</c>.</summary>
      /// <param name="callInvoker">The callInvoker to use to make remote calls.</param>
      public UserServiceClient(grpc::CallInvoker callInvoker) : base(callInvoker)
      {
      }
      /// <summary>Protected parameterless constructor to allow creation of test doubles.</summary>
      protected UserServiceClient() : base()
      {
      }
      /// <summary>Protected constructor to allow creation of configured clients.</summary>
      /// <param name="configuration">The client configuration.</param>
      protected UserServiceClient(ClientBaseConfiguration configuration) : base(configuration)
      {
      }

      /// <summary>
      /// Sends a greeting
      /// </summary>
      /// <param name="request">The request to send to the server.</param>
      /// <param name="headers">The initial metadata to send with the call. This parameter is optional.</param>
      /// <param name="deadline">An optional deadline for the call. The call will be cancelled if deadline is hit.</param>
      /// <param name="cancellationToken">An optional token for canceling the call.</param>
      /// <returns>The response received from the server.</returns>
      public virtual global::User.UserGetResponse Get(global::User.UserGetRequest request, grpc::Metadata headers = null, DateTime? deadline = null, CancellationToken cancellationToken = default(CancellationToken))
      {
        return Get(request, new grpc::CallOptions(headers, deadline, cancellationToken));
      }
      /// <summary>
      /// Sends a greeting
      /// </summary>
      /// <param name="request">The request to send to the server.</param>
      /// <param name="options">The options for the call.</param>
      /// <returns>The response received from the server.</returns>
      public virtual global::User.UserGetResponse Get(global::User.UserGetRequest request, grpc::CallOptions options)
      {
        return CallInvoker.BlockingUnaryCall(__Method_Get, null, options, request);
      }
      /// <summary>
      /// Sends a greeting
      /// </summary>
      /// <param name="request">The request to send to the server.</param>
      /// <param name="headers">The initial metadata to send with the call. This parameter is optional.</param>
      /// <param name="deadline">An optional deadline for the call. The call will be cancelled if deadline is hit.</param>
      /// <param name="cancellationToken">An optional token for canceling the call.</param>
      /// <returns>The call object.</returns>
      public virtual grpc::AsyncUnaryCall<global::User.UserGetResponse> GetAsync(global::User.UserGetRequest request, grpc::Metadata headers = null, DateTime? deadline = null, CancellationToken cancellationToken = default(CancellationToken))
      {
        return GetAsync(request, new grpc::CallOptions(headers, deadline, cancellationToken));
      }
      /// <summary>
      /// Sends a greeting
      /// </summary>
      /// <param name="request">The request to send to the server.</param>
      /// <param name="options">The options for the call.</param>
      /// <returns>The call object.</returns>
      public virtual grpc::AsyncUnaryCall<global::User.UserGetResponse> GetAsync(global::User.UserGetRequest request, grpc::CallOptions options)
      {
        return CallInvoker.AsyncUnaryCall(__Method_Get, null, options, request);
      }
      public virtual global::User.UserGetResponse Update(global::User.UserGetResponse request, grpc::Metadata headers = null, DateTime? deadline = null, CancellationToken cancellationToken = default(CancellationToken))
      {
        return Update(request, new grpc::CallOptions(headers, deadline, cancellationToken));
      }
      public virtual global::User.UserGetResponse Update(global::User.UserGetResponse request, grpc::CallOptions options)
      {
        return CallInvoker.BlockingUnaryCall(__Method_Update, null, options, request);
      }
      public virtual grpc::AsyncUnaryCall<global::User.UserGetResponse> UpdateAsync(global::User.UserGetResponse request, grpc::Metadata headers = null, DateTime? deadline = null, CancellationToken cancellationToken = default(CancellationToken))
      {
        return UpdateAsync(request, new grpc::CallOptions(headers, deadline, cancellationToken));
      }
      public virtual grpc::AsyncUnaryCall<global::User.UserGetResponse> UpdateAsync(global::User.UserGetResponse request, grpc::CallOptions options)
      {
        return CallInvoker.AsyncUnaryCall(__Method_Update, null, options, request);
      }
      /// <summary>Creates a new instance of client from given <c>ClientBaseConfiguration</c>.</summary>
      protected override UserServiceClient NewInstance(ClientBaseConfiguration configuration)
      {
        return new UserServiceClient(configuration);
      }
    }

    /// <summary>Creates service definition that can be registered with a server</summary>
    /// <param name="serviceImpl">An object implementing the server-side handling logic.</param>
    public static grpc::ServerServiceDefinition BindService(UserServiceBase serviceImpl)
    {
      return grpc::ServerServiceDefinition.CreateBuilder()
          .AddMethod(__Method_Get, serviceImpl.Get)
          .AddMethod(__Method_Update, serviceImpl.Update).Build();
    }

  }
}
#endregion
