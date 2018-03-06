using Client.Common;
using Client.Logging;
using Client.ViewModel;
using DTO;
using Entities;
using Entities.DTO;
using Entities.DTO.Core;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ProductTracking.Models.Core;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Entities.DTO.Util;
using System.Linq;
using System.Net;

namespace Client
{
    public partial class RestHub
    {

        static private readonly string APPLICATION_JSON = "application/json";
        static public readonly string BEARER_HEDER = "Bearer";

        #region Base URL
        static public readonly string BASE_URL = C.BASE_URL;
        static readonly string LOGIN_URL = BASE_URL + "/token";
        #endregion

        #region Open
        private static readonly string APP_STATIC = BASE_URL + "/api/Open/StaticData";
        #endregion

        #region User Related URLs
        static readonly string GET_CURATORS_URL = BASE_URL + "/api/User/GetAllCurators";
        static readonly string USER_ROLES_URL = BASE_URL + "/api/Tan/GetUserRoles";
        static readonly string USER_INFO_URL = BASE_URL + "/api/Tan/GetUserInfo";
        static readonly string USER_REPORTS_URL = BASE_URL + "/api/Tan/UserReactionsReports";
        static readonly string APPROVE_TANS_URL = BASE_URL + "/api/Tan/AproveTan";
        static readonly string REJECT_TANS_URL = BASE_URL + "/api/Tan/RejectTan";
        static readonly string SUBMIT_TANS_URL = BASE_URL + "/api/Tan/SubmitTan";
        #endregion

        #region TAN Related URLs
        static readonly string MY_TANS_URL = BASE_URL + "/api/Tan/Mytans";
        static readonly string ASSIGN_TANS_URL = BASE_URL + "/api/Tan/Assigntans";
        static readonly string UPDATE_REVIEW_ALLOW_URL = BASE_URL + "/api/Shipment/UpdateReviewAllowTag";
        static readonly string PULL_TASK = BASE_URL + "/api/Tan/Pulltask";
        static readonly string FULL_TAN = BASE_URL + "/api/tan/GetTan";
        static readonly string ROLEBASED_FULL_TAN = BASE_URL + "/api/tan/GetRoleBasedTanData";
        static readonly string SAVE_TAN = BASE_URL + "/api/tan/SaveTan";
        #endregion

        #region Delivery
        static readonly string MOVE_TO_DELIVERY = BASE_URL + "/api/Shipment/MoveToDelivery";
        static readonly string MOVE_TO_CATEGORY = BASE_URL + "/api/Shipment/MoveToCategory";
        static readonly string S8000_NAME_LOCATIONS = BASE_URL + "/api/Shipment/S8000NameLocations";
        static readonly string S8580_COMMENTS = BASE_URL + "/api/Shipment/S8580Comments";
        static readonly string EXTRACT_RSN = BASE_URL + "/api/Shipment/ExtractRSN";
        static readonly string UPDATE_BULK_FREETEXT = BASE_URL + "/api/Shipment/UpdateBulkFreeText";
        static readonly string GENERATE_XML = BASE_URL + "/api/Delivery/GenerateXML";
        static readonly string VERSIONS = BASE_URL + "/api/Tan/Versions";
        static readonly string GENERATE_ZIP = BASE_URL + "/api/Delivery/GenerateZip";
        static readonly string GENERATE_EMAIL = BASE_URL + "/api/Delivery/GenerateEmail";
        static readonly string DELIVERY_BATCHES = BASE_URL + "/api/Delivery/Batches";
        static readonly string BATCH_WISE_TANS = BASE_URL + "/api/Delivery/BatchWiseTans";
        static readonly string TANS_OF_DELIVERY = BASE_URL + "/api/Delivery/TansOfDelivery";
        static readonly string NEXT_DELIVERY_BATCH_NUMBER = BASE_URL + "/api/Delivery/GenerateNextBatchNumber";
        static readonly string UPDATE_DELIVERY_STATUS = BASE_URL + "/api/Shipment/UpdateDeliveryStatus";
        static readonly string REVERT_DELIVERY_TAN = BASE_URL + "/api/Delivery/Revert";
        #endregion

