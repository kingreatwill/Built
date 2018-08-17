using Built.Micro.ImageCloud.Mongo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Built.Micro.ImageCloud
{
    //if you are able to define your model
    public class Material : Entity
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }
}