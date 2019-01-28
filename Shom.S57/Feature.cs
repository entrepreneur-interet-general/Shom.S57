using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using Shom.ISO8211;
using S57.File;
using System.Diagnostics;

namespace S57
{
    // Feature Record
    //  0001 ISO/IEC 8211 Record Identifier
    //      FRID Feature Record Identifier
    //          FOID Feature Object Identifier
    //          <R> - ATTF Attributes
    //          <R> - NATF National Attributes
    //          FFPC Feature Record To Feature Object Pointer Control
    //          <R> - FFPT Feature Record To Feature Object Pointer
    //          FSPC Feature Record To Spatial Record Pointer Control
    //          <R> - FSPT Feature to Spatial Record Pointer
    public class Feature
    {
        private DataRecord _dataRecord;

        public BaseFile baseFile;
        public Cell cell;

        // FRID : Feature Record Identifier Field
        public uint RecordName;                         // RCNM
        public uint RecordIdentificationNumber;         // RCID
        public GeometricPrimitive Primitive;            // PRIM
        public uint Group;                              // GRUP     
        public uint Code;                               // OBJL
        public uint RecordVersion;                      // RVER
        public RecordUpdate RecordUpdateInstruction;    // RUIN

        // FOID : 
        public LongName lnam;                           // FOID Object Identifier Field
        public Dictionary<uint, string> Attributes;     // ATTF Attributes

        // FFPC : Feature Record To Feature Object Pointer Control
        public RecordUpdate FeatureObjectPointerUpdateInstruction;  // FFUI 
        public uint FeatureObjectPointerIndex;                      // FFIX : Index (position) of the adressed record pointer within the FFPT field(s) of the target record
        public uint NumberOfFeatureObjectPointers;                  // NFPT : Number of record pointers in the FFPT field(s) of the update record

        public List<FeatureRecordPointer> FeaturePtrs;      // <R> FFPT : Pointer Fields

        // FSPC : Feature Record to Spatial Record Pointer Control
        public RecordUpdate FeatureToSpatialRecordPointerUpdateInstruction; // FSUI
        public uint FeatureToSpatialRecordPointerIndex;             // FSIX : Index (position) of the adressed record pointer within the FSPT fields of the target record
        public uint NumberOfFeatureToSpatialRecordPointers;         // NSPT : Number of record pointers in the FSPT field of the target record

        public List<VectorRecordPointer> VectorPtrs;        // <R> FSPT : Feature Record to Spatial Record Pointer

        public override string ToString()
        {
            return Code + " " + lnam.ToString();
        }

        //
        // It might be necessary to go recursive on that one since a vector can be made of vectors...
        //
        public double PositionAccuracy
        {
            get {
                if (_positionAccuracy == uint.MaxValue)
                {
                    uint positionAccuracy = uint.MinValue;
                    foreach (var vectorPtr in VectorPtrs)
                    {
                        var vector = vectorPtr.Vector;
                        var posaccCode = S57Attributes.Get("POSACC").Code;
                        if (vector.Attributes != null && vector.Attributes.ContainsKey(posaccCode))
                        {
                            var sPosacc = vector.Attributes[posaccCode];
                            double posacc = double.Parse(sPosacc, CultureInfo.InvariantCulture );
                            if( posacc > positionAccuracy )
                            {
                                _positionAccuracy = posacc;
                            }
                        }
                    }
                }
                if (_positionAccuracy == uint.MaxValue)
                {
                    _positionAccuracy = 10; // by default, positionAccuracy = 10 m
                }
                return _positionAccuracy;
            }
        }

        private double _positionAccuracy = uint.MaxValue;
            // From Vector Attributes

        public string this[string i]                        // Attributes accessor thru acronym
        {
            get
            {
                var attr = S57Attributes.Get(i);
                if (attr == null || !Attributes.ContainsKey(attr.Code))
                {
                    return null;
                }
                return Attributes[attr.Code];
            }
        }

