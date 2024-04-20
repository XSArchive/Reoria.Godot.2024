using Godot;

namespace Reoria.Godot.Nodes.Engine;

public partial class NetworkManager : Node
{
    [Export]
    public ENetMultiplayerPeer Peer { get; protected set; }

    public static bool IsAvailable => GameEngine.Instance.Multiplayer.MultiplayerPeer != null;
    public static bool IsServer => GameEngine.Instance.Multiplayer.IsServer();
    public static long UniqueId => GameEngine.Instance.Multiplayer.GetUniqueId();
    public static MultiplayerPeer.ConnectionStatus ConnectionStatus => GameEngine.Instance.Multiplayer.MultiplayerPeer.GetConnectionStatus();

    public virtual bool ConnectToHost(string ipAddress, int port)
    {
        if (GameEngine.Instance.Multiplayer.MultiplayerPeer != null)
        {
            if (GameEngine.Instance.Multiplayer.MultiplayerPeer is not OfflineMultiplayerPeer)
            { return false; }
        }
        if (this.Peer != null)
        { return false; }

        this.Peer = new ENetMultiplayerPeer();
        Error result = this.Peer.CreateClient(ipAddress, port);

        if (result == Error.Ok)
        {
            GameEngine.Instance.Multiplayer.MultiplayerPeer = this.Peer;
            return true;
        }

        this.Peer = null;
        return false;
    }

    public virtual bool CreateHost(int port, int maxConnections)
    {
        if (GameEngine.Instance.Multiplayer.MultiplayerPeer != null)
        {
            if (GameEngine.Instance.Multiplayer.MultiplayerPeer is not OfflineMultiplayerPeer)
            { return false; }
        }
        if (this.Peer != null)
        { return false; }

        this.Peer = new ENetMultiplayerPeer();
        Error result = this.Peer.CreateServer(port, maxConnections);
        GD.Print(result.ToString());

        if (result == Error.Ok)
        {
            GameEngine.Instance.Multiplayer.MultiplayerPeer = this.Peer;
            return true;
        }

        this.Peer = null;
        return false;
    }

    public virtual bool CloseConnection()
    {
        if (GameEngine.Instance.Multiplayer.MultiplayerPeer != null && GameEngine.Instance.Multiplayer.MultiplayerPeer != this.Peer)
        { return false; }

        if (GameEngine.Instance.Multiplayer.MultiplayerPeer != null)
        {
            GameEngine.Instance.Multiplayer.MultiplayerPeer.Close();
            GameEngine.Instance.Multiplayer.MultiplayerPeer = null;
        }

        if (this.Peer != null)
        {
            this.Peer.Close();
            this.Peer = null;
        }

        return (GameEngine.Instance.Multiplayer.MultiplayerPeer is null) && (this.Peer is null);
    }

    public virtual bool PurgeConnection()
    {
        GameEngine.Instance.Multiplayer.MultiplayerPeer?.Close();
        GameEngine.Instance.Multiplayer.MultiplayerPeer = null;

        this.Peer?.Close();
        this.Peer = null;

        return (GameEngine.Instance.Multiplayer.MultiplayerPeer is null) && (this.Peer is null);
    }
}
