using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Drawing;
using System.Configuration;
//using DotSpatial.Data;

namespace Shom.GeoUtilities
{
    public class Tile
    {
        private const double originShift = 20037508.342789244; // 2 * Math.PI * 6378137 / 2.0;
        private const double tileSize = 256;
        private const double initialResolution = 156543.03392804062; // 2 * Math.PI * 6378137 / tileSize;

        public int Zoom;
        public int X;
        public int Y;

        private Tile()
        {}

        // FromWGS84: from lon,lat gives the google tile index
        // From http://wiki.openstreetmap.org/wiki/Slippy_map_tilenames
        // lower and upper bounds are 85;
        public static Tile FromWGS84(int zoom, double lonDeg, double latDeg)
        {
            Tile tile = new Tile();
            if (latDeg > 85) latDeg = 85;
            if (latDeg < -85) latDeg = -85;
            var latRad = GeoConvert.Radians(latDeg);
            var n = Math.Pow(2, zoom);
            tile.Zoom = zoom;
            tile.X = (int)Math.Floor(n * ((lonDeg + 180) / 360));
            tile.Y = (int)Math.Floor(((1.0d - Math.Log(Math.Tan(latRad) + (1 / Math.Cos(latRad))) / Math.PI) / 2.0d * n));
            return tile;
        }

        public static Tile FromXY(int zoom, int x, int y)
        {
            Tile tile = new Tile();
            tile.Zoom = zoom;
            tile.X = x;
            tile.Y = y;
            return tile;
        }

        public Tile GetZoom(int zoom)
        {
            if (Zoom < zoom)
            {
                return null;
            }
            if (Zoom == zoom)
            {
                return this;
            }
            var divider = Math.Pow(2, zoom - Zoom);
            Tile tile = new Tile();
            tile.Zoom = zoom;
            tile.X = (int)(X / divider);
            tile.Y = (int)(Y / divider);
            return tile;
        }

        public static Tile EmptyTile()
        {
            Tile tile = new Tile();
            tile.Zoom = -1;
            tile.X = -1;
            tile.Y = -1;
            return tile;
        }

        public bool IsEmpty
        {
            get { return Zoom == -1 && X == -1 && Y == -1; }
        }

        public string FilePath
        {
            get { return Zoom + "/" + X + "/" + Y + ".png"; }
        }


        private List<Tile> _neighbours = null;

        public List<Tile> Neighbours
        {
            get
            {
                if (_neighbours == null)
                {
                    _neighbours = GetNeighbours();
                }
                return _neighbours;
            }
        }

        private List<Tile> GetNeighbours()
        {
            List<Tile> neighbours = new List<Tile>();
            neighbours.Add(Tile.FromXY(Zoom, X - 1, Y - 1));
            neighbours.Add(Tile.FromXY(Zoom, X, Y - 1));
            neighbours.Add(Tile.FromXY(Zoom, X + 1, Y - 1));
            neighbours.Add(Tile.FromXY(Zoom, X - 1, Y));
            neighbours.Add(Tile.FromXY(Zoom, X + 1, Y));
            neighbours.Add(Tile.FromXY(Zoom, X - 1, Y + 1));
            neighbours.Add(Tile.FromXY(Zoom, X, Y + 1));
            neighbours.Add(Tile.FromXY(Zoom, X + 1, Y + 1));
            return neighbours;
        }

        private string ShapeFileName
        {
            get
            {
                return X + "_" + Y + ".shp";
            }
        }

        public double Resolution(int zoom)
        {
            return initialResolution / Math.Pow(2, zoom);
        }

        //def PixelsToMeters(self, px, py, zoom):
        //"Converts pixel coordinates in given zoom level of pyramid to EPSG:900913"

        //res = self.Resolution( zoom )
        //mx = px * res - self.originShift
        //my = py * res - self.originShift
        //return mx, my


        // http://gis.stackexchange.com/questions/17278/calculate-lat-lon-bounds-for-individual-tile-generated-from-gdal2tiles
        public double[] PixelsToMeters(double x, double y, int zoom)
        {
            double res = Resolution(zoom);
            double mx = x * res - originShift;
            double my = -(y * res - originShift);
            return new double[] { mx, my };
        }

        public double[] TileBounds()
        {
            double[] min = PixelsToMeters(X * tileSize, Y * tileSize, Zoom);
            double[] max = PixelsToMeters((X + 1) * tileSize, (Y + 1) * tileSize, Zoom);
            return new double[] { min[0], min[1], max[0], max[1] };
        }

        public double[] MetersToLatLon(double x, double y)
        {
            double lon = (x / originShift) * 180.0;
            double lat = (y / originShift) * 180.0;
            lat = 180 / Math.PI * (2 * Math.Atan(Math.Exp(lat * Math.PI / 180.0)) - Math.PI / 2.0);
            return new double[] { lat, lon };
        }

        public double[] TileLatLonBounds()
        {
            double[] bounds = TileBounds();
            double[] min = MetersToLatLon(bounds[0], bounds[1]);
            double[] max = MetersToLatLon(bounds[2], bounds[3]);
            return new double[4] { min[0], min[1], max[0], max[1] };
        }

        public override int GetHashCode()
        {
            return string.Format("{0}_{1}_{2}", Zoom, X, Y).GetHashCode();
        }
        public override bool Equals(object obj)
        {
            return Equals(obj as Tile);
        }
        public bool Equals(Tile obj)
        {
            return (obj != null && obj.Zoom == Zoom && obj.X == X && obj.Y == Y);
        }

    }
}
