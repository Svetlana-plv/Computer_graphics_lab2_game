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

namespace Pylaeva_lab2
{

    public class Shader
    {
        public int shaderHandle;

        public void LoadShader()
        {
            shaderHandle = GL.CreateProgram();
        }

        public static string LoadShaderSource(string filepath)
        {
            string shaderSource = "";
            try
            {
                using (StreamReader reader = new StreamReader("../../../Shaders/" + filepath))
                {
                    shaderSource = reader.ReadToEnd();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Failed to load shader source file:" + e.Message);
            }
            return shaderSource;
        }

        public void UseShader()
        {
            GL.UseProgram(shaderHandle);
        }
        public void DeleteShader()
        {
            GL.DeleteProgram(shaderHandle);
        }

    }
    internal class Game : GameWindow
    {
        int width, height;

        // Координаты вершин куба (передняя, правая, задняя, левая, верхняя, нижняя грани)
        List<Vector3> vertices = new List<Vector3>()
        {
            // Передняя грань
            new Vector3(-0.5f, -0.5f,  0.5f), // 0
            new Vector3( 0.5f, -0.5f,  0.5f), // 1
            new Vector3( 0.5f,  0.5f,  0.5f), // 2
            new Vector3(-0.5f,  0.5f,  0.5f), // 3
    
            // Правая грань
            new Vector3(0.5f, -0.5f,  0.5f),  // 4
            new Vector3(0.5f, -0.5f, -0.5f),  // 5
            new Vector3(0.5f,  0.5f, -0.5f),  // 6
            new Vector3(0.5f,  0.5f,  0.5f),  // 7
    
            // Задняя грань
            new Vector3( 0.5f, -0.5f, -0.5f), // 8
            new Vector3(-0.5f, -0.5f, -0.5f), // 9
            new Vector3(-0.5f,  0.5f, -0.5f), // 10
            new Vector3( 0.5f,  0.5f, -0.5f), // 11
    
            // Левая грань
            new Vector3(-0.5f, -0.5f, -0.5f), // 12
            new Vector3(-0.5f, -0.5f,  0.5f), // 13
            new Vector3(-0.5f,  0.5f,  0.5f), // 14
            new Vector3(-0.5f,  0.5f, -0.5f), // 15
    
            // Верхняя грань
            new Vector3(-0.5f, 0.5f,  0.5f),  // 16
            new Vector3( 0.5f, 0.5f,  0.5f),  // 17
            new Vector3( 0.5f, 0.5f, -0.5f),  // 18
            new Vector3(-0.5f, 0.5f, -0.5f),  // 19
    
            // Нижняя грань
            new Vector3(-0.5f, -0.5f, -0.5f), // 20
            new Vector3( 0.5f, -0.5f, -0.5f), // 21 
            new Vector3( 0.5f, -0.5f,  0.5f), // 22 
            new Vector3(-0.5f, -0.5f,  0.5f)  // 23
        };

        List<Vector2> texCoords = new List<Vector2>()
        {
            // Передняя грань
            new Vector2(0.0f, 0.0f),
            new Vector2(1.0f, 0.0f),
            new Vector2(1.0f, 1.0f),
            new Vector2(0.0f, 1.0f),
    
            // Правая грань
            new Vector2(0.0f, 0.0f),
            new Vector2(1.0f, 0.0f),
            new Vector2(1.0f, 1.0f),
            new Vector2(0.0f, 1.0f),

            // Задняя грань
            new Vector2(0.0f, 0.0f),
            new Vector2(1.0f, 0.0f),
            new Vector2(1.0f, 1.0f),
            new Vector2(0.0f, 1.0f),

            // Левая грань
            new Vector2(0.0f, 0.0f),
            new Vector2(1.0f, 0.0f),
            new Vector2(1.0f, 1.0f),
            new Vector2(0.0f, 1.0f),

            // Верхняя грань
            new Vector2(0.0f, 0.0f),
            new Vector2(1.0f, 0.0f),
            new Vector2(1.0f, 1.0f),
            new Vector2(0.0f, 1.0f),

            // Нижняя грань
            new Vector2(0.0f, 0.0f),
            new Vector2(1.0f, 0.0f),
            new Vector2(1.0f, 1.0f),
            new Vector2(0.0f, 1.0f)

        };

        uint[] indices = {

            // Передняя грань
            0, 1, 2,
            2, 3, 0,
    
            // Правая грань
            4, 5, 6,
            6, 7, 4,
    
            // Задняя грань
            8, 9, 10,
            10, 11, 8,
    
            // Левая грань
            12, 13, 14,
            14, 15, 12,
    
            // Верхняя грань
            16, 17, 18,
            18, 19, 16,
    
            // Нижняя грань
            20, 21, 22,
            22, 23, 20
        };

