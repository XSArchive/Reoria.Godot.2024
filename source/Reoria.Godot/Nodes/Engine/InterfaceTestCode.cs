using Godot;
using System;

namespace Reoria.Godot.Nodes.Engine;

public partial class InterfaceTestCode : Control
{
    public PanelContainer MainMenu { get; private set; }
    public PanelContainer NetworkMenu { get; private set; }
    public Button ButtonJoin { get; private set; }
    public Button ButtonHost { get; private set; }
    public Button ButtonQuit { get; private set; }
    public Button ButtonDisconnect { get; private set; }
    public Label LabelStatus { get; private set; }
    public Label LabelHost { get; private set; }
    public Label LabelId { get; private set; }

    public override void _Ready()
    {
        this.MainMenu ??= this.GetNode<PanelContainer>("MainMenu");
        this.NetworkMenu ??= this.GetNode<PanelContainer>("NetworkMenu");

        this.ButtonJoin ??= this.MainMenu.GetNode<VFlowContainer>("MenuContainer").GetNode<Button>("ButtonJoin");
        this.ButtonHost ??= this.MainMenu.GetNode<VFlowContainer>("MenuContainer").GetNode<Button>("ButtonHost");
        this.ButtonQuit ??= this.MainMenu.GetNode<VFlowContainer>("MenuContainer").GetNode<Button>("ButtonQuit");

        this.ButtonDisconnect ??= this.NetworkMenu.GetNode<FlowContainer>("MenuContainer").GetNode<Button>("ButtonDisconnect");
        this.LabelStatus ??= this.NetworkMenu.GetNode<FlowContainer>("MenuContainer").GetNode<Label>("LabelStatus");
        this.LabelHost ??= this.NetworkMenu.GetNode<FlowContainer>("MenuContainer").GetNode<Label>("LabelHost");
        this.LabelId ??= this.NetworkMenu.GetNode<FlowContainer>("MenuContainer").GetNode<Label>("LabelId");

        this.ButtonJoin.Pressed += this.ButtonJoin_Pressed;
        this.ButtonHost.Pressed += this.ButtonHost_Pressed;
        this.ButtonQuit.Pressed += this.ButtonQuit_Pressed;
        this.ButtonDisconnect.Pressed += this.ButtonDisconnect_Pressed;

        GameEngine.Instance.Multiplayer.ConnectedToServer += this.Multiplayer_ConnectedToServer;
        GameEngine.Instance.Multiplayer.ConnectionFailed += this.Multiplayer_ConnectionFailed;
        GameEngine.Instance.Multiplayer.ServerDisconnected += this.Multiplayer_ServerDisconnected;
        GameEngine.Instance.Multiplayer.PeerConnected += this.Multiplayer_PeerConnected;
        GameEngine.Instance.Multiplayer.PeerDisconnected += this.Multiplayer_PeerDisconnected;

        base._Ready();
    }

    public override void _Process(double delta)
    {
        if(NetworkManager.IsAvailable)
        {
            if (this.NetworkMenu.Visible)
            {
                try
                {
                    this.LabelStatus.Text = $"Status: {NetworkManager.ConnectionStatus}";
                    this.LabelHost.Text = $"Host: {(NetworkManager.IsServer ? "Server" : "Client")}";
                    this.LabelId.Text = $"ID: {NetworkManager.UniqueId}";
                }
                catch (Exception e)
                {
                    GD.PrintErr(e);
                    this.ButtonDisconnect_Pressed();
                }
            }
        }
        else
        {
            if(this.NetworkMenu.Visible)
            {
                this.ResetNetworkUi();
            }
        } 

        base._Process(delta);
    }

    private void ResetNetworkUi()
    {
        this.ButtonJoin.Disabled = false;
        this.ButtonHost.Disabled = false;
        this.ButtonDisconnect.Disabled = false;
        this.NetworkMenu.Visible = false;
        this.MainMenu.Visible = true;
        this.LabelStatus.Text = "Status: Not Connected";
        this.LabelHost.Text = "Host: 0.0.0.0";
        this.LabelId.Text = "ID: 0";
    }

    private void OpenNetworkUi()
    {
        this.NetworkMenu.Visible = true;
        this.MainMenu.Visible = false;
        this.LabelStatus.Text = $"Status: {NetworkManager.ConnectionStatus}";
        this.LabelHost.Text = $"Host: {(NetworkManager.IsServer ? "Server" : "Client")}";
        this.LabelId.Text = $"ID: {NetworkManager.UniqueId}";
    }

    private void ButtonQuit_Pressed() => this.GetTree().Quit(); 
    private void Multiplayer_ConnectedToServer() => this.OpenNetworkUi();
    private void Multiplayer_ConnectionFailed() => this.Multiplayer_ServerDisconnected();

    private void ButtonHost_Pressed()
    {
        this.ButtonJoin.Disabled = true;
        this.ButtonHost.Disabled = true;

        if (GameEngine.Instance.Network.CreateHost(7234, 32))
        {
            this.OpenNetworkUi();
            return;
        }

        this.ResetNetworkUi();
    }

    private void ButtonJoin_Pressed()
    {
        this.ButtonJoin.Disabled = true;
        this.ButtonHost.Disabled = true;

        if (GameEngine.Instance.Network.ConnectToHost("127.0.0.1", 7234))
        {
            
        }
    }

    private void ButtonDisconnect_Pressed()
    {
        this.ButtonDisconnect.Disabled = true;

        if (GameEngine.Instance.Network.CloseConnection())
        {
            this.ResetNetworkUi();
        }

        this.ButtonDisconnect.Disabled = false;
    }

    private void Multiplayer_ServerDisconnected()
    {
        if (GameEngine.Instance.Network.PurgeConnection())
        {
            this.ResetNetworkUi();
        }
    }

    private void Multiplayer_PeerConnected(long id)
    {
        if(NetworkManager.IsServer)
        {

        }

        GD.Print($"A new client with the ID '{id}' has connected to the server.");
    }

    private void Multiplayer_PeerDisconnected(long id)
    {
        if(NetworkManager.IsServer)
        {

        }

        GD.Print($"The client with the ID '{id}' has disconnected from the server.");
    }
}
