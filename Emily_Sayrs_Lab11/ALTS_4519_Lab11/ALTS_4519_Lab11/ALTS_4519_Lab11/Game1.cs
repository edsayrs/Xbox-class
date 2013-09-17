using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;

namespace ATLS_4519_Lab11
{
 
    struct MyOwnVertexFormat
    {
        private Vector3 position;
        private Vector2 texCoord;
        private Vector3 normal;

        public MyOwnVertexFormat(Vector3 position, Vector2 texCoord, Vector3 normal)
        {
            this.position = position;
            this.texCoord = texCoord;
            this.normal = normal;
        }

        public static VertexDeclaration VertexDeclaration = new VertexDeclaration
            (
            new VertexElement(0, VertexElementFormat.Vector3, VertexElementUsage.Position, 0),
            new VertexElement(sizeof(float) * 3, VertexElementFormat.Vector2, VertexElementUsage.TextureCoordinate, 0),
            new VertexElement(sizeof(float) * 5, VertexElementFormat.Vector3, VertexElementUsage.Normal,0)
            );
        }

    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        GraphicsDevice device;

        Effect effect;
        Matrix viewMatrix;
        Matrix projectionMatrix;
        VertexBuffer vertexBuffer;
        Vector3 cameraPos;

        Texture2D streetTexture;

        Model lamppostModel;
        Matrix[] LampModelTransforms;

        Model carModel;
        Texture2D[] carTextures;
        Matrix car1Matrix;
        Matrix car2Matrix;
        //Matrix car3Matrix;
        Matrix[] CarModelTransforms;

        Vector3 lightPos;

        float[] lightPower = new float[2];
        Vector4[] ltscrPositions = new Vector4[4];
        Matrix CameraViewProjection;
        Vector3[] camdists = new Vector3[4];
        float[] camrads = new float[4];
        float[] CamLtTimesGlow = new float[4];
        float[] LampGlowFactor = new float[4];

        float ambientPower;
        
        Matrix lightsViewProjectionMatrix;

        RenderTarget2D renderTarget;
        Texture2D shadowMap;

        Texture2D carLight;

