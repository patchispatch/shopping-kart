using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Shopping_Kart.Models;

namespace Shopping_Kart.Sprites
{
    public class Kart : Sprite
    {

        #region Properties
        // Control states
        private KeyboardState _currentKey;
        private KeyboardState _previousKey;

        private float _accelerationTimer = 0;
        private float _brakeTimer = 0;
        private float _speed;

        private Vector2 _moveDirection;
        private Vector2 _lookDirection;

        public float RotationSpeed = 4f;

        public float MaxSpeed = 8f;
        public float Acceleration = 1.1f;
        public float AccelerationGap = 0.1f;

        public float Brake = 1.25f;
        public float BrakeGap = 0.1f;

        public Input Input;
        #endregion

        #region Methods
        public Kart(Texture2D texture) : base(texture)
        {
            // Initialize directions
            _lookDirection = new Vector2(0, 0);
            _moveDirection = _lookDirection;

            _layer = 0f;
        }

        public override void Update(GameTime gameTime)
        {
            // Update control states
            _previousKey = _currentKey;
            _currentKey = Keyboard.GetState();

            // Move the kart
            Move(gameTime);
        }

        private void Move(GameTime gameTime)
        {
            // Checks the behaviour of accelerate button
            Accelerate(gameTime);

            // Checks the behaviour of brake button
            Braking(gameTime);

            // Check direction buttons
            Change_lookDirection();

            // Update the kart's position
            Position += (_moveDirection * _speed);

            // Clamp position to window
            Position = Vector2.Clamp(Position, new Vector2(_texture.Width / 2, _texture.Height / 2), 
                new Vector2(Game1.ScreenWidth - _texture.Width / 2, Game1.ScreenHeight - _texture.Height / 2));
        }

        private void Change_lookDirection()
        {
            // Left button pushed
            if (_currentKey.IsKeyDown(Input.Left))
            {
                _rotation -= MathHelper.ToRadians(RotationSpeed);
            }

            // Right button pushed
            if (_currentKey.IsKeyDown(Input.Right))
            {
                _rotation += MathHelper.ToRadians(RotationSpeed);
            }

            // Update look direction
            _lookDirection = new Vector2((float)Math.Sin(_rotation), -(float)Math.Cos(_rotation));
        }

        private void Accelerate(GameTime gameTime)
        {
            // If pushing
            if (_currentKey.IsKeyDown(Input.Accelerate))
            {
                // Move where you're heading when accelerating
                if (_moveDirection != _lookDirection)
                    _moveDirection = _lookDirection;

                // If not at max speed, accelerate at a certain frequence
                _accelerationTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;
                if (_speed < MaxSpeed && _accelerationTimer >= AccelerationGap)
                {
                    _speed += Acceleration;
                    _accelerationTimer = 0;
                }
            }

            // If not pushing
            if(_currentKey.IsKeyUp(Input.Accelerate))
            {
                // If moving, deccelerate until zero at a certain frequence
                _accelerationTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;
                if (_speed > 0 && _accelerationTimer >= AccelerationGap)
                {
                    _speed = Math.Max(_speed - Acceleration, 0);
                    _accelerationTimer = 0;
                }
            }
        }

        // TO DO: it doesn't fully brake, fix it
        private void Braking(GameTime gameTime)
        {
            // If pushing
            if (_currentKey.IsKeyDown(Input.Brake))
            {
                _brakeTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;
                if (_speed > 0 && _brakeTimer >= BrakeGap)
                {
                    _speed = Math.Max(_speed - Brake, 0);
                    _brakeTimer = 0;
                }
            }
        }
        #endregion
    }
}
