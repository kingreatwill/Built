/*
 * Licensed to the OpenSkywalking under one or more
 * contributor license agreements.  See the NOTICE file distributed with
 * this work for additional information regarding copyright ownership.
 * The ASF licenses this file to You under the Apache License, Version 2.0
 * (the "License"); you may not use this file except in compliance with
 * the License.  You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 *
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using SkyWalking.Components;
using SkyWalking.Context;
using SkyWalking.Context.Tag;
using SkyWalking.Context.Trace;

namespace SkyWalking.Diagnostics.Grpc
{
    public class GrpcHandlerDiagnosticProcessor : ITracingDiagnosticProcessor
    {
        public string ListenerName { get; } = GrpcDiagnosticListenerExtensions.DiagnosticListenerName;

        private readonly IContextCarrierFactory _contextCarrierFactory;

        public GrpcHandlerDiagnosticProcessor(IContextCarrierFactory contextCarrierFactory)
        {
            _contextCarrierFactory = contextCarrierFactory;
        }

        [DiagnosticName(GrpcDiagnosticListenerExtensions.ServerRequestMessageStore)]
        public void ServerRequest([Object] GrpcEventData eventData)
        {
            var contextCarrier = _contextCarrierFactory.Create();
            foreach (var item in contextCarrier.Items)
                item.HeadValue = eventData.Headers.Where(t => t.Key == item.HeadKey).FirstOrDefault()?.Value;

            var span = ContextManager.CreateExitSpan(eventData.Operation, contextCarrier, "");
            span.Tag("", "");
        }

        [DiagnosticName(GrpcDiagnosticListenerExtensions.ServerResponseMessageStore)]
        public void ServerResponse([Object] GrpcEventData eventData)
        {
            var span = ContextManager.ActiveSpan;
            if (span != null && eventData != null)
            {
                Tags.StatusCode.Set(span, eventData.ToString());
            }

            ContextManager.StopSpan(span);
        }

        //[DiagnosticName("System.Net.Http.Exception")]
        //public void HttpException([Property(Name = "Request")] HttpRequestMessage request,
        //    [Property(Name = "Exception")] Exception ex)
        //{
        //    var span = ContextManager.ActiveSpan;
        //    if (span != null && span.IsExit)
        //    {
        //        span.ErrorOccurred();
        //    }
        //}
    }
}