        Vector3[] lamppostPositions = new Vector3[4];

        
        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        private void SetUpVertices()
        {
            MyOwnVertexFormat[] vertices = new MyOwnVertexFormat[18];
            vertices[0] = new MyOwnVertexFormat(new Vector3(-20, 0, 10), new Vector2(-0.25f, 25.0f), new Vector3(0, 1, 0));
            vertices[1] = new MyOwnVertexFormat(new Vector3(-20, 0, -100), new Vector2(-0.25f, 0.0f), new Vector3(0, 1, 0));
            vertices[2] = new MyOwnVertexFormat(new Vector3(2, 0, 10), new Vector2(0.25f, 25.0f), new Vector3(0, 1, 0));
            vertices[3] = new MyOwnVertexFormat(new Vector3(2, 0, -100), new Vector2(0.25f, 0.0f), new Vector3(0, 1, 0));
            vertices[4] = new MyOwnVertexFormat(new Vector3(2, 0, 10), new Vector2(0.25f, 25.0f), new Vector3(-1, 0, 0));
            vertices[5] = new MyOwnVertexFormat(new Vector3(2, 0, -100), new Vector2(0.25f, 0.0f), new Vector3(-1, 0, 0));
            vertices[6] = new MyOwnVertexFormat(new Vector3(2, 1, 10), new Vector2(0.375f, 25.0f), new Vector3(-1, 0, 0));
            vertices[7] = new MyOwnVertexFormat(new Vector3(2, 1, -100), new Vector2(0.375f, 0.0f), new Vector3(-1, 0, 0));
            vertices[8] = new MyOwnVertexFormat(new Vector3(2, 1, 10), new Vector2(0.375f, 25.0f), new Vector3(0, 1, 0));
            vertices[9] = new MyOwnVertexFormat(new Vector3(2, 1, -100), new Vector2(0.375f, 0.0f), new Vector3(0, 1, 0));
            vertices[10] = new MyOwnVertexFormat(new Vector3(3, 1, 10), new Vector2(0.5f, 25.0f), new Vector3(0, 1, 0));
            vertices[11] = new MyOwnVertexFormat(new Vector3(3, 1, -100), new Vector2(0.5f, 0.0f), new Vector3(0, 1, 0));
            vertices[12] = new MyOwnVertexFormat(new Vector3(13, 1, 10), new Vector2(0.75f, 25.0f), new Vector3(0, 1, 0));
            vertices[13] = new MyOwnVertexFormat(new Vector3(13, 1, -100), new Vector2(0.75f, 0.0f), new Vector3(0, 1, 0));
            vertices[14] = new MyOwnVertexFormat(new Vector3(13, 1, 10), new Vector2(0.75f, 25.0f), new Vector3(-1, 0, 0));
            vertices[15] = new MyOwnVertexFormat(new Vector3(13, 1, -100), new Vector2(0.75f, 0.0f), new Vector3(-1, 0, 0));
            vertices[16] = new MyOwnVertexFormat(new Vector3(13, 21, 10), new Vector2(1.25f, 25.0f), new Vector3(-1, 0, 0));
            vertices[17] = new MyOwnVertexFormat(new Vector3(13, 21, -100), new Vector2(1.25f, 0.0f), new Vector3(-1, 0, 0));
            vertexBuffer = new VertexBuffer(device, MyOwnVertexFormat.VertexDeclaration, vertices.Length, BufferUsage.WriteOnly);
            vertexBuffer.SetData(vertices);
        }

       
        private void SetUpCamera()
        {
            cameraPos = new Vector3(-25, 13, 18);
            viewMatrix = Matrix.CreateLookAt(cameraPos, new Vector3(0, 2, -12), new Vector3(0, 1, 0));
            projectionMatrix = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4, device.Viewport.AspectRatio, 1.0f, 200.0f);
        }
      


        private void UpdateLightData()
        {
            
                ambientPower = 0.2f;
                lightPos = new Vector3(-18, 5, -2);

                // using an array for light power; [0] is in light, [1] is in shadow
                lightPower[0] = 2.0f;
                lightPower[1] = 1.6f;

                Matrix lightsView = Matrix.CreateLookAt(lightPos, new Vector3(-2, 3, -10),new Vector3(0, 1, 0));
                Matrix lightsProjection = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver2, 1f, 5f, 10000f);
                lightsViewProjectionMatrix = lightsView * lightsProjection;
                
                //These are the "real" lights
                lamppostPositions[0] = new Vector3(3f, 11.5f, -35f);
                lamppostPositions[1] = new Vector3(3f, 11.5f, -5f);
                
                //These are the shadows of the real lights
                lamppostPositions[2] = new Vector3(11f, 14.5f, -5f);
                lamppostPositions[3] = new Vector3(4.5f, 14.0f, -35f);
                
                //compute light screen positions
                CameraViewProjection = viewMatrix * projectionMatrix;
                for (int i = 0; i < 4; i++)
                {
                    ltscrPositions[i] = Vector4.Transform(lamppostPositions[i], CameraViewProjection);
                    ltscrPositions[i] /= ltscrPositions[i].W;
                    
                    // Compute the distance between each lamp and the camera
                    camdists[i] = cameraPos - lamppostPositions[i];
                    camrads[i] = 5.0f / camdists[i].Length();
                    CamLtTimesGlow[i] = camdists[i].Length() * LampGlowFactor[i];
                }
            
        }

        
        private void LoadCar()
        {
            carModel = Content.Load<Model>("racer");
            carTextures = new Texture2D[carModel.Meshes.Count + 2];//More effects than meshes (?)
            int i = 0;

            foreach (ModelMesh mesh in carModel.Meshes)
                foreach (BasicEffect currentEffect in mesh.Effects)
                    carTextures[i++] = currentEffect.Texture;

            foreach (ModelMesh mesh in carModel.Meshes)
                foreach (ModelMeshPart meshPart in mesh.MeshParts)
                    meshPart.Effect = effect.Clone();
            CarModelTransforms = new Matrix[carModel.Bones.Count];
            carModel.CopyAbsoluteBoneTransformsTo(CarModelTransforms);
            car1Matrix = Matrix.CreateScale(4f) * Matrix.CreateRotationY(MathHelper.Pi) * Matrix.CreateTranslation(-3, 0, -15);
            car2Matrix = Matrix.CreateScale(4f) * Matrix.CreateRotationY(MathHelper.Pi * 5.0f / 8.0f) * Matrix.CreateTranslation(-28, 0, -1.9f);
            //car3Matrix = Matrix.CreateScale(4f) * Matrix.CreateRotationY(MathHelper.Pi * 5.0f / 8.0f) * Matrix.CreateTranslation(-10, 0, -0.7f);
        }



        private void LoadLamp()
        {
            lamppostModel = Content.Load<Model>("lamppost");
            LampModelTransforms = new Matrix[lamppostModel.Bones.Count];
            lamppostModel.CopyAbsoluteBoneTransformsTo(LampModelTransforms);
            foreach (ModelMesh mesh in lamppostModel.Meshes)
                foreach (ModelMeshPart meshPart in mesh.MeshParts)
                    meshPart.Effect = effect.Clone();
        
        }

        private void DrawCar(Matrix wMatrix, string technique)
        {
            int i = 0;
            foreach (ModelMesh mesh in carModel.Meshes)
            {
                Matrix worldMatrix = CarModelTransforms[mesh.ParentBone.Index] * wMatrix;
                foreach (Effect currentEffect in mesh.Effects)
                {
                    currentEffect.CurrentTechnique = currentEffect.Techniques[technique];
                    if (technique != "ShadowMap")
                    {
                        currentEffect.Parameters["xShadowMap"].SetValue(shadowMap);
                       
                        currentEffect.Parameters["xTexture"].SetValue(carTextures[i++]);
                        currentEffect.Parameters["xWorld"].SetValue(worldMatrix);
                        currentEffect.Parameters["xLightPos"].SetValue(lightPos);
                        currentEffect.Parameters["xLightPower"].SetValue(lightPower);
                        currentEffect.Parameters["xAmbient"].SetValue(ambientPower);
                        currentEffect.Parameters["xCarLightTexture"].SetValue(carLight);
                        currentEffect.Parameters["xViewProjection"].SetValue(CameraViewProjection);
                       
                        currentEffect.Parameters["xLamppostPos"].SetValue(lamppostPositions);
                        currentEffect.Parameters["xLightAttenuation"].SetValue(4000);
                        currentEffect.Parameters["xLightFallOff"].SetValue(0.027f);
                        currentEffect.Parameters["xLtscrPositions"].SetValue(ltscrPositions);
                        currentEffect.Parameters["xCamRads"].SetValue(camrads);
                    }
                    currentEffect.Parameters["xLightsWorldViewProjection"].SetValue(worldMatrix * lightsViewProjectionMatrix);
                }

                mesh.Draw();
            }
        }
        private void DrawLampPost(float scale, Vector3 translation, string technique)
        {
           
            Matrix lampMatrix = Matrix.CreateScale(scale) * Matrix.CreateTranslation(translation);
            Matrix[] modelTransforms = new Matrix[lamppostModel.Bones.Count];
            lamppostModel.CopyAbsoluteBoneTransformsTo(modelTransforms);
            
            foreach (ModelMesh mesh in lamppostModel.Meshes)
            {
                Matrix worldMatrix = LampModelTransforms[mesh.ParentBone.Index] * lampMatrix;
                foreach (Effect currentEffect in mesh.Effects)
                {
                    currentEffect.CurrentTechnique = currentEffect.Techniques[technique];
                    if (technique != "ShadowMap")
                    {
                        
                        currentEffect.Parameters["xWorld"].SetValue(worldMatrix);
                        currentEffect.Parameters["xLightPos"].SetValue(lightPos);
                        currentEffect.Parameters["xLightPower"].SetValue(lightPower);
                        currentEffect.Parameters["xAmbient"].SetValue(ambientPower);
                        currentEffect.Parameters["xShadowMap"].SetValue(shadowMap);
                        currentEffect.Parameters["xCarLightTexture"].SetValue(carLight);
                        currentEffect.Parameters["xViewProjection"].SetValue(CameraViewProjection);
                        
                        currentEffect.Parameters["xLamppostPos"].SetValue(lamppostPositions);
                        currentEffect.Parameters["xCamLtTimesGlow"].SetValue(CamLtTimesGlow);
                        
                    }
                    currentEffect.Parameters["xLightsWorldViewProjection"].SetValue(worldMatrix * lightsViewProjectionMatrix);
                }
                mesh.Draw();
            }
        }


        private void DrawScene(string technique)
        {
            effect.CurrentTechnique = effect.Techniques[technique];
            if (technique != "ShadowMap")
            {
                effect.Parameters["xShadowMap"].SetValue(shadowMap);
                
                effect.Parameters["xTexture"].SetValue(streetTexture);
                effect.Parameters["xWorld"].SetValue(Matrix.Identity);
                effect.Parameters["xLightPos"].SetValue(lightPos);
                effect.Parameters["xLightPower"].SetValue(lightPower);
                effect.Parameters["xAmbient"].SetValue(ambientPower);
                effect.Parameters["xCarLightTexture"].SetValue(carLight);
                effect.Parameters["xViewProjection"].SetValue(CameraViewProjection);
                
                effect.Parameters["xLamppostPos"].SetValue(lamppostPositions);
                effect.Parameters["xLtscrPositions"].SetValue(ltscrPositions);
                effect.Parameters["xCamRads"].SetValue(camrads);
                effect.Parameters["xLightAttenuation"].SetValue(4000);
                effect.Parameters["xLightFallOff"].SetValue(0.027f);
            }
            effect.Parameters["xLightsWorldViewProjection"].SetValue(Matrix.Identity * lightsViewProjectionMatrix);
            foreach (EffectPass pass in effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                device.SetVertexBuffer(vertexBuffer);
                device.DrawPrimitives(PrimitiveType.TriangleStrip, 0, 18);
            }
        }
       
        protected override void Initialize()
        {
            graphics.PreferredBackBufferWidth = 700;
            graphics.PreferredBackBufferHeight = 700;
            graphics.IsFullScreen = false;
            graphics.ApplyChanges();
            Window.Title = "ATLS 4519 Lab 11 - HLSL Tutorial";

            LampGlowFactor[0] = 0.034f;
            LampGlowFactor[1] = 0.060f;
            LampGlowFactor[2] = 0.0f;
            LampGlowFactor[3] = 0.0f;

            base.Initialize();
        }
   

    
        protected override void LoadContent()
        {
            device = GraphicsDevice;

            effect = Content.Load<Effect>("Effect1");

            SetUpVertices();
            SetUpCamera();

            streetTexture = Content.Load<Texture2D>("streettexture");
            
            //Load cars
            LoadCar();

            //Load Lamp posts
            LoadLamp();

            PresentationParameters pp = device.PresentationParameters;
            renderTarget = new RenderTarget2D(device, pp.BackBufferWidth, pp.BackBufferHeight, true, pp.BackBufferFormat, pp.DepthStencilFormat);
            
            carLight = Content.Load<Texture2D>("carlight");
        }



        protected override void UnloadContent()
        {
        }
     

   
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            UpdateLightData();

            base.Update(gameTime);
        }



        protected override void Draw(GameTime gameTime)
        {
            device.SetRenderTarget(renderTarget);
            device.Clear(ClearOptions.Target | ClearOptions.DepthBuffer, Color.White, 1.0f, 0);

            DrawScene("ShadowMap");
            DrawCar(car1Matrix, "ShadowMap");
            DrawCar(car2Matrix, "ShadowMap");
            //DrawCar(car3Matrix, "ShadowMap");
            DrawLampPost(0.05f, new Vector3(4.0f, 1f, -35f), "ShadowMap");
            DrawLampPost(0.05f, new Vector3(4.0f, 1f, -5f), "ShadowMap");

            device.SetRenderTarget(null);
            shadowMap = renderTarget;

            // Reset 3D Defaults
            GraphicsDevice.BlendState = BlendState.Opaque;
            GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            GraphicsDevice.SamplerStates[0] = SamplerState.LinearWrap;

            device.Clear(ClearOptions.Target | ClearOptions.DepthBuffer, Color.Black, 1.0f, 0);
            DrawScene("ShadowedScene");
            DrawCar(car1Matrix, "ShadowedScene");
            DrawCar(car2Matrix, "ShadowedScene");
            DrawLampPost(0.05f, new Vector3(4.0f, 1f, -35f), "SimpleNormal");
            DrawLampPost(0.05f, new Vector3(4.0f, 1f, -5f), "SimpleNormal");
            
            base.Draw(gameTime);
        }

    }
}
