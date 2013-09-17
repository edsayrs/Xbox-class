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


namespace WindowsGame1
{
    public struct VertexPositionColorNormal
    {
        public Vector3 Position;
        public Color Color;
        public Vector3 Normal;
        public readonly static VertexDeclaration vertexDeclaration = new VertexDeclaration
        (
        new VertexElement(0, VertexElementFormat.Vector3, VertexElementUsage.Position, 0),
        new VertexElement(sizeof(float) * 3, VertexElementFormat.Color,
        VertexElementUsage.Color, 0),
        new VertexElement(sizeof(float) * 3 + 4, VertexElementFormat.Vector3, VertexElementUsage.Normal, 0)

        );

    }
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;// call graphics device loaded on this computer, must be initalized in load content 
        SpriteBatch spriteBatch;
        GraphicsDevice device;

        Effect effect;// declare new effect object 
        VertexPositionColorNormal[] vertices;
        //VertexPositionColor[] vertices;// declare array to store vertices for triangles (verteces and color)
        Matrix viewMatrix;//camera
        Matrix projectionMatrix;//camara

        //private float angle = 0f;//store current rotation angle 
        int[] indices;

        private int terrainWidth = 4;
        private int terrainLength = 3;

        private float[,] heightData;// array to store value of each points z coordinate 

        Matrix worldMatrix = Matrix.Identity; // World matrix identity
        Matrix worldTranslation = Matrix.Identity; // Translation
        Matrix worldRotation = Matrix.Identity; // Rotation

        VertexBuffer myVertexBuffer;
        IndexBuffer myIndexBuffer;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        /// 

        /*private void LoadHeightData()
        {
          heightData = new float[5, 5];
            heightData[0, 0] = 0;
            heightData[1, 0] = 0;
            heightData[2, 0] = 0;
            heightData[3, 0] = 0;
            heightData[4, 0] = 0;
            heightData[0, 1] = 0.5f;
            heightData[1, 1] = 0;
            heightData[2, 1] = -1.0f;
            heightData[3, 1] = 0.2f;
            heightData[4, 1] = .8f;
            heightData[0, 2] = 1.0f;
            heightData[1, 2] = 1.2f;
            heightData[2, 2] = 0.8f;
            heightData[3, 2] = 0;
            heightData[4, 2] = 1.4f;
            heightData[0, 3] = 1.0f;
            heightData[1, 3] = 1.2f;
            heightData[2, 3] = 0.8f;
            heightData[3, 3] = 0;
            heightData[4, 3] = 1.2f;
            heightData[0, 4] = 1.0f;
            heightData[1, 4] = 1.2f;
            heightData[2, 4] = 0.8f;
            heightData[3, 4] = 0;
            heightData[4, 4] = .7f;*/


        private void SetUpVertices()
        {

            /* vertices = new VertexPositionColor[5];
             vertices[0].Position = new Vector3(0f, 0f, 0f);
             vertices[0].Color = Color.White;
             vertices[1].Position = new Vector3(5f, 0f, 0f);
             vertices[1].Color = Color.White;
             vertices[2].Position = new Vector3(10f, 0f, 0f);
             vertices[2].Color = Color.White;
             vertices[3].Position = new Vector3(5f, 0f, -5f);
             vertices[3].Color = Color.White;
             vertices[4].Position = new Vector3(10f, 0f, -5f);
             vertices[4].Color = Color.White;*/
            float minHeight = float.MaxValue;
            float maxHeight = float.MinValue;
           
            for (int x = 0; x < terrainWidth; x++)
            {
                for (int y = 0; y < terrainLength; y++)
                {
                    if (heightData[x, y] < minHeight)
                        minHeight = heightData[x, y];
                    if (heightData[x, y] > maxHeight)
                        maxHeight = heightData[x, y];
                }
            }

            vertices = new VertexPositionColorNormal[terrainWidth * terrainLength];
            for (int x = 0; x < terrainWidth; x++)
            {
                for (int y = 0; y < terrainLength; y++)
                {
                    vertices[x + y * terrainWidth].Position = new Vector3(x, heightData[x, y], -y);
                    if (heightData[x, y] < minHeight + (maxHeight - minHeight) / 4)
                        vertices[x + y * terrainWidth].Color = Color.Blue;
                    else if (heightData[x, y] < minHeight + (maxHeight - minHeight) * 2 / 4)
                        vertices[x + y * terrainWidth].Color = Color.Green;
                    else if (heightData[x, y] < minHeight + (maxHeight - minHeight) * 3 / 4)
                        vertices[x + y * terrainWidth].Color = Color.Brown;
                    else
                        vertices[x + y * terrainWidth].Color = Color.White;
                    //vertices[x + y * terrainWidth].Position = new Vector3(x, 0, -y);
                    //vertices[x + y * terrainWidth].Position = new Vector3(x, heightData[x, y], -y);
                    //vertices[x + y * terrainWidth].Color = Color.White;
                }
            }
        }
        //(now we must call set up vertices method in load content)

