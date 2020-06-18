using System;
using System.Text;

namespace Shom.ISO8211
{
    public class RecordLeader
    {
        public char ApplicationIndicator;
        public int BaseAddressOfFieldArea;
        public EntryMap EntryMap;
        public char[] ExtendedCharacterSetIndicator;
        public int FieldControlLength;
        public char InlineCodeExtensionIndicator;
        public char InterchangeLevel;
        public char LeaderIdentifier;
        public int RecordLength;
        public char VersionNumber;

        public RecordLeader(ArraySegment<byte> bytes)
        {
            int _offset = bytes.Offset;
            for (int i = 0; i < 5; i++)
            {
                RecordLength += ((bytes.Array[bytes.Offset + i] - '0') * (int)Math.Pow(10, 4 - i));
            }

            InterchangeLevel = (char) bytes.Array[_offset + 5];
            LeaderIdentifier = (char) bytes.Array[_offset + 6];
            if (LeaderIdentifier == 'L' && InterchangeLevel != '3')
            {
                throw new NotImplementedException("Processing file with Interchange level " + InterchangeLevel);
            }

            InlineCodeExtensionIndicator = (char) bytes.Array[_offset + 7];
            if (LeaderIdentifier == 'L' && InlineCodeExtensionIndicator != 'E')
            {
                throw new NotImplementedException("Processing file with InlineCodeExtensionIndicator " + InlineCodeExtensionIndicator);
            }
            VersionNumber = (char) bytes.Array[_offset + 8];
            ApplicationIndicator = (char) bytes.Array[_offset + 9];
            if (LeaderIdentifier == 'L')
            {
                FieldControlLength = (bytes.Array[_offset + 10] - '0') * 10 + (bytes.Array[_offset + 11] - '0');
            }

            for (int i = 12; i < 17; i++)
            {
                BaseAddressOfFieldArea += ((bytes.Array[_offset + i] - '0') * (int)Math.Pow(10, 16 - i));
            }

            ExtendedCharacterSetIndicator = new[] {(char) bytes.Array[_offset + 17], (char) bytes.Array[_offset + 18], (char) bytes.Array[_offset + 19], '\0'}; //added \0

            //var entryMapBytes = new byte[4];
            //Array.Copy(bytes, 20, entryMapBytes, 0, 4);

            //var entryMap = new EntryMap(entryMapBytes);
            //EntryMap = entryMap;

            EntryMap = new EntryMap(bytes);


        }

        public override string ToString()
        {
            var sb = new StringBuilder();

            sb.Append("Length " + RecordLength + " bytes" + Environment.NewLine);
            sb.Append("Interchange Level " + InterchangeLevel + Environment.NewLine);
            sb.Append("Inline Code Extension Indicator " + InlineCodeExtensionIndicator + Environment.NewLine);
            sb.Append("Version Number " + VersionNumber);
            switch (VersionNumber)
            {
                case '1':
                    sb.Append(" == ISO8211:1994");
                    break;
                case ' ':
                    sb.Append(" == ISO8211:1985");
                    break;
                default:
                    sb.Append(" == ISO8211: Unknown Year");
                    break;
            }
            sb.Append(Environment.NewLine);
            sb.Append("Application Indicator Field " + ApplicationIndicator + Environment.NewLine);
            sb.Append("Field Control Length " + FieldControlLength + Environment.NewLine);
            sb.Append("Base address of data field area " + BaseAddressOfFieldArea + "(0x" +
                      BaseAddressOfFieldArea.ToString("X") + ")" + Environment.NewLine);
            var s = new string(ExtendedCharacterSetIndicator);
            sb.Append("Extended Character Set Indicator " + s + Environment.NewLine);

            sb.Append(EntryMap);

            return sb.ToString();
        }
    }
}