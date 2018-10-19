using System.Text;

namespace Shom.ISO8211
{
    public abstract class Record
    {
        public RecordDirectory Directory;
        public RecordLeader Leader;

        public override string ToString()
        {
            var sb = new StringBuilder();

            sb.Append(Leader);
            sb.Append(Directory);

            return sb.ToString();
        }
    }
}