using System;
using Microsoft.AspNet.SignalR;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace ProductTracking.Hubs
{
    public class LiveHub : Hub
    {
        public static string TAN_CURATED = "<script type='text/javascript'>tansCurated();todaysCuration();curationProgress();totalReactions();</script>";
        public static string TAN_ACCEPTED = "<script type='text/javascript'>qcAccepted();todaysQC();</script>";
        public static string ONLINE = "<script type='text/javascript'>online();</script>";
        public static ConcurrentDictionary<string, List<string>> UserConnectionIds = new ConcurrentDictionary<string, List<string>>();

        public void Curated()
        {
            Clients.All.curated(TAN_CURATED);
        }

        public void Accepted()
        {
            Clients.All.accepted(TAN_ACCEPTED);
        }

        public void Online(string msg)
        {
            Clients.All.online(ONLINE);
        }
        public void Progress()
        {
            Clients.All.progress("Processing . .");
        }
        public void DeliveryStatus(string msg)
        {
            Clients.All.deliveryStatus(msg);
        }
        public void Notification(string msg)
        {
            Clients.All.notification(msg);
        }
        public override Task OnConnected()
        {
            var userName = Context.User.Identity.GetUserName();
            var connectionId = Context.ConnectionId;
            List<string> conectionIds = UserConnectionIds.ContainsKey(connectionId) ? UserConnectionIds[connectionId] : new List<string>();
            conectionIds.Add(connectionId);
            UserConnectionIds[userName] = conectionIds;
            Clients.All.online(ONLINE);
            return base.OnConnected();
        }
        public override Task OnDisconnected(bool stopCalled)
        {
            string userName = Context.User.Identity.GetUserName();
            if (UserConnectionIds.ContainsKey(userName))
            {
                var conectionIds = UserConnectionIds[userName];
                conectionIds.Remove(Context.ConnectionId);
                UserConnectionIds[userName] = conectionIds;
                Clients.All.online(ONLINE);
            }
            return base.OnDisconnected(stopCalled);
        }
    }
}