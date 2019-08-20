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
        private float _residualTimer = 0;

        // Speed and direction of current movement
        private Vector2 _moveDirection;
        private float _speed;

        // Speed and direction of previous movement (residual speed)
        private Vector2 _residualDirection;
        private float _residualSpeed;

        // Pointing upfront the kart
        private Vector2 _lookDirection;

        public float RotationSpeed = 3.5f;

        // TO DO: friction system
        // These below are provisional

        // Acceleration values
        public float MaxSpeed = 7f;
        public float Acceleration = 1.1f;
        public float AccelerationGap = 0.1f;

        // Braking values
        public float Brake = 1.8f;
        public float BrakeGap = 0.1f;

        // Controls (keyboard only for now)
        public Input Input;
        #endregion

        #region Methods
        public Kart(Texture2D texture) : base(texture)
        {
            // Initialize directions
            _lookDirection = new Vector2(0, 0);
            _moveDirection = _lookDirection;

            _layer = 0f;

            // Scale of the sprite
            _scale = 0.5f;
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
            Position += (_moveDirection * _speed) + (_residualDirection * _residualSpeed);

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
                // Only first time
                if (_previousKey.IsKeyDown(Input.Accelerate))
                {
                    // Speed becomes residual
                    _residualSpeed = _speed;
                    _residualDirection = _moveDirection;
                    _speed = 0;
                }
            }
        }

        // Thinking about removing the brake
        private void Braking(GameTime gameTime)
        {
            // If pushing
            if (_currentKey.IsKeyDown(Input.Brake))
            {
                _brakeTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;
                if (_residualSpeed > 0 && _brakeTimer >= BrakeGap)
                {
                    _residualSpeed = Math.Max(_residualSpeed - Brake, 0);
                    _brakeTimer = 0;
                }
            }
            else
            {
                // Deccelerate slower
                _residualTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;
                if (_residualSpeed > 0 && _residualTimer >= BrakeGap)
                {
                    _residualSpeed = Math.Max(_residualSpeed - Acceleration, 0);
                    _residualTimer = 0;
                }
            }
        }
        #endregion
    }
}
