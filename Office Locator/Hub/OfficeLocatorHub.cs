using Microsoft.AspNet.SignalR;

namespace OffstageControls.OfficeLocator
{
    public class OfficeLocatorHub : Hub
    {
        public void Send(string name, string message)
        {
            Clients.All.addMessage(name, message);
        }

    }
}
