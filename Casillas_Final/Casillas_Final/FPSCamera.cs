using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;

namespace Casillas_Votta_Final
{
     public class FPSCamera : Microsoft.Xna.Framework.GameComponent
     {
          //Matrices
          public Matrix view { get; protected set; }
          public Matrix projection { get; protected set; }

          //Vectors
          public Vector3 position;
          public Vector3 velocity;
          Vector3 direction, up;

          //Floats
          float fov = MathHelper.ToRadians(90);
          float minViewRange = 1;
          float maxViewRange = 3000;

          //Mouse & Keyboard States
          public MouseState prevMouse, currMouse;
          public KeyboardState prevKb, currKb = Keyboard.GetState();
          GamePadState gp = GamePad.GetState(PlayerIndex.One);
          GamePadState prevGp = GamePad.GetState(PlayerIndex.One);

          //Rotation Variables
          // totals limit movement
          //current is the starting point
          float totalYaw = MathHelper.PiOver4;
          float currentYaw = 0;
          float totalPitch = MathHelper.PiOver4;
          float currentPitch = 0;

          //Camera Movement
          float speed = 4;
          Vector3 movement;

          // Physics
          float gravity = 20f;
          float maxGravity = 200f;
          float minGravity = 1f;

          bool jumping = false;
          bool isPlayerOne = false;

          public BoundingBox Bounds
          {
               get
               {
                    return new BoundingBox(position, position + new Vector3(10, 10, 10));
               }
          }

          public Vector3 GetDirection
          {
               get { return direction; }
          }

          public Viewport viewport { get; set; }

          public FPSCamera(Game game, Vector3 pos, Vector3 target, Vector3 up, Viewport viewport, bool isPlayerOne)
               : base(game)
          {
               //Build Cameras view matrix
               position = pos;
               velocity = Vector3.Zero;
               direction = target - pos;
               direction.Normalize();
               this.up = up;
               this.isPlayerOne = isPlayerOne;
               //updates camera view
               CreateLookAt();

               //Build Cameras Projection Matrix
               projection = Matrix.CreatePerspectiveFieldOfView(fov, (float)Game.Window.ClientBounds.Width / (float)Game.Window.ClientBounds.Height, minViewRange, maxViewRange);

               this.viewport = viewport;
          }

          public override void Initialize()
          {
               int halfScreenWidth = Game.Window.ClientBounds.Width / 2;
               int halfScreenHeight = Game.Window.ClientBounds.Height / 2;
               //Set mouse position and initialize state
               Mouse.SetPosition(halfScreenWidth, halfScreenHeight);

               prevMouse = Mouse.GetState();

               speed = 4;
               movement = Vector3.Zero;

               base.Initialize();
          }

          public override void Update(GameTime gameTime)
          {
               gp = GamePad.GetState(PlayerIndex.One);

               float yawAngle = 0;
               float pitchAngle = 0;

               if (isPlayerOne)
               {
                    #region Mouse controlled Camera Rotations

                    yawAngle = (-MathHelper.PiOver4 / 150) * (Mouse.GetState().X - prevMouse.X);

                    if (Math.Abs(currentYaw + yawAngle) < totalYaw)
                    {
                         direction = Vector3.Transform(direction, Matrix.CreateFromAxisAngle(up, yawAngle));
                         currentYaw += yawAngle;
                    }
                    else currentYaw = 0;

                    pitchAngle = (MathHelper.PiOver4 / 150) * (Mouse.GetState().Y - prevMouse.Y);

                    if (Math.Abs(currentPitch + pitchAngle) < totalPitch)
                    {
                         direction = Vector3.Transform(direction, Matrix.CreateFromAxisAngle(Vector3.Cross(up, direction), pitchAngle));
                         currentPitch += pitchAngle;
                    }

                    #endregion
               }
               if (!isPlayerOne)
               {
                    #region ControllerCOntrolled Rotations


                    yawAngle = (-MathHelper.PiOver4 / 150) * (gp.ThumbSticks.Right.X * 10);

                    if (Math.Abs(currentYaw + yawAngle) < totalYaw)
                    {
                         direction = Vector3.Transform(direction, Matrix.CreateFromAxisAngle(up, yawAngle));
                         currentYaw += yawAngle;
                    }
                    else currentYaw = 0;

                    pitchAngle = (MathHelper.PiOver4 / 150) * (-gp.ThumbSticks.Right.Y * 10);

                    if (Math.Abs(currentPitch + pitchAngle) < totalPitch)
                    {
                         direction = Vector3.Transform(direction, Matrix.CreateFromAxisAngle(Vector3.Cross(up, direction), pitchAngle));
                         currentPitch += pitchAngle;
                    }

                    #endregion
               }

               CreateLookAt();

               if (isPlayerOne) useKeyboard();
               else useController();

               applyPhysics((float)gameTime.ElapsedGameTime.TotalSeconds);

               prevGp = gp;
               prevKb = currKb;
               prevMouse = Mouse.GetState();

               base.Update(gameTime);
          }

          private void CreateLookAt()
          {
               view = Matrix.CreateLookAt(position, position + direction, up);
          }

          private void MoveForward(bool forward)
          {
               movement = direction;
               movement.Y = 0;
               movement.Normalize();

               if (forward) position += (movement * speed);
               else position -= (movement * (speed / 2));
          }

          private void Strafe(bool moveRight)
          {
               if (moveRight) position -= Vector3.Cross(up, direction) * speed;
               else position += Vector3.Cross(up, direction) * speed;
          }

          private void applyPhysics(float time)
          {
               velocity.Y -= gravity * time;
               position.X += velocity.X * time;
               position.Y += velocity.Y * time;

               if (jumping)
               {
                    gravity += 100f;
               }

               if (gravity >= maxGravity)
               {
                    gravity = maxGravity;
               }
          }

          public void resetJump()
          {
               jumping = false;
               gravity = minGravity;
          }

          private void jump()
          {
               jumping = true;
               velocity.Y += 120;
          }

          private void useKeyboard()
          {
               currKb = Keyboard.GetState();

               if (currKb.IsKeyDown(Keys.W) || currKb.IsKeyDown(Keys.Up)) MoveForward(true);

               if (currKb.IsKeyDown(Keys.S) || currKb.IsKeyDown(Keys.Down)) MoveForward(false);

               if (currKb.IsKeyDown(Keys.D) || currKb.IsKeyDown(Keys.Right)) Strafe(true);

               if (currKb.IsKeyDown(Keys.A) || currKb.IsKeyDown(Keys.Left)) Strafe(false);

               if (currKb.IsKeyDown(Keys.Space) && prevKb.IsKeyUp(Keys.Space) && !jumping) jump();
          }

          private void useController()
          {
               if (gp.ThumbSticks.Left.Y > 0) MoveForward(true);
               if (gp.ThumbSticks.Left.Y < 0) MoveForward(false);
               if (gp.ThumbSticks.Left.X > 0) Strafe(true);
               if (gp.ThumbSticks.Left.X < 0) Strafe(false);
               if (gp.IsButtonDown(Buttons.A)) jump();
          }
     }
}
