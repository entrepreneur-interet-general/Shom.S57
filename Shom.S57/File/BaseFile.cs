using System;
using System.IO;
using System.IO.Compression;
using System.Collections.Generic;
using Shom.ISO8211;
using System.Diagnostics;

namespace S57.File
{
    public enum S57LexicalLevel : uint
    {
        ASCIIText = 0,  // ASCII
        ISO8859 = 1,    // ISO 8859 part 1, Latin alphabet 1 repertoire (i.e. Western European Latin alphabet based languages.
        ISO10646 = 2    // Unicode (Universal Character Set repertoire UCS-2 implementation level 1)
    }
    
    
    public class BaseFile
    {
        public DataRecord DataSetGeneralInformationRecord = null;
        public DataRecord DataSetGeographicReferenceRecord = null;

        //2 dictinaries using either NAMEKey or LongName to point to a given value
        //reasons: features are referenced via LongName, update files referenced using NAMEKey
        private Dictionary<NAMEkey, Feature> eFeatureRecords; 
        public Dictionary<LongName, Feature> eFeatureObjects;

        private Dictionary<NAMEkey, Vector> eVectorRecords;

        // DSSI
        public VectorDataStructure vectorDataStructure;
        public S57LexicalLevel ATTFLexicalLevel;
        public S57LexicalLevel NATFLexicalLevel;
        public uint numberOfMetaRecords;
        public uint numberOfCartographicRecords;
        public uint numberOfGeoRecords;
        public uint numberOfCollectionRecords;
        public uint numberOfIsolatedNodeRecords;
        public uint numberOfConnectedNodeRecords;
        public uint numberOfEdgeRecords;
        public uint numberOfFaceRecords;

        // DSPM
        public CoordinateUnits coordinateUnits;
        public uint coordinateMultiplicationFactor;
        public uint soundingMultiplicationFactor;
        public uint compilationScaleOfData;
        public uint unitsOfPositionalAccuracy;
        public uint unitsOfDepthMeasurement;
        public uint unitsOfHeightMeasurement;
        public uint horizontalGeodeticDatum;
        public uint verticalDatum;
        public uint soundingDatum;

        // some private variables  
        DataField vrid, frid;
        uint rcnm;
        uint rcid;
        SFcontainer[] subFieldRow;
        List<string> tagLookup;

