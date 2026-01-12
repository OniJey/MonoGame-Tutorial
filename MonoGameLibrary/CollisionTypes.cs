namespace MonoGameLibrary.Graphics;

public enum CollisionTypes
{   
    /// <summary>
    /// Uses Circle for collision checks
    /// </summary>
    Circle,
    /// <summary>
    /// Using Rectange axis-aligned bounding box for collision checks
    /// </summary>
    AABB,
    /// <summary>
    /// Detects collision when the other sprite is outside of it's bounds
    /// </summary>
    Container
    /// <summary>
    /// To be implemented
    /// </summary>
    // Complex

}