        #region Query
        static readonly string My_QUERIES = BASE_URL + "/api/Query/UserQueries";
        static readonly string SAVE_QUERY = BASE_URL + "/api/Query/Save";
        static readonly string SUBMIT_QUERY = BASE_URL + "/api/Query/Submit";
        static readonly string REJECT_QUERY = BASE_URL + "/api/Query/Revert";
        static readonly string QUERY_STATUS = BASE_URL + "/api/Query/IsQueryActive";
        static readonly string ADD_RESPONSE = BASE_URL + "/api/Query/AddResponse";
        static readonly string QUERY_RESPONSES = BASE_URL + "/api/Query/Responses";
        static readonly string QUERY_WORKFLOW = BASE_URL + "/api/Query/Workflow";
        static readonly string ALL_USERS = BASE_URL + "/api/Query/Users";
        static readonly string QUERY_WORKFLOWS = BASE_URL + "/api/Query/Workflows";
        static readonly string SAVE_QUERY_WORKFLOWS = BASE_URL + "/api/Query/SaveWorkflows";
        static readonly string QUERIES_REPORT = BASE_URL + "/api/Query/QueriesReport";
        #endregion

        static readonly string TAN_KEYWORDS_URL = BASE_URL + "/api/Shipment/TanKeyWords";
        static readonly string EXCEPTION_URL = BASE_URL + "/api/Tan/SaveException";
        static readonly string SHIPMENTS_URL = BASE_URL + "/api/Shipment/Shipments";
        static readonly string TANS_BETWEEN_URL = BASE_URL + "/api/Shipment/TansBetweenBatches";
        static readonly string TANS_FROM_BATCHES_URL = BASE_URL + "/api/Shipment/TanFromBatches";

        public static string Sessionkey = "LKLK";
        /// <summary>
        /// Connects to the server and get the Roles registered in Database.
        /// </summary>
        /// <param name="userName"> User Loginname to get the rols for this user</param>
        /// <returns></returns>
        /// 

