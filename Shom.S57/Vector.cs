using System;
using System.Collections.Generic;
using Shom.ISO8211;
using S57.File;

namespace S57
{
    //Takes a S57 Vector Record, and augments it with some data to link it to Vectors, Features, extract geometry etc
    // Vector
    //  0001 ISO/IEC 8211 Record
    //      VRID Vector Record Identifier
    //      <R> ATTV Vector Record Attributes
    //      VRPC Vector Record Pointer Control
    //      <R> VRPT Vector Record Pointer
    //      SGCC Coordinate Control
    //      <R> SG2D 2D Coordinate
    //      <R> SG3D 3D Coordinate 
    //      <R> ARCC Arc/Curve
    //          <R> AR2D
    //          <R> EL2D
    //          <R> CT2D

    public class Vector
    {
        public NAMEkey namekey;
        public DataRecord VectorRecord;
        public Dictionary<S57Att, string> Attributes;
        public VectorRecordPointer enhVectorPtrs = null;
        public Geometry geometry;
        public List<SoundingData> SoundingList;
        public Geometry Geometry
        {
            get
            {
                // Point ( IsolatedNode || ConnectedNode )
                if (namekey.RecordName == (uint)VectorType.isolatedNode || namekey.RecordName == (uint)VectorType.connectedNode)
                {
                    return geometry;
                }
                // Edge
                else if (namekey.RecordName == (uint)VectorType.edge)
                {
                    if (geometry != null && !(geometry is Point))
                    {
                        return geometry;
                    }
                    geometry = new Line();
                    if (enhVectorPtrs != null)
                    {
                        int mask = enhVectorPtrs.TagIndex["MASK"];
                        int name = enhVectorPtrs.TagIndex["NAME"];
                        for (int i = 0; i < enhVectorPtrs.Values.Count; i++)
                        {
                            if (enhVectorPtrs.Values[i].GetUInt32(mask) == (uint)Masking.Show ||
                               enhVectorPtrs.Values[i].GetUInt32(mask) == (uint)Masking.NotRelevant)
                            {
                                Vector vector = enhVectorPtrs.VectorList[i];
                                if (vector != null)
                                {
                                    ((Line)geometry).points.Add(vector.Geometry as Point);
                                }
                            }
                        }
                    }
                    return geometry;
                }
                return null;
            }
        }
        public Vector(NAMEkey _namekey, DataRecord _VectorRecord)
        {
            namekey = _namekey;
            VectorRecord = _VectorRecord;
            var vrpt = _VectorRecord.Fields.GetFieldByTag("VRPT");
            if (vrpt != null)
                enhVectorPtrs = new VectorRecordPointer(vrpt.subFields);
            var attv = _VectorRecord.Fields.GetFieldByTag("ATTV");
            if (attv != null)
            {
                Attributes = GetAttributes(attv);
            }
        }
        public static Dictionary<S57Att, string> GetAttributes(DataField field)
        {
            object[] subFieldRow;
            if (field.Tag == "ATTF" || field.Tag == "NATF" || field.Tag == "ATTV")
            {
                Dictionary<S57Att, string> values = new Dictionary<S57Att, string>();
                int attl = field.subFields.TagIndex["ATTL"];
                int atvl = field.subFields.TagIndex["ATVL"];
                for (int i = 0; i < field.subFields.Values.Count; i++)
                {
                    subFieldRow = field.subFields.Values[i];
                    values.Add((S57Att)subFieldRow.GetUInt32(attl), subFieldRow.GetString(atvl));
                }
                return values;
            }
            return null;
        }
    }    
}
