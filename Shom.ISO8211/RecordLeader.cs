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

        public RecordLeader(byte[] bytes)
        {
            var sbRecordLength = new StringBuilder();
            for (int i = 0; i < 5; i++)
            {
                sbRecordLength.Append((char) bytes[i]);
            }

            RecordLength = Int32.Parse(sbRecordLength.ToString());

            InterchangeLevel = (char) bytes[5];
            LeaderIdentifier = (char) bytes[6];
            if (LeaderIdentifier == 'L' && InterchangeLevel != '3')
            {
                throw new NotImplementedException("Processing file with Interchange level " + InterchangeLevel);
            }

            InlineCodeExtensionIndicator = (char) bytes[7];
            if (LeaderIdentifier == 'L' && InlineCodeExtensionIndicator != 'E')
            {
                throw new NotImplementedException("Processing file with InlineCodeExtensionIndicator " + InlineCodeExtensionIndicator);
            }
            VersionNumber = (char) bytes[8];
            ApplicationIndicator = (char) bytes[9];
            var fieldControlLengthChars = new[] {(char) bytes[10], (char) bytes[11]};
            if (LeaderIdentifier == 'L')
            {
                FieldControlLength = Int32.Parse(new string(fieldControlLengthChars));
            }

            var sbBaseAddress = new StringBuilder();
            for (int i = 12; i < 17; i++)
            {
                sbBaseAddress.Append((char) bytes[i]);
            }
            BaseAddressOfFieldArea = Int32.Parse(sbBaseAddress.ToString());

            ExtendedCharacterSetIndicator = new[] {(char) bytes[17], (char) bytes[18], (char) bytes[19]};

            var entryMapBytes = new byte[4];
            Array.Copy(bytes, 20, entryMapBytes, 0, 4);

            var entryMap = new EntryMap(entryMapBytes);


            EntryMap = entryMap;
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