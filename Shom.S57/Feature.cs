using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Shom.ISO8211;
using S57.File;

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
        // FRID : Feature Record Identifier Field
        public NAMEkey namekey;
        public DataRecord FeatureRecord;
        public Dictionary<uint, string> Attributes;     // ATTF Attributes
        public VectorRecordPointer enhVectorPtrs = null;
        public FeatureObjectPointer enhFeaturePtrs = null;
        public GeometricPrimitive Primitive;            // PRIM        // FOID : 
        public uint Group;                              // GRUP     
        public uint Code;                               // OBJL
        public LongName lnam;                           // FOID Object Identifier Field
        // some private variables  
        uint agen;
        uint fidn;
        uint fids;
        object[] subFieldRow;
        Dictionary<string, int> tagLookup;
        public Geometry GetGeometry()
        {
            int count;
            switch (Primitive)
            {
                case GeometricPrimitive.Point:
                    {
                        if (enhVectorPtrs.Values[0] == null || enhVectorPtrs.VectorList[0] == null) return null;
                        return enhVectorPtrs.VectorList[0].Geometry;
                    }
                case GeometricPrimitive.Line:
                    {
                        //initialize StartNode Pointer for later check how to build geometry
                        //VectorRecordPointer StartNode = new VectorRecordPointer();
                        Vector StartNode;
                        //initialize Contour to accumulate lines
                        Line Contour = new Line();
                        for (int i = 0; i < enhVectorPtrs.Values.Count; i++)
                        {
                            //first, check if vector exist, and if it is supposed to be visible 
                            //(to improve: masked points should still be added for correct topology, just not rendered later)
                            int mask = enhVectorPtrs.TagIndex["MASK"];
                            int ornt = enhVectorPtrs.TagIndex["ORNT"];
                            if (enhVectorPtrs.VectorList[i] == null || enhVectorPtrs.Values[i].GetUInt32(mask) == (uint)Masking.Mask) break;

                            //next, check if edge needs to be reversed for the intended usage 
                            //Do this on a copy to not mess up original record wich might be used elsewhere
                            var edge = new List<Point>((enhVectorPtrs.VectorList[i].Geometry as Line).points);
                            if (enhVectorPtrs.Values[i].GetUInt32(ornt) == (uint)Orientation.Reverse)
                            {
                                edge.Reverse();
                                //note: Reversing is not reversing the VectorRecordPointers, see S57 Spec 3.1 section 3.20 (i.e. index [0] is now end node instead of start node)
                                //therefore assign private StartNode pointer for some later checks 
                                StartNode = enhVectorPtrs.VectorList[i].enhVectorPtrs.VectorList[1];
                            }
                            else
                            {
                                StartNode = enhVectorPtrs.VectorList[i].enhVectorPtrs.VectorList[0];
                            }

                            //now check if Contour has already accumulated points, if not just add current edge
                            count = Contour.points.Count;
                            if (count > 0)
                            {
                                //now check if existing contour should be extended
                                if (Contour.points[count - 1] == StartNode.Geometry as Point)
                                {
                                    Contour.points.AddRange(edge.GetRange(1, edge.Count - 1));
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
                        Vector StartNode;
                        //initialize Contour to accumulate boundaries
                        Area Contour = new Area();
                        for (int i = 0; i < enhVectorPtrs.Values.Count; i++)
                        {
                            //first, check if vector exist, and if it is supposed to be visible
                            //(to improve: masked points should still be added for correct topology, just not rendered later)
                            int mask = enhVectorPtrs.TagIndex["MASK"];
                            int ornt = enhVectorPtrs.TagIndex["ORNT"];
                            if (enhVectorPtrs.VectorList[i] == null || enhVectorPtrs.Values[i].GetUInt32(mask) == (uint)Masking.Mask) break;

                            //next, check if edge needs to be reversed for the intended usage 
                            //Do this on a copy to not mess up original record wich might be used elsewhere
                            //var edge = new List<Point>((vectorPtr.Vector.Geometry as Line).points);

                            //var edge = new List<Point>((vectorPtr.Vector.Geometry as Line).points);
                            var edge = new List<Point>((enhVectorPtrs.VectorList[i].Geometry as Line).points);

                            if (enhVectorPtrs.Values[i].GetUInt32(ornt) == (uint)Orientation.Reverse)
                            {
                                edge.Reverse();
                                //note: Reversing is not reversing the VectorRecordPointers, see S57 Spec 3.1 section 3.20 (i.e. index [0] is now end node instead of start node)
                                //therefore assign private StartNode pointer for some later checks 
                                StartNode = enhVectorPtrs.VectorList[i].enhVectorPtrs.VectorList[1];
                            }
                            else
                            {
                                StartNode = enhVectorPtrs.VectorList[i].enhVectorPtrs.VectorList[0];
                            }

                            //now check if Contour has already accumulated points, if not just add current edge
                            count = Contour.points.Count;
                            if (count > 0)
                            {
                                //now check if existing contour should be extended, or if a new one for the next interior boundery should be started 
                                if (Contour.points[count - 1] == StartNode.Geometry as Point)
                                {
                                    Contour.points.AddRange(edge.GetRange(1, edge.Count - 1));
                                }
                                else
                                {
                                    //done, add current polygon boundery to ContourSet 
                                    //verify that polygone is closed last point in list equals first
                                    if (Contour.points[count - 1] == Contour.points[0])
                                    {
                                        ContourSet.Areas.Add(Contour);
                                    }
                                    else
                                    {
                                        //Debug.WriteLine("Panic: current polygon is not complete, and current egde is not extending it");
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
        public List<SoundingData> ExtractSoundings()
        {
            return enhVectorPtrs.VectorList[0].SoundingList;
        }

        public Feature(NAMEkey _namekey, DataRecord _FeatureRecord)
        {
            namekey = _namekey;
            FeatureRecord = _FeatureRecord;
            var fspt = _FeatureRecord.Fields.GetFieldByTag("FSPT");
            if (fspt != null)
                enhVectorPtrs = new VectorRecordPointer(fspt.subFields);
            var ffpt = _FeatureRecord.Fields.GetFieldByTag("FFPT");
            if (ffpt != null)
                enhFeaturePtrs = new FeatureObjectPointer(ffpt.subFields);
            // FRID : Feature Record Identifier
            var frid = _FeatureRecord.Fields.GetFieldByTag("FRID");
            if (frid != null)
            {
                Primitive = (GeometricPrimitive)frid.subFields.GetUInt32(0, "PRIM");
                Group = frid.subFields.GetUInt32(0, "GRUP");
                Code = frid.subFields.GetUInt32(0, "OBJL");
            }
            // FOID : Feature Object Identifier
            var foid = _FeatureRecord.Fields.GetFieldByTag("FOID");
            if (foid != null)
            {
                subFieldRow = foid.subFields.Values[0];
                tagLookup = foid.subFields.TagIndex;
                agen = subFieldRow.GetUInt32(tagLookup["AGEN"]);
                fidn = subFieldRow.GetUInt32(tagLookup["FIDN"]);
                fids = subFieldRow.GetUInt32(tagLookup["FIDS"]);
                lnam = new LongName(agen, fidn, fids);
            }
            // ATTF : Attributes
            var attr = _FeatureRecord.Fields.GetFieldByTag("ATTF");
            if (attr != null)
            {
                Attributes = Vector.GetAttributes(attr);
            }
            // NATF : National attributes NATF.
            var natf = _FeatureRecord.Fields.GetFieldByTag("NATF");
            if (natf != null)
            {
                var natfAttr = Vector.GetAttributes(natf);
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
        }
    }
    public class oldFeature
    {
        public BaseFile baseFile;
        public Cell cell;

        // FRID : Feature Record Identifier Field
        public NAMEkey namekey;
        public GeometricPrimitive Primitive;            // PRIM
        public uint Group;                              // GRUP     
        public uint Code;                               // OBJL


        // FOID : 
        public LongName lnam;                           // FOID Object Identifier Field
        public Dictionary<uint, string> Attributes;     // ATTF Attributes
        public List<oldFeatureRecordPointer> FeaturePtrs;      // <R> FFPT : Pointer Fields

        // some private variables  
        object[] subFieldRow;

        public override string ToString()
        {
            return Code + " " + lnam.ToString();
        }

        //
        // It might be necessary to go recursive on that one since a vector can be made of vectors...
        //
        //public double PositionAccuracy
        //{
        //    get {
        //        if (_positionAccuracy == uint.MaxValue)
        //        {
        //            uint positionAccuracy = uint.MinValue;
        //            foreach (var vectorPtr in enVectorPtrs)
        //            {
        //                var vector = vectorPtr.Vector;
        //                var posaccCode = S57Attributes.Get("POSACC").Code;
        //                if (vector.Attributes != null && vector.Attributes.ContainsKey(posaccCode))
        //                {
        //                    var sPosacc = vector.Attributes[posaccCode];
        //                    double posacc = double.Parse(sPosacc, CultureInfo.InvariantCulture );
        //                    if( posacc > positionAccuracy )
        //                    {
        //                        _positionAccuracy = posacc;
        //                    }
        //                }
        //            }
        //        }
        //        if (_positionAccuracy == uint.MaxValue)
        //        {
        //            _positionAccuracy = 10; // by default, positionAccuracy = 10 m
        //        }
        //        return _positionAccuracy;
        //    }
        //}

        //private double _positionAccuracy = uint.MaxValue;
        //    // From Vector Attributes

        //public string this[string i]                        // Attributes accessor thru acronym
        //{
        //    get
        //    {
        //        var attr = S57Attributes.Get(i);
        //        if (attr == null || !Attributes.ContainsKey(attr.Code))
        //        {
        //            return null;
        //        }
        //        return Attributes[attr.Code];
        //    }
        //}        

        public oldFeature(KeyValuePair<NAMEkey,DataRecord> frecord, BaseFile baseFile, Cell cell)
        {
            this.baseFile = baseFile;
            this.cell = cell;
            namekey = frecord.Key;        

            // <R> FFPT : Feature Record To Feature Object Pointer
            var ffpt = frecord.Value.Fields.GetFieldByTag("FFPT");
            if (ffpt != null) FeaturePtrs = GetFFPTs( ffpt );


        }

        // Link to Feature Objects
        public List<oldFeatureRecordPointer> GetFFPTs( DataField field )
        {
            if (field.Tag == "FFPT")
            {
                List<oldFeatureRecordPointer> result = new List<oldFeatureRecordPointer>();
                int lnam = field.subFields.TagIndex["LNAM"];
                int rind = field.subFields.TagIndex["RIND"];
                int comt = field.subFields.TagIndex["COMT"];
                for (int i = 0; i < field.subFields.Values.Count; i++)
                {
                    subFieldRow = field.subFields.Values[i];
                    oldFeatureRecordPointer featureLink = new oldFeatureRecordPointer();
                    featureLink.LNAM = new LongName(subFieldRow.GetBytes(lnam));
                    featureLink.RIND = (Relationship)subFieldRow.GetUInt32(rind);
                    featureLink.Comment = subFieldRow.GetString(comt);
                    result.Add(featureLink);
                }
                return result;
            }
            else return null;
        }
    }
}
