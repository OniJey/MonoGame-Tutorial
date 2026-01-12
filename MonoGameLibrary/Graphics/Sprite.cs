using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
namespace MonoGameLibrary.Graphics;

public class Sprite
{
    private Action<Sprite, Sprite> _triggerAction {get; set;}

    private Vector2 _previousPosition {get; set;}

    /// <summary>
    /// The position of the sprite relative to the window
    /// </summary>
    public Vector2 Position {get; set;}

    /// <summary>
    /// The region that the sprite occupies in it's TextureAtlas
    /// </summary>
    public TextureRegion Region {get; set;}

    /// <summary>
    /// The color mask of the sprite
    /// </summary>
    /// <remarks>
    /// Default: Color.White
    /// </remarks>
    public Color Color {get; set;} = Color.White;

    /// <summary>
    /// The SpriteEffects applied to the sprite on render
    /// </summary>
    /// <remarks>
    /// Default: SpriteEffects.None
    /// </remarks>
    public SpriteEffects SpriteEffects {get; set;} = SpriteEffects.None;


    /// <summary>
    /// The scale that the sprite is rendered with
    /// </summary>
    /// <remarks>
    /// Default (1,1)
    /// </remarks>
    public Vector2 Scale {get; set;} = Vector2.One;

    /// <summary>
    /// The origin for the sprite's rotation and transition
    /// </summary>
    /// <remarks>
     /// Default (0,0)
     /// </remarks>
    public Vector2 Origin {get; set;} = Vector2.Zero;

    /// <summary>
    /// The rotation of the sprite, in radians
    /// </summary>
    /// <remarks>
    /// Default: 0
    /// </remarks>
    public float Rotation {get; set;} = 0;

    /// <summary>
    /// The depth that the Sprite is drawn at, determines draw order if SpriteBatch.Start has been called 
    /// with the parameters "sortMode: SpriteSortMode.FrontToBack" or "sortMode: SpriteSortMode.BackToFront"
    /// </summary>
    public float Depth {get; set;} = 0;

    /// <summary>
    /// The width of the rendered sprite
    /// </summary>
    /// <remarks>
    /// calculated by taking the width of the sprite's region and multiplying it by the scale's X component
    /// </remarks>
    public float Width => Region.Width * Scale.X;

    /// <summary>
    /// The height of the rendered sprite
    /// </summary>
    /// <remarks>
    /// calculated by taking the height of the sprite's region and multiplying by the scale's Y component
    /// </remarks>
    public float Height => Region.Height * Scale.Y;

    /// <summary>
    /// The Axis-Aligned Bounding Box used in colision checks
    /// </summary>
    /// <remarks>
    /// will reset to 
    /// </remarks>
    public Rectangle AABB  
    {
        get => new Rectangle((Position - Origin).ToPoint(), new Vector2(Width, Height).ToPoint()); 
        set => Position = value.Location.ToVector2(); 
    }

    /// <summary>
    /// Circle used in collisionChecks if Sprite's CollisionType is Circle
    /// </summary>
    public Circle CollisionCircle {get; set;} = Circle.Empty;


    /// <summary>
    /// The radius of the collisionCircle
    /// </summary>
    public int CollisionRadius {get; set;} = 0;


    /// <summary>
    /// The CollisionType that this sprite uses to calculate whether or not this sprite and another are colliding
    /// </summary>
    public CollisionTypes CollisionType {get; set;} = CollisionTypes.AABB;


    /// <summary>
    /// The CollisionReaction that determines what happens when this sprite and another collide
    /// </summary>
    public CollisionReactions CollisionReaction {get; set;} = CollisionReactions.None;

    public Action<Sprite, Sprite> TriggerAction
    {
       get => _triggerAction;
       set => _triggerAction = value; 
    } 

    /// <summary>
    /// creates an empty sprite
    /// </summary>
    public Sprite(){}

    /// <summary>
    /// creates a sprite that uses the specified region as its texture
    /// </summary>
    /// <param name="region">A region of a texture file</param>
    public Sprite(TextureRegion region)
    {
        Region = region;
    }

    /// <summary>
    /// moves the scale and rotate origin to the center of the sprite
    /// </summary>
    public void CenterOrigin()
    {
        Origin = new Vector2(Region.Width, Region.Height) * 0.5f;
    }