        public Geometry GetGeometry()
        {
            switch (Primitive)
            {
                case GeometricPrimitive.Point:
                    {
                        if (VectorPtrs[0] == null || VectorPtrs[0].Vector == null) return null;
                        return VectorPtrs[0].Vector.Geometry;
                    }
                case GeometricPrimitive.Line:
                    {
                        //initialize StartNode Pointer for later check how to build geometry
                        VectorRecordPointer StartNode = new VectorRecordPointer();
                        //initialize Contour to accumulate lines
                        Line Contour = new Line();
                        for (int i = 0; i < VectorPtrs.Count(); i++)
                        {
                            var vectorPtr = VectorPtrs[i];
                            //first, check if vector exist, and if it is supposed to be visible 
                            //(to improve: masked points should still be added for correct topology, just not rendered later)
                            if (vectorPtr.Vector == null || vectorPtr.Mask == Masking.Mask) break;

                            //next, check if edge needs to be reversed for the intended usage 
                            //Do this on a copy to not mess up original record wich might be used elsewhere
                            var edge = new List<Point>((vectorPtr.Vector.Geometry as Line).points);
                            if (vectorPtr.Orientation == Orientation.Reverse)
                            {
                                edge.Reverse();
                                //note: Reversing is not reversing the VectorRecordPointers, see S57 Spec 3.1 section 3.20 (i.e. index [0] is now end node instead of start node)
                                //therefore assign private StartNode pointer for some later checks 
                                StartNode = vectorPtr.Vector.VectorRecordPointers[1];
                            }
                            else
                            {
                                StartNode = vectorPtr.Vector.VectorRecordPointers[0];
                            }

                            //now check if Contour has already accumulated points, if not just add current edge
                            if (Contour.points.Count() > 0)
                            {
                                //now check if existing contour should be extended
                                if (Contour.points.Last() == StartNode.Vector.Geometry as Point)
                                {
                                    Contour.points.AddRange(edge.Skip(1));
                                }
                            }
                            else
                            {
                                //add current edge points to new contour
                                Contour.points.AddRange(edge);
                            }
                        }
                        //done, finish up and return
                        return Contour;
                    }
                case GeometricPrimitive.Area:
                    {
                        //initialize PolygonSet to accumulate 1 exterior and (if available) repeated interior boundaries
                        PolygonSet ContourSet = new PolygonSet();
                        //initialize StartNode Pointer for check how to build geometry
                        VectorRecordPointer StartNode = new VectorRecordPointer();
                        //initialize Contour to accumulate boundaries
                        Area Contour = new Area();
                        for (int i = 0; i < VectorPtrs.Count(); i++)
                        {
                            var vectorPtr = VectorPtrs[i];
                            //first, check if vector exist, and if it is supposed to be visible
                            //(to improve: masked points should still be added for correct topology, just not rendered later)
                            if (vectorPtr.Vector == null || vectorPtr.Mask == Masking.Mask) break;

                            //next, check if edge needs to be reversed for the intended usage 
                            //Do this on a copy to not mess up original record wich might be used elsewhere
                            var edge = new List<Point>((vectorPtr.Vector.Geometry as Line).points);
                            if (vectorPtr.Orientation == Orientation.Reverse)
                            {
                                edge.Reverse();
                                //note: Reversing is not reversing the VectorRecordPointers, see S57 Spec 3.1 section 3.20 (i.e. index [0] is now end node instead of start node)
                                //therefore assign private StartNode pointer for some later checks 
                                StartNode = vectorPtr.Vector.VectorRecordPointers[1];
                            }
                            else
                            {
                                StartNode = vectorPtr.Vector.VectorRecordPointers[0];
                            }

                            //now check if Contour has already accumulated points, if not just add current edge
                            if (Contour.points.Count() > 0)
                            {
                                //now check if existing contour should be extended, or if a new one for the next interior boundery should be started 
                                if (Contour.points.Last() == StartNode.Vector.Geometry as Point)
                                {
                                    Contour.points.AddRange(edge.Skip(1));
                                }
                                else
                                {
                                    //done, add current polygon boundery to ContourSet 
                                    //verify that polygone is closed last point in list equals first
                                    if (Contour.points.Last() == Contour.points.First())
                                    {
                                        ContourSet.Areas.Add(Contour);
                                    }
                                    else
                                    {
                                        Debug.WriteLine("Panic: current polygon is not complete, and current egde is not extending it");
                                    }
                                    //initialize new contour to accumulate next boundery, add current edge to it
                                    Contour = new Area(); 
                                    Contour.points.AddRange(edge);
                                }
                            }
                            else
                            {
                                //add current edge points to new contour
                                Contour.points.AddRange(edge);
                            }                            
                        }
                        //done, finish up and return
                        ContourSet.Areas.Add(Contour);
                        return ContourSet;
                    }
            }
            return null;
        }


