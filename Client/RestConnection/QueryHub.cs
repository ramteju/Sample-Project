
using Client.Common;
using Client.Logging;
using DTO;
using Entities.DTO;
using Newtonsoft.Json;
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
        public static async Task<RestStatus> MyQueries()
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

                        HttpResponseMessage response = await httpClient.GetAsync($"{My_QUERIES}?allUsers={(U.RoleId == 4 ? true : false)}");
                        using (HttpContent content = response.Content)
                        {
                            string responseText = await content.ReadAsStringAsync();
                            status.HttpCode = response.StatusCode;
                            status.HttpResponse = responseText;
                            if (status.HttpCode == System.Net.HttpStatusCode.OK)
                            {
                                MyQueriesDTO myQueries = JsonConvert.DeserializeObject<MyQueriesDTO>(responseText);
                                status.UserObject = myQueries;
                                status.StatusMessage = "User Queries Loaded Successfully . .";
                            }
                            else
                            {
                                status.StatusMessage = "Can't Load Queries";
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
            }
            return null;
        }
        public static async Task<RestStatus> SaveQuery(QueryDTO queryDto)
        {
            
            try
            {
                ;
                RestStatus status = new RestStatus();
                List<string> tanNums = new List<string>();
                if (!string.IsNullOrEmpty(Sessionkey))
                {
                    using (var httpClient = new HttpClient())
                    {
                        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(BEARER_HEDER, Sessionkey);
                        httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(APPLICATION_JSON));

                        StringContent stringContent = new StringContent("productId=1");

                        HttpResponseMessage response = await httpClient.PostAsync(SAVE_QUERY, new StringContent(JsonConvert.SerializeObject(queryDto), Encoding.UTF8, "application/json"));
                        using (HttpContent content = response.Content)
                        {
                            string responseText = await content.ReadAsStringAsync();
                            status.HttpCode = response.StatusCode;
                            status.HttpResponse = responseText;
                            if (status.HttpCode == System.Net.HttpStatusCode.OK)
                            {
                                status.StatusMessage = responseText;
                                status.UserObject = true;
                            }
                            else
                                status.StatusMessage = "Can't Save Query Details . . .";
                        }
                    }
                    ;
                }
                return status;
            }
            catch (Exception ex)
            {
                Log.This(ex);
                throw;
            }
        }
        public static async Task<RestStatus> Submit(int queryId, string msg)
        {
            
            try
            {
                ;
                RestStatus status = new RestStatus();
                List<string> tanNums = new List<string>();
                if (!string.IsNullOrEmpty(Sessionkey))
                {
                    using (var httpClient = new HttpClient())
                    {
                        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(BEARER_HEDER, Sessionkey);
                        httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(APPLICATION_JSON));
                        StringContent stringContent = new StringContent("productId=1");
                        HttpResponseMessage response = await httpClient.PostAsync(SUBMIT_QUERY + "?queryId=" + queryId + "&msg=" + msg, stringContent);
                        using (HttpContent content = response.Content)
                        {
                            string responseText = await content.ReadAsStringAsync();
                            status.HttpCode = response.StatusCode;
                            status.HttpResponse = responseText;
                            if (status.HttpCode == System.Net.HttpStatusCode.OK)
                            {
                                status.StatusMessage = responseText;
                                status.UserObject = true;
                            }
                            else
                                status.StatusMessage = "Can't Submit Query. . .";
                        }
                    }
                    ;
                }
                return status;
            }
            catch (Exception ex)
            {
                Log.This(ex);
                throw;
            }
        }

        public static async Task<RestStatus> Revert(int queryId, string msg)
        {
            
            try
            {
                ;
                RestStatus status = new RestStatus();
                List<string> tanNums = new List<string>();
                if (!string.IsNullOrEmpty(Sessionkey))
                {
                    using (var httpClient = new HttpClient())
                    {
                        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(BEARER_HEDER, Sessionkey);
                        httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(APPLICATION_JSON));
                        StringContent stringContent = new StringContent("productId=1");
                        HttpResponseMessage response = await httpClient.PostAsync(REJECT_QUERY + "?queryId=" + queryId + "&msg=" + msg, stringContent);
                        using (HttpContent content = response.Content)
                        {
                            string responseText = await content.ReadAsStringAsync();
                            status.HttpCode = response.StatusCode;
                            status.HttpResponse = responseText;
                            if (status.HttpCode == System.Net.HttpStatusCode.OK)
                            {
                                status.StatusMessage = responseText;
                                status.UserObject = true;
                            }
                            else
                                status.StatusMessage = "Can't Reject Query . . .";
                        }
                    }
                    ;
                }
                return status;
            }
            catch (Exception ex)
            {
                Log.This(ex);
                throw;
            }
        }


        public static async Task<RestStatus> IsQueryActive(int tanId)
        {
            
            try
            {
                ;
                RestStatus status = new RestStatus();
                List<string> tanNums = new List<string>();
                if (!string.IsNullOrEmpty(Sessionkey))
                {
                    using (var httpClient = new HttpClient())
                    {
                        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(BEARER_HEDER, Sessionkey);
                        httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(APPLICATION_JSON));
                        StringContent stringContent = new StringContent("productId=1");
                        HttpResponseMessage response = await httpClient.PostAsync(QUERY_STATUS + "?tanID=" + tanId , stringContent);
                        using (HttpContent content = response.Content)
                        {
                            string responseText = await content.ReadAsStringAsync();
                            status.HttpCode = response.StatusCode;
                            status.HttpResponse = responseText;
                            if (status.HttpCode == System.Net.HttpStatusCode.OK)
                            {
                                status.StatusMessage = responseText;
                                status.UserObject = true;
                            }
                            else
                                status.StatusMessage = "Can't Get Query status. . .";
                        }
                    }
                    ;
                }
                return status;
            }
            catch (Exception ex)
            {
                Log.This(ex);
                throw;
            }
        }
        public static async Task<RestStatus> AddResponse(string queryResponse, int queryId)
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

                        HttpResponseMessage response = await httpClient.PostAsync(ADD_RESPONSE + "?response=" + queryResponse + "&id=" + queryId, stringContent);
                        using (HttpContent content = response.Content)
                        {
                            string responseText = await content.ReadAsStringAsync();
                            status.HttpCode = response.StatusCode;
                            status.HttpResponse = responseText;
                            if (status.HttpCode == System.Net.HttpStatusCode.OK)
                            {
                                status.StatusMessage = responseText;
                                status.UserObject = true;
                            }
                            else
                                status.StatusMessage = "Can't Add Response . . .";
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
        public static async Task<RestStatus> Responses(int queryId)
        {
            
            try
            {
                ;
                RestStatus status = new RestStatus();
                List<string> tanNums = new List<string>();
                if (!string.IsNullOrEmpty(Sessionkey))
                {
                    using (var httpClient = new HttpClient())
                    {
                        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(BEARER_HEDER, Sessionkey);
                        httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(APPLICATION_JSON));

                        StringContent stringContent = new StringContent("productId=1");

                        HttpResponseMessage response = await httpClient.PostAsync(QUERY_RESPONSES + "?id=" + queryId, stringContent);
                        using (HttpContent content = response.Content)
                        {
                            string responseText = await content.ReadAsStringAsync();
                            status.HttpCode = response.StatusCode;
                            status.HttpResponse = responseText;
                            if (status.HttpCode == System.Net.HttpStatusCode.OK)
                            {
                                List<QueryResponseDTO> queryResponses = JsonConvert.DeserializeObject<List<QueryResponseDTO>>(responseText);
                                status.UserObject = queryResponses;
                                status.StatusMessage = "User Query Responses Loaded Successfully . .";
                            }
                            else
                                status.StatusMessage = "Can't Get Responses . . .";
                        }
                    }
                    ;
                }
                return status;
            }
            catch (Exception ex)
            {
                Log.This(ex);
                throw;
            }
        }

        public static async Task<RestStatus> QueryWorkflow(int queryId)
        {
            
            try
            {
                ;
                RestStatus status = new RestStatus();
                List<string> tanNums = new List<string>();
                if (!string.IsNullOrEmpty(Sessionkey))
                {
                    using (var httpClient = new HttpClient())
                    {
                        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(BEARER_HEDER, Sessionkey);
                        httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(APPLICATION_JSON));

                        StringContent stringContent = new StringContent("productId=1");

                        HttpResponseMessage response = await httpClient.PostAsync(QUERY_WORKFLOW + "?id=" + queryId, stringContent);
                        using (HttpContent content = response.Content)
                        {
                            string responseText = await content.ReadAsStringAsync();
                            status.HttpCode = response.StatusCode;
                            status.HttpResponse = responseText;
                            if (status.HttpCode == System.Net.HttpStatusCode.OK)
                            {
                                QueryWorkflowDTO queryWorkflow = JsonConvert.DeserializeObject<QueryWorkflowDTO>(responseText);
                                status.UserObject = queryWorkflow;
                                status.StatusMessage = "Workflows Loaded Successfully . .";
                            }
                            else
                                status.StatusMessage = "Can't Get worflow . . .";
                        }
                    }
                    ;
                }
                return status;
            }
            catch (Exception ex)
            {
                Log.This(ex);
                throw;
            }
        }
        public static async Task<RestStatus> Users()
        {
            
            try
            {
                ;
                RestStatus status = new RestStatus();
                List<string> tanNums = new List<string>();
                if (!string.IsNullOrEmpty(Sessionkey))
                {
                    using (var httpClient = new HttpClient())
                    {
                        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(BEARER_HEDER, Sessionkey);
                        httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(APPLICATION_JSON));

                        StringContent stringContent = new StringContent(String.Empty);

                        HttpResponseMessage response = await httpClient.PostAsync(ALL_USERS, stringContent);
                        using (HttpContent content = response.Content)
                        {
                            string responseText = await content.ReadAsStringAsync();
                            status.HttpCode = response.StatusCode;
                            status.HttpResponse = responseText;
                            if (status.HttpCode == System.Net.HttpStatusCode.OK)
                            {
                                List<String> users = JsonConvert.DeserializeObject<List<String>>(responseText);
                                status.UserObject = users;
                                status.StatusMessage = "Users Loaded Successfully . .";
                            }
                            else
                                status.StatusMessage = "Can't Get Users . . .";
                        }
                    }
                    ;
                }
                return status;
            }
            catch (Exception ex)
            {
                Log.This(ex);
                throw;
            }
        }
        public static async Task<RestStatus> QueryWorkflows()
        {
            
            try
            {
                ;
                RestStatus status = new RestStatus();
                List<string> tanNums = new List<string>();
                if (!string.IsNullOrEmpty(Sessionkey))
                {
                    using (var httpClient = new HttpClient())
                    {
                        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(BEARER_HEDER, Sessionkey);
                        httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(APPLICATION_JSON));

                        StringContent stringContent = new StringContent(String.Empty);

                        HttpResponseMessage response = await httpClient.PostAsync(QUERY_WORKFLOWS, stringContent);
                        using (HttpContent content = response.Content)
                        {
                            string responseText = await content.ReadAsStringAsync();
                            status.HttpCode = response.StatusCode;
                            status.HttpResponse = responseText;
                            if (status.HttpCode == System.Net.HttpStatusCode.OK)
                            {
                                List<QueryWorkflowUserDTO> users = JsonConvert.DeserializeObject<List<QueryWorkflowUserDTO>>(responseText);
                                status.UserObject = users;
                                status.StatusMessage = "Query Workflows Loaded Successfully . .";
                            }
                            else
                                status.StatusMessage = "Can't Get Query Workflows . . .";
                        }
                    }
                    ;
                }
                return status;
            }
            catch (Exception ex)
            {
                Log.This(ex);
                throw;
            }
        }
        public static async Task<RestStatus> SaveQueryWorkflows(List<QueryWorkflowUserDTO> dtos)
        {
            
            try
            {
                ;
                RestStatus status = new RestStatus();
                using (var httpClient = new HttpClient())
                {
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(BEARER_HEDER, Sessionkey);
                    httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(APPLICATION_JSON));
                    StringContent payload = new StringContent(JsonConvert.SerializeObject(dtos), Encoding.UTF8, "application/json");
                    HttpResponseMessage response = await httpClient.PostAsync(SAVE_QUERY_WORKFLOWS, payload);
                    using (HttpContent content = response.Content)
                    {
                        string responseText = await content.ReadAsStringAsync();
                        status.HttpCode = response.StatusCode;
                        status.HttpResponse = responseText;
                        if (status.HttpCode == System.Net.HttpStatusCode.OK)
                            status.StatusMessage = responseText;
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
        public static async Task<RestStatus> QueryReport(QueryReportRequestDTO dto)
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
                        StringContent payload = new StringContent(JsonConvert.SerializeObject(dto), Encoding.UTF8, "application/json");
                        HttpResponseMessage response = await httpClient.PostAsync(QUERIES_REPORT, payload);
                        using (HttpContent content = response.Content)
                        {
                            string responseText = await content.ReadAsStringAsync();
                            status.HttpCode = response.StatusCode;
                            status.HttpResponse = responseText;
                            if (status.HttpCode == System.Net.HttpStatusCode.OK)
                            {
                                List<QueryReportEntryDTO> reportEntries = JsonConvert.DeserializeObject<List<QueryReportEntryDTO>>(responseText);
                                status.UserObject = reportEntries;
                                status.StatusMessage = "Query Report Loaded Successfully . .";
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
    }
}
