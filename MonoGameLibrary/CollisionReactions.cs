using System.Collections.Generic;

namespace MonoGameLibrary.Graphics;
public enum CollisionReactions
{
    /// <summary>
    /// Block The colliding sprite from intersecting with this one
    /// </summary>
    Block,
    /// <summary>
    /// Block the other sprite from intersecting with this one without moving this sprite
    /// </summary>
    BlockAnchored,
    /// <summary>
    /// Bounce with the colliding sprite according to the velocities of both sprites
    /// </summary>
    Bounce,
    /// <summary>
    /// Trigger this sprite's TriggerAction
    /// </summary>
    Trigger,
    /// <summary>
    /// Block the colliding sprite from intersecting with this one & trigger this sprite's TriggerAction
    /// </summary>
    BlockTrigger,
    /// <summary>
    /// Block the other sprite from intersecting with this sprite without moving this sprite and trigger this sprite's TriggerAction
    /// </summary>
    BlockTriggerAnchored,
    /// <summary>
    /// Bounce with the colliding sprite according to the velocities of both sprites & trigger the sprite's TriggerAction
    /// </summary>
    BounceTrigger,
    /// <summary>
    /// Nothing will happen when this sprite collides with another sprite
    /// </summary>
    None
}