using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Shom.ISO8211;
using S57.File;

namespace S57
{
    public class Vector
    {
        public DataRecord _vr;

        public VectorName vectorName;
        public uint rver;
        public uint ruin;
        public Dictionary<uint, string> Attributes;

        public uint vectorPointerUpdateInstruction;
        public uint recordPointerIndex;
        public uint nbVectorRecordPointers;
        public List<VectorRecordPointer> VectorRecordPointers = null;

        private Geometry geometry;

        public DataRecord DataRecord
        {
            get { return _vr; }
        }

        // TO BE IMPROVED: interior boundaries, etc
        public Geometry Geometry
        {
            get
            {
                // Point ( IsolatedNode || ConnectedNode )
                if (vectorName.Type == VectorType.isolatedNode ||
                    vectorName.Type == VectorType.connectedNode)
                {
                    return geometry;
                }
                // Edge
                else if (vectorName.Type == VectorType.edge)
                {
                    if (geometry != null && !(geometry is Point) )
                    {
                        return geometry;
                    }
                    geometry = new Line();
                    if (VectorRecordPointers != null)
                    {
                        foreach (VectorRecordPointer vectorRecordPointer in VectorRecordPointers)
                        {
                            if (vectorRecordPointer.Mask == Masking.Show ||
                                vectorRecordPointer.Mask == Masking.NotRelevant)
                            {
                                var vector = vectorRecordPointer.Vector;
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

        public string this[string i] // Attributes accessor thru acronym
        {
            get
            {
                var attr = S57Attributes.Get(i);
                if (attr == null || Attributes == null || !Attributes.ContainsKey(attr.Code))
                {
                    return null;
                }
                return Attributes[attr.Code];
            }
        }

        public Vector(S57Reader reader, DataRecord vr, BaseFile baseFile) {
            _vr = vr;
            BuildFromDataRecord(reader, vr, baseFile );
        }

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

        public void BuildFromDataRecord( S57Reader reader, DataRecord vr, BaseFile baseFile )
        {
            // Record Identifier Field
            var vrid = vr.Fields.GetFieldByTag("VRID");
            if (vrid != null)
            {
                var rcnm = vrid.GetUInt32("RCNM");
                var rcid = vrid.GetUInt32("RCID");
                vectorName = new VectorName(rcnm, rcid);
                rver = vrid.GetUInt32("RVER");
                ruin = vrid.GetUInt32("RUIN");
            }

            // Attribute Field
            var attr = vr.Fields.GetFieldByTag("ATTV");
            if (attr != null)
            {
                Attributes = Feature.GetAttributes( attr, baseFile );
            }

            // VRPC Pointer Control Field
            var vrpc = vr.Fields.GetFieldByTag("VRPC");
            if (vrpc != null)
            {
                vectorPointerUpdateInstruction = vrpc.GetUInt32("VPUI");
                recordPointerIndex = vrpc.GetUInt32("VPIX");
                nbVectorRecordPointers = vrpc.GetUInt32("NVPT");
            }

            // <R> VRPT Pointer Field
            var vrpt = vr.Fields.GetFieldByTag("VRPT");
            if (vrpt != null)
            {
                VectorRecordPointers = GetVRPTs( vrpt );
            }

            // SGCC Coordinate Control Field
            var sgcc = vr.Fields.GetFieldByTag("SGCC");
            if (sgcc != null)
            {
                var coordinateUdpateInstruction = sgcc.GetUInt32("CCUI"); 
                var coordinateIndex = sgcc.GetUInt32("CCIX");
                var numberOfCoordinates = sgcc.GetUInt32("CCNC");
            }

            // Coordinate Fields
            var sg2d = vr.Fields.GetFieldByTag("SG2D");
            if (sg2d != null)
            {
                if (vectorName.Type == VectorType.connectedNode || vectorName.Type == VectorType.isolatedNode)
                {
                    var ycoo = sg2d.GetDouble("YCOO");
                    var xcoo = sg2d.GetDouble("XCOO");
                    geometry = new Point(xcoo, ycoo);
                }
                else
                {
                    var bytes = sg2d.Bytes;
                    int currentIndex = 0;
                    int length = bytes.Length - 1;
                    geometry = new Line();
                    var line = geometry as Line;
                    reader.BindVectorToVectorRecordPointsOf(this);
                    line.points.Add(VectorRecordPointers[0].Vector.geometry as Point);
                    while (currentIndex < length && bytes[currentIndex] != DataField.UnitTerminator)
                    {
                        var point = new Point();
                        for (int i = 0; i < 4; i++)
                        {
                            int tempVal = bytes[currentIndex++];
                            for (int j = 0; j < i; j++)
                            {
                                tempVal = tempVal << 8;
                            }
                            point.Y += tempVal;
                        }
                        point.Y /= baseFile.coordinateMultiplicationFactor;
                        for (int i = 0; i < 4; i++)
                        {
                            int tempVal = bytes[currentIndex++];
                            for (int j = 0; j < i; j++)
                            {
                                tempVal = tempVal << 8;
                            }
                            point.X += tempVal;
                        }
                        point.X /= baseFile.coordinateMultiplicationFactor;
                        line.points.Add(point);
                    }
                    line.points.Add(VectorRecordPointers[1].Vector.geometry as Point);
                }
            }
            else
            {
                var sg3d = vr.Fields.GetFieldByTag("SG3D");
                if (sg3d != null)
                {
                    var ycoo = sg3d.GetDouble("YCOO");
                    var xcoo = sg3d.GetDouble("XCOO");
                    geometry = new Point(xcoo, ycoo);
                }
                else
                {
                    var arcc = vr.Fields.GetFieldByTag("ARCC");
                    // Not managed
                }
            }

        }

        public List<VectorRecordPointer> GetVRPTs(DataField field)
        {
            List<VectorRecordPointer> result = new List<VectorRecordPointer>();
            if (field.Tag == "VRPT")
            {
                int currentIndex = 0;
                while (field.Bytes[currentIndex] != DataField.FieldTerminator && currentIndex < field.Bytes.Length)
                {
                    VectorRecordPointer vectorRecordPointer = new VectorRecordPointer();
                    var rcnm = field.Bytes[currentIndex++]; ;
                    uint unsignedValue = 0;
                    for (int i = 0; i < 4; i++)
                    {
                        UInt32 tempVal = field.Bytes[currentIndex++];
                        for (int j = 0; j < i; j++)
                        {
                            tempVal = tempVal << 8;
                        }
                        unsignedValue += tempVal;
                    }
                    var rcid = unsignedValue;
                    vectorRecordPointer.Name = new VectorName(rcnm, rcid);
                    vectorRecordPointer.Orientation = (Orientation)field.Bytes[currentIndex++];
                    vectorRecordPointer.Topology = (TopologyIndicator)field.Bytes[currentIndex++];
                    vectorRecordPointer.Usage = (Usage)field.Bytes[currentIndex++];
                    vectorRecordPointer.Mask = (Masking)field.Bytes[currentIndex++];
                    result.Add(vectorRecordPointer);
                }
            }
            return result;
        }
    }
}
