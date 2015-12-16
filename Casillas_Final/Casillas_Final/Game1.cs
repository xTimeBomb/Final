using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System.Diagnostics;

namespace Casillas_Votta_Final
{
     /// <summary>
     /// This is the main type for your game
     /// </summary>
     public class Game1 : Microsoft.Xna.Framework.Game
     {
          public static int GAME_WIDTH = 0;
          public static int GAME_HEIGHT = 0;

          GraphicsDeviceManager graphics;
          SpriteBatch spriteBatch;

          public enum GameState
          {
               MENU,
               MENU_COOP,
               PLAYING,
               INSTRUCTIONS,
               PAUSED,
               EXIT
          }
          private GameState currentGameState = GameState.MENU;

          //Game Components
          public static FPSCamera camera; // Player 1
          public static FPSCamera camera2; // Player 2

          Vector3 cameraPosition = new Vector3(40, 200, 40);
          Vector3 camera2Position = new Vector3(490, 0, 490);
          Vector3 center = new Vector3(250,0,250);

          List<SquareTile> tiles = new List<SquareTile>();

          Matrix translation = Matrix.Identity;
          Matrix rotation = Matrix.Identity;

          int numberOfPlayers = 1;

          // Menu
          SpriteFont menuFont;
          String[] menuItems = { "Play", "Co-op", "Instructions", "Quit" };
          int currentSelectedIndex = 0;

          public Game1()
          {
               graphics = new GraphicsDeviceManager(this);

               GAME_WIDTH = graphics.PreferredBackBufferWidth = 1000;
               GAME_HEIGHT = graphics.PreferredBackBufferHeight = 800;

               graphics.ApplyChanges();
               Content.RootDirectory = "Content";
          }

          protected override void Initialize()
          {
               // TODO: Add your initialization logic here



               //When adding tiles the numbers are represented as follows
               // 0 type tile allows you to see the TOP
               // 1 type tile allows you to see the FRONT
               // 2 type tile allows you to see the BOTTOM
               // 3 type tile allows you to see the BACK
               // 4 type tile allows you to see the LEFT
               // 5 type tile allows you to see the RIGHT

               #region Create World Tiles
               for (int i = 0; i < 10; i++)
               {
                    tiles.Add(new SquareTile(this, new Vector3(-1, 3, i), 0));
                    tiles.Add(new SquareTile(this, new Vector3(0, 3, i), 0));
                    tiles.Add(new SquareTile(this, new Vector3(1, 3, i), 0));
               }
               #endregion

               //RasterizerState rs = new RasterizerState();
               //rs.CullMode = CullMode.None;
               //GraphicsDevice.RasterizerState = rs;

               base.Initialize();
          }

          protected override void LoadContent()
          {
               spriteBatch = new SpriteBatch(GraphicsDevice);

               foreach (SquareTile tile in tiles)
               {
                    tile.LoadContent(GraphicsDevice, Content);
               }

               menuFont = Content.Load<SpriteFont>("GameFont");
          }

          protected override void UnloadContent()
          {
               // TODO: Unload any non ContentManager content here
          }

          KeyboardState oldKeyState;
          protected override void Update(GameTime gameTime)
          {
               KeyboardState newKeyState = Keyboard.GetState();

               switch (currentGameState)
               {
                    case GameState.MENU:
                         #region Menu Logic
                         if (newKeyState.IsKeyDown(Keys.Up) && oldKeyState.IsKeyUp(Keys.Up)) currentSelectedIndex--;

                         if (newKeyState.IsKeyDown(Keys.Down) && oldKeyState.IsKeyUp(Keys.Down)) currentSelectedIndex++;

                         if (currentSelectedIndex < 0) currentSelectedIndex = menuItems.Length - 1;
                         if (currentSelectedIndex > menuItems.Length - 1) currentSelectedIndex = 0;

                         if (newKeyState.IsKeyDown(Keys.Enter))
                         {
                              if (currentSelectedIndex == 0)
                                   setGameState(Game1.GameState.PLAYING);
                              else if (currentSelectedIndex == 1)
                                   setGameState(Game1.GameState.MENU_COOP);
                              else if (currentSelectedIndex == 2)
                                   setGameState(Game1.GameState.INSTRUCTIONS);
                              else if (currentSelectedIndex == 3)
                                   setGameState(Game1.GameState.EXIT);

                              setUpCameras();
                         }
                         #endregion
                         break;
                    case GameState.PAUSED:
                         break;
                    case GameState.PLAYING:
                         checkCollision(camera);
                         checkCollision(camera2);

                         if (newKeyState.IsKeyDown(Keys.Escape)) this.Exit();

                         if (newKeyState.IsKeyDown(Keys.C)) Debug.WriteLine(camera.position);

                         //if (newKeyState.IsKeyDown(Keys.Q)) Debug.WriteLine();

                         #region Mouse Contraints

                         if (Mouse.GetState().X < 0)
                         {
                              Mouse.SetPosition(this.Window.ClientBounds.Width, Mouse.GetState().Y);
                              camera.prevMouse = Mouse.GetState();
                         }

                         if (Mouse.GetState().X > this.Window.ClientBounds.Width)
                         {
                              Mouse.SetPosition(0, Mouse.GetState().Y);
                              camera.prevMouse = Mouse.GetState();
                         }

                         if (Mouse.GetState().Y < 0)

                              Mouse.SetPosition(Mouse.GetState().X, 0);

                         if (Mouse.GetState().Y > this.Window.ClientBounds.Height)

                              Mouse.SetPosition(Mouse.GetState().X, this.Window.ClientBounds.Height);

                         #endregion

                         rotation = Matrix.CreateFromYawPitchRoll(0, 0, 0);
                         break;
                    case GameState.EXIT:
                         Exit();
                         break;
               }

               oldKeyState = newKeyState;

               base.Update(gameTime);
          }