        private void SetUpIndices()
        {
            /* indices = new int[9];
             indices[0] = 3;
             indices[1] = 1;
             indices[2] = 0;
             indices[3] = 4;
             indices[4] = 2;
             indices[5] = 1;
             indices[6] = 1;
             indices[7] = 3;
             indices[8] = 4;*/
            /* indices = new int[(terrainWidth - 1) * (terrainLength - 1) * 3];
             int counter = 0;
             for (int y = 0; y < terrainLength - 1; y++)
             {
                 for (int x = 0; x < terrainWidth - 1; x++)
                 {
                     int lowerLeft = x + y * terrainWidth;
                     int lowerRight = (x + 1) + y * terrainWidth;
                     int topLeft = x + (y + 1) * terrainWidth;
                     int topRight = (x + 1) + (y + 1) * terrainWidth;
                     indices[counter++] = topLeft;
                     indices[counter++] = lowerRight;
                     indices[counter++] = lowerLeft;
                 }
             }*/
            indices = new int[(terrainWidth - 1) * (terrainLength - 1) * 6];
            int counter = 0;
            for (int y = 0; y < terrainLength - 1; y++)
            {
                for (int x = 0; x < terrainWidth - 1; x++)
                {
                    int lowerLeft = x + y * terrainWidth;
                    int lowerRight = (x + 1) + y * terrainWidth;
                    int topLeft = x + (y + 1) * terrainWidth;
                    int topRight = (x + 1) + (y + 1) * terrainWidth;
                    indices[counter++] = topLeft;
                    indices[counter++] = lowerRight;
                    indices[counter++] = lowerLeft;
                    indices[counter++] = topLeft;
                    indices[counter++] = topRight;
                    indices[counter++] = lowerRight;
                }
            }
        }

