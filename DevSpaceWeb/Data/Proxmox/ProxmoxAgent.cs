using Corsinvest.ProxmoxVE.Api;
using DevSpaceWeb.Data.Teams;

namespace DevSpaceWeb.Data.Proxmox;

public class ProxmoxAgent
{
    public ProxmoxAgent(TeamProxmox data)
    {
        Connect(data);
    }

    public void Connect(TeamProxmox data)
    {
        if (Client != null)
            Client = null!;
        Client = new PveClient(data.Ip, data.Port);
        Client.ApiToken = $"{data.User}!{data.Token}={data.Secret}";
    }

    public PveClient Client = null!;
}
