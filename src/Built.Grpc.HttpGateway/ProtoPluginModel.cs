using System;
using System.Collections.Generic;
using System.Text;

namespace Built.Grpc.HttpGateway
{
    public class ProtoPluginModel
    {
        public string FileName { get; set; }
        public string DllFileMD5 { get; set; }
        public string ProtoFileMD5 { get; set; }
    }
}