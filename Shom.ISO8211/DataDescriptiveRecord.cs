namespace Shom.ISO8211
{
    public class DataDescriptiveRecord : Record
    {
        public DataDescriptiveRecordFields Fields;

        public override string ToString()
        {
            return base.ToString() + Fields;
        }
    }
}