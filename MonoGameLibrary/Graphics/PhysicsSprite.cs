using System;
using System.ComponentModel;
using Microsoft.Xna.Framework;


namespace MonoGameLibrary.Graphics;

public class PhysicsSprite : Sprite
{
    /// <summary>
    /// How fast and in what direction the velocity of the sprite changes in pixels/second/second
    /// </summary>
    public Vector2 Acceleration {get; set;} = Vector2.Zero;

    /// <summary>
    /// How fast the sprite is going and the direction it's going in in pixels/second
    /// </summary>
    public Vector2 Velocity {get; set;} = Vector2.Zero;

    /// <summary>
    /// Unit vector representing the direction the sprite is moving in
    /// </summary>
    public Vector2 Direction => Vector2.Normalize(Velocity);

    /// <summary>
    /// Unit vector representing the directino of the sprite's accleration
    /// </summary>
    public Vector2 AccelerationDirection => Vector2.Normalize(Velocity);

    /// <summary>
    /// float representing the speed of the sprite
    /// </summary>
    public float Speed => Velocity.Length();

    /// <summary>
    /// float representing the rate of change in the speed of the sprite
    /// </summary>
    public float AccelerationMagnitude => Acceleration.Length();


    public PhysicsSprite(){}

    /// <summary>
    /// Instantiates a new physicsSprite with no velocity or acceleration that uses the specified TextureRegion as a texture
    /// </summary>
    /// <param name="region">The TextureRegion that will render when Draw() is called on this sprite</param>
    public PhysicsSprite(TextureRegion region)
    {
        Region = region;
    }

    /// <summary>
    /// Instantiates a new physicsSprite with no velocity or acceleration that uses the specified TextureRegion as a texture
    /// </summary>
    /// <param name="region">The TextureRegion that will render when Draw() is called on this sprite</param>
    /// <param name="velocity">The speed and direction of the Sprite</param>
    public PhysicsSprite(TextureRegion region, Vector2 velocity)
    {
        Region = region;
        Velocity = velocity;
    }

    /// <summary>
    /// Instantiates a new physicsSprite with no velocity or acceleration that uses the specified TextureRegion as a texture
    /// </summary>
    /// <param name="region">The TextureRegion that will render when Draw() is called on this sprite</param>
    /// <param name="velocity">The speed in pixels/second and direction of the Sprite</param>
    /// <param name="acceleration">The change in velocity per second</param>
    public PhysicsSprite(TextureRegion region, Vector2 velocity, Vector2 acceleration)
    {
        Region = region;
        Velocity = velocity;
        Acceleration = acceleration;
    }

    /// <summary>
    /// moves the sprite according the the gametime
    /// </summary>
    /// <param name="gameTime">the GameTime</param>
    public void Update(GameTime gameTime)
    {
        //the time that's passed from last frame to this one
        float deltaTime = (float) gameTime.ElapsedGameTime.TotalSeconds;

        //perform the kinematic equation
        Vector2 toMove = Velocity * deltaTime + 0.5f * Acceleration * (deltaTime * deltaTime);

        Move(toMove);
    }


    /// <summary>
    /// Does the CollisionReaction with the spriteSpeicified
    /// </summary>
    /// <param name="other">The sprite to perform collision with</param>
    new public void  doCollisionReaction(Sprite other)
    {
        if(!CollidesWith(other)) return;
        base.doCollisionReaction(other);

        switch(CollisionReaction)
        {
            case CollisionReactions.None:
                return;
            case CollisionReactions.Bounce:
                Bounce(other);
                return;
            case CollisionReactions.BounceTrigger:
                Bounce(other);
                Trigger(other);
                return;
        }
    }

    /// <summary>
    /// Mirror this and the other sprite's velocitites
    /// </summary>
    /// <param name="other">the colliding sprite</param>
    private void Bounce(Sprite other)
    {
        if(CollisionType == CollisionTypes.Circle &&
            other.CollisionType != CollisionTypes.Container)
        {
            //get the normal between the objects to reflect over
            Vector2 normal = CollisionCircle.getNormal(other.Position.ToPoint());
            normal.Normalize();
            
            //reflect over the normal
            Velocity = Velocity - 2 * Vector2.Dot(Velocity, normal) * normal;
            //prevent these objects from colliding
            other.Block(this, true);
        } else
        {
            int overlapLeft;
            int overlapRight;
            int overlapTop;
            int overlapBottom;

            Rectangle r2 = other.AABB;


            if(CollisionType == CollisionTypes.Circle)
            {
                Circle c1 = CollisionCircle;

                overlapLeft = c1.Right - r2.Left;
                overlapRight = r2.Right - c1.Left;
                overlapTop = c1.Bottom - r2.Top;
                overlapBottom = r2.Bottom - c1.Top;
            } else
            {
                Rectangle r1 = AABB;
                overlapLeft = r1.Right - r2.Left;
                overlapRight = r2.Right - r1.Left;
                overlapTop = r1.Bottom - r2.Top;
                overlapBottom = r2.Bottom - r1.Top;
            }

            


            //the normal to reflect over
            Vector2 normal = Vector2.Zero;

            int minOverlap = Math.Min(Math.Min(overlapLeft, overlapRight), Math.Min(overlapTop, overlapBottom));

            if(minOverlap == overlapLeft)
            {
                normal = new Vector2(-1, 0);
            } else if (minOverlap == overlapRight)
            {
                normal = new Vector2(1,0);
            } else if (minOverlap == overlapTop)
            {
                normal = new Vector2(0, -1);
            } else if (minOverlap == overlapBottom)
            {
                normal = new Vector2(0, 1);
            }
            
            
            if(other.CollisionType == CollisionTypes.Container && Vector2.Dot(Velocity, normal) > 0 ||
                other.CollisionType == CollisionTypes.AABB && Vector2.Dot(Velocity, normal) < 0)
                //reflect over the normal if the normal and velocity are at least perpendicular
                Velocity = Velocity - 2 * Vector2.Dot(Velocity, normal) * normal;

            //prevent intersection between the sprites
            other.Block(this, true);
        }
    }
}