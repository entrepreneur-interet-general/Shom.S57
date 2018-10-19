using System;

namespace Shom.GeoUtilities
{
    public class GeoUtilities
    {
        public static double ToDecimalDegrees(int degrees, int minutes, int seconds, char dir)
        {
            double decimalDegrees = degrees;
            decimalDegrees += (double)minutes / 60;
            decimalDegrees += (double)seconds / 3600;
            if (dir == 'S' || dir == 'W')
            {
                decimalDegrees = -decimalDegrees;
            }
            return decimalDegrees;
        }

        public static bool InSameTileArea(double lon1, double lat1, double lon2, double lat2, int zoom)
        {
            (int tx1, int ty1) = LonLatToTile(lon1, lat1, zoom);
            (int tx2, int ty2) = LonLatToTile(lon2, lat2, zoom);
            return (Math.Abs(tx1 - tx2) <= 1 && Math.Abs(ty1 - ty2) <= 1);
        }

        public static (int, int) LonLatToTile(double lon, double lat, int zoom)
        {
            (double mx, double my) = LonLatToMeters(lon, lat);
            return MetersToTile(mx, my, zoom);
        }

        public static (double, double) LonLatToMeters(double lon, double lat)
        {
            double mx = lon * originShift / 180;
            double my = Math.Log(Math.Tan((90 + lat) * Math.PI / 360)) / (Math.PI / 180);
            my = my * originShift / 180;
            return (mx, my);
        }

        public static (int, int) MetersToTile(double mx, double my, int zoom)
        {
            (double px, double py) = MetersToPixels(mx, my, zoom);
            return PixelsToTile(px, py);
        }

        public static (double, double) MetersToPixels(double mx, double my, int zoom)
        {
            double res = Resolution(zoom);
            double px = (mx + originShift) / res;
            double py = (my + originShift) / res;
            return (px, py);
        }

        public static (int, int) PixelsToTile(double px, double py)
        {
            int tx = (int)(Math.Ceiling(px / 256) - 1);
            int ty = (int)(Math.Ceiling(py / 256) - 1);
            return (tx, ty);
        }

        public static double Resolution(int zoom)
        {
            return initialResolution / Math.Pow(2, zoom);
        }

        private static double originShift = 2 * Math.PI * 6378137 / 2.0;
        private static double initialResolution = 2 / Math.PI * 6378137 / 256;
    }

}
