using System;
using Microsoft.Xna.Framework;

namespace MonoGameLibrary;

public struct Circle : IEquatable<Circle>
{
    /// <summary>
    /// empty circle for copying, more efficient than instantiating a new circle every time
    /// </summary>
    private static readonly Circle _empty = new Circle();

    /// <summary>
    /// the center of the circle
    /// </summary>
    public readonly Point Position;

    /// <summary>
    /// the radius of the circle
    /// </summary>
    public readonly int Radius;

    /// <summary>
    /// the X componant of the radius
    /// </summary>
    public readonly int X => Position.X;

    /// <summary>
    /// the y componant of the position
    /// </summary>
    public readonly int Y => Position.Y;

    /// <summary>
    /// the rightmost bound of the circle
    /// </summary>
    public readonly int Right => X + Radius;

    /// <summary>
    /// the leftmost bound of the circle
    /// </summary>
    public readonly int Left => X - Radius;

    /// <summary>
    /// the highest bound of the circle
    /// </summary>
    public readonly int Top => Y - Radius;

    /// <summary>
    /// the lowest bound of the circle  
    /// </summary>
    public readonly int Bottom => Y + Radius;

    /// <summary>
    /// true if the circle is empty
    /// </summary>
    public readonly bool isEmpty => Position == Point.Zero && Radius == 0;

    /// <summary>
    /// pubilc accessor of _empty
    /// </summary>
    public static Circle Empty = _empty;

    /// <summary>
    /// Creates a circle at the specified position
    /// </summary>
    /// <param name="position">the position of the circle</param>
    /// <param name="radius">the radius of the circle</param>
    public Circle(Point position, int radius)
    {
        Position = position;
        Radius = radius;
    }

    /// <summary>
    /// creates a circle at the specified position
    /// </summary>
    /// <param name="x">the x componant of the position</param>
    /// <param name="y">the y componant of the position</param>
    /// <param name="radius">the radius of the circle</param>
    public Circle(int x, int y, int radius)
    {
        Position = new Point(x, y);
        Radius = radius;
    }
    

    /// <summary>
    /// returns true if the circle intersects with the given circle
    /// </summary>
    /// <param name="other">the other circle</param>
    public bool Intersects(Circle other)
    {
        int radiiSquared = (Radius + other.Radius) * (Radius + other.Radius);
        float distanceSquared = Vector2.DistanceSquared(this.Position.ToVector2(), other.Position.ToVector2());
        return distanceSquared < radiiSquared;
    }


    /// <summary>
    /// Returns true if the given Rectangle intersects this Circle
    /// </summary>
    /// <param name="rect">The Rectangle to check</param>
    /// <returns></returns>
    public bool Intersects(Rectangle rect)
    {
        Vector2 closestPoint = new Vector2(
            Math.Clamp(Position.X, rect.Left, rect.Right),
            Math.Clamp(Position.Y, rect.Top, rect.Bottom)
        );

        float DistanceSquared = Vector2.DistanceSquared(closestPoint, Position.ToVector2());
        return DistanceSquared < (Radius*Radius);
    }

    /// <summary>
    /// returns true if this circle is completely within the given rectangle
    /// </summary>
    /// <param name="rect">the rectangle to check against</param>
    public bool inRectangle(Rectangle rect)
    {
        return (
            Left > rect.Left &&
            Right < rect.Right &&
            Top > rect.Top &&
            Bottom < rect.Bottom
        );
    }

    /// <summary>
    /// Returns true if the circle's radius and position are the same as the given circle's
    /// </summary>
    /// <param name="other">the other circle instance</param>
    public readonly bool Equals(Circle other) => Radius == other.Radius && Position == other.Position;

    /// <summary>
    /// Returns true if the circle's radius and position are the same as the given circle's
    /// </summary>
    /// <param name="other">the other circle instance</param>
    public override readonly bool Equals(object other) => other is Circle && Equals(other);

    /// <summary>
    /// returns this circle as a HashCode
    /// </summary>
    /// <returns></returns>
    public override int GetHashCode() => HashCode.Combine(Position, Radius);

    /// <summary>
    /// gets the normal between this circle and a point
    /// </summary>
    /// <param name="point">the position to get the normal between this circle and it</param>
    public Vector2 getNormal(Point point)
    {
        return (Position - point).ToVector2();
    }

    /// <summary>
    /// equates two circles
    /// </summary>
    /// <param name="rhs">the right hand side circle</param>
    /// <param name="lhs">the left hand side circle</param>
    /// <returns>true if the circles' position and radius are identicle; false otherwise</returns>
    public static bool operator ==(Circle rhs, Circle lhs) => lhs.Equals(rhs);
    public static bool operator !=(Circle rhs, Circle lhs) => !lhs.Equals(rhs);
}