        int VAO; 
        int VBO;
        int EBO;
        public Shader shaderProgram = new Shader();
        int textureID;
        int textureVBO;
        float yRot = 0f;
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

            // 1. Загрузка и компиляция шейдеров
            shaderProgram.LoadShader();
            int vertexShader = GL.CreateShader(ShaderType.VertexShader);
            GL.ShaderSource(vertexShader, Shader.LoadShaderSource("shader.vert"));
            GL.CompileShader(vertexShader);
            int fragmentShader = GL.CreateShader(ShaderType.FragmentShader);
            GL.ShaderSource(fragmentShader, Shader.LoadShaderSource("shader.frag"));
            GL.CompileShader(fragmentShader);
            GL.AttachShader(shaderProgram.shaderHandle, vertexShader);
            GL.AttachShader(shaderProgram.shaderHandle, fragmentShader);
            GL.LinkProgram(shaderProgram.shaderHandle);

            // 2. Настройка VAO, VBO и EBO
            VAO = GL.GenVertexArray();
            GL.BindVertexArray(VAO);

            // Настройка вершинного буфера
            VBO = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, VBO);
            GL.BufferData(BufferTarget.ArrayBuffer, vertices.Count * Vector3.SizeInBytes, vertices.ToArray(), BufferUsageHint.StaticDraw);
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 0, 0);
            GL.EnableVertexArrayAttrib(VAO, 0);

            // Настройка буфера текстурных координат
            textureVBO = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, textureVBO);
            GL.BufferData(BufferTarget.ArrayBuffer, texCoords.Count * Vector2.SizeInBytes, texCoords.ToArray(), BufferUsageHint.StaticDraw);
            GL.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, 0, 0);
            GL.EnableVertexArrayAttrib(VAO, 1);

            // Настройка индексного буфера
            EBO = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, EBO);
            GL.BufferData(BufferTarget.ElementArrayBuffer, indices.Length * sizeof(uint), indices, BufferUsageHint.StaticDraw);

            // 3. Отвязка буферов
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.BindVertexArray(0);

            // 4. Загрузка текстуры
            textureID = GL.GenTexture();
            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, textureID);

            // Параметры текстуры
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);

            // Загрузка изображения
            StbImage.stbi_set_flip_vertically_on_load(1);
            ImageResult boxTexture = ImageResult.FromStream(File.OpenRead("../../../Textures/box.jpg"), ColorComponents.RedGreenBlueAlpha);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, boxTexture.Width, boxTexture.Height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, boxTexture.Data);

            // Отвязка текстуры
            GL.BindTexture(TextureTarget.Texture2D, 0);

            GL.Enable(EnableCap.DepthTest);

            camera = new Camera(width, height, Vector3.Zero);
            CursorState = CursorState.Grabbed;
        }
        protected override void OnUnload()
        {
            base.OnLoad();

            GL.DeleteBuffer(VAO);
            GL.DeleteBuffer(VBO);
            GL.DeleteBuffer(EBO);
            shaderProgram.DeleteShader();
            GL.DeleteTexture(textureID);
        }
        protected override void OnRenderFrame(FrameEventArgs args)
        {
            base.OnRenderFrame(args);

            yRot += (float)args.Time * 60f; 

            GL.ClearColor(0.3f, 0.3f, 1f, 1f);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            shaderProgram.UseShader();
            GL.BindTexture(TextureTarget.Texture2D, textureID);

            Matrix4 model = Matrix4.CreateRotationY(45f);
            Matrix4 view = camera.GetViewMatrix();
            Matrix4 projection = camera.GetProjection();
            Matrix4 translation = Matrix4.CreateTranslation(0f, 0f, -3f);
            model *= translation * Matrix4.CreateRotationY(MathHelper.DegreesToRadians(yRot));

            int modelLocation = GL.GetUniformLocation(shaderProgram.shaderHandle, "model");
            int viewLocation = GL.GetUniformLocation(shaderProgram.shaderHandle, "view");
            int projectionLocation = GL.GetUniformLocation(shaderProgram.shaderHandle, "projection");

            GL.UniformMatrix4(modelLocation, true, ref model);
            GL.UniformMatrix4(viewLocation, true, ref view);
            GL.UniformMatrix4(projectionLocation, true, ref projection);

            GL.BindVertexArray(VAO);
            GL.DrawElements(PrimitiveType.Triangles, indices.Length, DrawElementsType.UnsignedInt, 0);


            Context.SwapBuffers();
        }
        protected override void OnUpdateFrame(FrameEventArgs args)
        {
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

    }
}