    /// <summary>
    /// Draws the sprite
    /// </summary>
    /// <param name="batch">The SpriteBatch instance that will draw the sprite</param>
    /// <param name="position">The position the sprite will be drawn</param>
    public void Draw(SpriteBatch batch, Vector2 position)
    {
        Position = position;
        Region.Draw(
            batch,
            Position,
            Color,
            Rotation,
            Origin,
            Scale,
            SpriteEffects,
            Depth
        );
    }

    /// <summary>
    /// moves the sprite in teh specified direction by the specified magnitude of pixels
    /// </summary>
    /// <param name="magnitude">the amount of pixels in the directino to move</param>
    /// <param name="direction"></param>
    public void Move(Vector2 direction, float magnitude)
    {
        direction.Normalize();
        Vector2 movementVector = direction*magnitude;

        _previousPosition = Position;
        Position += movementVector;

        if(CollisionType == CollisionTypes.Circle)
        {
            updateCircle();
        }
    }
    
    /// <summary>
    /// moves the sprite by adding the movement vector to the sprite's current position
    /// </summary>
    /// <param name="movementVector"></param>
    public void Move(Vector2 movementVector)
    {
        _previousPosition = Position;
        Position += movementVector;

        if(CollisionType == CollisionTypes.Circle)
        {
            updateCircle();
        }
    }

    private void updateCircle()
    {
        CollisionCircle = new Circle(Position.ToPoint(), CollisionRadius);
    }

    /// <summary>
    /// Returns wheter or not the sprite is colliding with the given sprite
    /// </summary>
    /// <param name="collisionPartner">The sprite to check collision with</param>
    public bool CollidesWith(Sprite collisionPartner)
    {
        return (CollisionType, collisionPartner.CollisionType) switch
        {
            (CollisionTypes.Circle, CollisionTypes.Circle) =>
            CollisionCircle.Intersects(collisionPartner.CollisionCircle),

            (CollisionTypes.Circle, CollisionTypes.AABB) => 
            CollisionCircle.Intersects(collisionPartner.AABB),

            (CollisionTypes.AABB, CollisionTypes.Circle) => 
            collisionPartner.CollisionCircle.Intersects(AABB),

            (CollisionTypes.AABB, CollisionTypes.AABB) => 
            AABB.Intersects(collisionPartner.AABB), 

            (CollisionTypes.Container, CollisionTypes.Circle) => 
            !collisionPartner.CollisionCircle.inRectangle(AABB),

            (CollisionTypes.Circle, CollisionTypes.Container) =>
            !CollisionCircle.inRectangle(collisionPartner.AABB),

            (CollisionTypes.Container, CollisionTypes.AABB) =>
            !AABB.Contains(collisionPartner.AABB),

            (CollisionTypes.AABB, CollisionTypes.Container) =>
            !collisionPartner.AABB.Contains(AABB),
            _ => false
        };
    }

