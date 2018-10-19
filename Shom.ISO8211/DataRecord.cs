namespace Shom.ISO8211
{
    public class DataRecord : Record
    {
        public DataRecordFields Fields;

        public override string ToString()
        {
            return base.ToString() + Fields;
        }
    }
}