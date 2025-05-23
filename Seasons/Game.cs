using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using OpenTK.Audio.OpenAL;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using StbImageSharp;

namespace Seasons
{
    internal class Game : GameWindow
    {
        int width, height;

        //public Shader shaderProgram = new Shader();
        private SeasonManager seasonManager;
        private House house;
        private Environment environment;
        private Shader shader;
        Camera camera;

        public Game(int width, int height) : base
        (GameWindowSettings.Default, NativeWindowSettings.Default)
        {
            this.CenterWindow(new Vector2i(width, height));
            this.height = height;
            this.width = width;
        }

        protected override void OnLoad()
        {
            base.OnLoad();

            // Инициализация OpenGL
            GL.ClearColor(0.2f, 0.3f, 0.3f, 1.0f);
            GL.Enable(EnableCap.DepthTest);
            GL.Disable(EnableCap.CullFace);
            GL.Disable(EnableCap.DepthTest);

            // Создание менеджера сезонов
            seasonManager = new SeasonManager();

            // Загрузка шейдера
            shader = new Shader();
            shader.LoadShader();
            CompileShaders();

            // Создание объектов сцены
            house = new House(seasonManager);

            camera = new Camera(width, height, new Vector3(0f, 0f, -2f));
            //camera = new Camera(width, height, Vector3.Zero);
            CursorState = CursorState.Grabbed;
        }

        private void CompileShaders()
        {
            int vertexShader = GL.CreateShader(ShaderType.VertexShader);
            GL.ShaderSource(vertexShader, Shader.LoadShaderSource("shader.vert"));
            GL.CompileShader(vertexShader);

            int fragmentShader = GL.CreateShader(ShaderType.FragmentShader);
            GL.ShaderSource(fragmentShader, Shader.LoadShaderSource("shader.frag"));
            GL.CompileShader(fragmentShader);

            GL.AttachShader(shader.shaderHandle, vertexShader);
            GL.AttachShader(shader.shaderHandle, fragmentShader);
            GL.LinkProgram(shader.shaderHandle);

            GL.DetachShader(shader.shaderHandle, vertexShader);
            GL.DetachShader(shader.shaderHandle, fragmentShader);
            GL.DeleteShader(vertexShader);
            GL.DeleteShader(fragmentShader);
        }

        protected override void OnUnload()
        {
            base.OnLoad();
            house.Dispose();
            //environment.Dispose();
            shader.DeleteShader();

            /*GL.DeleteBuffer(VAO);
            GL.DeleteBuffer(VBO);
            GL.DeleteBuffer(EBO);
            shader.DeleteShader();
            GL.DeleteTexture(textureID);
            */
        }

        protected override void OnRenderFrame(FrameEventArgs args)
        {
            base.OnRenderFrame(args);

            GL.ClearColor(0.3f, 0.3f, 1f, 1f);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            shader.UseShader();

            Matrix4 model = Matrix4.Identity; // Без поворота
            Matrix4 view = camera.GetViewMatrix();
            Matrix4 projection = camera.GetProjection();


            int modelLocation = GL.GetUniformLocation(shader.shaderHandle, "model");
            int viewLocation = GL.GetUniformLocation(shader.shaderHandle, "view");
            int projectionLocation = GL.GetUniformLocation(shader.shaderHandle, "projection");

            GL.UniformMatrix4(modelLocation, true, ref model);
            GL.UniformMatrix4(viewLocation, true, ref view);
            GL.UniformMatrix4(projectionLocation, true, ref projection);

            house.Render(shader, seasonManager.CurrentSeason);

            Context.SwapBuffers();
        }
        protected override void OnUpdateFrame(FrameEventArgs args)
        {
            seasonManager.Update((float)args.Time);
            if (KeyboardState.IsKeyDown(Keys.Escape))
            {
                Close();
            }
            MouseState mouse = MouseState;
            KeyboardState input = KeyboardState;
            base.OnUpdateFrame(args);
            camera.Update(input, mouse, args);
        }
        protected override void OnResize(ResizeEventArgs e)
        {
            base.OnResize(e);
            GL.Viewport(0, 0, e.Width, e.Height);
            this.width = e.Width;
            this.height = e.Height;
        }

        /*private void UpdateBackground(SeasonManager.Season season)
        {
            switch (season)
            {
                case SeasonManager.Season.Winter:
                    GL.ClearColor(0.7f, 0.8f, 1.0f, 1.0f); // Голубоватый
                    break;
                case SeasonManager.Season.Spring:
                    GL.ClearColor(0.8f, 0.9f, 1.0f, 1.0f); // Светлый
                    break;
                case SeasonManager.Season.Summer:
                    GL.ClearColor(0.4f, 0.6f, 1.0f, 1.0f); // Яркий
                    break;
                case SeasonManager.Season.Autumn:
                    GL.ClearColor(0.6f, 0.6f, 0.7f, 1.0f); // Сероватый
                    break;
            }
        }*/
        
    }
}
