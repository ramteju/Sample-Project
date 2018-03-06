using Client.Common;
using Client.Logging;
using Entities;
using Entities.DTO;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Client.RestConnection
{
    public partial class ReportHub
    {

        static private readonly string APPLICATION_JSON = "application/json";
        static public readonly string BEARER_HEDER = "Bearer";

        #region Base URL
        static public readonly string BASE_URL = C.BASE_URL;
        static readonly string LOGIN_URL = BASE_URL + "/token";
        #endregion

        static readonly string ERROR_REPORT_URL = BASE_URL + "/api/ReportController/ReportHub";
        static readonly string GET_ERROR_REPORT_DATA = BASE_URL + "/api/Tan/GetErrorReportData";
        static readonly string ADD_ERROR_REPORT_DATA = BASE_URL + "/api/Tan/AddErrorReport";


        public static async Task<RestStatus> GetErrorPercentage(ErrorPercentageDto errPercdto)
        {

            try
            {
                RestStatus status = new RestStatus();
                using (var httpClient = new HttpClient())
                {
                    HttpResponseMessage response = await httpClient.PostAsync(ERROR_REPORT_URL, new StringContent(JsonConvert.SerializeObject(errPercdto), Encoding.UTF8, "application/json"));                    
                    using (HttpContent content = response.Content)
                    {
                        string responseText = await content.ReadAsStringAsync();
                        status.HttpCode = response.StatusCode;
                        if (status.HttpCode == System.Net.HttpStatusCode.OK)
                        {
                            status.HttpResponse = responseText;
                        }
                        else
                        {
                            dynamic state = JsonConvert.DeserializeObject<dynamic>(status.HttpResponse);
                            status.StatusMessage = state.error_description;
                        }
                    }
                }
                return status;
            }
            catch (Exception ex)
            {
                Log.This(ex);
                throw;
            }
        }

        public static async Task<RestStatus> GetErrorReportData(ErrorReportDto errPercdto)
        {

            try
            {
                RestStatus status = new RestStatus();
                using (var httpClient = new HttpClient())
                {
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(BEARER_HEDER, RestHub.Sessionkey);
                    httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(APPLICATION_JSON));

                    HttpResponseMessage response = await httpClient.PostAsync(GET_ERROR_REPORT_DATA, new StringContent(JsonConvert.SerializeObject(errPercdto), Encoding.UTF8, "application/json"));
                    using (HttpContent content = response.Content)
                    {
                        string responseText = await content.ReadAsStringAsync();
                        status.HttpCode = response.StatusCode;
                        if (status.HttpCode == System.Net.HttpStatusCode.OK)
                            status.UserObject = JsonConvert.DeserializeObject<ErrorReportDto>(responseText);
                    }
                }
                return status;
            }
            catch (Exception ex)
            {
                Log.This(ex);
                throw;
            }
        }

        public static async Task<RestStatus> AddReportData(ErrorReport errPercdto)
        {
            try
            {
                RestStatus status = new RestStatus();
                using (var httpClient = new HttpClient())
                {
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(BEARER_HEDER, RestHub.Sessionkey);
                    httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(APPLICATION_JSON));

                    HttpResponseMessage response = await httpClient.PostAsync(ADD_ERROR_REPORT_DATA, new StringContent(JsonConvert.SerializeObject(errPercdto), Encoding.UTF8, "application/json"));
                    using (HttpContent content = response.Content)
                    {
                        string responseText = await content.ReadAsStringAsync();
                        status.HttpResponse = responseText;
                        status.HttpCode = response.StatusCode;
                        if (status.HttpCode == System.Net.HttpStatusCode.OK)
                            status.UserObject = JsonConvert.DeserializeObject<bool>(responseText);
                    }
                }
                return status;
            }
            catch (Exception ex)
            {
                Log.This(ex);
                throw;
            }
        }
    }
}