          protected override void Draw(GameTime gameTime)
          {
               GraphicsDevice.Clear(Color.CornflowerBlue);

               if (currentGameState == GameState.PLAYING)
               {
                    if (numberOfPlayers >= 1)
                    {

                         foreach (SquareTile tile in tiles) tile.Draw(GraphicsDevice);
                         GraphicsDevice.Viewport = camera.viewport;
                    }

                    if (numberOfPlayers == 2)
                    {

                         foreach (SquareTile tile in tiles) tile.Draw2(GraphicsDevice);
                         GraphicsDevice.Viewport = camera2.viewport;
                    }
               }

               if (currentGameState == GameState.MENU)
               {
                    spriteBatch.Begin();
                    for (int i = 0; i < menuItems.Length; i++)
                    {
                         Vector2 fontMetrics = menuFont.MeasureString(menuItems[i]);

                         float xModifier = (currentSelectedIndex == i) ? 0 : fontMetrics.X * 0.15f;
                         float yModifier = (currentSelectedIndex == i) ? 0 : -5;

                         spriteBatch.DrawString(
                              menuFont,
                              menuItems[i],
                              new Vector2((Game1.GAME_WIDTH / 2 - fontMetrics.X / 2) + xModifier, (i * (fontMetrics.Y - 10)) + (Game1.GAME_HEIGHT / 2) - fontMetrics.Y - yModifier),
                              Color.White, 0,
                              Vector2.Zero,
                              (currentSelectedIndex == i) ? 1f : 0.7f,
                              SpriteEffects.None, 1);
                    }
                    spriteBatch.End();
               }

               base.Draw(gameTime);
          }

          private void checkCollision(FPSCamera camera)
          {
               foreach (SquareTile tile in tiles)
               {
                    if (camera.Bounds.Intersects(tile.Bounds))
                    {
                         switch (tile.TileType)
                         {
                              case 0:
                                   camera.position.Y = tile.TilePosition.Y;
                                   camera.resetJump();
                                   break;
                              case 1:
                                   //works fine
                                   camera.position.Z = tile.TilePosition.Z - 10;
                                   break;
                              case 2:
                                   break;
                              case 3:
                                   //bounces player
                                   camera.position.Z = tile.TilePosition.Z + 3;
                                   break;
                              case 4:
                                   //works fine
                                   camera.position.X = tile.TilePosition.X - 10;
                                   break;
                              case 5:
                                   //bounces player
                                   camera.position.X = tile.TilePosition.X + 3;
                                   break;
                         }
                    }
               }
          }

          public void setGameState(GameState state)
          {
               if (state == GameState.MENU_COOP)
               {
                    numberOfPlayers = 2;
                    state = GameState.PLAYING;
               }
               currentGameState = state;
          }

          private void setUpCameras()
          {
               Viewport vp1, vp2, defaultView;
               defaultView = GraphicsDevice.Viewport;
               defaultView.Width = Window.ClientBounds.Width;
               defaultView.Height = Window.ClientBounds.Height;

               vp1 = GraphicsDevice.Viewport;
               vp2 = GraphicsDevice.Viewport;

               vp1.X = 0;
               vp1.Y = 0;

               if (numberOfPlayers == 1)
               {
                    vp1.Width = defaultView.Width;
                    vp1.Height = defaultView.Height;
               }
               else if (numberOfPlayers == 2)
               {
                    vp1.X = 0;
                    vp1.Y = defaultView.Height / 2;

                    vp1.Width = defaultView.Width;
                    vp1.Height = defaultView.Height / 2;

                    vp2.X = 0;
                    vp2.Y = 0;
                    vp2.Width = defaultView.Width;
                    vp2.Height = defaultView.Height / 2;
               }

               camera = new FPSCamera(this, cameraPosition, center, new Vector3(0, 1, 0), vp1, true);
               camera2 = new FPSCamera(this, camera2Position, center, new Vector3(0, 1, 0), vp2, false);

               Components.Add(camera);
               Components.Add(camera2);
          }
     }
}
