using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Transactions;
using System.Data.Entity;
using ProductTracking.Models;

namespace BlueService.Service
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Service1" in both code and config file together.
    public class BlueData : IBlueData
    {
        private readonly ApplicationDbContext core = new ApplicationDbContext();

        public ProductTracking.Models.Core.Tan GetAllFilePaths()
        {
            try
            {

                ProductTracking.Models.Core.Tan tan = core.Tans.Where(t => t.OCRStatus == "Progress").FirstOrDefault();
                    if (tan == null)
                    {
                        if (core.Tans.Where(x => x.OCRStatus == "Live").FirstOrDefault() != null)
                        {
                        ProductTracking.Models.Core.Tan tans = core.Tans.Where(x => x.OCRStatus == "Live").FirstOrDefault();
                            return tans;
                        }

                    }
              
                return tan;

            }
            catch (Exception ex)
            {

                throw ex;
            }
        }


        public string ServiceTest()
        {
            throw new NotImplementedException();
        }

      

        public void UpdateFileModel(ProductTracking.Models.Core.Tan tanupdate)
        {
            using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required, TimeSpan.FromMinutes(30)))
            {
                
            //    core.Tans.Add(tanupdate);
               // core.Entry(tanupdate).State = EntityState.Modified;
                core.SaveChanges();
                scope.Complete();
            }
            
        }


    }
}
