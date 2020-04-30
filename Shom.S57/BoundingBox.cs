namespace S57
{
    public class BoundingBox
    {
        public double westLongitude = 180;         // We start with upper limit values to be refined
        public double eastLongitude = -180;
        public double southLatitude = 90;
        public double northLatitude = -90;

        public static BoundingBox FromS57(S57Reader s57 )
        {
            var boundingBox = new BoundingBox();
            var mapCovers = s57.GetFeaturesOfClass(S57Obj.M_COVR);
            foreach (var mapCover in mapCovers)
            {
                var geom = mapCover.GetGeometry(false);
                if (geom is Area)
                {
                    var area = geom as Area;
                    foreach (var point in area.points)
                    {
                        if (point.X < boundingBox.westLongitude)
                        {
                            boundingBox.westLongitude = point.X;
                        }
                        else if (point.X > boundingBox.eastLongitude)
                        {
                            boundingBox.eastLongitude = point.X;
                        }
                        if (point.Y > boundingBox.northLatitude)
                        {
                            boundingBox.northLatitude = point.Y;
                        }
                        else if (point.Y < boundingBox.southLatitude)
                        {
                            boundingBox.southLatitude = point.Y;
                        }
                    }
                }
            }
            return boundingBox;
        }

    }

}
