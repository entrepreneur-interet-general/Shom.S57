using System;

namespace Shom.GeoUtilities
{
    public static class GeoConvert
    {
        public static float Radians(float angle)
        {
            return (float)(angle / 180 * Math.PI);
        }

        public static float Degrees(float angle)
        {
            return (float)(angle * 180 / Math.PI);
        }

        public static double Radians(double angle)
        {
            return angle / 180 * Math.PI;
        }

        public static double Degrees(double angle)
        {
            return angle * 180 / Math.PI;
        }

        public static int NormalizeAngle(int angle)
        {
            var newAngle = angle;
            while (newAngle <= -180) newAngle += 360;
            while (newAngle > 180) newAngle -= 360;
            return newAngle;
        }

        public static double NormalizeAngle(double angle)
        {
            var newAngle = angle;
            while (newAngle <= -180) newAngle += 360;
            while (newAngle > 180) newAngle -= 360;
            return newAngle;
        }

        public static double From180To180(double angle)
        {
            var newAngle = angle;
            while (newAngle <= -180) newAngle += 360;
            while (newAngle > 180) newAngle -= 360;
            return newAngle;
        }

        public static int From180To180(int angle)
        {
            var newAngle = angle;
            while (newAngle <= -180) newAngle += 360;
            while (newAngle > 180) newAngle -= 360;
            return newAngle;
        }

        public static double From0To360(double angle)
        {
            var newAngle = angle;
            while (newAngle < 0) newAngle += 360;
            while (newAngle >= 360) newAngle -= 360;
            return newAngle;
        }

        public static int From0To360(int angle)
        {
            var newAngle = angle;
            while (newAngle < 0) newAngle += 360;
            while (newAngle >= 360) newAngle -= 360;
            return newAngle;
        }

        public static double DMSToDouble(double degrees, double minutes, double seconds)
        {
            //Decimal degrees = 
            //   whole number of degrees, 
            //   plus minutes divided by 60, 
            //   plus seconds divided by 3600
            return degrees + (minutes / 60) + (seconds / 3600);
        }

        public static double DMSToDouble(string dms)
        {
            string[] coordComponents = dms.Split(new char[] { '°', '\'', '.' });
            double coordValue = GeoConvert.DMSToDouble(int.Parse(coordComponents[0]), int.Parse(coordComponents[1]), int.Parse(coordComponents[2]));
            if (coordComponents[3].Equals("S") || coordComponents[3].Equals("W"))
            {
                coordValue = -coordValue;
            }
            return coordValue;
        }

        public static int[] DoubleToDMS(double coord)
        {
            int sec = (int)Math.Round(coord * 3600);
            int deg = sec / 3600;
            sec = Math.Abs(sec % 3600);
            int min = sec / 60;
            sec %= 60;
            return new int[] { deg, min, sec };
        }
    }
}