        private void CalculateNormals()
        {
            for (int i = 0; i < vertices.Length; i++)
                vertices[i].Normal = new Vector3(0, 0, 0);
            for (int i = 0; i < indices.Length / 3; i++)
            {
                int index1 = indices[i * 3];
                int index2 = indices[i * 3 + 1];
                int index3 = indices[i * 3 + 2];
                Vector3 side1 = vertices[index1].Position - vertices[index3].Position;
                Vector3 side2 = vertices[index1].Position - vertices[index2].Position;
                Vector3 normal = Vector3.Cross(side1, side2);
                vertices[index1].Normal += normal;
                vertices[index2].Normal += normal;
                vertices[index3].Normal += normal;
            }
            for (int i = 0; i < vertices.Length; i++)
                vertices[i].Normal.Normalize();
        }
        private void SetUpCamera()
        {
            //camera on negative z axis:

            //viewMatrix = Matrix.CreateLookAt(new Vector3(0, 0, 50), new Vector3(0, 0, 0), new Vector3(0, 1, 0));
            //camera on positive z axis:
            //viewMatrix = Matrix.CreateLookAt(new Vector3(0, 0, 50), new Vector3(0, 0, 0), new Vector3(0, 1, 0));
            //rotate camera angle to the left:
            // viewMatrix = Matrix.CreateLookAt(new Vector3(0, 50, 0), new Vector3(0, 0, 0), new Vector3(1, 0, 0));
            //viewMatrix = Matrix.CreateLookAt(new Vector3(0, 10, 0), new Vector3(0, 0, 0), new Vector3(0, 0, -1));
            viewMatrix = Matrix.CreateLookAt(new Vector3(60, 80, -80), new Vector3(0, 0, 0), new Vector3(0, 1, 0));
            projectionMatrix = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4, device.Viewport.AspectRatio, 1.0f, 300.0f);
        }
        private void CopyToBuffers()
        {
            myVertexBuffer = new VertexBuffer(device, VertexPositionColorNormal.vertexDeclaration, vertices.Length, BufferUsage.WriteOnly);
            myVertexBuffer.SetData(vertices);
            myIndexBuffer = new IndexBuffer(device, typeof(int), indices.Length, BufferUsage.WriteOnly);
            myIndexBuffer.SetData(indices);
        }

        private void LoadHeightData(Texture2D heightMap)
        {
            terrainWidth = heightMap.Width;
            terrainLength = heightMap.Height; // Texture2D uses the instance variable names Width and Height
            Color[] heightMapColors = new Color[terrainWidth * terrainLength];
            heightMap.GetData(heightMapColors);
            heightData = new float[terrainWidth, terrainLength];
            for (int x = 0; x < terrainWidth; x++)
                for (int y = 0; y < terrainLength; y++)
                    heightData[x, y] = heightMapColors[x + y * terrainWidth].R / 5.0f; //5.0 is a scale factor
        }



        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            //specify basic window settings (size, title, etc)
            graphics.PreferredBackBufferWidth = 700;
            graphics.PreferredBackBufferHeight = 700;
            graphics.IsFullScreen = false;// run in window, not full screen 
            graphics.ApplyChanges();// apply changes 
            Window.Title = "ATLS 4519 Lab7 - Terrain Tutorial";// title of window


            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // initalize graphics device
            device = graphics.GraphicsDevice;
            effect = Content.Load<Effect>("effects"); //initalize effect variable, "effects" refferences effects.fx
            Texture2D heightMap = Content.Load<Texture2D>("heightmap");
            LoadHeightData(heightMap);
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            //LoadHeightData();
            SetUpCamera();
            SetUpVertices();
            //(now tell device to draw triangle in draw)


            SetUpIndices();
            CalculateNormals();
            CopyToBuffers();

        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            //angle += 0.005f;//increase rotation angle by .005f each frame 
            KeyboardState keyState = Keyboard.GetState();
            /*if (keyState.IsKeyDown(Keys.PageUp))
                angle += 0.05f;
            if (keyState.IsKeyDown(Keys.PageDown))
                angle -= 0.05f;*/
            //Rotation
            if (keyState.IsKeyDown(Keys.PageUp))
            {
                worldRotation = Matrix.CreateRotationY(0.1f);
            }
            else if (keyState.IsKeyDown(Keys.PageDown))
            {
                worldRotation = Matrix.CreateRotationY(-0.1f);
            }
            else
            {
                worldRotation = Matrix.CreateRotationY(0);
            }
            //Translation
            if (keyState.IsKeyDown(Keys.Left))
            {
                worldTranslation = Matrix.CreateTranslation(-.1f, 0, 0);
            }
            else if (keyState.IsKeyDown(Keys.Right))
            {
                worldTranslation = Matrix.CreateTranslation(.1f, 0, 0);
            }
            else if (keyState.IsKeyDown(Keys.Up))
            {
                worldTranslation = Matrix.CreateTranslation(0, .1f, 0);
            }
            else if (keyState.IsKeyDown(Keys.Down))
            {
                worldTranslation = Matrix.CreateTranslation(0, -.1f, 0);
            }
            else if (keyState.IsKeyDown(Keys.Q))
            {
                worldTranslation = Matrix.CreateTranslation(0, 0, .1f);
            }
            else if (keyState.IsKeyDown(Keys.Z))
            {
                worldTranslation = Matrix.CreateTranslation(0, 0, -.1f);
            }
            else
            {
                worldTranslation = Matrix.CreateTranslation(0, 0, 0);
            }
            worldMatrix *= worldTranslation * worldRotation;




            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            //device.Clear(Color.DarkSlateBlue);//"clear" clears buffer for window and sets backgournd color 
            device.Clear(ClearOptions.Target | ClearOptions.DepthBuffer, Color.Black, 1.0f, 0);

            RasterizerState rs = new RasterizerState();//disable culling 
            rs.CullMode = CullMode.None;//disable culling
            //rs.FillMode = FillMode.WireFrame;
            rs.FillMode = FillMode.Solid;
            device.RasterizerState = rs;//disable culling 

            //rotate:
            //Matrix worldMatrix = Matrix.CreateRotationY(3 * angle);//create world matrix 

            //translate and rotate 
            //Matrix worldMatrix = Matrix.CreateTranslation(-20.0f / 3.0f, -10.0f / 3.0f, 0) * Matrix.CreateRotationZ(angle);

            //arbitraty axis
            //Vector3 rotAxis = new Vector3(3 * angle, angle, 2 * angle);
            //rotAxis.Normalize();
            //Matrix worldMatrix = Matrix.CreateTranslation(-20.0f / 3.0f, -10.0f / 3.0f, 0) * Matrix.CreateFromAxisAngle(rotAxis, angle);
            // Matrix worldMatrix = Matrix.Identity;

            //Matrix worldMatrix = Matrix.CreateTranslation(-terrainWidth / 2.0f, 0, terrainLength / 2.0f);
            // Matrix worldMatrix = Matrix.CreateTranslation(-terrainWidth / 2.0f, 0, terrainLength / 2.0f) * Matrix.CreateRotationY(angle);

            //effect.CurrentTechnique = effect.Techniques["ColoredNoShading"];// activates effect from effects.fx file 
            effect.CurrentTechnique = effect.Techniques["Colored"];
            effect.Parameters["xView"].SetValue(viewMatrix);
            effect.Parameters["xProjection"].SetValue(projectionMatrix);


            effect.Parameters["xWorld"].SetValue(worldMatrix);
            Vector3 lightDirection = new Vector3(1.0f, -1.0f, -1.0f);
            lightDirection.Normalize();
            effect.Parameters["xLightDirection"].SetValue(lightDirection);
            effect.Parameters["xAmbient"].SetValue(0.5f);
            effect.Parameters["xEnableLighting"].SetValue(true);
            //effect.Parameters["xWorld"].SetValue(worldMatrix);//pass this world matrix to the effect 
            //effect.Parameters["xWorld"].SetValue(Matrix.Identity);
           

            foreach (EffectPass pass in effect.CurrentTechnique.Passes)//iterate through the passes of the pretransformed technique 
            {
                pass.Apply();// ALL DRAWING CODE MUST BE PUT IN AFTER THIS

                // tell graphics card to draw one triangle from vertices array, starting at vertex 0. 
                // device.DrawUserPrimitives(PrimitiveType.TriangleList, vertices, 0, 1,  VertexPositionColor.VertexDeclaration);
                //"trianglelist" indicates that vertices array contains a lost of triangles (one triangle) 
              
                //device.DrawUserIndexedPrimitives(PrimitiveType.TriangleList, vertices, 0, vertices.Length, indices, 0, indices.Length / 3, VertexPositionColor.VertexDeclaration);
                device.DrawUserIndexedPrimitives(PrimitiveType.TriangleList, vertices, 0, vertices.Length, indices, 0, indices.Length / 3, VertexPositionColorNormal.vertexDeclaration);
               
                device.Indices = myIndexBuffer;
                device.SetVertexBuffer(myVertexBuffer);
                device.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, vertices.Length, 0, indices.Length / 3);

            }
            base.Draw(gameTime);
        }
    }
}
