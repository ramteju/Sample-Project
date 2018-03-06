using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using Entities;

namespace BlueService.Service
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IService1" in both code and config file together.
    [ServiceContract]
    public interface IBlueData
    {

        [OperationContract]
        string ServiceTest();

        [OperationContract]
        ProductTracking.Models.Core.Tan GetAllFilePaths();

        [OperationContract]
        void UpdateFileModel(ProductTracking.Models.Core.Tan tan);
        // TODO: Add your service operations here
    }    
   
}
