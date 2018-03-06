using System;
using System.Collections.Generic;
using System.Linq;
using Client.Models;
using Client.Logging;
using System.IO;

namespace Client.Common
{
    //shortname for tan store
    public static class T
    {
        public static int TanId { get; set; }
        public static string TanNumber { get; set; }
        public static string CanNumber { get; set; }
        public static string BatchNumber { get; set; }
        public static string Curator { get; set; }
        public static string Reviewer { get; set; }
        public static string QC { get; set; }
        public static int Version { get; set; }

        private static Dictionary<string, TanChemicalVM> TanChemicalDict = new Dictionary<string, TanChemicalVM>();
        public static string TanDataFilePath
        {
            get
            {
                string localPath = Path.Combine(C.LocalStoragePath, TanId.ToString(), TanId + ".json");
                if (!Directory.Exists(Path.GetDirectoryName(localPath)))
                    Directory.CreateDirectory(Path.GetDirectoryName(localPath));
                return localPath;
            }
        }

        public static string TanDataFilePathBackUp
        {
            get
            {
                string localPath = Path.Combine(C.LocalStoragePath, TanId.ToString(), TanId + DateTime.Now.ToString("yyyyMMddHHmmssfff") + "_BackUp.json");
                if (!Directory.Exists(Path.GetDirectoryName(localPath)))
                    Directory.CreateDirectory(Path.GetDirectoryName(localPath));
                return localPath;
            }
        }

        public static string NumsExportPdfPath
        {
            get
            {
                string localPath = Path.Combine(C.LocalStoragePath, TanId.ToString(), TanNumber + "_NumExport.pdf");
                if (!Directory.Exists(Path.GetDirectoryName(localPath)))
                    Directory.CreateDirectory(Path.GetDirectoryName(localPath));
                return localPath;
            }
        }

        public static string MasterTanDataFilePath
        {
            get
            {
                string localPath = Path.Combine(C.LocalStoragePath, TanId.ToString(), ($"MasterTan_{TanId}.json"));
                if (!Directory.Exists(Path.GetDirectoryName(localPath)))
                    Directory.CreateDirectory(Path.GetDirectoryName(localPath));
                return localPath;
            }
        }

        public static string TanFolderpath
        {
            get
            {
                string localPath = Path.Combine(C.LocalStoragePath, TanId.ToString());
                if (!Directory.Exists(localPath))
                    Directory.CreateDirectory(localPath);
                return localPath;
            }
        }

        public static void ResetTanChemicalDict()
        {
            try
            {
                TanChemicalDict.Clear();
                foreach (var entry in S.ChemicalDict)
                {
                    var tanChemicalVM = new TanChemicalVM
                    {
                        AllImagePaths = entry.Value.AllImagePaths,
                        ChemicalmataData = entry.Value.ChemicalmataData,
                        ChemicalType = entry.Value.ChemicalType,
                        CompoundNo = entry.Value.CompoundNo,
                        GenericName = entry.Value.GenericName,
                        Id = entry.Value.Id,
                        ImagePath = entry.Value.ImagePath,
                        Images = entry.Value.Images,
                        InChiKey = entry.Value.InChiKey,
                        MolString = entry.Value.MolString,
                        Name = entry.Value.Name,
                        NUM = entry.Value.NUM,
                        RegNumber = entry.Value.RegNumber,
                        SearchName = entry.Value.SearchName,
                        StructureType = entry.Value.StructureType
                    };
                    TanChemicalDict[entry.Key] = tanChemicalVM;
                }
            }
            catch (Exception ex)
            {
                Log.This(ex);
            }
        }
        public static Dictionary<string, TanChemicalVM> GetDictionary()
        {
            return TanChemicalDict;
        }
        public static TanChemicalVM FindByInchiKey(string inchiKey)
        {
            try
            {
                var S9000Chemicals = TanChemicalDict.Where(entry => entry.Value.ChemicalType == DTO.ChemicalType.S9000);
                var S8500Chemicals = TanChemicalDict.Where(entry => entry.Value.ChemicalType == DTO.ChemicalType.S8500);
                if (S9000Chemicals.Where(entry => entry.Value.InChiKey == inchiKey).FirstOrDefault().Value != null)
                    return S9000Chemicals.Where(entry => entry.Value.InChiKey == inchiKey).FirstOrDefault().Value;
                else if (S8500Chemicals.Where(entry => entry.Value.InChiKey == inchiKey).FirstOrDefault().Value != null)
                    return S8500Chemicals.Where(entry => entry.Value.InChiKey == inchiKey).FirstOrDefault().Value;
                return null;
            }
            catch (Exception ex)
            {
                Log.This(ex);
                return null;
            }
        }
    }
}
