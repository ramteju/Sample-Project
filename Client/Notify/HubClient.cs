using Microsoft.AspNet.SignalR.Client;
using System;
using System.Threading;

namespace Client.Notify
{
    public class HubClient
    {
        static public event EventHandler<String> NotificationReceived = delegate { };
        public static string signalRId = null;

        public static void InitHub()
        {
            try
            {
                string qs = "machine=" + Environment.MachineName;
                var connection = new HubConnection(RestHub.BASE_URL, qs);
                IHubProxy hubProxy = connection.CreateHubProxy("LiveHub");
                connection.Headers.Add("Authorization", RestHub.BEARER_HEDER + " " + RestHub.Sessionkey);
                connection.Start().Wait();
                signalRId = connection.ConnectionId;

                hubProxy.On("deliveryStatus", message =>
                {
                    try
                    {
                        Thread thread = new Thread(() =>
                        {
                            NotificationReceived.Invoke(String.Empty, message);
                        });
                        thread.SetApartmentState(ApartmentState.STA);
                        thread.Start();
                    }
                    catch (Exception ex)
                    {

                    }
                });

                hubProxy.On("notification", message =>
                {
                    try
                    {
                        Thread thread = new Thread(() =>
                        {
                            NotificationReceived.Invoke(String.Empty, message);
                        });
                        thread.SetApartmentState(ApartmentState.STA);
                        thread.Start();
                    }
                    catch (Exception ex)
                    {

                    }
                });
            }
            catch (Exception ex)
            {

            }
        }
    }
}
