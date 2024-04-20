using Godot;
using System;

namespace Reoria.Godot.Nodes.Engine;

/// <summary>
/// Defines functionality of the game engine <see cref="Node"/>.
/// </summary>
public partial class GameEngine : Node
{
    /// <summary>
    /// The reference to the global <see cref="GameEngine"/> <see cref="Node"/> instance.
    /// </summary>
    private static GameEngine instance;

    /// <summary>
    /// The reference to the global <see cref="GameEngine"/> <see cref="Node"/> instance.
    /// </summary>
    public static GameEngine Instance
    {
        get
        {
            // Check to see if the instance is assigned, and return it if it has, if it has not then close the game.
            return instance is not null ? instance
                : throw new NullReferenceException("The game instance has been lost, the game will now close.");
        }
    }

    /// <summary>
    /// Defines the reference to the <see cref="GameEngine"/>'s interface <see cref="Node"/>.
    /// </summary>
    public Control Interface { get; private set; }

    /// <summary>
    /// Constructs a new <see cref="GameEngine"/> instance and assignes the reference to it to <see cref="Instance"/>.
    /// </summary>
    public GameEngine()
    {
        // Check to see if the engine has already been created.
        if (instance is not null)
        {
            // Yes it has, we can not allow "Dave" to do this.
            throw new InvalidOperationException("The game can not load more then one instance, the game will now close.");
        }

        // Assign this instance of the engine to the instance variable.
        instance = this;
    }

    /// <summary>
    /// Called when the node is "ready", i.e. when both the node and its children have
    /// entered the scene tree. If the node has children, their <see cref="Node._Ready"/> 
    /// callbacks get triggered first, and the parent node will receive the ready 
    /// notification afterwards.
    /// </summary>
    public override void _Ready()
    {
        // Fetch and assign the child node variables if they have not been assigned yet.
        this.Interface ??= this.GetNode<Control>("Interface");

        // Pass to the base class' function.
        base._Ready();
    }
}
