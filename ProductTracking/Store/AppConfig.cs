using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace ProductTracking.Store
{
    public static class C
    {
        public static readonly string ShipmentSharedPath = ConfigurationManager.AppSettings["SharedDirectory"];
        public static readonly string DELIVERY_PATH = ConfigurationManager.AppSettings["DELIVERY_PATH"];    
        //public static readonly string ShipmentSharedPath = ConfigurationManager.AppSettings["ShipmentOutputPath"];
    }
}