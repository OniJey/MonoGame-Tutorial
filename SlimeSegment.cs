using System;
using System.Collections.Generic;
using MonoGameLibrary.Graphics;
using Microsoft.Xna.Framework;
using MonoGameLibrary;
using MonoGameTutorial.Scenes;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGameTutorial;

 class SlimeSegment
{
    private TextureAtlas _atlas = TextureAtlas.FromFile(Core.Content, "images/atlas.xml");
    private static Directions _direction;
    private int index;
    private Random _rng = new Random();
    private static TileMap _tileMap = TileMap.FromFile(Core.Content, "images/map-definition.xml");
    public enum Directions
    {
        Right,
        Left,
        Up,
        Down
    }
    public AnimatedSprite Sprite;
    
    private static bool hasTurned;
    private static double _moveTimer = 0;
    private static double _moveSeconds = 0.25;
    private static int _length => Segments.Count;
    public static List<SlimeSegment> Segments;
    public static SlimeSegment Head => (Segments != null)? Segments[0] : null;
    public static SlimeSegment Tail => (Segments != null)? Segments[_length - 1] : null;
    public static int Score;

    public Vector2 Position 
    {
        get => Sprite.Position; 
        set => Sprite.Position = value;
    }

    public SlimeSegment(int count, Vector2 position)
    {
        //initialization logic
        if(Segments == null)
        {
            hasTurned = false;
            Segments = new List<SlimeSegment>();
            _tileMap.Scale*=4;
        }
        //create the segment's sprite
        Sprite = _atlas.CreateAnimatedSprite("slime-animation", $"Slime{index}");
        Sprite.CollisionReaction = CollisionReactions.Trigger;
        Sprite.TriggerAction = slimeCollisionAction;
        Sprite.Scale *= 4;

        //initialize teh
        index = _length;
        Segments.Add(this);
        Position = position;


        if(count > 1)
        {
            new SlimeSegment(--count, new Vector2(Position.X - _tileMap.TileWidth, Position.Y));
        }
    }

    public static void Turn(Directions newDirection)
    {
        if(!hasTurned)
            _direction = (newDirection == Reverse(_direction))? _direction : newDirection;
            hasTurned = true;

    }

    private static void Move()
    {
        Vector2 newPos = Vector2.Zero;
        switch(_direction)
        {
            case Directions.Right:
                newPos = new Vector2(Head.Position.X + _tileMap.TileWidth, Head.Position.Y);
                break;
            case Directions.Left:
                newPos = new Vector2(Head.Position.X - _tileMap.TileWidth, Head.Position.Y);
                break;
            case Directions.Up:
                newPos = new Vector2(Head.Position.X, Head.Position.Y - _tileMap.TileHeight);
                break;
            case Directions.Down:
                newPos = new Vector2(Head.Position.X, Head.Position.Y + _tileMap.TileHeight);
                break;
        }
        Tail.Position = newPos;

        SlimeSegment tail = Tail;
        Segments.RemoveAt(_length - 1);
        Segments.Insert(0, tail);
        hasTurned = false;
    }

    private void slimeCollisionAction(Sprite slime, Sprite other)
    {
        if(other.Name == "bat")
        {
            PhysicsSprite bat = (PhysicsSprite) other;
            int column = _rng.Next(2, _tileMap.Columns - 2);
            int row = _rng.Next(2, _tileMap.Rows -2);

            bat.Position = new Vector2(column * _tileMap.TileWidth, row * _tileMap.TileHeight);
            Vector2 NewVelocity = new Vector2(_rng.Next(1,100)/100f, _rng.Next(1, 100)/100f);
            NewVelocity.Normalize();
            NewVelocity *= bat.Velocity.Length();
            bat.Velocity = NewVelocity;

            Score += 100;
        } else
        {
            if(Core.s_activeScene is GameScene scene)
            {   
                //initiate game over if the slime collides with the bounding box or another slime
                scene.GameOver();
                //make each of the segments disapear
                for(int i = _length -1; i >= 0; i--)
                {
                    Segments[i].Sprite.Color = new Color(0,0,0,0);
                }
            } 
            else
                throw new InvalidOperationException("How did you even get here");
        }
    }

    private static Directions Reverse(Directions direction)
    {
        switch(direction)
        {
            case Directions.Right:
                return Directions.Left;
            case Directions.Left:
                return Directions.Right;
            case Directions.Up:
                return Directions.Down;
            case Directions.Down:
                return Directions.Up;
            default:
                return Directions.Right;
        }
    }

    public static void Update(GameTime gameTime)
    {
        foreach(SlimeSegment segment in Segments)
        {
            AnimatedSprite sprite = segment.Sprite;

            sprite.Update(gameTime);
        }
        if(Score/100+4 > _length)
        {
            new SlimeSegment(1, Tail.Position);
        }
        _moveTimer += gameTime.ElapsedGameTime.TotalSeconds;
        
        if(_moveTimer >= _moveSeconds)
        {
            _moveTimer %= _moveSeconds;
            Move();
        }
    }

    public static void Reset()
    {
        Score = 0;
        _direction = Directions.Right;
        Segments.Clear();
    }

    public static void Draw(SpriteBatch batch)
    {
        foreach(SlimeSegment segment in Segments)
        {
            AnimatedSprite sprite = segment.Sprite;

            sprite.Draw(batch, sprite.Position);
        }
    }
}