        public Feature(DataRecord record, BaseFile baseFile, Cell cell)
        {
            _dataRecord = record;

            this.baseFile = baseFile;
            this.cell = cell;

            var v001 = record.Fields.GetFieldByTag("0001");

            // FRID : Feature Record Identifier
            var frid = record.Fields.GetFieldByTag("FRID");
            if (frid != null)
            {
                RecordIdentificationNumber = frid.GetUInt32("RCNM");
                RecordName = frid.GetUInt32("RCID");
                Primitive = (GeometricPrimitive)frid.GetUInt32("PRIM");
                Group = frid.GetUInt32("GRUP");
                Code = frid.GetUInt32("OBJL");
                RecordVersion = frid.GetUInt32("RVER");
                RecordUpdateInstruction  = (RecordUpdate)frid.GetUInt32("RUIN");
            }

            // FOID : Feature Object Identifier
            var foid = record.Fields.GetFieldByTag("FOID");
            if (foid != null)
            {
                var agen = foid.GetUInt32("AGEN");
                var fidn = foid.GetUInt32("FIDN");
                var fids = foid.GetUInt32("FIDS");
                lnam = new LongName(agen, fidn, fids);
            }

            // ATTF : Attributes
            var attr = record.Fields.GetFieldByTag("ATTF");
            if (attr != null)
            {
                Attributes = GetAttributes( attr, baseFile );
            }

            // NATF : National attributes NATF.
            var natf = record.Fields.GetFieldByTag("NATF");
            if (natf != null)
            {
                var natfAttr = GetAttributes( natf, baseFile );
                if (Attributes != null)
                {
                    foreach (var entry in natfAttr)
                    {
                        Attributes.Add(entry.Key, entry.Value);
                    }
                }
                else
                {
                    Attributes = natfAttr;
                }
            }

            // FFPC : Feature Record To Feature Object Pointer Control
            var ffpc = record.Fields.GetFieldByTag("FFPC");
            if (ffpc != null)
            {
                FeatureObjectPointerUpdateInstruction = (RecordUpdate)ffpc.GetUInt32("FFUI");
                FeatureObjectPointerIndex = ffpc.GetUInt32("FFIX");
                NumberOfFeatureObjectPointers = ffpc.GetUInt32("NFPT");
            }

            // <R> FFPT : Feature Record To Feature Object Pointer
            var ffpt = record.Fields.GetFieldByTag("FFPT");
            if (ffpt != null)
            {
                FeaturePtrs = GetFFPTs( ffpt );
                //var lnam = new LongName(ffpt.GetBytes("LNAM"));
                //var rind = ffpt.GetUInt32("RIND");
                //var comt = ffpt.GetString("COMT");
            }


            // FSPC : Feature Record to Spatial Record Pointer Control
            var fspc = record.Fields.GetFieldByTag("FSPC");
            if (fspc != null)
            {
                FeatureToSpatialRecordPointerUpdateInstruction = (RecordUpdate)fspc.GetUInt32("FSUI");
                FeatureToSpatialRecordPointerIndex = fspc.GetUInt32("FSIX");
                NumberOfFeatureToSpatialRecordPointers  = fspc.GetUInt32("NSPT");
            }

            // FSPT : Feature Record to Spatial Record Pointer
            var fspt = record.Fields.GetFieldByTag("FSPT");
            if (fspt != null)
            {
                VectorPtrs = GetFSPTs( fspt );
            }
        }

