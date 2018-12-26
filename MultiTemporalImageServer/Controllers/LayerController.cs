using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using LogicalCacheLibrary;

namespace MultiTemporalImageServer.Controllers
{
    [Route("api")]
    public class LayerController : ControllerBase
    {

        // GET api/
        [HttpGet("tms")]
        public ActionResult<IEnumerable<int>> Get()
        {
            return new int[] { 1143, 1144 };
        }

        // GET api/1143/1/3/5
        [HttpGet("{tm:int}/{level:int}/{row:int}/{col:int}")]
        public ActionResult<string> Get(int tm,int level,int row, int col)
        {
            using (var stream = new MemoryStream())
            {
                var cacheManager = LogicalCache.GetInstance(@"C:\Data\Projects\MultiTemporalImageServer\config.json");
                string format;
                cacheManager.GetTile(tm, level, row, col, stream,out format);
                var cacheBytes = stream.GetBuffer();
                if(cacheBytes.Length==0)
                {
                    return NotFound();
                }
                else {
                    return File(cacheBytes, format);
                }
                
            }
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
