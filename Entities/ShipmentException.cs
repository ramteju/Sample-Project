using ProductTracking.Models.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
   public class ShipmentException
    {
        public int Id { get; set; }
        public string BatchNo { get; set; }
        public string StackTrace { get; set; }
        public string Message { get; set; }
        public string Status { get; set; }
        public DateTime DateCreated { get; set; }
    }
    public enum Status
    {
        New,
        close,
        Live
    }
    public class ShippmentUploadStatus
    {
        public int Id { get; set; }
        public string BatchNo { get; set; }
        public string Status { get; set; }
        public DateTime? DateCreated { get; set; }
        public string Path { get; set; }
        public string NumImagesPath { get; set; }
    }
    public enum ShippmentUploadEnumStatus
    {
        ProcessProgress,
        ProcessCompleted,
        CadViewProgress,
        CadViewCompleted
    }
    public enum ShipmentStatus
    {
        BackupShippmentFile,
        ExtractingImageFolder,
        CreatingTanWiseFolder,
        GenerateTifPdf,
        TanWiseKeyWordsList,
        RenameSgmlFile,
        TanWiseArticle,
        Substance,
        Hexcode,
        UpdatingTan,
        SaveBatch,
        DuplicateTan,
        SaveTan,
        SavingTanDatas,
        SavingTanKeyWords,
        CGMFile
    }
    public class ApplicationSettings
    {
        public int Id { get; set; }
        public string Type { get; set; }
        public string key { get; set; }
        public string status { get; set; }
    }
    public class ShippmentOCR
    {
        public int Id { get; set; }
        public string Shipment { get; set; }
        public string OCRPath { get; set; }
        public string Status { get; set; }
        public DateTime Datecreated { get; set; }
        public DateTime? DateUpdated { get; set; }
    }

    public class ShippmentBatchExcel
    {
        public string TAN { get; set; }
        public string VOL { get; set; }
        public string IS { get; set; }
        public string DocClass { get; set; }
        public string ProcessingNotes { get; set; }
        public string Completebydate { get; set; }
    }
    public class ShippmentUploadedExcel
    {
        public int Id { get; set; }
        public string excelPath { get; set; }
        public string SpreadSheetName { get; set; }
        public DateTime? RecievedDate { get; set; }
        public string BatchNumber { get; set; }
        public DateTime UploadedDate { get; set; }
        public string RemainingTans { get; set; }
        public virtual ICollection<Batch> Batches { get; set; }
    }

}