        public BaseFile(Iso8211Reader reader)
        {
            //Current this works because we know the two records are special
            DataSetGeneralInformationRecord = reader.ReadDataRecord();
            var dssi = DataSetGeneralInformationRecord.Fields.GetFieldByTag("DSSI");
            if (dssi != null)
            {
                subFieldRow = dssi.subFields.Values[0];
                tagLookup = dssi.subFields.TagIndex;
                vectorDataStructure = (VectorDataStructure)subFieldRow.GetUInt32(tagLookup.IndexOf("DSTR"));
                ATTFLexicalLevel = (S57LexicalLevel)subFieldRow.GetUInt32(tagLookup.IndexOf("AALL"));
                NATFLexicalLevel = (S57LexicalLevel)subFieldRow.GetUInt32(tagLookup.IndexOf("NALL"));
                numberOfMetaRecords = subFieldRow.GetUInt32(tagLookup.IndexOf("NOMR"));
                numberOfCartographicRecords = subFieldRow.GetUInt32(tagLookup.IndexOf("NOCR"));
                numberOfGeoRecords = subFieldRow.GetUInt32(tagLookup.IndexOf("NOGR"));
                numberOfCollectionRecords = subFieldRow.GetUInt32(tagLookup.IndexOf("NOLR"));
                numberOfIsolatedNodeRecords = subFieldRow.GetUInt32(tagLookup.IndexOf("NOIN"));
                numberOfConnectedNodeRecords = subFieldRow.GetUInt32(tagLookup.IndexOf("NOCN"));
                numberOfEdgeRecords = subFieldRow.GetUInt32(tagLookup.IndexOf("NOED"));
                numberOfFaceRecords = subFieldRow.GetUInt32(tagLookup.IndexOf("NOFA"));
            }

            DataSetGeographicReferenceRecord = reader.ReadDataRecord();
            var dspm = DataSetGeographicReferenceRecord.Fields.GetFieldByTag("DSPM");
            if (dspm != null)
            {
                subFieldRow = dspm.subFields.Values[0];
                tagLookup = dspm.subFields.TagIndex;
                horizontalGeodeticDatum = subFieldRow.GetUInt32(tagLookup.IndexOf("HDAT"));
                verticalDatum = subFieldRow.GetUInt32(tagLookup.IndexOf("VDAT"));
                soundingDatum = subFieldRow.GetUInt32(tagLookup.IndexOf("SDAT"));
                compilationScaleOfData = subFieldRow.GetUInt32(tagLookup.IndexOf("CSCL"));
                unitsOfDepthMeasurement = subFieldRow.GetUInt32(tagLookup.IndexOf("DUNI"));
                unitsOfHeightMeasurement = subFieldRow.GetUInt32(tagLookup.IndexOf("HUNI"));
                unitsOfPositionalAccuracy = subFieldRow.GetUInt32(tagLookup.IndexOf("PUNI"));
                coordinateUnits = (CoordinateUnits)subFieldRow.GetUInt32(tagLookup.IndexOf("COUN"));
                coordinateMultiplicationFactor = subFieldRow.GetUInt32(tagLookup.IndexOf("COMF"));
                soundingMultiplicationFactor = subFieldRow.GetUInt32(tagLookup.IndexOf("SOMF"));
                // COMT
            }

            // DSPR Dataset projection
            // DSRC Dataset registration control
            // DSHT Dataset history
            // DSAC Dataset accuracy
            // CATD catalogue directory
            // CATX Catalogue cross reference

            eFeatureRecords = new Dictionary<NAMEkey, Feature>();
            eFeatureObjects = new Dictionary<LongName, Feature>();
            eVectorRecords = new Dictionary<NAMEkey, Vector>();

            var nextRec = reader.ReadDataRecord(); 

            while (nextRec != null)
            {
                if(nextRec.Fields.FindFieldByTag("VRID"))
                {
                    vrid = nextRec.Fields.GetFieldByTag("VRID");
                    rcnm = vrid.subFields.GetUInt32(0, "RCNM");
                    rcid = vrid.subFields.GetUInt32(0, "RCID");
                    var key = new NAMEkey(rcnm, rcid);
                    Vector newVec = new Vector(key, nextRec);
                    eVectorRecords.Add(key, newVec);
                }
                else
                {
                    if (nextRec.Fields.FindFieldByTag("FRID"))
                    {
                        frid = nextRec.Fields.GetFieldByTag("FRID");
                        rcnm = frid.subFields.GetUInt32(0, "RCNM");
                        rcid = frid.subFields.GetUInt32(0, "RCID");
                        //consider using lnam as key (from FOID field), challenge: update files deleting a feature record do not encode lnam of that feature record
                        var key = new NAMEkey(rcnm, rcid); 
                        Feature newFeat = new Feature(key, nextRec);
                        eFeatureRecords.Add(key, newFeat);
                    }
                }
                nextRec = reader.ReadDataRecord();
            }
        }
        public void BindVectorPointersOfVectors()
        {
            foreach (var eVec in eVectorRecords)
            {
                if (eVec.Value.enhVectorPtrs != null)
                {
                    int name = eVec.Value.enhVectorPtrs.TagIndex.IndexOf("NAME");
                    eVec.Value.enhVectorPtrs.VectorList = new List<Vector>(); //to ensure updates do not extend the list
                    foreach (var ptr in eVec.Value.enhVectorPtrs.Values)
                    {
                        Vector target = eVectorRecords[new NAMEkey(ptr.GetBytes(name))];
                        eVec.Value.enhVectorPtrs.VectorList.Add(target);
                    }
                }
            }
        }
        public void BindVectorPointersOfFeatures()
        {
            foreach (var eFeat in eFeatureRecords)
            {
                if (eFeat.Value.enhVectorPtrs != null)
                {
                    int name = eFeat.Value.enhVectorPtrs.TagIndex.IndexOf("NAME");
                    eFeat.Value.enhVectorPtrs.VectorList = new List<Vector>(); //to ensure updates do not extend the list
                    foreach (var ptr in eFeat.Value.enhVectorPtrs.Values)
                    {
                        Vector target = eVectorRecords[new NAMEkey(ptr.GetBytes(name))];
                        eFeat.Value.enhVectorPtrs.VectorList.Add(target);
                    }
                }
            }
        }
        public void BindFeatureObjectPointers()
        {
            //features in eFeatureRecords are keyed by NAMEkey. create new dictionary to key them via LongName
            //essential to speadup Feature lookup because Features are linked via LongName
            foreach (var eFeat in eFeatureRecords)
            {
                eFeatureObjects.Add(eFeat.Value.lnam, eFeat.Value);
            }
            foreach (var eFeat in eFeatureObjects)
            {
                if (eFeat.Value.enhFeaturePtrs != null)
                {
                    int lnam = eFeat.Value.enhFeaturePtrs.TagIndex.IndexOf("LNAM");
                    eFeat.Value.enhFeaturePtrs.FeatureList = new List<Feature>(); //to ensure updates do not extend the list
                    foreach (var ptr in eFeat.Value.enhFeaturePtrs.Values)
                    {
                        Feature target = eFeatureObjects[new LongName(ptr.GetBytes(lnam))];
                        eFeat.Value.enhFeaturePtrs.FeatureList.Add(target);
                    }
                }
            }
            //var test = new Dictionary<LongName, Feature>();
            //int cc = 0;
            //foreach (var eFeat in eFeatureObjects)
            //{
            //    cc++;
            //    if (eFeat.Value.enhFeaturePtrs != null)
            //    {
            //        if (eFeat.Value.enhFeaturePtrs.FeatureList.Count > 0)
            //        {
            //            test.Add(eFeat.Value.lnam, eFeat.Value);
            //            string type = ((S57Obj)(eFeat.Value.ObjectCode)).ToString();
            //            //string name = eFeat.Key.FeatureIdentificationNumber.ToString();
            //            string rcid = eFeat.Value.namekey.RecordIdentificationNumber.ToString();
            //            string description = null;
            //            eFeat.Value.Attributes.TryGetValue(S57Att.OBJNAM, out description);
            //            string targetinfo = null;
            //            int counter = 0;
            //            foreach (var bla in eFeat.Value.enhFeaturePtrs.FeatureList)
            //            {
            //                string relationship = ((Relationship)eFeat.Value.FeatureRecord.Fields.GetFieldByTag("FFPT").subFields.GetUInt32(counter++, "RIND")).ToString();
            //                //string targetrelationship = ((Relationship)bla.FeatureRecord.Fields.GetFieldByTag("FFPT").subFields.GetUInt32(0, "RIND")).ToString();
            //                string targettype = ((S57Obj)(bla.ObjectCode)).ToString();
            //                //string targetname = bla.lnam.FeatureIdentificationNumber.ToString();
            //                string targetname = bla.namekey.RecordIdentificationNumber.ToString();
            //                targetinfo = targetinfo + targetname + "." + targettype + "." + relationship + " ";
            //            }
            //            Console.WriteLine(rcid + " " + type + " " + description + targetinfo);
            //        }
            //    }
            //}
        }
        public void BuildVectorGeometry()
        {
            foreach (var eVec in eVectorRecords)
            {
                //check if geometry has already been extracted 
                if (eVec.Value.geometry == null)
                {
                    //first verify that geometry for Vectorpointer has been calculated yet. if not, calculate it
                    if (eVec.Value.enhVectorPtrs != null)
                    {
                        foreach (var testnode in eVec.Value.enhVectorPtrs.VectorList)
                        {
                            if (testnode.Geometry == null)
                                ExtractGeometry(new KeyValuePair<NAMEkey, Vector>(testnode.namekey, testnode));
                        }
                    }
                    //once all is good, extract the geometry for this node
                    ExtractGeometry(eVec);
                }                
            }
        }
        private void ExtractGeometry(KeyValuePair<NAMEkey, Vector> eVec)
        {
            var sg2d = eVec.Value.VectorRecord.Fields.GetFieldByTag("SG2D");
            if (sg2d != null)
            {
                if (eVec.Key.RecordName == (uint)VectorType.connectedNode || eVec.Key.RecordName == (uint)VectorType.isolatedNode)
                {
                    eVec.Value.geometry = GetNodeGeometry(sg2d);
                }
                else //edge or face
                {
                    eVec.Value.geometry = GetFaceGeometry(eVec, sg2d);
                }
            }

            var sg3d = eVec.Value.VectorRecord.Fields.GetFieldByTag("SG3D");
            //var sg3d = vrecord.Value.Fields.GetFieldByTag("SG3D");
            if (sg3d != null)
            {
                eVec.Value.SoundingList = new List<SoundingData>();
                int ycoo = sg3d.subFields.TagIndex.IndexOf("YCOO");
                int xcoo = sg3d.subFields.TagIndex.IndexOf("XCOO");
                int ve3d = sg3d.subFields.TagIndex.IndexOf("VE3D");
                for (int i = 0; i < sg3d.subFields.Values.Count; i++)
                {
                    var point = new SoundingData();
                    subFieldRow = sg3d.subFields.Values[i];
                    point.Y = subFieldRow.GetDouble(ycoo) / coordinateMultiplicationFactor;
                    point.X = subFieldRow.GetDouble(xcoo) / coordinateMultiplicationFactor;
                    point.depth = subFieldRow.GetDouble(ve3d) / soundingMultiplicationFactor;
                    eVec.Value.SoundingList.Add(point);
		    // to print in file or in console the soundings point information, you should uncomment the next line.
		    //string l = point.X +", "+ point.Y + ", "+ point.depth;
                }
            }
        }
        private Point GetNodeGeometry(DataField sg2d)
        {
            Point point = new Point();
            int ycoo = sg2d.subFields.TagIndex.IndexOf("YCOO");
            int xcoo = sg2d.subFields.TagIndex.IndexOf("XCOO");
            subFieldRow = sg2d.subFields.Values[0];
            point.Y = subFieldRow.GetDouble(ycoo) / coordinateMultiplicationFactor;
            point.X = subFieldRow.GetDouble(xcoo) / coordinateMultiplicationFactor;
            return point;
        }
        private Line GetFaceGeometry(KeyValuePair<NAMEkey, Vector> _eVec, DataField sg2d)
        {
            Line line = new Line();
            int ycoo = sg2d.subFields.TagIndex.IndexOf("YCOO");
            int xcoo = sg2d.subFields.TagIndex.IndexOf("XCOO");
            line.points.Add(_eVec.Value.enhVectorPtrs.VectorList[0].geometry as Point); //will fail when geometry of pointed Vector has not been calculated yet
            for (int i = 0; i < sg2d.subFields.Values.Count; i++)
            {
                var point = new Point();
                subFieldRow = sg2d.subFields.Values[i];
                point.Y = subFieldRow.GetDouble(ycoo) / coordinateMultiplicationFactor;
                point.X = subFieldRow.GetDouble(xcoo) / coordinateMultiplicationFactor;
                line.points.Add(point);
            }
            line.points.Add(_eVec.Value.enhVectorPtrs.VectorList[1].geometry as Point);
            return line;
        }
        public void ApplyUpdateFile(UpdateFile updateFile)
        {
            DataField target_vrid, target_frid;
            RecordUpdate ruin;
            uint rver, target_rver;

            DataField attv, target_attv;
            DataField attf, target_attf;
            DataField natf, target_natf;
            bool attribute_found = false;

            DataField vrpt, target_vrpt, vrpc;
            RecordUpdate vpui;
            int vpix, nvpt;

            DataField coordinatefield, target_coordinatefield, sgcc;
            RecordUpdate ccui;
            int ccix, ccnc;

            DataField ffpt, target_ffpt, ffpc;
            RecordUpdate ffui;
            int ffix, nfpt;

            DataField fspt, target_fspt, fspc;
            RecordUpdate fsui;
            int fsix, nspt;

            SFcontainer[] target_row;

            //throw new NotImplementedException("ApplyUpdateFile not implemented");
            foreach (DataRecord vr in updateFile.UpdateVectorRecords)
            {
                // Record Identifier Field
                vrid = vr.Fields.GetFieldByTag("VRID");
                subFieldRow = vrid.subFields.Values[0];
                tagLookup = vrid.subFields.TagIndex;
                rcnm = subFieldRow.GetUInt32(tagLookup.IndexOf("RCNM"));
                rcid = subFieldRow.GetUInt32(tagLookup.IndexOf("RCID"));
                rver = subFieldRow.GetUInt32(tagLookup.IndexOf("RVER"));
                ruin = (RecordUpdate)subFieldRow.GetUInt32(tagLookup.IndexOf("RUIN"));
                NAMEkey updateNAMEkey = new NAMEkey(rcnm, rcid);
                if (ruin == RecordUpdate.Insert)
                {
                    Vector newVec = new Vector(updateNAMEkey, vr);
                    eVectorRecords.Add(updateNAMEkey, newVec);
                }
                else
                {
                    target_vrid = eVectorRecords[updateNAMEkey].VectorRecord.Fields.GetFieldByTag("VRID");
                    target_rver = target_vrid.subFields.GetUInt32(0, "RVER");
                    if (ruin == RecordUpdate.Delete)
                    {
                        if (target_rver == rver - 1)
                        {
                            eVectorRecords.Remove(updateNAMEkey);
                        }
                    }
                    else if (ruin == RecordUpdate.Modify)
                    {
                        attv = vr.Fields.GetFieldByTag("ATTV"); //attribute update not tested
                        if (attv != null && target_rver == rver - 1)
                        {
                            int atvl, target_atvl;
                            int attl, target_attl;
                            target_attv = eVectorRecords[updateNAMEkey].VectorRecord.Fields.GetFieldByTag("ATTV");
                            if (target_attv == null)
                            {
                                eVectorRecords[updateNAMEkey].VectorRecord.Fields.Add(attv);
                            }
                            else
                            {
                                attl = attv.subFields.TagIndex.IndexOf("ATTL");
                                target_attl = target_attv.subFields.TagIndex.IndexOf("ATTL");
                                atvl = attv.subFields.TagIndex.IndexOf("ATVL");
                                target_atvl = target_attv.subFields.TagIndex.IndexOf("ATVL");
                                foreach (var row in attv.subFields.Values)
                                {
                                    attribute_found = false;
                                    for (int i = 0; i < target_attv.subFields.Values.Count; i++)
                                    {
                                        target_row = target_attv.subFields.Values[i];
                                        if (target_row[target_attl].Equals(row[attl])) //if attribute found, overwrite value
                                        {
                                            target_attv.subFields.Values[i][target_atvl] = row[atvl];
                                            attribute_found = true;
                                            break;
                                        }
                                    }
                                    if (!attribute_found)
                                    {
                                        target_attv.subFields.Values.Add(row); //risky, assumes same indices of ATTL and ATVl in source and target
                                    }
                                }
                            }
                        }

                        // VRPC update instruction Pointer Control Field
                        vrpc = vr.Fields.GetFieldByTag("VRPC");
                        if (vrpc != null && target_rver == rver - 1)
                        {
                            vpui = (RecordUpdate)vrpc.subFields.GetUInt32(0, "VPUI");
                            vpix = (int)vrpc.subFields.GetUInt32(0, "VPIX") - 1; //c sharp indices start at 0 while S57 indices start at 1
                            nvpt = (int)vrpc.subFields.GetUInt32(0, "NVPT"); //number of vector record pointers to patch
                            vrpt = vr.Fields.GetFieldByTag("VRPT");
                            target_vrpt = eVectorRecords[updateNAMEkey].VectorRecord.Fields.GetFieldByTag("VRPT");
                            if (vpui == RecordUpdate.Insert)
                            {
                                if (target_vrpt == null) //insert can also mean add to non-existing VRPT.
                                {
                                    eFeatureRecords[updateNAMEkey].FeatureRecord.Fields.Add(vrpt);
                                }
                                else
                                {
                                    for (int i = 0; i < nvpt; i++)
                                    {
                                        target_vrpt.subFields.Values.Insert(vpix + i, vrpt.subFields.Values[i]); //maybe risky, assumes same subfield order in source and target
                                    }
                                }
                            }
                            else if (vpui == RecordUpdate.Delete)
                            {
                                for (int i = 0; i < nvpt; i++)
                                {
                                    target_vrpt.subFields.Values.RemoveAt(vpix);
                                }
                            }
                            else if (vpui == RecordUpdate.Modify)
                            {
                                for (int i = 0; i < nvpt; i++)
                                {
                                    target_vrpt.subFields.Values[vpix + i] = vrpt.subFields.Values[i]; //maybe risky, assumes same subfield order in source and target
                                }
                            }
                        }

                        // SGCC update instruction Coordinate Control Field
                        sgcc = vr.Fields.GetFieldByTag("SGCC");
                        if (sgcc != null && target_rver == rver - 1)
                        {
                            ccui = (RecordUpdate)sgcc.subFields.GetUInt32(0, "CCUI");
                            ccix = (int)sgcc.subFields.GetUInt32(0, "CCIX") - 1; //c sharp indices start at 0 while S57 indices start at 1
                            ccnc = (int)sgcc.subFields.GetUInt32(0, "CCNC"); //number of vector record pointers to patch
                            coordinatefield = vr.Fields.GetFieldByTag("SG2D");
                            target_coordinatefield = eVectorRecords[updateNAMEkey].VectorRecord.Fields.GetFieldByTag("SG2D");
                            if (target_coordinatefield == null)
                            {
                                coordinatefield = vr.Fields.GetFieldByTag("SG3D");
                                target_coordinatefield = eVectorRecords[updateNAMEkey].VectorRecord.Fields.GetFieldByTag("SG3D");
                            }
                            if (ccui == RecordUpdate.Insert)
                            {
                                if (target_coordinatefield == null) //insert can also mean add to non-existing coordinatefield.
                                {
                                    eFeatureRecords[updateNAMEkey].FeatureRecord.Fields.Add(coordinatefield);
                                }
                                else
                                {
                                    for (int i = 0; i < ccnc; i++)
                                    {
                                        target_coordinatefield.subFields.Values.Insert(ccix + i, coordinatefield.subFields.Values[i]); //maybe risky, assumes same subfield order in source and target
                                    }
                                }
                            }
                            else if (ccui == RecordUpdate.Delete)
                            {
                                for (int i = 0; i < ccnc; i++)
                                {
                                    target_coordinatefield.subFields.Values.RemoveAt(ccix);
                                }
                            }
                            else if (ccui == RecordUpdate.Modify)
                            {
                                for (int i = 0; i < ccnc; i++)
                                {
                                    target_coordinatefield.subFields.Values[ccix + i] = coordinatefield.subFields.Values[i]; //risky, assumes same subfield order in source and target
                                }
                            }
                        }
                        target_vrid.subFields.Values[0][target_vrid.subFields.TagIndex.IndexOf("RVER")].uintValue = rver;
                    }
                }
            }

            foreach (DataRecord fr in updateFile.UpdateFeatureRecords)
            {
                // Record Identifier Field
                frid = fr.Fields.GetFieldByTag("FRID");
                subFieldRow = frid.subFields.Values[0];
                tagLookup = frid.subFields.TagIndex;
                rcnm = subFieldRow.GetUInt32(tagLookup.IndexOf("RCNM"));
                rcid = subFieldRow.GetUInt32(tagLookup.IndexOf("RCID"));
                rver = subFieldRow.GetUInt32(tagLookup.IndexOf("RVER"));
                ruin = (RecordUpdate)subFieldRow.GetUInt32(tagLookup.IndexOf("RUIN"));
                NAMEkey updateNAMEkey = new NAMEkey(rcnm, rcid);
                if (ruin == RecordUpdate.Insert)
                {
                    Feature newFeat = new Feature(updateNAMEkey, fr);
                    eFeatureRecords.Add(updateNAMEkey, newFeat);
                }
                else
                {
                    target_frid = eFeatureRecords[updateNAMEkey].FeatureRecord.Fields.GetFieldByTag("FRID");
                    target_rver = target_frid.subFields.GetUInt32(0, "RVER");
                    if (ruin == RecordUpdate.Delete)
                    {
                        if (target_rver == rver - 1)
                        {
                            eFeatureRecords.Remove(updateNAMEkey);
                        }

                    }
                    else if (ruin == RecordUpdate.Modify)
                    {
                        attf = fr.Fields.GetFieldByTag("ATTF"); //attribute update not tested
                        if (attf != null && target_rver == rver - 1)
                        {
                            int atvl, target_atvl;
                            int attl, target_attl;
                            target_attf = eFeatureRecords[updateNAMEkey].FeatureRecord.Fields.GetFieldByTag("ATTF");
                            if (target_attf == null)
                            {
                                eFeatureRecords[updateNAMEkey].FeatureRecord.Fields.Add(attf);
                            }
                            else
                            {
                                attl = attf.subFields.TagIndex.IndexOf("ATTL");
                                target_attl = target_attf.subFields.TagIndex.IndexOf("ATTL");
                                atvl = attf.subFields.TagIndex.IndexOf("ATVL");
                                target_atvl = target_attf.subFields.TagIndex.IndexOf("ATVL");
                                foreach (var row in attf.subFields.Values)
                                {
                                    attribute_found = false;
                                    for (int i = 0; i < target_attf.subFields.Values.Count; i++)
                                    {
                                        target_row = target_attf.subFields.Values[i];
                                        if (target_row[target_attl].Equals(row[attl])) //if attribute found, overwrite value
                                        {
                                            target_attf.subFields.Values[i][target_atvl] = row[atvl];
                                            attribute_found = true;
                                            break;
                                        }
                                    }
                                    if (!attribute_found)
                                    {
                                        target_attf.subFields.Values.Add(row); //risky, assumes same indices of ATTL and ATVl in source and target
                                    }
                                }
                            }
                        }
                        natf = fr.Fields.GetFieldByTag("NATF"); //attribute update not tested
                        if (natf != null && target_rver == rver - 1)
                        {
                            int atvl, target_atvl;
                            int attl, target_attl;
                            target_natf = eFeatureRecords[updateNAMEkey].FeatureRecord.Fields.GetFieldByTag("NATF");
                            if (target_natf == null)
                            {
                                eFeatureRecords[updateNAMEkey].FeatureRecord.Fields.Add(natf);
                            }
                            else
                            {
                                attl = natf.subFields.TagIndex.IndexOf("ATTL");
                                target_attl = target_natf.subFields.TagIndex.IndexOf("ATTL");
                                atvl = natf.subFields.TagIndex.IndexOf("ATVL");
                                target_atvl = target_natf.subFields.TagIndex.IndexOf("ATVL");
                                foreach (var row in natf.subFields.Values)
                                {
                                    attribute_found = false;
                                    for (int i = 0; i < target_natf.subFields.Values.Count; i++)
                                    {
                                        target_row = target_natf.subFields.Values[i];
                                        if (target_row[target_attl].Equals(row[attl])) //if attribute found, overwrite value
                                        {
                                            target_natf.subFields.Values[i][target_atvl] = row[atvl];
                                            attribute_found = true;
                                            break;
                                        }
                                    }
                                    if (!attribute_found)
                                    {
                                        target_natf.subFields.Values.Add(row); //risky, assumes same indices of ATTL and ATVl in source and target
                                    }
                                }
                            }
                        }

                        // FFPC update instruction Pointer Control Field
                        ffpc = fr.Fields.GetFieldByTag("FFPC");
                        if (ffpc != null && target_rver == rver - 1)
                        {
                            ffui = (RecordUpdate)ffpc.subFields.GetUInt32(0, "FFUI");
                            ffix = (int)ffpc.subFields.GetUInt32(0, "FFIX") - 1; //c sharp indices start at 0 while S57 indices start at 1
                            nfpt = (int)ffpc.subFields.GetUInt32(0, "NFPT"); //number of vector record pointers to patch
                            ffpt = fr.Fields.GetFieldByTag("FFPT");
                            target_ffpt = eFeatureRecords[updateNAMEkey].FeatureRecord.Fields.GetFieldByTag("FFPT");
                            var target = eFeatureRecords[updateNAMEkey];
                            if (ffui == RecordUpdate.Insert) 
                            {
                                if (target_ffpt == null) //insert can also mean add to non-existing FFPT.
                                {
                                    eFeatureRecords[updateNAMEkey].FeatureRecord.Fields.Add(ffpt);
                                }
                                else
                                {
                                    for (int i = 0; i < nfpt; i++)
                                    {
                                        target_ffpt.subFields.Values.Insert(ffix + i, ffpt.subFields.Values[i]); //maybe risky, assumes same subfield order in source and target
                                    }
                                }
                            }
                            else if (ffui == RecordUpdate.Delete)
                            {
                                for (int i = 0; i < nfpt; i++)
                                {
                                    target_ffpt.subFields.Values.RemoveAt(ffix);
                                }
                            }
                            else if (ffui == RecordUpdate.Modify)
                            {
                                for (int i = 0; i < nfpt; i++)
                                {
                                    target_ffpt.subFields.Values[ffix + i] = ffpt.subFields.Values[i]; //maybe risky, assumes same subfield order in source and target
                                }
                            }
                        }

                        // FSPC update instruction Pointer Control Field
                        fspc = fr.Fields.GetFieldByTag("FSPC");
                        if (fspc != null && target_rver == rver - 1)
                        {
                            fsui = (RecordUpdate)fspc.subFields.GetUInt32(0, "FSUI");
                            fsix = (int)fspc.subFields.GetUInt32(0, "FSIX") - 1; //c sharp indices start at 0 while S57 indices start at 1
                            nspt = (int)fspc.subFields.GetUInt32(0, "NSPT"); //number of vector record pointers to patch
                            fspt = fr.Fields.GetFieldByTag("FSPT");
                            target_fspt = eFeatureRecords[updateNAMEkey].FeatureRecord.Fields.GetFieldByTag("FSPT");
                            if (fsui == RecordUpdate.Insert)
                            {
                                if (target_fspt == null) //insert can also mean add to non-existing FSPT.
                                {
                                    eFeatureRecords[updateNAMEkey].FeatureRecord.Fields.Add(fspt);
                                }
                                else
                                {
                                    for (int i = 0; i < nspt; i++)
                                    {
                                        target_fspt.subFields.Values.Insert(fsix + i, fspt.subFields.Values[i]); //maybe risky, assumes same subfield order in source and target
                                    }
                                }
                            }
                            else if (fsui == RecordUpdate.Delete)
                            {
                                for (int i = 0; i < nspt; i++)
                                {
                                    target_fspt.subFields.Values.RemoveAt(fsix);
                                }
                            }
                            else if (fsui == RecordUpdate.Modify)
                            {
                                for (int i = 0; i < nspt; i++)
                                {
                                    target_fspt.subFields.Values[fsix + i] = fspt.subFields.Values[i]; //maybe risky, assumes same subfield order in source and target
                                }
                            }
                        }
                        target_frid.subFields.Values[0][target_frid.subFields.TagIndex.IndexOf("RVER")].uintValue = rver;
                    }
                }
            }
        }
    }
}
