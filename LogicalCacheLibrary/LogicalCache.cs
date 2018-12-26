using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using ImageProcessor;
using System.Drawing;
using Newtonsoft.Json;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;

namespace LogicalCacheLibrary
{

    public class LogicalCache
    {

        public Configuration config;
        public CacheSchema schema;
        static private LogicalCache instant;

        private const int BUNDLX_MAXIDX = 128;
        private const int COMPACT_CACHE_HEADER_LENGTH = 64;
        private const String BUNDLE_EXT = ".bundle";
        private SortedDictionary<int,CacheInfo> rawCaches = new SortedDictionary<int, CacheInfo>();


        //Load Configuration
        private LogicalCache(string configuration)
        {
            using (StreamReader reader = new StreamReader(configuration))
            {
                config = JsonConvert.DeserializeObject<Configuration>(reader.ReadToEnd());
                schema = new CacheSchema(config.schema);
                rawCaches.Clear();
                foreach (var cache_conf in config.caches)
                {
                    CacheInfo cache = new CacheInfo(cache_conf.path, cache_conf.name);
                    rawCaches.Add(cache_conf.ts, cache);
                }
            }
        }

        private byte[] GetTileBytes(string pathToCacheRoot, int level, int row, int col)
        {
            string bundleFile = BuildBundleFilePath(pathToCacheRoot, level, row, col);
            if(!File.Exists(bundleFile))
            {
                return null;
            }
            int index = BUNDLX_MAXIDX * (row % BUNDLX_MAXIDX) + (col % BUNDLX_MAXIDX);
            using (FileStream source = File.OpenRead(bundleFile))
            {

                BinaryReader reader = new BinaryReader(source);

                source.Seek((index * 8) + COMPACT_CACHE_HEADER_LENGTH, SeekOrigin.Begin);

                byte[] offsetAndSize = new byte[8];
                reader.Read(offsetAndSize, 0, 8);

                byte[] offsetBytes = new byte[8];
                Buffer.BlockCopy(offsetAndSize, 0, offsetBytes, 0, 5);
                long offset = BitConverter.ToUInt32(offsetBytes, 0);

                byte[] sizeBytes = new byte[4];
                Buffer.BlockCopy(offsetAndSize, 5, sizeBytes, 0, 3);
                int size = BitConverter.ToInt32(sizeBytes, 0);

                if (size == 0)
                    return null;

                source.Seek(offset, SeekOrigin.Begin);
                byte[] tile = new byte[size];
                reader.Read(tile, 0, size);
                return tile;
            }

        }

        private String BuildBundleFilePath(string pathToCacheRoot, int level, int row, int col)
        {
            StringBuilder bundlePath = new StringBuilder(pathToCacheRoot + @"\_alllayers");

            int baseRow = (row / BUNDLX_MAXIDX) * BUNDLX_MAXIDX;
            int baseCol = (col / BUNDLX_MAXIDX) * BUNDLX_MAXIDX;

            String zoomStr = level.ToString();
            if (zoomStr.Length < 2)
                zoomStr = "0" + zoomStr;

            StringBuilder rowStr = new StringBuilder(baseRow.ToString("X"));
            StringBuilder colStr = new StringBuilder(baseCol.ToString("X"));

            // column and rows are at least 4 characters long
            const int padding = 4;

            while (colStr.Length < padding)
                colStr.Insert(0, "0");

            while (rowStr.Length < padding)
                rowStr.Insert(0, "0");

            if (!pathToCacheRoot.EndsWith(Path.DirectorySeparatorChar.ToString()))
                bundlePath.Append(Path.DirectorySeparatorChar);
            bundlePath.Append("L").Append(zoomStr).Append(Path.DirectorySeparatorChar).Append("R").Append(rowStr.ToString().ToLower())
                .Append("C").Append(colStr.ToString().ToLower()).Append(BUNDLE_EXT);

            return bundlePath.ToString();
        }


        //Get Tile
        public void GetTile(int ts, int level, int row, int column, Stream output, out string format)
        {
            format = "image/jpeg";
            bool png_flag = false;
            Stack<byte[]> chain = new Stack<byte[]>();
            var subset = from c in rawCaches
                         where c.Key <= ts
                         orderby c.Key descending
                         select c;
            foreach (var rawCache in subset)
            {
                byte[] tile = GetTileBytes(rawCache.Value.Root, level, row, column);
                if (tile != null)
                {
                    Bitmap tile_image = new Bitmap(new MemoryStream(tile));
                    chain.Push(tile);
                    if (tile_image.RawFormat.Equals(ImageFormat.Jpeg))
                    {
                        png_flag = false;
                        break;
                    }
                    else
                    {
                        png_flag = true;
                        continue;
                    }
                }
            }
            bool blend_flag = false;
            ImageFactory imgfac = new ImageFactory();
            var count = chain.Count;
            if (count == 1)
            {
                var tile = chain.Pop();
                output.Write(tile, 0, tile.Length);
                if(png_flag)
                {
                    format = "image/png";
                }
                return;
            }
            for (int i=0;i<count;i++)
            {
                var tile = chain.Pop();
                if (!blend_flag)
                {
                    imgfac.Load(tile);
                    blend_flag = true;
                }
                else
                {
                    var tile_image = new Bitmap(new MemoryStream(tile));
                    imgfac.Overlay(new ImageProcessor.Imaging.ImageLayer() { Image = tile_image });
                }
            }
            imgfac.Save(output);
            if (png_flag)
            {
                format = "image/png";
            }
            return;
        }

        
        static public LogicalCache GetInstance(string configuration)
        {
            if(LogicalCache.instant==null)
            {
                instant = new LogicalCache(configuration);
            }
            return instant;
        }

    }
}