        public static Dictionary<uint, string> GetAttributes( DataField field, BaseFile baseFile )
        {
            int currentIndex = 0;
            if (field.Tag == "ATTF" || field.Tag == "ATTV" || field.Tag == "NATF" )
            {
                LexicalLevel lexicalLevel = field.Tag == "ATTF" || field.Tag == "ATTV" ? baseFile.ATTFLexicalLevel : baseFile.NATFLexicalLevel;
                Dictionary<uint, string> values = new Dictionary<uint, string>();
                uint key = 0;
                while( currentIndex < field.Bytes.Length && field.Bytes[currentIndex] != DataField.FieldTerminator)
                {
                    foreach (SubFieldDefinition subFieldDefinition in field.FieldDescription.SubFieldDefinitions)
                    {
                        if (subFieldDefinition.FormatTypeCode == FormatTypeCode.CharacterData)
                        {
                            var sb = new StringBuilder();
                            if (subFieldDefinition.SubFieldWidth == 0)
                            {
                                if (lexicalLevel == LexicalLevel.ISO10646)
                                {
                                    while (currentIndex < field.Bytes.Length && field.Bytes[currentIndex] != DataField.UnitTerminator)
                                    {
                                        sb.Append(BitConverter.ToChar(field.Bytes, currentIndex));
                                        currentIndex += 2;
                                    }
                                }
                                else if (lexicalLevel == LexicalLevel.ISO8859)
                                {
                                    System.Text.Encoding iso8859 = System.Text.Encoding.GetEncoding("iso-8859-1");
                                    int startIndex = currentIndex;
                                    while (currentIndex < field.Bytes.Length && field.Bytes[currentIndex] != DataField.UnitTerminator)
                                    {
                                        currentIndex++;
                                    }
                                    string val = iso8859.GetString(field.Bytes, startIndex, currentIndex - startIndex );
                                    //string val2 = Encoding.GetEncoding(1252).GetString(field.Bytes, startIndex, currentIndex - startIndex);
                                    sb.Append(val);
                                    currentIndex++;
                                }
                                else
                                {
                                    while (currentIndex < field.Bytes.Length && field.Bytes[currentIndex] != DataField.UnitTerminator)
                                    {
                                        sb.Append((char)field.Bytes[currentIndex]);
                                        currentIndex++;
                                    }
                                    //Consume the Terminator
                                    currentIndex++;
                                }
                            }
                            else
                            {
                                for (int i = 0; i < subFieldDefinition.SubFieldWidth; i++)
                                {
                                    sb.Append((char)field.Bytes[currentIndex]);
                                    currentIndex++;
                                }
                            }
                            var s = sb.ToString();
                            if (!field.SubFields.ContainsKey(subFieldDefinition.Tag))
                                field.SubFields.Add(subFieldDefinition.Tag, s);
                            if (!values.ContainsKey(key))
                            {
                                values.Add(key, s);
                            }
                            else
                            {
                                // Attributes Error: declared key is missing
                            }
                        }
                        else if (subFieldDefinition.FormatTypeCode == FormatTypeCode.LsofBinaryForm)
                        {
                            switch (subFieldDefinition.BinaryFormSubType)
                            {
                                case ExtendedBinaryForm.IntegerSigned:
                                    if (subFieldDefinition.BinaryFormPrecision != 4)
                                    {
                                        throw new NotImplementedException("Only handle Signed Ints of 4 bytes");
                                    }
                                    int signedValue = 0;
                                    for (int i = 0; i < subFieldDefinition.BinaryFormPrecision; i++)
                                    {
                                        int tempVal = field.Bytes[currentIndex++];
                                        for (int j = 0; j < i; j++)
                                        {
                                            tempVal = tempVal << 8;
                                        }
                                        signedValue += tempVal;
                                    }
                                    field.SubFields.Add(subFieldDefinition.Tag, signedValue);
                                    break;
                                case ExtendedBinaryForm.IntegerUnsigned:
                                    if (subFieldDefinition.BinaryFormPrecision > 4)
                                    {
                                        throw new NotImplementedException("Only handle unsigned Ints 4 bytes or less");
                                    }

                                    UInt32 unsignedValue = 0;
                                    for (int i = 0; i < subFieldDefinition.BinaryFormPrecision; i++)
                                    {
                                        UInt32 tempVal = field.Bytes[currentIndex++];
                                        for (int j = 0; j < i; j++)
                                        {
                                            tempVal = tempVal << 8;
                                        }
                                        unsignedValue += tempVal;
                                    }

                                    if (!field.SubFields.ContainsKey(subFieldDefinition.Tag))
                                        field.SubFields.Add(subFieldDefinition.Tag, unsignedValue);
                                    key = unsignedValue;
                                    break;
                                default:
                                    throw new NotImplementedException("Unhandled LsofBinaryForm");
                            }
                        }
                        else if (subFieldDefinition.FormatTypeCode == FormatTypeCode.ExplicitPoint)
                        {
                            if (subFieldDefinition.SubFieldWidth == 0)
                            {
                                throw new Exception("Expected a subfield width for Explicit Point Type");
                            }

                            var tempSb = new StringBuilder();

                            for (int i = 0; i < subFieldDefinition.SubFieldWidth; i++)
                            {
                                tempSb.Append((char)field.Bytes[currentIndex]);
                                currentIndex++;
                            }

                            double value = 0;
                            value = Double.Parse(tempSb.ToString(), CultureInfo.InvariantCulture);

                            field.SubFields.Add(subFieldDefinition.Tag, value);
                        }
                        else if (subFieldDefinition.FormatTypeCode == FormatTypeCode.BitStringData)
                        {
                            if (subFieldDefinition.SubFieldWidth == 0)
                            {
                                throw new Exception("Expected a subfield width for Bit String Data");
                            }
                            //divide by 8 and round up
                            int bytesToRead = (subFieldDefinition.SubFieldWidth + (8 - 1)) / 8;
                            byte[] newByteArray = new byte[bytesToRead];
                            for (int i = 0; i < bytesToRead; i++)
                            {
                                newByteArray[i] = field.Bytes[currentIndex];
                                currentIndex++;
                            }
                            field.SubFields.Add(subFieldDefinition.Tag, newByteArray);
                        }
                        else
                        {
                            throw new Exception("Unhandled subField type :" + subFieldDefinition.FormatTypeCode);
                        }

                        //if (field.Bytes[field.Bytes.Length - 1] != DataField.FieldTerminator) throw new Exception("Expected Field Terminator");
                    }

                }
                return values;
            }
            return null;
        }



