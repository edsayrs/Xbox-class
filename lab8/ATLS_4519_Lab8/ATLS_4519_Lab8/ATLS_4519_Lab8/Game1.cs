using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace ATLS_4519_Lab8
{

    public class Game1 : Microsoft.Xna.Framework.Game
    {
        #region Constants
        // Terrain texture mapping constants
        public const float MAX_HT = 30.0f;
        public const float MIN_HT = 0.0f;
        public const float SAND_UPPER = 0.266f * MAX_HT; // 8
        public const float GRASS_MID = 0.4f * MAX_HT; // 12
        public const float GRASS_RANGE = 0.2f * MAX_HT; // 12 +/- 6
        public const float ROCK_MID = 0.666f * MAX_HT; // 20
        public const float ROCK_RANGE = 0.2f * MAX_HT; // 20 +/- 6
        public const float SNOW_LOWER = 0.8f * MAX_HT; // 24

        // Camera control constants
        public const float ROTATION_SPEED = 0.3f;
        public const float MOVE_SPEED = 30.0f;

        //Lighting Constants
        // Vector3 LIGHT_DIRECTION = new Vector3(1.0f, -1.0f, 1.0f);  //use (1.0f, -1.0f, 1.0f) to flip light
        //public const float AMBIENT_LIGHT_LEVEL = 0.7f;

        //Water Constants
        public const float WATER_HEIGHT = 5.0f;
        public const float WAVE_LENGTH = 0.2f;
        public const float WAVE_HEIGHT = 0.3f;
        public Vector4 DULL_COLOR = new Vector4(0.3f, 0.4f, 0.5f, 1.0f);
        public const float WATER_DIRTINESS = 0.2f; // Increase to make the water "dirtier"
        public const float WIND_FORCE = 0.0005f; // It doesn't take much
        public Vector3 WIND_DIRECTION = new Vector3(1, 0, 1);

        //cloud constants
        public const int CLOUD_SIZE = 32; //use 64 to make smaller clouds
        public const float OVERCAST_FACTOR = 1.2f; // Increase to make more cloudy
        public const float TIME_DIV = 4000.0f; // Increase to make clouds move more slowly
        public Vector4 SKY_TOP_COLOR = new Vector4(0.3f, 0.3f, 0.8f, 1);

        //Tree Constants
        public const int TREE_MIN_HT = 8; // Min terrain height for trees
        public const int TREE_MAX_HT = 14; // Max terrain height for trees
        public const int TREE_MAX_SLOPE = 15; // Max terrain slope on which to put trees

        public const int TREEMAP_HI_THOLD = 200; // Noise map value for HI_NUM_TREES trees
        public const int TREEMAP_MED_THOLD = 150; // Noise map value for MED_NUM_TREES trees
        public const int TREEMAP_LOW_THOLD = 100; // Noise map value for LOW_NUM_TREES trees
        public const int HI_NUM_TREES = 5; // Num trees at TREEMAP_HI_THOLD noise value
        public const int MED_NUM_TREES = 4; // Num trees at TREEMAP_MED_THOLD noise value
        public const int LOW_NUM_TREES = 3; // Num trees at TREEMAP_LOW_THOLD noise value
        public const float BB_ALPHA_TEST_VALUE = 0.6f; // useful values are .5 to .9

        #endregion//constants

        #region Vertex Structs
        public struct VertexPositionColorNormal
        {
            public Vector3 Position;
            public Color Color;
            public Vector3 Normal;

            public readonly static VertexDeclaration vertexDeclaration = new VertexDeclaration
            (
                new VertexElement(0, VertexElementFormat.Vector3, VertexElementUsage.Position, 0),
                new VertexElement(sizeof(float) * 3, VertexElementFormat.Color, VertexElementUsage.Color, 0),
                new VertexElement(sizeof(float) * 3 + 4, VertexElementFormat.Vector3, VertexElementUsage.Normal, 0)
            );
        }
        public struct VertexMultitextured
        {
            public Vector3 Position;
            public Vector3 Normal;
            public Vector4 TextureCoordinate;
            public Vector4 TexWeights;
            public readonly static VertexDeclaration vertexDeclaration = new VertexDeclaration
            (
            new VertexElement(0, VertexElementFormat.Vector3,
            VertexElementUsage.Position, 0),
            new VertexElement(sizeof(float) * 3, VertexElementFormat.Vector3, VertexElementUsage.Normal, 0),
            new VertexElement(sizeof(float) * 6, VertexElementFormat.Vector4, VertexElementUsage.TextureCoordinate, 0),
            new VertexElement(sizeof(float) * 10, VertexElementFormat.Vector4, VertexElementUsage.TextureCoordinate, 1)
            );
        }
        #endregion//vertex structs

        #region Class Game1 Variables
        GraphicsDeviceManager graphics;
        GraphicsDevice device;

        Matrix viewMatrix;
        Matrix projectionMatrix;
        Matrix reflectionViewMatrix;

        Effect effect;

        VertexMultitextured[] vertices;
        int[] indices;
        VertexBuffer myVertexBuffer;
        IndexBuffer myIndexBuffer;

        VertexBuffer waterVertexBuffer;
        VertexBuffer treeVertexBuffer;
        IndexBuffer treeIndexBuffer;

        private int terrainWidth;
        private int terrainLength;
        private float[,] heightData;

        Vector3 cameraPosition = new Vector3(130, 30, -50);
        float leftrightRot = MathHelper.PiOver2;
        float updownRot = -MathHelper.Pi / 10.0f;

        MouseState originalMouseState;//move mouse to center of screen after each update
        //mouse will not allow you to exit, so TO EXIT USE ALT + F4***

        Texture2D grassTexture;
        Texture2D sandTexture;
        Texture2D rockTexture;
        Texture2D snowTexture;
        Texture2D waterBumpMap;
        Texture2D treeTexture;
        Texture2D treeMap;

        Texture2D cloudMap;
        Model skyDome;

        List<Vector3> treeList;

        RenderTarget2D refractionRenderTarget;
        Texture2D refractionMap;

        RenderTarget2D reflectionRenderTarget;
        Texture2D reflectionMap;

        RenderTarget2D cloudsRenderTarget;

        Texture2D cloudStaticMap;

        VertexPositionTexture[] fullScreenVertices;

        Matrix worldMatrix = Matrix.Identity;
        Matrix worldTranslation = Matrix.Identity;
        Matrix worldRotation = Matrix.Identity;
        #endregion//Class Game1 Variables


        #region Class Game1 Constructor
        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }
        #endregion//class game1 constructor

        #region Terrain Methods

        private void SetUpVertices()
        {
            vertices = new VertexMultitextured[terrainWidth * terrainLength];
            for (int x = 0; x < terrainWidth; x++)
            {
                for (int y = 0; y < terrainLength; y++)
                {
                    /*vertices[x + y * terrainWidth].Position = new Vector3(x, heightData[x, y], -y);
                    vertices[x + y * terrainWidth].TextureCoordinate.X = (float)x / MAX_HT;
                    vertices[x + y * terrainWidth].TextureCoordinate.Y = (float)y / MAX_HT;
                    */
                    vertices[x + y * terrainWidth].Position = new Vector3(x, heightData[x, y], -y);
                    vertices[x + y * terrainWidth].TextureCoordinate.X = (float)x / MAX_HT;
                    vertices[x + y * terrainWidth].TextureCoordinate.Y = (float)y / MAX_HT;
                    vertices[x + y * terrainWidth].TexWeights.X = MathHelper.Clamp(1.0f - Math.Abs(heightData[x, y] - MIN_HT) / SAND_UPPER, 0, 1);
                    vertices[x + y * terrainWidth].TexWeights.Y = MathHelper.Clamp(1.0f - Math.Abs(heightData[x, y] - GRASS_MID) / GRASS_RANGE, 0, 1);
                    vertices[x + y * terrainWidth].TexWeights.Z = MathHelper.Clamp(1.0f - Math.Abs(heightData[x, y] - ROCK_MID) / ROCK_RANGE, 0, 1);
                    vertices[x + y * terrainWidth].TexWeights.W = MathHelper.Clamp(1.0f - Math.Abs(heightData[x, y] - MAX_HT) / SNOW_LOWER, 0, 1);

                    float total = vertices[x + y * terrainWidth].TexWeights.X;
                    total += vertices[x + y * terrainWidth].TexWeights.Y;
                    total += vertices[x + y * terrainWidth].TexWeights.Z;
                    total += vertices[x + y * terrainWidth].TexWeights.W;
                    vertices[x + y * terrainWidth].TexWeights.X /= total;
                    vertices[x + y * terrainWidth].TexWeights.Y /= total;
                    vertices[x + y * terrainWidth].TexWeights.Z /= total;
                    vertices[x + y * terrainWidth].TexWeights.W /= total;
                }

            }
        }


        private void SetUpIndices()
        {
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

        private void CopyToBuffers()
        {
            /*myVertexBuffer = new VertexBuffer(device, typeof(VertexMultitextured), vertices.Length, BufferUsage.WriteOnly);
            myVertexBuffer.SetData(vertices);
             */
            myVertexBuffer = new VertexBuffer(device, VertexMultitextured.vertexDeclaration, vertices.Length, BufferUsage.WriteOnly);
            myVertexBuffer.SetData(vertices);


            myIndexBuffer = new IndexBuffer(device, typeof(int), indices.Length, BufferUsage.WriteOnly);
            myIndexBuffer.SetData(indices);
        }

        private void LoadHeightData(Texture2D heightMap)
        {
            terrainWidth = heightMap.Width;
            terrainLength = heightMap.Height;
            float minimumHeight = float.MaxValue;
            float maximumHeight = float.MinValue;
            Color[] heightMapColors = new Color[terrainWidth * terrainLength];
            heightMap.GetData(heightMapColors);
            heightData = new float[terrainWidth, terrainLength];
            for (int x = 0; x < terrainWidth; x++)
                for (int y = 0; y < terrainLength; y++)
                {
                    heightData[x, y] = heightMapColors[x + y * terrainWidth].R;
                    if (heightData[x, y] < minimumHeight) minimumHeight = heightData[x, y];
                    if (heightData[x, y] > maximumHeight) maximumHeight = heightData[x, y];
                }
            for (int x = 0; x < terrainWidth; x++)
                for (int y = 0; y < terrainLength; y++)
                    heightData[x, y] = (heightData[x, y] - minimumHeight) / (maximumHeight - minimumHeight) * MAX_HT;
        }

        private void DrawTerrain(Matrix currentViewMatrix)
        {
            effect.CurrentTechnique = effect.Techniques["MultiTextured"];
            effect.Parameters["xTexture0"].SetValue(sandTexture);
            effect.Parameters["xTexture1"].SetValue(grassTexture);
            effect.Parameters["xTexture2"].SetValue(rockTexture);
            effect.Parameters["xTexture3"].SetValue(snowTexture);

            Matrix dworldMatrix = Matrix.Identity;
            effect.Parameters["xView"].SetValue(currentViewMatrix);
            effect.Parameters["xProjection"].SetValue(projectionMatrix);
            effect.Parameters["xWorld"].SetValue(dworldMatrix);

            Vector3 lightDirection = new Vector3(1.0f, -1.0f, -1.0f); // was (-0.5f, -1, -0.5f)

            lightDirection.Normalize();
            effect.Parameters["xLightDirection"].SetValue(lightDirection);
            effect.Parameters["xAmbient"].SetValue(0.5f); // was 0.5f
            effect.Parameters["xEnableLighting"].SetValue(true);

            foreach (EffectPass pass in effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                device.Indices = myIndexBuffer;
                device.SetVertexBuffer(myVertexBuffer);
                device.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, vertices.Length, 0, indices.Length / 3);
            }
        }

        #endregion//terrain methods


        #region Water Methods
        private Plane CreatePlane(float height, Vector3 planeNormalDirection, Matrix currentViewMatrix, bool clipSide)
        {
            planeNormalDirection.Normalize();
            Vector4 planeCoeffs = new Vector4(planeNormalDirection, height);
            if (clipSide) planeCoeffs *= -1;
            Matrix worldViewProjection = currentViewMatrix * projectionMatrix;
            Matrix inverseWorldViewProjection = Matrix.Invert(worldViewProjection);
            inverseWorldViewProjection = Matrix.Transpose(inverseWorldViewProjection);
            planeCoeffs = Vector4.Transform(planeCoeffs, inverseWorldViewProjection);
            Plane finalPlane = new Plane(planeCoeffs);
            return finalPlane;
        }

        private void DrawRefractionMap()
        {
            Plane refractionPlane = CreatePlane(WATER_HEIGHT + 1.5f, new Vector3(0, 1, 0), viewMatrix, false);
            effect.Parameters["ClipPlane0"].SetValue(new Vector4(refractionPlane.Normal, refractionPlane.D));
            // Enable clipping for the purpose of creating a refraction map 
            effect.Parameters["Clipping"].SetValue(true);

            device.SetRenderTarget(refractionRenderTarget);
            device.Clear(ClearOptions.Target | ClearOptions.DepthBuffer, Color.Black, 1.0f, 0);
            DrawTerrain(viewMatrix);
            refractionMap = refractionRenderTarget;
            //Turn clipping off
            effect.Parameters["Clipping"].SetValue(false);
            device.SetRenderTarget(null);
        }

        private void DrawReflectionMap()
        {
            Plane reflectionPlane = CreatePlane(WATER_HEIGHT - 0.5f, new Vector3(0, -1, 0), reflectionViewMatrix, true);
            effect.Parameters["ClipPlane0"].SetValue(new Vector4(-reflectionPlane.Normal, -reflectionPlane.D));

            // Enable clipping for the purpose of creating a reflection map
            effect.Parameters["Clipping"].SetValue(true);

            device.SetRenderTarget(reflectionRenderTarget);
            device.Clear(ClearOptions.Target | ClearOptions.DepthBuffer, Color.Black, 1.0f, 0);

            DrawSkyDome(reflectionViewMatrix);
            DrawTrees(reflectionViewMatrix);
            DrawTerrain(reflectionViewMatrix);

            // Turn clipping off
            effect.Parameters["Clipping"].SetValue(false);

            device.SetRenderTarget(null);
            reflectionMap = reflectionRenderTarget;
        }

        private void SetUpWaterVertices()
        {
            VertexPositionTexture[] waterVertices = new VertexPositionTexture[6];
            waterVertices[0] = new VertexPositionTexture(new Vector3(0, WATER_HEIGHT, 0), new Vector2(0, 1));
            waterVertices[2] = new VertexPositionTexture(new Vector3(terrainWidth, WATER_HEIGHT, -terrainLength), new Vector2(1, 0));
            waterVertices[1] = new VertexPositionTexture(new Vector3(0, WATER_HEIGHT, -terrainLength), new Vector2(0, 0));
            waterVertices[3] = new VertexPositionTexture(new Vector3(0, WATER_HEIGHT, 0), new Vector2(0, 1));
            waterVertices[5] = new VertexPositionTexture(new Vector3(terrainWidth, WATER_HEIGHT, 0), new Vector2(1, 1));
            waterVertices[4] = new VertexPositionTexture(new Vector3(terrainWidth, WATER_HEIGHT, -terrainLength), new Vector2(1, 0));
            waterVertexBuffer = new VertexBuffer(device, VertexPositionTexture.VertexDeclaration, waterVertices.Length, BufferUsage.WriteOnly); waterVertexBuffer.SetData(waterVertices);
        }

        private void DrawWater(float time)
        {
            effect.CurrentTechnique = effect.Techniques["Water"];
            Matrix worldMatrix = Matrix.Identity;
            effect.Parameters["xWorld"].SetValue(worldMatrix);
            effect.Parameters["xView"].SetValue(viewMatrix);
            effect.Parameters["xReflectionView"].SetValue(reflectionViewMatrix);
            effect.Parameters["xProjection"].SetValue(projectionMatrix);
            effect.Parameters["xReflectionMap"].SetValue(reflectionMap);
            effect.Parameters["xRefractionMap"].SetValue(refractionMap);

            effect.Parameters["xWaterBumpMap"].SetValue(waterBumpMap);
            effect.Parameters["xWaveLength"].SetValue(WAVE_LENGTH);
            effect.Parameters["xWaveHeight"].SetValue(WAVE_HEIGHT);

            effect.Parameters["xCamPos"].SetValue(cameraPosition);
            effect.Parameters["xDirtyWaterFactor"].SetValue(WATER_DIRTINESS);
            effect.Parameters["xDullColor"].SetValue(DULL_COLOR);

            effect.Parameters["xTime"].SetValue(time);
            effect.Parameters["xWindForce"].SetValue(WIND_FORCE);
            effect.Parameters["xWindDirection"].SetValue(WIND_DIRECTION);

            foreach (EffectPass pass in effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                device.SetVertexBuffer(waterVertexBuffer);
                device.DrawPrimitives(PrimitiveType.TriangleList, 0, 2);
            }
        }
        #endregion//water methods

        #region Skydome Methods
        private void SetUpFullscreenVertices()
        {
            VertexPositionTexture[] vertices = new VertexPositionTexture[4];
            vertices[0] = new VertexPositionTexture(new Vector3(-1, 1, 0f), new Vector2(0, 1));
            vertices[1] = new VertexPositionTexture(new Vector3(1, 1, 0f), new Vector2(1, 1));
            vertices[2] = new VertexPositionTexture(new Vector3(-1, -1, 0f), new Vector2(0, 0));
            vertices[3] = new VertexPositionTexture(new Vector3(1, -1, 0f), new Vector2(1, 0));
            fullScreenVertices = vertices;
        }
        private void DrawSkyDome(Matrix currentViewMatrix)
        {
            GraphicsDevice.DepthStencilState = DepthStencilState.DepthRead;
            Matrix[] modelTransforms = new Matrix[skyDome.Bones.Count];
            skyDome.CopyAbsoluteBoneTransformsTo(modelTransforms);
            Matrix wMatrix = Matrix.CreateTranslation(0, -0.3f, 0) * Matrix.CreateScale(500) * Matrix.CreateTranslation(cameraPosition);
            foreach (ModelMesh mesh in skyDome.Meshes)
            {
                foreach (Effect currentEffect in mesh.Effects)
                {
                    Matrix mworldMatrix = modelTransforms[mesh.ParentBone.Index] * wMatrix;

                    //currentEffect.CurrentTechnique = currentEffect.Techniques["Textured"];
                    currentEffect.CurrentTechnique = currentEffect.Techniques["SkyDome"];
                    currentEffect.Parameters["xSkyTopColor"].SetValue(SKY_TOP_COLOR);

                    //currentEffect.Parameters["xAmbient"].SetValue(1.0f);
                    currentEffect.Parameters["xWorld"].SetValue(mworldMatrix);
                    currentEffect.Parameters["xView"].SetValue(currentViewMatrix);
                    currentEffect.Parameters["xProjection"].SetValue(projectionMatrix);
                    currentEffect.Parameters["xTexture"].SetValue(cloudMap);
                    currentEffect.Parameters["xEnableLighting"].SetValue(false);
                }
                mesh.Draw();
            }
            GraphicsDevice.DepthStencilState = DepthStencilState.Default;
        }
        private Texture2D CreateStaticMap(int resolution)
        {
            Random rand = new Random();
            Color[] noisyColors = new Color[resolution * resolution];
            for (int x = 0; x < resolution; x++)
                for (int y = 0; y < resolution; y++)
                    noisyColors[x + y * resolution] = new Color(new Vector3((float)rand.Next(1000) / 1000.0f, 0, 0));
            Texture2D noiseImage = new Texture2D(device, resolution, resolution, true, SurfaceFormat.Color);
            noiseImage.SetData(noisyColors);
            return noiseImage;
        }
        private void GeneratePerlinNoise(float time)
        {
            device.SetRenderTarget(cloudsRenderTarget);
            device.Clear(ClearOptions.Target | ClearOptions.DepthBuffer, Color.Black, 1.0f, 0);
            effect.CurrentTechnique = effect.Techniques["PerlinNoise"];
            effect.Parameters["xTexture"].SetValue(cloudStaticMap);
            effect.Parameters["xOvercast"].SetValue(OVERCAST_FACTOR);
            effect.Parameters["xTime"].SetValue(time / TIME_DIV);

            foreach (EffectPass pass in effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                device.DrawUserPrimitives(PrimitiveType.TriangleStrip, fullScreenVertices, 0, 2);
            }
            device.SetRenderTarget(null);
            cloudMap = cloudsRenderTarget;
        }
        #endregion//skydome methods




        #region Camera Methods

        private void SetUpCamera()
        {
            UpdateViewMatrix();
            projectionMatrix = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4, device.Viewport.AspectRatio, 0.3f, 1000.0f);
        }

        private void UpdateViewMatrix()
        {
            Matrix cameraRotation = Matrix.CreateRotationX(updownRot) * Matrix.CreateRotationY(leftrightRot);
            Vector3 cameraOriginalTarget = new Vector3(0, 0, -1);
            Vector3 cameraRotatedTarget = Vector3.Transform(cameraOriginalTarget, cameraRotation);
            Vector3 cameraFinalTarget = cameraPosition + cameraRotatedTarget;
            Vector3 cameraOriginalUpVector = new Vector3(0, 1, 0);
            Vector3 cameraRotatedUpVector = Vector3.Transform(cameraOriginalUpVector, cameraRotation);
            viewMatrix = Matrix.CreateLookAt(cameraPosition, cameraFinalTarget, cameraRotatedUpVector);
            Vector3 reflCameraPosition = cameraPosition; reflCameraPosition.Y = -cameraPosition.Y + WATER_HEIGHT * 2;
            Vector3 reflTargetPos = cameraFinalTarget; reflTargetPos.Y = -cameraFinalTarget.Y + WATER_HEIGHT * 2;
            Vector3 cameraRight = Vector3.Transform(new Vector3(1, 0, 0), cameraRotation);
            Vector3 invUpVector = Vector3.Cross(cameraRight, reflTargetPos - reflCameraPosition);
            reflectionViewMatrix = Matrix.CreateLookAt(reflCameraPosition, reflTargetPos, invUpVector);
        }
        private void AddToCameraPosition(Vector3 vectorToAdd)
        {
            Matrix cameraRotation = Matrix.CreateRotationX(updownRot) * Matrix.CreateRotationY(leftrightRot);
            Vector3 rotatedVector = Vector3.Transform(vectorToAdd, cameraRotation);
            cameraPosition += MOVE_SPEED * rotatedVector;
            UpdateViewMatrix();
        }
        #endregion//camera mathods


        #region user input methods

        private void ProcessInput(float amount)
        {
            MouseState currentMouseState = Mouse.GetState();//reterive current mouse state 
            if (currentMouseState != originalMouseState)// is this state different from starting mouse pos?
            {
                //based on difference in mouse pos from origonal mouse pos, rotate this amount, times time passed since last frame. 
                float xDifference = currentMouseState.X - originalMouseState.X;
                float yDifference = currentMouseState.Y - originalMouseState.Y;
                leftrightRot -= ROTATION_SPEED * xDifference * amount;
                updownRot -= ROTATION_SPEED * yDifference * amount;
                Mouse.SetPosition(device.Viewport.Width / 2, device.Viewport.Height / 2);
                UpdateViewMatrix();
            }

            Vector3 moveVector = new Vector3(0, 0, 0);
            KeyboardState keyState = Keyboard.GetState();
            if (keyState.IsKeyDown(Keys.Up) || keyState.IsKeyDown(Keys.W))
                moveVector += new Vector3(0, 0, -1);
            if (keyState.IsKeyDown(Keys.Down) || keyState.IsKeyDown(Keys.S))
                moveVector += new Vector3(0, 0, 1);
            if (keyState.IsKeyDown(Keys.Right) || keyState.IsKeyDown(Keys.D))
                moveVector += new Vector3(1, 0, 0);
            if (keyState.IsKeyDown(Keys.Left) || keyState.IsKeyDown(Keys.A))
                moveVector += new Vector3(-1, 0, 0);
            if (keyState.IsKeyDown(Keys.Q))
                moveVector += new Vector3(0, 1, 0);
            if (keyState.IsKeyDown(Keys.Z))
                moveVector += new Vector3(0, -1, 0);
            AddToCameraPosition(moveVector * amount);
        }
        #endregion// user input methods

        #region Texture Loads (nested in load content)
        private void LoadTextures()
        {
            sandTexture = Content.Load<Texture2D>("sand");
            rockTexture = Content.Load<Texture2D>("rock");
            snowTexture = Content.Load<Texture2D>("snow");
            grassTexture = Content.Load<Texture2D>("grass");
            cloudMap = Content.Load<Texture2D>("cloudMap");
            waterBumpMap = Content.Load<Texture2D>("waterbump");
            cloudStaticMap = CreateStaticMap(CLOUD_SIZE);
            treeTexture = Content.Load<Texture2D>("tree");
            treeMap = Content.Load<Texture2D>("treeMap");
        }
        #endregion//texture loads

        #region Tree Methods
        private void CreateBBVertsAndIntsFromList()
        {
            VertexPositionTexture[] billboardVertices = new
            VertexPositionTexture[treeList.Count * 4];
            int[] billboardIndices = new int[treeList.Count * 6];
            int j = 0;
            int i = 0;
            foreach (Vector3 currentV3 in treeList)
            {
                // Create vertices
                billboardVertices[i + 0] = new VertexPositionTexture(currentV3, new
                Vector2(0, 0));
                billboardVertices[i + 1] = new VertexPositionTexture(currentV3, new
                Vector2(0, 1));
                billboardVertices[i + 2] = new VertexPositionTexture(currentV3, new
                Vector2(1, 1));
                billboardVertices[i + 3] = new VertexPositionTexture(currentV3, new
                Vector2(1, 0));
                //Create indices
                billboardIndices[j++] = i + 0;
                billboardIndices[j++] = i + 3;
                billboardIndices[j++] = i + 2;
                billboardIndices[j++] = i + 2;
                billboardIndices[j++] = i + 1;
                billboardIndices[j++] = i + 0;
                i += 4;
            }
            treeVertexBuffer = new VertexBuffer(device,
            VertexPositionTexture.VertexDeclaration, billboardVertices.Length,
            BufferUsage.WriteOnly);
            treeVertexBuffer.SetData(billboardVertices);
            treeIndexBuffer = new IndexBuffer(device, IndexElementSize.ThirtyTwoBits,
            treeList.Count * 6, BufferUsage.WriteOnly);
            treeIndexBuffer.SetData(billboardIndices);
        }
        /*private void GenerateTreePositions(VertexMultitextured[] terrainVertices)
        {
            treeList = new List<Vector3>();
            treeList.Add(terrainVertices[3310].Position);
            treeList.Add(terrainVertices[3315].Position);
            treeList.Add(terrainVertices[3320].Position);
            treeList.Add(terrainVertices[3325].Position);
        }*/

        private void GenerateTreePositions(VertexMultitextured[] terrainVertices)
        {
            Color[] treeMapColors = new Color[treeMap.Width * treeMap.Height];
            treeMap.GetData(treeMapColors);

            int[,] noiseData = new int[treeMap.Width, treeMap.Height];
            for (int x = 0; x < treeMap.Width; x++)
                for (int y = 0; y < treeMap.Height; y++)
                    noiseData[x, y] = treeMapColors[y + x * treeMap.Height].R;

            treeList = new List<Vector3>();
            Random random = new Random();

            for (int x = 0; x < terrainWidth; x++)
            {
                for (int y = 0; y < terrainLength; y++)
                {
                    float terrainHeight = heightData[x, y];
                    if ((terrainHeight > TREE_MIN_HT) && (terrainHeight < TREE_MAX_HT))
                    {
                        float flatness = Vector3.Dot(terrainVertices[x + y * terrainWidth].Normal, new Vector3(0, 1, 0));
                        float minFlatness = (float)Math.Cos(MathHelper.ToRadians(TREE_MAX_SLOPE));
                        if (flatness > minFlatness)
                        {
                            float relx = (float)x / (float)terrainWidth;
                            float rely = (float)y / (float)terrainLength;
                            float noiseValueAtCurrentPosition = noiseData[(int)(relx * treeMap.Width), (int)(rely * treeMap.Height)];
                            float treeDensity;
                            if (noiseValueAtCurrentPosition > TREEMAP_HI_THOLD)
                                treeDensity = HI_NUM_TREES;
                            else if (noiseValueAtCurrentPosition > TREEMAP_MED_THOLD)
                                treeDensity = MED_NUM_TREES;
                            else if (noiseValueAtCurrentPosition > TREEMAP_LOW_THOLD)
                                treeDensity = LOW_NUM_TREES;
                            else
                                treeDensity = 0;
                            for (int currDetail = 0; currDetail < treeDensity; currDetail++)
                            {
                                float rand1 = (float)random.Next(1000) / 1000.0f;
                                float rand2 = (float)random.Next(1000) / 1000.0f;
                                Vector3 treePos = new Vector3((float)x - rand1, 0, -(float)y - rand2);
                                treePos.Y = heightData[x, y];
                                treeList.Add(treePos);
                            }
                        }
                    }
                }
            }
        }


        /*private void DrawTrees(Matrix currentViewMatrix)
                {
                    effect.CurrentTechnique = effect.Techniques["CylBillboard"];
                    effect.Parameters["xWorld"].SetValue(Matrix.Identity);
                    effect.Parameters["xView"].SetValue(currentViewMatrix);
                    effect.Parameters["xProjection"].SetValue(projectionMatrix);
                    effect.Parameters["xCamPos"].SetValue(cameraPosition);
                    effect.Parameters["xAllowedRotDir"].SetValue(new Vector3(0, 1, 0));
                    effect.Parameters["xBillboardTexture"].SetValue(treeTexture);

                    int numVertices = treeList.Count * 4;
                    int numTriangles = treeList.Count * 2;

                    device.SetVertexBuffer(treeVertexBuffer);
                    device.Indices = treeIndexBuffer;

                    // Turn on alpha blending
                    GraphicsDevice.BlendState = BlendState.AlphaBlend;
                    GraphicsDevice.DepthStencilState = DepthStencilState.DepthRead;
                    effect.CurrentTechnique.Passes[0].Apply();
                    device.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, numVertices, 0, numTriangles);
                    
                    // Reset graphics adapter to default state
                    GraphicsDevice.BlendState = BlendState.Opaque;
                    GraphicsDevice.DepthStencilState = DepthStencilState.Default;
                }*/
        private void DrawTrees(Matrix currentViewMatrix)
        {
            effect.CurrentTechnique = effect.Techniques["CylBillboard"];
            effect.Parameters["xWorld"].SetValue(Matrix.Identity);
            effect.Parameters["xView"].SetValue(currentViewMatrix);
            effect.Parameters["xProjection"].SetValue(projectionMatrix);
            effect.Parameters["xCamPos"].SetValue(cameraPosition);
            effect.Parameters["xAllowedRotDir"].SetValue(new Vector3(0, 1, 0));
            effect.Parameters["xBillboardTexture"].SetValue(treeTexture);
            effect.Parameters["xBBAlphaTestValue"].SetValue(BB_ALPHA_TEST_VALUE);

            int numVertices = treeList.Count * 4;
            int numTriangles = treeList.Count * 2;

            device.SetVertexBuffer(treeVertexBuffer);
            device.Indices = treeIndexBuffer;

            GraphicsDevice.BlendState = BlendState.AlphaBlend;

            // First draw the "solid" part of the tree
            GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            effect.Parameters["xAlphaTestGreater"].SetValue(true);
            effect.CurrentTechnique.Passes[0].Apply();
            device.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, numVertices, 0, numTriangles);

            // Now draw the rest of the tree
            GraphicsDevice.DepthStencilState = DepthStencilState.DepthRead;
            effect.Parameters["xAlphaTestGreater"].SetValue(false);
            effect.CurrentTechnique.Passes[0].Apply();
            device.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, numVertices, 0, numTriangles);

            // Reset graphics adapter to default state
            GraphicsDevice.BlendState = BlendState.Opaque;
            GraphicsDevice.DepthStencilState = DepthStencilState.Default;
        }

        #endregion//tree methods

        #region Initialize
        protected override void Initialize()
        {
            graphics.PreferredBackBufferWidth = 600;
            graphics.PreferredBackBufferHeight = 600;
            graphics.IsFullScreen = false;
            graphics.ApplyChanges();
            Window.Title = "ATLS 4519 Lab8  - Terrain Tutorial II";

            base.Initialize();
        }
        #endregion//initialize

        #region LoadContent
        protected override void LoadContent()
        {
            device = graphics.GraphicsDevice;

            effect = Content.Load<Effect>("effects");

            SetUpCamera();

            //begin with mouse at center of screen, and store state data
            Mouse.SetPosition(device.Viewport.Width / 2, device.Viewport.Height / 2);
            originalMouseState = Mouse.GetState();

            skyDome = Content.Load<Model>("dome");
            skyDome.Meshes[0].MeshParts[0].Effect = effect.Clone();

            PresentationParameters pp = device.PresentationParameters;
            refractionRenderTarget = new RenderTarget2D(device, pp.BackBufferWidth, pp.BackBufferHeight, false, pp.BackBufferFormat, pp.DepthStencilFormat);
            reflectionRenderTarget = new RenderTarget2D(device, pp.BackBufferWidth, pp.BackBufferHeight, false, pp.BackBufferFormat, pp.DepthStencilFormat);
            cloudsRenderTarget = new RenderTarget2D(device, pp.BackBufferWidth, pp.BackBufferHeight, false, pp.BackBufferFormat, pp.DepthStencilFormat);

            Texture2D heightMap = Content.Load<Texture2D>("heightmap2");
            LoadHeightData(heightMap);
            LoadTextures();

            SetUpVertices();
            SetUpIndices();

            CalculateNormals();
            CopyToBuffers();

            SetUpWaterVertices();

            GenerateTreePositions(vertices);
            CreateBBVertsAndIntsFromList();

            SetUpFullscreenVertices();

        }

        #endregion//load content


        #region UnloadContent
        protected override void UnloadContent()
        {
        }
        #endregion//upload content

        #region Update
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            /* KeyboardState keyState = Keyboard.GetState();

             //Rotation
             if (keyState.IsKeyDown(Keys.PageUp))
             {
                 worldRotation = Matrix.CreateRotationY(0.01f);
             }

             else if (keyState.IsKeyDown(Keys.PageDown))
             {
                 worldRotation = Matrix.CreateRotationY(-0.01f);
             }
             else
             {
                 worldRotation = Matrix.CreateRotationY(0);
             }*/

            float timeDifference = (float)gameTime.ElapsedGameTime.TotalMilliseconds / 1000.0f;
            ProcessInput(timeDifference);

            worldMatrix *= worldTranslation * worldRotation;

            base.Update(gameTime);
        }
        #endregion//update

        #region Draw
        protected override void Draw(GameTime gameTime)
        {
            float time = (float)gameTime.TotalGameTime.TotalMilliseconds / 100.0f;
            DrawRefractionMap();
            DrawReflectionMap();
            GeneratePerlinNoise(time);
            device.Clear(ClearOptions.Target | ClearOptions.DepthBuffer, Color.Black, 1.0f, 0);
            DrawSkyDome(viewMatrix);
            DrawTerrain(viewMatrix);
            DrawWater(time);
            DrawTrees(viewMatrix);
            base.Draw(gameTime);

        }
        #endregion//Draw
    }
}


