using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicalCacheLibrary
{
    public class Configuration
    {
        public IList<Cache_conf> caches; 
        public string schema;

    }

    public class Cache_conf
    {
        public string name;
        public string path;
        public int ts;
    }




}
