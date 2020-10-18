using System;
using System.Collections;

namespace HTTPServer
{
    public class RequsetObject
    {
        public String type { get; set; }
        public String name { get; set; }
        public String command { get; set; }

        public RequsetObject(string type, string name, string command)
        {
            this.type = type;
            this.name = name;
            this.command = command;
        }
        public RequsetObject() { }

        //public override string ToString()
        //{
        //    return "Our object is : \ntype: "+type+" name: "+name+" command: "+command;
        //}
    }
}