        // Link to Feature Objects
        public List<FeatureRecordPointer> GetFFPTs( DataField field )
        {
            List<FeatureRecordPointer> result = new List<FeatureRecordPointer>();
            int currentIndex = 0;
            if (field.Tag == "FFPT")
            {
                while (field.Bytes[currentIndex] != DataField.FieldTerminator && currentIndex < field.Bytes.Length )
                {
                    FeatureRecordPointer featureLink = new FeatureRecordPointer();
                    foreach (SubFieldDefinition subFieldDefinition in field.FieldDescription.SubFieldDefinitions)
                    {
                        var tag = subFieldDefinition.Tag;
                        if (tag == "LNAM")
                        {
                            int bytesToRead = (subFieldDefinition.SubFieldWidth + (8 - 1)) / 8;
                            byte[] newByteArray = GetFixedByteArray(field, ref currentIndex, bytesToRead);
                            featureLink.LNAM  = new LongName(newByteArray);
                        }
                        else if (tag == "RIND")
                        {
                            if (subFieldDefinition.BinaryFormPrecision > 4)
                            {
                                throw new NotImplementedException("Only handle unsigned Ints 4 bytes or less");
                            }

                            //UInt32 unsignedValue = 0;
                            //currentIndex = GetUint4Bytes(field, currentIndex, out unsignedValue);
                            //featureLink.rind = (Relationship)unsignedValue;
                            featureLink.Rind = (Relationship)field.Bytes[currentIndex++];
                        }
                        else if (tag == "COMT")
                        {

                        }
                    }
                    result.Add(featureLink);
                    currentIndex++;
                }
            }
            return result;
        }
        
        // Link to Vector Objects
        public List<VectorRecordPointer> GetFSPTs(DataField field)
        {
            List<VectorRecordPointer> result = new List<VectorRecordPointer>();
            if (field.Tag == "FSPT")
            {
                int currentIndex = 0;
                while (field.Bytes[currentIndex] != DataField.FieldTerminator && currentIndex < field.Bytes.Length )
                {
                    VectorRecordPointer spatialLink = new VectorRecordPointer();
                    var rcnm = field.Bytes[currentIndex++];
                    uint rcid;
                    currentIndex = GetUint4Bytes(field, currentIndex, out rcid);
                    spatialLink.Name = new VectorName(rcnm, rcid);
                    spatialLink.Orientation = (Orientation)field.Bytes[currentIndex++];
                    spatialLink.Usage = (Usage)field.Bytes[currentIndex++];
                    spatialLink.Mask = (Masking)field.Bytes[currentIndex++];
                    result.Add(spatialLink);
                }
            }
            return result;
        }

