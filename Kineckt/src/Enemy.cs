﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;


namespace Kineckt
{
    public class Enemy : ModelRenderer
    {
        private const float Speed = 5;
        private static Model model;
        private static Texture texture;
        private static Rect rectangle = new Rect(new Vector2(400,400),new Vector2(5,5));
        private static readonly Vector3 MinPosition = new Vector3(-40, -200, -20);
        private static readonly Vector3 MaxPosition = new Vector3(40, 200, 30);

        

        enum State
        {
            Left,
            Right,
        }

        State _state = State.Left;

        public Enemy(GraphicsDevice graphicsDevice, RenderTarget2D shadow) : base("Enemy", graphicsDevice, shadow)
        {
            Texture = texture;
            Model = model;
        }

        public static void LoadContent(ContentManager content)
        {
            model = content.Load<Model>("models/baseEnemy");
            //texture = content.Load<Texture2D>("");
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            
            var deltaTime = (float) gameTime.ElapsedGameTime.TotalSeconds;

            if (_state == State.Left)
            {
                Position += Vector3.Left * deltaTime * Speed;
                rectangle.Position.X = Position.X;
                rectangle.Position.Y = Position.Z;
                if (Position.X < MinPosition.X)
                {
                    _state = State.Right;
                }
            }

            else if (_state == State.Right)
            {
                Position += Vector3.Right * deltaTime * Speed;
                rectangle.Position.X = Position.X;
                rectangle.Position.Y = Position.Z;
                if (Position.X > MaxPosition.X)
                {
                    _state = State.Left;
                }
            }
            var y = (float)Math.Sin(gameTime.TotalGameTime.TotalSeconds * 4.2f);
            Position = new Vector3(Position.X, y, Position.Z);

        }
    }
}
