using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shopping_Kart.Sprites
{
    public class Sprite : Component, ICloneable
    {
        #region Fields
        protected Texture2D _texture;     // Texture of the sprite
        protected Vector2 _position;      // Position of the sprite
        protected Vector2 _origin;        // Origin of the sprite
        protected float _rotation;        // Rotation of the sprite
        protected float _scale;           // Scale of the sprite
        protected float _layer;           // Layer in which is the sprite
        #endregion

        #region Properties
        public Vector2 Position
        {
            get { return _position; }
            set { _position = value; }
        }

        public Vector2 Origin
        {
            get { return _origin; }
            set { _origin = value; }
        }

        public Rectangle Rectangle
        {
            get { return new Rectangle((int)Position.X, (int)Position.Y, _texture.Width, _texture.Height); }
        }

        public float Rotation
        {
            get { return _rotation; }
            set { _rotation = value; }
        }

        public Color Colour { get; set; }

        public bool IsRemoved { get; set; }

        public float Layer
        {
            get { return _layer; }
            set { _layer = value; }
        }

        // Area of potential collisions:
        public Rectangle CollisionArea
        {
            get
            {
                return new Rectangle(Rectangle.X, Rectangle.Y, MathHelper.Max(Rectangle.Width, Rectangle.Height), MathHelper.Max(Rectangle.Width, Rectangle.Height));
            }
        }

        public Matrix Transform
        {
            get
            {
                return Matrix.CreateTranslation(new Vector3(-Origin, 0)) *
                  Matrix.CreateRotationZ(Rotation) *
                  Matrix.CreateTranslation(new Vector3(Position, 0));
            }
        }

        // Texture data to check pixel collisions
        public readonly Color[] TextureData;
        #endregion

        #region Methods
        public Sprite(Texture2D texture)
        {
            _texture = texture;
            Origin = new Vector2(_texture.Width / 2, texture.Height / 2);
            _scale = 1;
            Colour = Color.White;

            // Save texture data into array
            TextureData = new Color[_texture.Width * _texture.Height];
            _texture.GetData(TextureData);
        }

        public bool Intersects(Sprite sprite)
        {
            if (this.TextureData == null)
                return false;

            if (sprite.TextureData == null)
                return false;

            // Calculate a matrix which transforms from A's local space into
            // world space and then into B's local space
            var transformAToB = this.Transform * Matrix.Invert(sprite.Transform);

            // When a point moves in A's local space, it moves in B's local space with a
            // fixed direction and distance proportional to the movement in A.
            // This algorithm steps through A one pixel at a time along A's X and Y axes
            // Calculate the analogous steps in B:
            var stepX = Vector2.TransformNormal(Vector2.UnitX, transformAToB);
            var stepY = Vector2.TransformNormal(Vector2.UnitY, transformAToB);

            // Calculate the top left corner of A in B's local space
            // This variable will be reused to keep track of the start of each row
            var yPosInB = Vector2.Transform(Vector2.Zero, transformAToB);

            for (int yA = 0; yA < this.Rectangle.Height; yA++)
            {
                // Start at the beginning of the row
                var posInB = yPosInB;

                for (int xA = 0; xA < this.Rectangle.Width; xA++)
                {
                    // Round to the nearest pixel
                    var xB = (int)Math.Round(posInB.X);
                    var yB = (int)Math.Round(posInB.Y);

                    if (0 <= xB && xB < sprite.Rectangle.Width &&
                        0 <= yB && yB < sprite.Rectangle.Height)
                    {
                        // Get the colors of the overlapping pixels
                        var colourA = this.TextureData[xA + yA * this.Rectangle.Width];
                        var colourB = sprite.TextureData[xB + yB * sprite.Rectangle.Width];

                        // If both pixel are not completely transparent
                        if (colourA.A != 0 && colourB.A != 0)
                        {
                            return true;
                        }
                    }

                    // Move to the next pixel in the row
                    posInB += stepX;
                }

                // Move to the next row
                yPosInB += stepY;
            }

            // No intersection found
            return false;
        }

        // Draw
        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(_texture, Position, null, Colour, _rotation, _origin, _scale, SpriteEffects.None, _layer);
        }

        // Update
        public override void Update(GameTime gameTime)
        {
            throw new NotImplementedException();
        }

        // Clone
        public object Clone()
        {
            var sprite = this.MemberwiseClone() as Sprite;

            return sprite;
        }
        #endregion
    }
}
