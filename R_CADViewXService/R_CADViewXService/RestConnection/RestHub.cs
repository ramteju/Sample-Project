using Entities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace R_CADViewXService.RestConnection
{
    public class RestHub
    {

        static private readonly string APPLICATION_JSON = "application/json";
        static public readonly string BEARER_HEDER = "Bearer";
        public static readonly string BASE_URL = ConfigurationManager.AppSettings["rest_base_url"];
        public static readonly string GET_ShipmentUploadStatus_URL = $"{BASE_URL}/api/Shipment/GetShipmentUploadStatus";
        public static readonly string UPDATE_ShipmentUploadStatus_URL = $"{BASE_URL}/api/Shipment/UpdateShipmentUploadStatus";
        public static async Task<RestStatus> GetShipmentUploadStatus()
        {

            try
            {
                RestStatus status = new RestStatus();
                using (var httpClient = new HttpClient())
                {
                    httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(APPLICATION_JSON));
                    StringContent emptyContent = new StringContent(String.Empty);
                    HttpResponseMessage response = await httpClient.GetAsync(GET_ShipmentUploadStatus_URL);
                    using (HttpContent content = response.Content)
                    {
                        string responseText = await content.ReadAsStringAsync();
                        status.HttpCode = response.StatusCode;
                        status.HttpResponse = responseText;
                        if (status.HttpCode == System.Net.HttpStatusCode.OK)
                        {
                            var ShipmentUploadStatus = JsonConvert.DeserializeObject<ShippmentUploadStatus>(responseText);
                            status.UserObject = ShipmentUploadStatus;
                            status.StatusMessage = "ShipmentUploadStatus Loaded Successfully . .";
                            status.HttpCode = System.Net.HttpStatusCode.OK;
                        }
                        else
                            status.StatusMessage = responseText;
                    }
                }
                return status;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public static async Task<RestStatus> UpdateShipmentUploadStatus(ShippmentUploadStatus ShippmentUploadStatus)
        {

            try
            {
                RestStatus status = new RestStatus();
                using (var httpClient = new HttpClient())
                {
                    httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(APPLICATION_JSON));
                    StringContent emptyContent = new StringContent(String.Empty);
                    HttpResponseMessage response = await httpClient.PostAsync(UPDATE_ShipmentUploadStatus_URL, new StringContent(JsonConvert.SerializeObject(ShippmentUploadStatus), Encoding.UTF8, "application/json"));
                    using (HttpContent content = response.Content)
                    {
                        string responseText = await content.ReadAsStringAsync();
                        status.HttpCode = response.StatusCode;
                        status.HttpResponse = responseText;
                        if (status.HttpCode == System.Net.HttpStatusCode.OK)
                        {
                            status.StatusMessage = "Shipmentstatus updated Successfully . .";
                            status.HttpCode = System.Net.HttpStatusCode.OK;
                        }
                        else
                            status.StatusMessage = responseText;
                    }
                }
                return status;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
