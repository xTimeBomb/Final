using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace Casillas_Votta_Final
{
     public class SquareTile
     {
          VertexPositionTexture[] verts;
          VertexBuffer buff;
          BasicEffect effect;
          Matrix translation = Matrix.Identity;
          Matrix rotation = Matrix.Identity;
          SamplerState clamp;
          Texture2D texture;

          int tileWidth = 50;
          int tileLength = 50;

          int tileType;
          public int TileType
          {
               get { return tileType; }
          }
          Vector3 tilePosition;
          public Vector3 TilePosition
          {
               get { return tilePosition; }
               set { tilePosition = value; }
          }

          BoundingBox bounds;
          public BoundingBox Bounds
          {
               get { return bounds; }
               set { bounds = value; }
          }

          public SquareTile(Game game, Vector3 position, int tileType)
          {
               this.tilePosition = new Vector3(position.X * 50f, position.Y * 50f, position.Z * 50f);
               this.tileType = tileType;
          }

          public void LoadContent(GraphicsDevice graphicsDevice, ContentManager Content)
          {
               List<Vector3> points = new List<Vector3>();
               verts = new VertexPositionTexture[4];

               switch (tileType)
               {
                    //top
                    case 0:
                         verts[0] = new VertexPositionTexture(new Vector3(tilePosition.X, tilePosition.Y - tileWidth / 2, tilePosition.Z), new Vector2(0, 0));
                         verts[1] = new VertexPositionTexture(new Vector3(tilePosition.X + tileLength, tilePosition.Y - tileWidth / 2, tilePosition.Z), new Vector2(1, 0));
                         verts[2] = new VertexPositionTexture(new Vector3(tilePosition.X, tilePosition.Y - tileWidth / 2, tilePosition.Z + tileWidth), new Vector2(0, 1));
                         verts[3] = new VertexPositionTexture(new Vector3(tilePosition.X + tileLength, tilePosition.Y - tileWidth / 2, tilePosition.Z + tileLength), new Vector2(1, 1));
                         points.Add(new Vector3(0, tilePosition.Y, 0));
                         break;
                    //front
                    case 1:
                         verts[0] = new VertexPositionTexture(new Vector3(tilePosition.X, tilePosition.Y - tileWidth / 2, tilePosition.Z), new Vector2(0, 0));
                         verts[1] = new VertexPositionTexture(new Vector3(tilePosition.X + tileLength, tilePosition.Y - tileWidth / 2, tilePosition.Z), new Vector2(1, 0));
                         verts[2] = new VertexPositionTexture(new Vector3(tilePosition.X, tilePosition.Y + tileWidth / 2, tilePosition.Z), new Vector2(0, 1));
                         verts[3] = new VertexPositionTexture(new Vector3(tilePosition.X + tileLength, tilePosition.Y + tileWidth / 2, tilePosition.Z), new Vector2(1, 1));
                         break;
                    //bottom
                    case 2:
                         verts[0] = new VertexPositionTexture(new Vector3(tilePosition.X, tilePosition.Y - tileWidth / 2, tilePosition.Z + tileWidth), new Vector2(0, 1));
                         verts[1] = new VertexPositionTexture(new Vector3(tilePosition.X + tileLength, tilePosition.Y - tileWidth / 2, tilePosition.Z + tileLength), new Vector2(1, 1));
                         verts[2] = new VertexPositionTexture(new Vector3(tilePosition.X, tilePosition.Y - tileWidth / 2, tilePosition.Z), new Vector2(0, 0));
                         verts[3] = new VertexPositionTexture(new Vector3(tilePosition.X + tileLength, tilePosition.Y - tileWidth / 2, tilePosition.Z), new Vector2(1, 0));

                         break;
                    //back
                    case 3:
                         verts[2] = new VertexPositionTexture(new Vector3(tilePosition.X, tilePosition.Y - tileWidth / 2, tilePosition.Z), new Vector2(0, 0));
                         verts[3] = new VertexPositionTexture(new Vector3(tilePosition.X + tileLength, tilePosition.Y - tileWidth / 2, tilePosition.Z), new Vector2(1, 0));
                         verts[0] = new VertexPositionTexture(new Vector3(tilePosition.X, tilePosition.Y + tileWidth / 2, tilePosition.Z), new Vector2(0, 1));
                         verts[1] = new VertexPositionTexture(new Vector3(tilePosition.X + tileLength, tilePosition.Y + tileWidth / 2, tilePosition.Z), new Vector2(1, 1));
                         break;
                    case 4:
                         verts[0] = new VertexPositionTexture(new Vector3(tilePosition.X, tilePosition.Y - tileWidth / 2, tilePosition.Z + tileWidth), new Vector2(0, 0));
                         verts[1] = new VertexPositionTexture(new Vector3(tilePosition.X, tilePosition.Y - tileWidth / 2, tilePosition.Z), new Vector2(1, 0));
                         verts[2] = new VertexPositionTexture(new Vector3(tilePosition.X, tilePosition.Y + tileWidth / 2, tilePosition.Z + tileWidth), new Vector2(0, 1));
                         verts[3] = new VertexPositionTexture(new Vector3(tilePosition.X, tilePosition.Y + tileWidth / 2, tilePosition.Z), new Vector2(1, 1));
                         break;
                    case 5:
                         verts[2] = new VertexPositionTexture(new Vector3(tilePosition.X, tilePosition.Y - tileWidth / 2, tilePosition.Z + tileWidth), new Vector2(0, 0));
                         verts[3] = new VertexPositionTexture(new Vector3(tilePosition.X, tilePosition.Y - tileWidth / 2, tilePosition.Z), new Vector2(1, 0));
                         verts[0] = new VertexPositionTexture(new Vector3(tilePosition.X, tilePosition.Y + tileWidth / 2, tilePosition.Z + tileWidth), new Vector2(0, 1));
                         verts[1] = new VertexPositionTexture(new Vector3(tilePosition.X, tilePosition.Y + tileWidth / 2, tilePosition.Z), new Vector2(1, 1));
                         break;
               }

               for (int i = 0; i < verts.Length; i++)
               {
                    points.Add(verts[i].Position);
               }

               bounds = BoundingBox.CreateFromPoints(points);


               buff = new VertexBuffer(graphicsDevice, typeof(VertexPositionTexture), verts.Length, BufferUsage.None);
               buff.SetData(verts);

               effect = new BasicEffect(graphicsDevice);

               clamp = new SamplerState
               {
                    AddressU = TextureAddressMode.Clamp,
                    AddressV = TextureAddressMode.Clamp
               };

               texture = Content.Load<Texture2D>(@"floorTile");

          }

          public void Draw(GraphicsDevice gDevice)
          {
               gDevice.SamplerStates[0] = clamp;
               gDevice.SetVertexBuffer(buff);

               effect.World = rotation * translation;

               effect.View = Game1.camera.view;
               effect.Projection = Game1.camera.projection;
               effect.Texture = texture;
               effect.TextureEnabled = true;

               foreach (EffectPass pass in effect.CurrentTechnique.Passes)
               {
                    pass.Apply();
                    gDevice.DrawUserPrimitives<VertexPositionTexture>(PrimitiveType.TriangleStrip, verts, 0, (verts.Length / 2));
               }
          }

          public void Draw2(GraphicsDevice gDevice)
          {
              gDevice.SamplerStates[0] = clamp;
              gDevice.SetVertexBuffer(buff);

              effect.World = rotation * translation;

              effect.View = Game1.camera2.view;
              effect.Projection = Game1.camera2.projection;
              effect.Texture = texture;
              effect.TextureEnabled = true;

              foreach (EffectPass pass in effect.CurrentTechnique.Passes)
              {
                  pass.Apply();
                  gDevice.DrawUserPrimitives<VertexPositionTexture>(PrimitiveType.TriangleStrip, verts, 0, (verts.Length / 2));
              }
          }
     }
}
