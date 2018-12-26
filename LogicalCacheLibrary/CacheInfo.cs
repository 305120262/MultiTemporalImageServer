using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.IO;

namespace LogicalCacheLibrary
{
    public class CacheInfo
    {
        public CacheSchema Schema { get; }
        public string Root { get; }
        public string Name { get; }

        public CacheInfo(string root,string name)
        {
            Schema = new CacheSchema(root + "/conf.xml");
            Root = root;
            Name = name;
        }


    }
}