        // Helpers
        private static byte[] GetFixedByteArray(DataField field, ref int currentIndex, int bytesToRead)
        {
            byte[] newByteArray = new byte[bytesToRead];
            for (int i = 0; i < bytesToRead; i++)
            {
                newByteArray[i] = field.Bytes[currentIndex];
                currentIndex++;
            }

            return newByteArray;
        }
        private static int GetUint4Bytes(DataField field, int currentIndex, out uint unsignedValue)
        {
            unsignedValue = 0;
            for (int i = 0; i < 4; i++)
            {
                UInt32 tempVal = field.Bytes[currentIndex++];
                for (int j = 0; j < i; j++)
                {
                    tempVal = tempVal << 8;
                }
                unsignedValue += tempVal;
            }
            return currentIndex;
        }

        public List<SoundingData> ExtractSoundings()
        {
            DataRecord dr = VectorPtrs[0].Vector.DataRecord;
            var sg3d = dr.Fields.GetFieldByTag("SG3D");
            var bytes = sg3d.Bytes;
            var length = bytes.Count() - 1;
            int currentIndex = 0;
            var soundingDatas = new List<SoundingData>();
            while (currentIndex < length && bytes[currentIndex] != DataField.UnitTerminator)
            {
                var soundingData = new SoundingData();
                for (int i = 0; i < 4; i++)
                {
                    int tempVal = bytes[currentIndex++];
                    for (int j = 0; j < i; j++)
                    {
                        tempVal = tempVal << 8;
                    }
                    soundingData.Y += tempVal;
                }
                soundingData.Y /= baseFile.coordinateMultiplicationFactor;
                for (int i = 0; i < 4; i++)
                {
                    int tempVal = bytes[currentIndex++];
                    for (int j = 0; j < i; j++)
                    {
                        tempVal = tempVal << 8;
                    }
                    soundingData.X += tempVal;
                }
                soundingData.X /= baseFile.coordinateMultiplicationFactor;
                for (int i = 0; i < 4; i++)
                {
                    int tempVal = bytes[currentIndex++];
                    for (int j = 0; j < i; j++)
                    {
                        tempVal = tempVal << 8;
                    }
                    soundingData.depth += tempVal;
                }
                soundingData.depth /= baseFile.soundingMultiplicationFactor;
                soundingDatas.Add(soundingData);
            }
            return soundingDatas;
        }

        /*
        public bool IsBuoy
        {
            get
            {
                return Code == S57Objects.BOYCAR ||
                       Code == S57Objects.BOYLAT ||
                       Code == S57Objects.BOYINB ||
                       Code == S57Objects.BOYSPP ||
                       Code == S57Objects.BOYISD ||
                       Code == S57Objects.BOYSAW;
            }
        }

        public bool IsBeacon
        {
            get
            {
                return Code == S57Objects.BCNCAR ||
                    Code == S57Objects.BCNISD ||
                    Code == S57Objects.BCNLAT ||
                    Code == S57Objects.BCNSAW ||
                    Code == S57Objects.BCNSPP;
            }
        }

        public bool IsCardinal
        {
            get
            {
                return Code == S57Objects.BCNCAR || Code == S57Objects.BOYCAR;
            }
        }


        public bool IsLateral
        {
            get
            {
                return Code == S57Objects.BCNLAT || Code == S57Objects.BOYLAT;
            }
        }

        public bool IsSafeWater
        {
            get
            {
                return Code == S57Objects.BCNSAW || Code == S57Objects.BOYSAW;
            }
        }

        public bool IsIsolatedDanger
        {
            get {
                return Code == S57Objects.BCNISD || Code == S57Objects.BOYISD;
            }
        }

        public bool IsSpecialMark
        {
            get {
                return Code == S57Objects.BCNSPP || Code == S57Objects.BOYSPP; 
            }
        }

        public bool IsAggregation
        {
            get { return Code == S57Objects.C_AGGR; }
        }
        */        
    }
}
