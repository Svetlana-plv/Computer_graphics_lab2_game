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
    internal class House : GameObject
    {
        private SeasonalTexture wallsTexture;
        private SeasonalTexture roofTexture;

        public House(SeasonManager seasonManager)
        {
            // 1. Вершины (8 точек: 4 для передней грани + 4 для верхней)
            this.vertices = new List<Vector3>()
{
    // Передняя грань (Z = 0.5)
    new Vector3(-0.5f, -0.5f, 0.5f), // 0 - левый нижний
    new Vector3(0.5f, -0.5f, 0.5f),  // 1 - правый нижний
    new Vector3(0.5f, 0.5f, 0.5f),   // 2 - правый верхний
    new Vector3(-0.5f, 0.5f, 0.5f),  // 3 - левый верхний

};

            // 2. Текстурные координаты (для каждой вершины)
            this.texCoords = new List<Vector2>()
{
    // Передняя грань
    new Vector2(0.0f, 0.0f), // 0
    new Vector2(1.0f, 0.0f), // 1
    new Vector2(1.0f, 1.0f), // 2
    new Vector2(0.0f, 1.0f), // 3

};

            // 3. Индексы для треугольников (12 индексов = 4 треугольника)
            this.indices = new uint[]
            {
    // Передняя грань (2 треугольника)
    0, 1, 2,
    2, 3, 0

            };


            // Инициализация тетраэдра (крыша)
            // ...

            wallsTexture = new SeasonalTexture(seasonManager,
                "../../../Textures/Background_winter.jpg",
                "../../../Textures/Background_spring.jpg",
                "../../../Textures/Background_summer.jpg",
                "../../../Textures/Background_autumn.jpg");

            roofTexture = new SeasonalTexture(seasonManager,
                "../../../Textures/Background_autumn.jpg",
                "../../../Textures/Background_autumn.jpg",
                "../../../Textures/Background_autumn.jpg",
                "../../../Textures/Background_autumn.jpg");

            LoadTextures();
        }

        public override void LoadTextures()
        {
            // Инициализация VAO, VBO и EBO для стен дома
            VAO = GL.GenVertexArray();
            GL.BindVertexArray(VAO);

            VBO = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, VBO);
            GL.BufferData(BufferTarget.ArrayBuffer, vertices.Count * Vector3.SizeInBytes, vertices.ToArray(), BufferUsageHint.StaticDraw);

            int texVBO = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, texVBO);
            GL.BufferData(BufferTarget.ArrayBuffer, texCoords.Count * Vector2.SizeInBytes, texCoords.ToArray(), BufferUsageHint.StaticDraw);

            EBO = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, EBO);
            GL.BufferData(BufferTarget.ElementArrayBuffer, indices.Length * sizeof(uint), indices, BufferUsageHint.StaticDraw);

            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 0, 0);
            GL.EnableVertexAttribArray(0);

            GL.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, 0, 0);
            GL.EnableVertexAttribArray(1);

            GL.BindVertexArray(0);
        }

        public override void Render(Shader shader, SeasonManager.Season currentSeason)
        {
            // Рендер стен
            GL.BindTexture(TextureTarget.Texture2D, wallsTexture.GetTexture(currentSeason));
            GL.BindVertexArray(VAO);
            GL.DrawElements(PrimitiveType.Triangles, indices.Length, DrawElementsType.UnsignedInt, 0);

            // Рендер крыши
            //GL.BindTexture(TextureTarget.Texture2D, roofTexture.GetTexture(currentSeason));
            // ... отрисовка тетраэдра
        }

        
        public override void Dispose()
        {
            GL.DeleteVertexArray(VAO);
            GL.DeleteBuffer(VBO);
            GL.DeleteBuffer(EBO);
            //GL.DeleteTexture(textureID);
        }
    }
}

