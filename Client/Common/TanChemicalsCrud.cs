using Client.Logging;
using Client.Models;
using Client.ViewModels;
using ProductTracking.Models.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.Common
{
    public static class TanChemicalsCrud
    {
        public static void AddTanChemicalToList(List<TanChemical> TanChemicals, TanChemicalVM chemical, int TanId)
        {
            try
            {
                TanChemicals.Add(new TanChemical
                {
                    ChemicalType = chemical.ChemicalType,
                    CompoundNo = chemical.CompoundNo,
                    GenericName = chemical.GenericName,
                    Id = chemical.Id,
                    ImagePath = chemical.ImagePath,
                    MetaData = chemical.ChemicalmataData,
                    Name = chemical.Name,
                    NUM = chemical.NUM,
                    RegNumber = chemical.RegNumber,
                    TanId = TanId,
                    MolString = chemical.MolString,
                    InchiKey = chemical.InChiKey,
                    Formula = chemical.MolFormula
                });
                ((App.Current.MainWindow as MainWindow).DataContext as MainVM).TanVM.UpdateUnusedNUMs();
                ((App.Current.MainWindow as MainWindow).DataContext as MainVM).TanVM.UpdateNumsView();
            }
            catch (Exception ex)
            {
                Log.This(ex);
            }
        }
    }
}
