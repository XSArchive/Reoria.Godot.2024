using Godot;

namespace Reoria.Godot.Nodes.Engine;

public partial class NetworkManager : Node
{
    [Export]
    public ENetMultiplayerPeer Peer { get; protected set; }

    public static bool IsAvailable => GetMultiplayer().MultiplayerPeer != null;
    public static bool IsServer => GetMultiplayer().IsServer();
    public static long UniqueId => GetMultiplayer().GetUniqueId();
    public static MultiplayerPeer.ConnectionStatus ConnectionStatus => GetMultiplayer().MultiplayerPeer.GetConnectionStatus();
    protected static MultiplayerApi GetMultiplayer() => GameEngine.Instance.Multiplayer;

    public virtual bool ConnectToHost(string ipAddress, int port)
    {
        if (GetMultiplayer().MultiplayerPeer != null)
        {
            if (GetMultiplayer().MultiplayerPeer is not OfflineMultiplayerPeer)
            { return false; }
        }
        if (this.Peer != null)
        { return false; }

        this.Peer = new ENetMultiplayerPeer();
        Error result = this.Peer.CreateClient(ipAddress, port);

        if (result == Error.Ok)
        {
            GetMultiplayer().MultiplayerPeer = this.Peer;
            return true;
        }

        this.Peer = null;
        return false;
    }

    public virtual bool CreateHost(int port, int maxConnections)
    {
        if (GetMultiplayer().MultiplayerPeer != null)
        {
            if (GetMultiplayer().MultiplayerPeer is not OfflineMultiplayerPeer)
            { return false; }
        }
        if (this.Peer != null)
        { return false; }

        this.Peer = new ENetMultiplayerPeer();
        Error result = this.Peer.CreateServer(port, maxConnections);
        GD.Print(result.ToString());

        if (result == Error.Ok)
        {
            GetMultiplayer().MultiplayerPeer = this.Peer;
            return true;
        }

        this.Peer = null;
        return false;
    }

    public virtual bool CloseConnection()
    {
        if (GetMultiplayer().MultiplayerPeer != null && GetMultiplayer().MultiplayerPeer != this.Peer)
        { return false; }

        if (GetMultiplayer().MultiplayerPeer != null)
        {
            GetMultiplayer().MultiplayerPeer.Close();
            GetMultiplayer().MultiplayerPeer = null;
        }

        if (this.Peer != null)
        {
            this.Peer.Close();
            this.Peer = null;
        }

        return (GetMultiplayer().MultiplayerPeer is null) && (this.Peer is null);
    }

    public virtual bool PurgeConnection()
    {
        GetMultiplayer().MultiplayerPeer?.Close();
        GetMultiplayer().MultiplayerPeer = null;

        this.Peer?.Close();
        this.Peer = null;

        return (GetMultiplayer().MultiplayerPeer is null) && (this.Peer is null);
    }
}
