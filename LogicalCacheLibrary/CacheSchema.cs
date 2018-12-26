using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace LogicalCacheLibrary
{
    public class CacheSchema
    {
        public double OriginX { get; set; }
        public double OriginY { get; set; }
        public int TileRows { get; set; }
        public int TileColumns { get; set; }
        public List<Tuple<int, double, double>> LODs { get; set; }

        public CacheSchema(string root)
        {
            LODs = new List<Tuple<int, double, double>>();
            LoadFromSchemaFile(root);
        }

        private void LoadFromSchemaFile(string path)
        {
            XElement root = XElement.Load(path);
            XElement origin = root.Element("TileCacheInfo").Element("TileOrigin");
            OriginX = double.Parse(origin.Element("X").Value);
            OriginY = double.Parse(origin.Element("Y").Value);

            TileRows = int.Parse(root.Element("TileCacheInfo").Element("TileRows").Value);
            TileColumns = int.Parse(root.Element("TileCacheInfo").Element("TileCols").Value);

            IEnumerable<XElement> lods = root.Element("TileCacheInfo").Element("LODInfos").Elements("LODInfo");
            foreach (XElement lod in lods)
            {
                LODs.Add(Tuple.Create(int.Parse(lod.Element("LevelID").Value), double.Parse(lod.Element("Scale").Value), double.Parse(lod.Element("Resolution").Value)));

            }

        }

        public TileInfo GetTileInfoFromXY(double scale, double x, double y)
        {
            TileInfo tile = new TileInfo();
            int i = 0;
            bool flag = false;
            for (; i < LODs.Count; i++)
            {
                if (scale < LODs[i].Item2)
                {
                    flag = true;
                    continue;
                }   
                else
                {
                    break;
                }
            }
            //i = i == 0 ? 0 : i - 1;
            if(!flag)
            {
                i = 0;
            }
            Tuple<int, double, double> lod = LODs[i];
            tile.Column = (int)Math.Floor((x - OriginX) / (TileColumns * lod.Item3));
            tile.Row = (int)Math.Floor((OriginY - y) / (TileRows * lod.Item3));
            tile.Level = lod.Item1;
            return tile;

        }



        public Tuple<double, double, double, double> GetTileEnvelop(int level, int row, int column)
        {
            int i = 0;
            for (; i < LODs.Count; i++)
            {
                if (LODs[i].Item1 == level)
                {
                    break;
                }
            }
            double resolution = LODs[i].Item3;
            double x = OriginX + column * TileColumns * resolution;
            double y = OriginY - row * TileRows * resolution;
            return new Tuple<double, double, double, double>(x, x + TileColumns * resolution, y - TileRows * resolution, y);
        }

        public Tuple<double, double, double, double> GetTileEnvelop(TileInfo tile)
        {
            int i = 0;
            for (; i < LODs.Count; i++)
            {
                if (LODs[i].Item1 == tile.Level)
                {
                    break;
                }
            }
            double resolution = LODs[i].Item3;
            double x = OriginX + tile.Column * TileColumns * resolution;
            double y = OriginY - tile.Row * TileRows * resolution;
            return new Tuple<double, double, double, double>(x, x + TileColumns * resolution, y - TileRows * resolution, y);
        }
    }
}