    public void doCollisionReaction(Sprite collisionPartner)
    {
        if(!CollidesWith(collisionPartner)) return;
        switch(CollisionReaction)
        {
            case CollisionReactions.None:
                return;
            case CollisionReactions.Block:
                Block(collisionPartner, false);
                break;
            case CollisionReactions.BlockAnchored:
                Block(collisionPartner, true);
                break;
            case CollisionReactions.Trigger:
                Trigger(collisionPartner);
                break;
            case CollisionReactions.BlockTrigger:
                Trigger(collisionPartner);
                Block(collisionPartner, false);
                break;
            case CollisionReactions.BlockTriggerAnchored:
                Trigger(collisionPartner);
                Block(collisionPartner, true);
                break;
        }
    }

/// <summary>
/// Block this sprite and the other from intersecting
/// </summary>
/// <param name="other">The colliding sprite</param>
/// <param name="isOtherAnchored">Whether or not blocking should move this sprite back</param>
public void Block(Sprite other, bool isAnchored)
{

    Vector2 separation = Vector2.Zero;
    
    if(other.CollisionType == CollisionTypes.Circle && 
       CollisionType == CollisionTypes.Circle)
    {
        // Circle-Circle separation
        float distance = (Position != other.Position)? Vector2.Distance(Position, other.Position): 1;
        float overlap = CollisionCircle.Radius + other.CollisionCircle.Radius - distance;
        Vector2 normal = (Position != other.Position)? Vector2.Normalize(Position - other.Position) : Vector2.One;

        separation = normal*overlap;
        
    } 
    else if(other.CollisionType != CollisionTypes.Circle && 
        CollisionType != CollisionTypes.Circle)
    {
        // AABB-AABB separation - find minimum separation
        Rectangle r1 = AABB;
        Rectangle r2 = other.AABB;
        bool isContainer = CollisionType == CollisionTypes.Container;
        
        if(isContainer)
        {
            // For containers, we push objects back inside
            // Calculate how far outside the container the object is on each side
            int overlapLeft = r1.Left - r2.Left;      
            int overlapRight = r2.Right - r1.Right;   
            int overlapTop = r1.Top - r2.Top;         
            int overlapBottom = r2.Bottom - r1.Bottom;
            
            // Push back from whichever edge is violated most
            if(overlapLeft > 0)
            {
                separation.X = -overlapLeft;
            }
            else if(overlapRight > 0)
            {
                separation.X = overlapRight;
            }
            
            if(overlapTop > 0)
            {
                separation.Y = -overlapTop;
            }
            else if(overlapBottom > 0)
            {
                separation.Y = overlapBottom;
            }
        } else {
            // Standard AABB collision - find minimum separation
            int overlapLeft = r1.Right - r2.Left;
            int overlapRight = r2.Right - r1.Left;
            int overlapTop = r1.Bottom - r2.Top;
            int overlapBottom = r2.Bottom - r1.Top;
            
            // Find minimum overlap (shortest separation distance)
            int minOverlapX = Math.Min(overlapLeft, overlapRight);
            int minOverlapY = Math.Min(overlapTop, overlapBottom);
            
            if(minOverlapX < minOverlapY)
            {
                // Separate horizontally
                int direction = (overlapLeft < overlapRight) ? -1 : 1;
                separation = (minOverlapX > 0)? new Vector2(direction * minOverlapX, 0) : new Vector2(1,0);
            }
            else
            {
                // Separate vertically
                int direction = (overlapTop < overlapBottom) ? -1 : 1;
                separation = (minOverlapY > 0)? new Vector2(0, direction * minOverlapY) : new Vector2(0,1);
            }
        }
    } else {
        // Circle-AABB separation
        Circle circle;
        Rectangle rect;
        
        if(CollisionType == CollisionTypes.Circle)
        {
            circle = CollisionCircle;
            rect = other.AABB;
        }
        else
        {
            circle = other.CollisionCircle;
            rect = AABB;
        }
        
        Vector2 circlePos = circle.Position.ToVector2();
        float leftDistance = circlePos.X - rect.Left;
        float rightDistance = rect.Right - circlePos.X;
        float topDistance = circle.Y - rect.Top;
        float bottomDistance = rect.Bottom - circle.Y;

        float closestSide = Math.Min(Math.Min(leftDistance, rightDistance), Math.Min(topDistance, bottomDistance));

        Vector2 closestPoint = Vector2.Zero;
        switch(closestSide)
        {
            case var _ when closestSide == leftDistance:
                closestPoint = new Vector2(rect.Left, circlePos.Y);
                break;
            case var _ when closestSide == rightDistance:
                closestPoint = new Vector2(rect.Right, circlePos.Y);
                break;
            case var _ when closestSide == topDistance:
                closestPoint = new Vector2(circlePos.X, rect.Top);
                break;
            case var _ when closestSide == bottomDistance:
                closestPoint = new Vector2(circle.X, rect.Bottom);
                break;
        }
        
        Vector2 diff = circlePos - closestPoint;
        float distance = (diff.Length() > 0.0001f)? diff.Length() : 1;
        Vector2 normal = diff / distance;
        float overlap = circle.Radius - distance;
        separation = normal * overlap;
    }

    //Move the sprites away from each other
    if(isAnchored)
    {
        other.Move(-separation);
    }
    else
    {
        other.Move(-separation*0.5f);
        Move(separation*0.5f);
    }
}

    protected void Trigger(Sprite other)
    {
        other.TriggerAction(this, other);
    }

    /// <summary>
    /// returns whether or not the Sprite's bounding box is within the bounds of the provided GameWindow instance
    /// </summary>
    /// <param name="window">The GameWindow instance to check</param>
    public bool OnScreen(GameWindow window)
    {
        return window.ClientBounds.Contains(AABB);
    }
}