        public static async Task<RestStatus> UserRoles(string userName)
        {
            try
            {
                RestStatus status = new RestStatus();
                using (var httpClient = new HttpClient())
                {
                    StringContent emptyContent = new StringContent(String.Empty);
                    HttpResponseMessage response = await httpClient.GetAsync(
                        USER_ROLES_URL + "?UserName=" + userName);
                    using (HttpContent content = response.Content)
                    {
                        string responseText = await content.ReadAsStringAsync();
                        status.HttpCode = response.StatusCode;
                        status.HttpResponse = responseText;
                        if (status.HttpCode == System.Net.HttpStatusCode.OK)
                        {
                            JArray tansArray = JArray.Parse(responseText);
                            List<ProductRole> productRoles = new List<ProductRole>();
                            foreach (var productRole in tansArray)
                            {
                                productRoles.Add(new ProductRole()
                                {
                                    RoleId = (int)productRole["Id"],
                                    RoleName = (string)productRole["Name"]
                                });
                            }
                            status.UserObject = productRoles;
                            status.StatusMessage = "User Roles Loaded Successfully . .";
                        }
                        else
                            status.StatusMessage = responseText;
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
        /// <summary>
        /// Connect the server and authenticate the user
        /// </summary>
        /// <param name="userName">User Loginname </param>
        /// <param name="password">User Password</param>
        /// <returns></returns>
        public static async Task<RestStatus> LoginUser(string userName, string password)
        {

            try
            {
                RestStatus status = new RestStatus();
                using (var httpClient = new HttpClient())
                {
                    userName = $"{userName}----{Environment.UserName}----{System.Net.Dns.GetHostName()}----{System.Net.Dns.GetHostEntry(System.Net.Dns.GetHostName()).AddressList.GetValue(1).ToString()}----{S.Version}";
                    StringContent loginContent = new StringContent("grant_type=password&username=" + userName + "&password=" + System.Web.HttpUtility.UrlEncode(password));
                    HttpResponseMessage response = await httpClient.PostAsync(LOGIN_URL, loginContent);
                    IEnumerable<string> userIds;
                    U.UserId = null;
                    using (HttpContent content = response.Content)
                    {
                        string responseText = await content.ReadAsStringAsync();
                        status.HttpCode = response.StatusCode;
                        status.HttpResponse = responseText;
                        if (status.HttpCode == System.Net.HttpStatusCode.OK)
                        {
                            if (response.Headers.TryGetValues(Constants.USER_ID, out userIds))
                                U.UserId = userIds.First();
                            if (!String.IsNullOrEmpty(U.UserId))
                            {
                                JObject json = JObject.Parse(responseText);
                                Sessionkey = (string)json["access_token"];
                                status.StatusMessage = "User logged in successfully . .";
                            }
                            else
                            {
                                status.HttpCode = System.Net.HttpStatusCode.NonAuthoritativeInformation;
                                status.StatusMessage = "Sufficient User Information Is Not Found In Server";
                            }
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

        public static async Task<RestStatus> UserReactionsReports(int currentRoleId)
        {

            try
            {
                RestStatus status = new RestStatus();
                using (var httpClient = new HttpClient())
                {
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(BEARER_HEDER, Sessionkey);
                    httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(APPLICATION_JSON));

                    HttpResponseMessage response = await httpClient.GetAsync(USER_REPORTS_URL + "?currentRoleId=" + currentRoleId);
                    using (HttpContent content = response.Content)
                    {
                        string responseText = await content.ReadAsStringAsync();
                        status.HttpCode = response.StatusCode;
                        status.HttpResponse = responseText;
                        if (status.HttpCode == System.Net.HttpStatusCode.OK)
                        {
                            List<UserReportsDTO> reportsDto = JsonConvert.DeserializeObject<List<UserReportsDTO>>(responseText);
                            status.UserObject = reportsDto;
                            status.StatusMessage = "User Reports Loaded Successfully . .";
                        }
                        else
                            status.StatusMessage = responseText;
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

        /// <summary>
        /// Get the Users permissions level like He can submit/Approve/Reject
        /// </summary>
        /// <param name="RoleID">User RoleId</param>
        /// <returns></returns>
        public static async Task<RestStatus> UserPermissionsInfo(int RoleID)
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

                        HttpResponseMessage response = await httpClient.GetAsync(USER_INFO_URL + "?productCode=1&currentRole=" + RoleID);
                        using (HttpContent content = response.Content)
                        {
                            string responseText = await content.ReadAsStringAsync();
                            status.HttpCode = response.StatusCode;
                            status.HttpResponse = responseText;
                            if (status.HttpCode == System.Net.HttpStatusCode.OK)
                            {
                                JObject json = JObject.Parse(responseText);
                                var r = json["roles"];
                                status.UserObject = new UserInfoDTO()
                                {
                                    canSubmit = (bool)json["canSubmit"],
                                    canApprove = (bool)json["canApprove"],
                                    canReject = (bool)json["canReject"]
                                };
                                status.StatusMessage = "User Info Loaded Successfully . .";
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
        public static async Task<RestStatus> MyTans(int currentRole, bool pullTasks)
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

                        StringContent stringContent = new StringContent("productId=1");

                        HttpResponseMessage response = await httpClient.GetAsync(MY_TANS_URL + "?productId=1&currentRole=" + currentRole + "&pullTasks=" + pullTasks);
                        using (HttpContent content = response.Content)
                        {
                            string responseText = await content.ReadAsStringAsync();
                            status.HttpCode = response.StatusCode;
                            status.StatusMessage = responseText;
                            if (status.HttpCode == System.Net.HttpStatusCode.OK)
                            {
                                List<TaskDTO> tans = JsonConvert.DeserializeObject<List<TaskDTO>>(responseText);
                                status.UserObject = tans;
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

        public static async Task<RestStatus> AllowForReview(List<int> batchIds)
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

                        StringContent stringContent = new StringContent("productId=1");

                        HttpResponseMessage response = await httpClient.PostAsync(UPDATE_REVIEW_ALLOW_URL, new StringContent(JsonConvert.SerializeObject(batchIds), Encoding.UTF8, "application/json"));
                        using (HttpContent content = response.Content)
                        {
                            string responseText = await content.ReadAsStringAsync();
                            status.HttpCode = response.StatusCode;
                            status.HttpResponse = responseText;
                            if (status.HttpCode == System.Net.HttpStatusCode.OK)
                            {
                                bool result = JsonConvert.DeserializeObject<bool>(responseText);
                                status.UserObject = result;
                                status.StatusMessage = "User Tans Assigned Successfully . .";
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

                throw;
            }
        }

        public static async Task<RestStatus> AssignTans(List<int> tanIds, string toUserId, int currentRoleId, string comment, Role targetRole)
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

                        StringContent stringContent = new StringContent("productId=1");

                        HttpResponseMessage response = await httpClient.PostAsync($"{ASSIGN_TANS_URL}?toUserId={toUserId}&currentRoleId={currentRoleId}&comment={comment}&role={targetRole}", new StringContent(JsonConvert.SerializeObject(tanIds), Encoding.UTF8, "application/json"));
                        using (HttpContent content = response.Content)
                        {
                            string responseText = await content.ReadAsStringAsync();
                            status.HttpCode = response.StatusCode;
                            status.HttpResponse = responseText;
                            if (status.HttpCode == System.Net.HttpStatusCode.OK)
                            {
                                bool result = JsonConvert.DeserializeObject<bool>(responseText);
                                status.UserObject = result;
                                status.StatusMessage = "User Tans Assigned Successfully . .";
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

        public static async Task<RestStatus> TanKeyWords()
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

                        StringContent stringContent = new StringContent("productId=1");

                        HttpResponseMessage response = await httpClient.GetAsync(TAN_KEYWORDS_URL);
                        using (HttpContent content = response.Content)
                        {
                            string responseText = await content.ReadAsStringAsync();
                            status.HttpCode = response.StatusCode;
                            status.HttpResponse = responseText;
                            if (status.HttpCode == System.Net.HttpStatusCode.OK)
                            {
                                List<TanKeywords> result = JsonConvert.DeserializeObject<List<TanKeywords>>(responseText);
                                status.UserObject = result;
                                status.StatusMessage = "User Tans Assigned Successfully . .";
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


        public static async Task<RestStatus> PullTask(int currentRole, bool pullTasks)
        {
            RestStatus status = new RestStatus();
            List<string> tanNums = new List<string>();
            if (!string.IsNullOrEmpty(Sessionkey))
            {
                using (var httpClient = new HttpClient())
                {
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(BEARER_HEDER, Sessionkey);
                    httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(APPLICATION_JSON));

                    StringContent stringContent = new StringContent("productId=1");

                    HttpResponseMessage response = await httpClient.GetAsync(PULL_TASK + "?productId=1&currentRole=" + currentRole + "&pullTasks=" + pullTasks);
                    using (HttpContent content = response.Content)
                    {
                        string responseText = await content.ReadAsStringAsync();
                        status.HttpCode = response.StatusCode;
                        status.HttpResponse = responseText;
                        if (status.HttpCode == System.Net.HttpStatusCode.OK)
                        {
                            PullTask pullTask = JsonConvert.DeserializeObject<PullTask>(responseText);
                            status.UserObject = pullTask;
                            status.StatusMessage = "User Tans Pulled Successfully . .";
                        }
                        else
                            status.StatusMessage = responseText;
                    }
                }
            }
            return status;
        }
        public static async Task<RestStatus> SubmitTan(int tanId, int currentRole)
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

                        StringContent stringContent = new StringContent("");

                        HttpResponseMessage response = await httpClient.PostAsync(
                            SUBMIT_TANS_URL + "?tanId=" + tanId + "&currentRole=" + currentRole,
                            stringContent);
                        using (HttpContent content = response.Content)
                        {
                            string responseText = await content.ReadAsStringAsync();
                            status.HttpCode = response.StatusCode;
                            status.HttpResponse = responseText;
                            if (status.HttpCode == System.Net.HttpStatusCode.OK && responseText.Equals("true"))
                            {
                                status.StatusMessage = responseText;
                                status.UserObject = true;
                            }
                            else
                            {
                                status.StatusMessage = responseText;
                                status.UserObject = false;
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
        public static async Task<RestStatus> ApproveTan(int tanId, int currentRole, [Optional] bool QCRequired)
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

                        StringContent stringContent = new StringContent("");

                        HttpResponseMessage response = await httpClient.PostAsync(
                            APPROVE_TANS_URL + "?tanId=" + tanId + "&currentRole=" + currentRole + "&QCRequired=" + QCRequired,
                            stringContent);
                        using (HttpContent content = response.Content)
                        {
                            string responseText = await content.ReadAsStringAsync();
                            status.HttpCode = response.StatusCode;
                            status.HttpResponse = responseText;
                            if (status.HttpCode == System.Net.HttpStatusCode.OK && responseText.Equals("true"))
                            {
                                status.StatusMessage = responseText;
                                status.UserObject = true;
                            }
                            else
                            {
                                status.StatusMessage = "Can't Approve TAN !";
                                status.UserObject = false;
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

        public static async Task<RestStatus> RejectTan(int tanId, int productRoleId)
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

                        StringContent stringContent = new StringContent("");

                        HttpResponseMessage response = await httpClient.PostAsync(REJECT_TANS_URL + "?tanId=" + tanId + "&currentRole=" + productRoleId, stringContent);
                        using (HttpContent content = response.Content)
                        {
                            string responseText = await content.ReadAsStringAsync();
                            status.HttpCode = response.StatusCode;
                            status.HttpResponse = responseText;
                            if (status.HttpCode == System.Net.HttpStatusCode.OK && responseText.Equals("true"))
                            {
                                status.StatusMessage = responseText;
                                status.UserObject = true;
                            }
                            else
                            {
                                status.StatusMessage = "Can't Process TAN !";
                                status.UserObject = false;
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


        public static async Task<RestStatus> GetTan(int id)
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

                        StringContent stringContent = new StringContent("productId=1");

                        HttpResponseMessage response = await httpClient.GetAsync(FULL_TAN + "?tanId=" + id + "&currentRole=" + U.RoleId);
                        using (HttpContent content = response.Content)
                        {
                            string responseText = await content.ReadAsStringAsync();
                            status.HttpCode = response.StatusCode;
                            status.HttpResponse = responseText;
                            if (status.HttpCode == System.Net.HttpStatusCode.OK)
                            {
                                var tanData = JsonConvert.DeserializeObject<TanInfoDTO>(responseText);
                                status.UserObject = tanData;
                                status.StatusMessage = "User Tans Loaded Successfully . .";
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

        public static async Task<RestStatus> GetRoleBasedTan(int id, int Role)
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

                        StringContent stringContent = new StringContent("productId=1");

                        HttpResponseMessage response = await httpClient.GetAsync(ROLEBASED_FULL_TAN + "?tanId=" + id + "&currentRole=" + Role);
                        using (HttpContent content = response.Content)
                        {
                            string responseText = await content.ReadAsStringAsync();
                            status.HttpCode = response.StatusCode;
                            status.HttpResponse = responseText;
                            if (status.HttpCode == System.Net.HttpStatusCode.OK)
                            {
                                var tanData = JsonConvert.DeserializeObject<TanInfoDTO>(responseText);
                                status.UserObject = tanData;
                                status.StatusMessage = "Tan Data Loaded Successfully . .";
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


        //public static async Task<Stream> GetTanThroughwcf(int id)
        //{
        //    
        //    try
        //    {
        //        //SplashManager.ShowText("Dowloading Tan....");
        //        //using (S3ConnectionClient client = new S3ConnectionClient())
        //        //{
        //        //    client.Endpoint.Binding.SendTimeout = new TimeSpan(10, 1, 30);
        //        //    return await Task<Stream>.Factory.StartNew(() =>
        //        //    {
        //        //        Stream stream;
        //        //        ;
        //        //        stream = client.GetS3Object(id);
        //        //        return stream;
        //        //    });
        //        //}


        //    }
        //    catch (Exception ex)
        //    {
        //        Log.This(ex);
        //        throw;
        //    }
        //}
        public static async Task<RestStatus> SaveTan(OfflineDTO ffflineDTO, int currentRoleId)
        {
            RestStatus status = new RestStatus();
            try
            {
                var start = DateTime.Now;
                string dataToSave = JsonConvert.SerializeObject(ffflineDTO);
                Debug.WriteLine($"Prepared string from object to pass as Post parameter in  {(DateTime.Now - start).TotalSeconds} seconds. Json Lenght: {dataToSave.Length}");
                List<string> tanNums = new List<string>();
                if (!string.IsNullOrEmpty(Sessionkey))
                {
                    HttpClientHandler handler = new HttpClientHandler()
                    {
                        AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
                    };
                    using (var httpClient = new HttpClient(handler))
                    {
                        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(BEARER_HEDER, Sessionkey);
                        httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(APPLICATION_JSON));
                        StringContent stringContent = new StringContent("productId=1");

                        HttpResponseMessage response = await httpClient.PostAsync(SAVE_TAN + "?currentUserRole=" + currentRoleId + "&UserName=" + U.UserName, new StringContent(dataToSave, Encoding.UTF8, "application/json"));
                        using (HttpContent content = response.Content)
                        {
                            string responseText = await content.ReadAsStringAsync();
                            status.HttpCode = response.StatusCode;
                            status.StatusMessage = responseText;
                            if (status.HttpCode == System.Net.HttpStatusCode.OK)
                                status.HttpCode = System.Net.HttpStatusCode.OK;
                        }
                    }
                }
                return status;
            }
            catch (Exception ex)
            {
                Log.This(ex);
                status.HttpCode = HttpStatusCode.InternalServerError;
                status.StatusMessage = "Some error occured.";
                return status;
            }
        }

        public static async Task<RestStatus> Shipments()
        {
            RestStatus status = new RestStatus();
            if (!string.IsNullOrEmpty(Sessionkey))
            {
                using (var httpClient = new HttpClient())
                {
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(BEARER_HEDER, Sessionkey);
                    httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(APPLICATION_JSON));


                    HttpResponseMessage response = await httpClient.GetAsync(SHIPMENTS_URL);
                    using (HttpContent content = response.Content)
                    {
                        string responseText = await content.ReadAsStringAsync();
                        status.HttpCode = response.StatusCode;
                        status.HttpResponse = responseText;
                        if (status.HttpCode == System.Net.HttpStatusCode.OK)
                        {
                            var Batches = JsonConvert.DeserializeObject<List<BatchDTO>>(responseText);
                            status.UserObject = Batches;
                            status.StatusMessage = "Shipments Loaded Successfully . .";
                        }
                        else
                            status.StatusMessage = responseText;
                    }
                }
            }
            return status;
        }

        public static async Task<RestStatus> GetAllCurators()
        {
            RestStatus status = new RestStatus();
            if (!string.IsNullOrEmpty(Sessionkey))
            {
                using (var httpClient = new HttpClient())
                {
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(BEARER_HEDER, Sessionkey);
                    httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(APPLICATION_JSON));

                    HttpResponseMessage response = await httpClient.GetAsync(GET_CURATORS_URL);
                    using (HttpContent content = response.Content)
                    {
                        string responseText = await content.ReadAsStringAsync();
                        status.HttpCode = response.StatusCode;
                        status.HttpResponse = responseText;
                        if (status.HttpCode == System.Net.HttpStatusCode.OK)
                        {
                            var curators = JsonConvert.DeserializeObject<List<UserDTO>>(responseText);
                            status.UserObject = curators;
                            status.StatusMessage = "curators Loaded Successfully . .";
                        }
                        else
                            status.StatusMessage = responseText;
                    }
                }
            }
            return status;
        }

        public static async Task<RestStatus> GetStaticData()
        {

            try
            {
                RestStatus status = new RestStatus();
                using (var httpClient = new HttpClient())
                {
                    StringContent stringContent = new StringContent(String.Empty);
                    HttpResponseMessage response = await httpClient.GetAsync(APP_STATIC);
                    using (HttpContent content = response.Content)
                    {
                        string responseText = await content.ReadAsStringAsync();
                        status.HttpCode = response.StatusCode;
                        status.HttpResponse = responseText;
                        if (status.HttpCode == System.Net.HttpStatusCode.OK)
                        {
                            status.UserObject = JsonConvert.DeserializeObject<AppStaticDTO>(responseText);
                            status.StatusMessage = "Static Data Loaded Successfully . .";
                        }
                        else
                            status.StatusMessage = responseText;
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
