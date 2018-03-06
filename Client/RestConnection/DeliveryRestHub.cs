using Client.Common;
using Client.Logging;
using Entities;
using Entities.DTO;
using Entities.DTO.Delivery;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
    public partial class RestHub
    {
        public static async Task<RestStatus> Batches()
        {

            try
            {
                RestStatus status = new RestStatus();
                if (!string.IsNullOrEmpty(Sessionkey))
                {
                    using (var httpClient = new HttpClient())
                    {
                        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(BEARER_HEDER, Sessionkey);
                        httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(APPLICATION_JSON));
                        StringContent emptyContent = new StringContent(String.Empty);
                        HttpResponseMessage response = await httpClient.GetAsync(SHIPMENTS_URL);
                        using (HttpContent content = response.Content)
                        {
                            string responseText = await content.ReadAsStringAsync();
                            status.HttpCode = response.StatusCode;
                            status.HttpResponse = responseText;
                            if (status.HttpCode == System.Net.HttpStatusCode.OK)
                            {
                                var batchesList = JsonConvert.DeserializeObject<List<BatchDTO>>(responseText);
                                status.UserObject = batchesList;
                                status.StatusMessage = "Batches Loaded Successfully . .";
                                status.HttpCode = System.Net.HttpStatusCode.OK;
                            }
                            else
                                status.StatusMessage = responseText;
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
        public static async Task<RestStatus> DeleveryBatches()
        {

            try
            {
                RestStatus status = new RestStatus();
                if (!string.IsNullOrEmpty(Sessionkey))
                {
                    using (var httpClient = new HttpClient())
                    {
                        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(BEARER_HEDER, Sessionkey);
                        httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(APPLICATION_JSON));
                        StringContent emptyContent = new StringContent(String.Empty);
                        HttpResponseMessage response = await httpClient.GetAsync(DELIVERY_BATCHES);
                        using (HttpContent content = response.Content)
                        {
                            string responseText = await content.ReadAsStringAsync();
                            status.HttpCode = response.StatusCode;
                            status.HttpResponse = responseText;
                            if (status.HttpCode == System.Net.HttpStatusCode.OK)
                            {
                                var batchesList = JsonConvert.DeserializeObject<List<DeliveryBatchDTO>>(responseText);
                                status.UserObject = batchesList;
                                status.StatusMessage = "Batches Loaded Successfully . .";
                                status.HttpCode = System.Net.HttpStatusCode.OK;
                            }
                            else
                                status.StatusMessage = responseText;
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
        public static async Task<RestStatus> TansBetweenBatches(int fromBatchNumber, int toBatchNumber, int tanCategory)
        {
            try
            {
                RestStatus status = new RestStatus();
                if (!string.IsNullOrEmpty(Sessionkey))
                {
                    using (var httpClient = new HttpClient())
                    {
                        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(BEARER_HEDER, Sessionkey);
                        httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(APPLICATION_JSON));
                        StringContent parameters = new StringContent(String.Empty);
                        HttpResponseMessage response = await httpClient.GetAsync(TANS_BETWEEN_URL +
                            "?fromBatchNumber=" + fromBatchNumber +
                            "&toBatchNumber=" + toBatchNumber +
                             "&tanCategory=" + tanCategory
                            );
                        using (HttpContent content = response.Content)
                        {
                            string responseText = await content.ReadAsStringAsync();
                            status.HttpCode = response.StatusCode;
                            status.HttpResponse = responseText;
                            if (status.HttpCode == System.Net.HttpStatusCode.OK)
                            {
                                status.UserObject = JsonConvert.DeserializeObject<List<BatchTanDto>>(responseText);
                                status.StatusMessage = "Batches Loaded Successfully . .";
                                status.HttpCode = System.Net.HttpStatusCode.OK;
                            }
                            else
                            {
                                status.StatusMessage = responseText;
                                status.HttpResponse = responseText;
                            }

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

        public static async Task<RestStatus> TansFromBatches(List<int> batchNos, int tanCategory)
        {
            try
            {
                RestStatus status = new RestStatus();
                if (!string.IsNullOrEmpty(Sessionkey))
                {
                    using (var httpClient = new HttpClient())
                    {
                        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(BEARER_HEDER, Sessionkey);
                        httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(APPLICATION_JSON));
                        StringContent parameters = new StringContent(String.Empty);
                        HttpResponseMessage response = await httpClient.PostAsync($"{TANS_FROM_BATCHES_URL}?tanCategory={tanCategory}",
                            new StringContent(JsonConvert.SerializeObject(batchNos), Encoding.UTF8, "application/json"));
                        using (HttpContent content = response.Content)
                        {
                            string responseText = await content.ReadAsStringAsync();
                            status.HttpCode = response.StatusCode;
                            status.HttpResponse = responseText;
                            if (status.HttpCode == System.Net.HttpStatusCode.OK)
                            {
                                status.UserObject = JsonConvert.DeserializeObject<List<BatchTanDto>>(responseText);
                                status.StatusMessage = "Batches Loaded Successfully . .";
                                status.HttpCode = System.Net.HttpStatusCode.OK;
                            }
                            else
                            {
                                status.StatusMessage = responseText;
                                status.HttpResponse = responseText;
                            }

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

        public static async Task<RestStatus> MoveToCategory(MoveTansDTO moveTansDto)
        {

            try
            {
                RestStatus status = new RestStatus();
                List<string> tanNums = new List<string>();
                if (!string.IsNullOrEmpty(Sessionkey))
                {
                    using (var httpClient = new HttpClient())
                    {
                        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(BEARER_HEDER, Sessionkey);
                        httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(APPLICATION_JSON));
                        StringContent stringContent = new StringContent(String.Empty);
                        HttpResponseMessage response = await httpClient.PostAsync(MOVE_TO_CATEGORY, new StringContent(JsonConvert.SerializeObject(moveTansDto), Encoding.UTF8, "application/json"));
                        using (HttpContent content = response.Content)
                        {
                            string responseText = await content.ReadAsStringAsync();
                            status.HttpCode = response.StatusCode;
                            status.HttpResponse = responseText;
                            if (status.HttpCode == System.Net.HttpStatusCode.OK)
                            {
                                status.StatusMessage = responseText;
                                status.HttpCode = System.Net.HttpStatusCode.OK;
                            }
                            else
                                status.StatusMessage = responseText;
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
        public static async Task<RestStatus> MoveToDelivery(MoveTansDTO moveTansDto)
        {

            try
            {
                RestStatus status = new RestStatus();
                List<string> tanNums = new List<string>();
                if (!string.IsNullOrEmpty(Sessionkey))
                {
                    using (var httpClient = new HttpClient())
                    {
                        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(BEARER_HEDER, Sessionkey);
                        httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(APPLICATION_JSON));
                        StringContent stringContent = new StringContent(String.Empty);
                        HttpResponseMessage response = await httpClient.PostAsync(MOVE_TO_DELIVERY, new StringContent(JsonConvert.SerializeObject(moveTansDto), Encoding.UTF8, "application/json"));
                        using (HttpContent content = response.Content)
                        {
                            string responseText = await content.ReadAsStringAsync();
                            status.HttpCode = response.StatusCode;
                            status.HttpResponse = responseText;
                            if (status.HttpCode == System.Net.HttpStatusCode.OK)
                            {
                                status.HttpCode = System.Net.HttpStatusCode.OK;
                                status.StatusMessage = responseText;
                            }
                            else
                                status.StatusMessage = responseText;
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
        public static async Task<RestStatus> S8000NameLocations(int batchId, int tanCategory)
        {
            try
            {
                RestStatus status = new RestStatus();
                List<string> tanNums = new List<string>();
                if (!string.IsNullOrEmpty(Sessionkey))
                {
                    using (var httpClient = new HttpClient())
                    {
                        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(BEARER_HEDER, Sessionkey);
                        httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(APPLICATION_JSON));
                        StringContent stringContent = new StringContent(String.Empty);
                        HttpResponseMessage response = await httpClient.PostAsync(S8000_NAME_LOCATIONS + "?batchId=" + batchId + "&tanCategory=" + tanCategory, stringContent);
                        using (HttpContent content = response.Content)
                        {
                            string responseText = await content.ReadAsStringAsync();
                            status.HttpCode = response.StatusCode;
                            status.HttpResponse = responseText;
                            if (status.HttpCode == System.Net.HttpStatusCode.OK)
                            {
                                status.UserObject = JsonConvert.DeserializeObject<List<S8000NameLocationDTO>>(responseText);
                                status.StatusMessage = "S8000 Name Locations Loaded Successfully . .";
                                status.HttpCode = System.Net.HttpStatusCode.OK;
                            }
                            else
                                status.StatusMessage = responseText;
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
        public static async Task<RestStatus> S8580Comments(int batchId, int tanCategory)
        {

            try
            {
                RestStatus status = new RestStatus();
                List<string> tanNums = new List<string>();
                if (!string.IsNullOrEmpty(Sessionkey))
                {
                    using (var httpClient = new HttpClient())
                    {
                        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(BEARER_HEDER, Sessionkey);
                        httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(APPLICATION_JSON));
                        StringContent stringContent = new StringContent(String.Empty);
                        HttpResponseMessage response = await httpClient.PostAsync(S8580_COMMENTS + "?batchId=" + batchId + "&tanCategory=" + tanCategory, stringContent);
                        using (HttpContent content = response.Content)
                        {
                            string responseText = await content.ReadAsStringAsync();
                            status.HttpCode = response.StatusCode;
                            status.HttpResponse = responseText;
                            if (status.HttpCode == System.Net.HttpStatusCode.OK)
                            {
                                status.UserObject = JsonConvert.DeserializeObject<List<S8580CommentsDTO>>(responseText);
                                status.StatusMessage = "S8508 Comments Loaded Successfully . .";
                                status.HttpCode = System.Net.HttpStatusCode.OK;
                            }
                            else
                                status.StatusMessage = responseText;
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
        public static async Task<RestStatus> ExtractRsn(int batchId, int tanCategory)
        {

            try
            {
                RestStatus status = new RestStatus();
                List<string> tanNums = new List<string>();
                if (!string.IsNullOrEmpty(Sessionkey))
                {
                    using (var httpClient = new HttpClient())
                    {
                        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(BEARER_HEDER, Sessionkey);
                        httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(APPLICATION_JSON));
                        StringContent stringContent = new StringContent(String.Empty);
                        HttpResponseMessage response = await httpClient.PostAsync(EXTRACT_RSN + "?batchId=" + batchId + "&tanCategory=" + tanCategory, stringContent);
                        using (HttpContent content = response.Content)
                        {
                            string responseText = await content.ReadAsStringAsync();
                            status.HttpCode = response.StatusCode;
                            status.HttpResponse = responseText;
                            if (status.HttpCode == System.Net.HttpStatusCode.OK)
                            {
                                status.UserObject = JsonConvert.DeserializeObject<List<ExtractRSNDto>>(responseText);
                                status.StatusMessage = "RSNs Loaded Successfully . .";
                                status.HttpCode = System.Net.HttpStatusCode.OK;
                            }
                            else
                                status.StatusMessage = responseText;
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
        public static async Task<RestStatus> UpdateFreeTextBulk(FreeTextBulkDto dto)
        {

            try
            {
                RestStatus status = new RestStatus();
                List<string> tanNums = new List<string>();
                if (!string.IsNullOrEmpty(Sessionkey))
                {
                    using (var httpClient = new HttpClient())
                    {
                        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(BEARER_HEDER, Sessionkey);
                        httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(APPLICATION_JSON));
                        StringContent stringContent = new StringContent(String.Empty);
                        HttpResponseMessage response = await httpClient.PostAsync(UPDATE_BULK_FREETEXT, new StringContent(JsonConvert.SerializeObject(dto), Encoding.UTF8, "application/json"));
                        using (HttpContent content = response.Content)
                        {
                            string responseText = await content.ReadAsStringAsync();
                            status.HttpCode = response.StatusCode;
                            status.HttpResponse = responseText;
                            if (status.HttpCode == System.Net.HttpStatusCode.OK)
                                status.HttpCode = System.Net.HttpStatusCode.OK;
                            else
                                status.StatusMessage = responseText;
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
        public static async Task<RestStatus> GenerateXML(int id)
        {

            try
            {
                RestStatus status = new RestStatus();
                List<string> tanNums = new List<string>();
                if (!string.IsNullOrEmpty(Sessionkey))
                {
                    using (var httpClient = new HttpClient())
                    {
                        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(BEARER_HEDER, Sessionkey);
                        httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(APPLICATION_JSON));
                        StringContent stringContent = new StringContent(String.Empty);
                        HttpResponseMessage response = await httpClient.PostAsync(GENERATE_XML + "?id=" + id, stringContent);
                        using (HttpContent content = response.Content)
                        {
                            string responseText = await content.ReadAsStringAsync();
                            status.HttpCode = response.StatusCode;
                            status.HttpResponse = responseText;
                            if (status.HttpCode == System.Net.HttpStatusCode.OK)
                            {
                                status.HttpCode = System.Net.HttpStatusCode.OK;
                                status.UserObject = JsonConvert.DeserializeObject<GenerateXMLDTO>(responseText);
                                status.StatusMessage = "XML Generated Successfully . .";
                            }
                            else if (status.HttpCode == System.Net.HttpStatusCode.InternalServerError)
                            {
                                dynamic exception = JsonConvert.DeserializeObject(responseText);
                                status.StatusMessage = exception.ExceptionMessage;
                                status.HttpResponse = responseText;
                            }
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
        public static async Task<RestStatus> GenerateZIP(int id, long maxZipSize)
        {

            try
            {
                RestStatus status = new RestStatus();
                List<string> tanNums = new List<string>();
                if (!string.IsNullOrEmpty(Sessionkey))
                {
                    using (var httpClient = new HttpClient())
                    {
                        httpClient.Timeout = TimeSpan.FromMinutes(10);
                        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(BEARER_HEDER, Sessionkey);
                        httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(APPLICATION_JSON));
                        StringContent stringContent = new StringContent(String.Empty);
                        HttpResponseMessage response = await httpClient.PostAsync(GENERATE_ZIP +
                            "?id=" + id +
                            "&maxZipSize=" + maxZipSize,
                            stringContent);
                        using (HttpContent content = response.Content)
                        {
                            string responseText = await content.ReadAsStringAsync();
                            status.HttpCode = response.StatusCode;
                            status.HttpResponse = responseText;
                            if (status.HttpCode == System.Net.HttpStatusCode.OK)
                            {
                                status.UserObject = JsonConvert.DeserializeObject<ZipResultDTO>(responseText);
                                status.StatusMessage = "ZIP Generated Successfully . .";
                            }
                            else
                                status.StatusMessage = responseText;
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
        public static async Task<RestStatus> GenerateEmail(int id)
        {

            try
            {
                RestStatus status = new RestStatus();
                List<string> tanNums = new List<string>();
                if (!string.IsNullOrEmpty(Sessionkey))
                {
                    using (var httpClient = new HttpClient())
                    {
                        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(BEARER_HEDER, Sessionkey);
                        httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(APPLICATION_JSON));
                        StringContent stringContent = new StringContent(String.Empty);
                        HttpResponseMessage response = await httpClient.PostAsync(GENERATE_EMAIL + "?id=" + id, stringContent);
                        using (HttpContent content = response.Content)
                        {
                            string responseText = await content.ReadAsStringAsync();
                            status.HttpCode = response.StatusCode;
                            status.HttpResponse = responseText;
                            if (status.HttpCode == System.Net.HttpStatusCode.OK)
                            {
                                status.UserObject = responseText;
                                status.StatusMessage = "Email Generated Successfully . .";
                            }
                            else
                                status.StatusMessage = responseText;
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
        public static async Task<RestStatus> Versions(string tanNumber)
        {

            try
            {
                RestStatus status = new RestStatus();
                List<string> tanNums = new List<string>();
                if (!string.IsNullOrEmpty(Sessionkey))
                {
                    using (var httpClient = new HttpClient())
                    {
                        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(BEARER_HEDER, Sessionkey);
                        httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(APPLICATION_JSON));
                        StringContent stringContent = new StringContent(String.Empty);
                        HttpResponseMessage response = await httpClient.PostAsync(VERSIONS + "?tanNumber=" + tanNumber, stringContent);
                        using (HttpContent content = response.Content)
                        {
                            string responseText = await content.ReadAsStringAsync();
                            status.HttpCode = response.StatusCode;
                            status.HttpResponse = responseText;
                            if (status.HttpCode == System.Net.HttpStatusCode.OK)
                            {
                                var dtos = JsonConvert.DeserializeObject<List<TanHistoryDTO>>(responseText);
                                status.UserObject = dtos;
                                status.StatusMessage = "Versions Loaded Successfully . .";
                            }
                            else
                                status.StatusMessage = responseText;
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
        public static async Task<RestStatus> DeliveryBatchSummary()
        {

            try
            {
                RestStatus status = new RestStatus();
                List<string> tanNums = new List<string>();
                if (!string.IsNullOrEmpty(Sessionkey))
                {
                    using (var httpClient = new HttpClient())
                    {
                        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(BEARER_HEDER, Sessionkey);
                        httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(APPLICATION_JSON));
                        StringContent stringContent = new StringContent(String.Empty);
                        HttpResponseMessage response = await httpClient.PostAsync(BATCH_WISE_TANS, stringContent);
                        using (HttpContent content = response.Content)
                        {
                            string responseText = await content.ReadAsStringAsync();
                            status.HttpCode = response.StatusCode;
                            status.HttpResponse = responseText;
                            if (status.HttpCode == System.Net.HttpStatusCode.OK)
                            {
                                status.HttpCode = System.Net.HttpStatusCode.OK;
                                status.UserObject = JsonConvert.DeserializeObject<List<DeliveryBatchDTO>>(responseText);
                                status.StatusMessage = "Batches Loaded Successfully . .";
                            }
                            else
                                status.StatusMessage = responseText;
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
        public static async Task<RestStatus> TansOfDelivery(int id, bool isZeroRxns, bool isQueried)
        {

            try
            {
                RestStatus status = new RestStatus();
                List<string> tanNums = new List<string>();
                if (!string.IsNullOrEmpty(Sessionkey))
                {
                    using (var httpClient = new HttpClient())
                    {
                        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(BEARER_HEDER, Sessionkey);
                        httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(APPLICATION_JSON));
                        StringContent stringContent = new StringContent(String.Empty);
                        HttpResponseMessage response = await httpClient.PostAsync(TANS_OF_DELIVERY
                            + "?id=" + id
                            + "&isZeroRxns=" + isZeroRxns
                            + "&isQueried=" + isQueried
                            , stringContent);
                        using (HttpContent content = response.Content)
                        {
                            string responseText = await content.ReadAsStringAsync();
                            status.HttpCode = response.StatusCode;
                            status.HttpResponse = responseText;
                            if (status.HttpCode == System.Net.HttpStatusCode.OK)
                            {
                                status.HttpCode = System.Net.HttpStatusCode.OK;
                                status.UserObject = JsonConvert.DeserializeObject<List<DeliveryTanDTO>>(responseText);
                                status.StatusMessage = "Tans Loaded Successfully . .";
                            }
                            else
                                status.StatusMessage = responseText;
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
        public static async Task<RestStatus> GenerateNextBatchNumber(int batchId)
        {

            try
            {
                RestStatus status = new RestStatus();
                List<string> tanNums = new List<string>();
                if (!string.IsNullOrEmpty(Sessionkey))
                {
                    using (var httpClient = new HttpClient())
                    {
                        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(BEARER_HEDER, Sessionkey);
                        httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(APPLICATION_JSON));
                        StringContent stringContent = new StringContent(String.Empty);
                        HttpResponseMessage response = await httpClient.PostAsync(NEXT_DELIVERY_BATCH_NUMBER + "?batchId=" + batchId, stringContent);
                        using (HttpContent content = response.Content)
                        {
                            string responseText = await content.ReadAsStringAsync();
                            status.HttpCode = response.StatusCode;
                            status.HttpResponse = responseText;
                            if (status.HttpCode == System.Net.HttpStatusCode.OK)
                            {
                                status.HttpCode = System.Net.HttpStatusCode.OK;
                                status.StatusMessage = responseText;
                            }
                            else
                                status.StatusMessage = responseText;
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

        public static async Task<RestStatus> UpdateDeliveryStatus(int batchId)
        {

            try
            {
                RestStatus status = new RestStatus();
                List<string> tanNums = new List<string>();
                if (!string.IsNullOrEmpty(Sessionkey))
                {
                    using (var httpClient = new HttpClient())
                    {
                        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(BEARER_HEDER, Sessionkey);
                        httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(APPLICATION_JSON));
                        StringContent stringContent = new StringContent(String.Empty);
                        HttpResponseMessage response = await httpClient.PostAsync(UPDATE_DELIVERY_STATUS + "?batchId=" + batchId, stringContent);
                        using (HttpContent content = response.Content)
                        {
                            string responseText = await content.ReadAsStringAsync();
                            status.HttpCode = response.StatusCode;
                            status.HttpResponse = responseText;
                            if (status.HttpCode == System.Net.HttpStatusCode.OK)
                            {
                                status.HttpCode = System.Net.HttpStatusCode.OK;
                                status.StatusMessage = responseText;
                            }
                            else
                                status.StatusMessage = responseText;
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

        public static async Task<RestStatus> RevertDeliveryTAN(int id, int role, string msg)
        {

            try
            {
                RestStatus status = new RestStatus();
                List<string> tanNums = new List<string>();
                if (!string.IsNullOrEmpty(Sessionkey))
                {
                    using (var httpClient = new HttpClient())
                    {
                        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(BEARER_HEDER, Sessionkey);
                        httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(APPLICATION_JSON));
                        StringContent stringContent = new StringContent(String.Empty);
                        HttpResponseMessage response = await httpClient.PostAsync(REVERT_DELIVERY_TAN
                            + "?id=" + id
                            + "&role=" + role
                            + "&msg=" + msg
                            , stringContent);
                        using (HttpContent content = response.Content)
                        {
                            string responseText = await content.ReadAsStringAsync();
                            status.HttpCode = response.StatusCode;
                            status.HttpResponse = responseText;
                            status.StatusMessage = responseText;
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
    }
}
