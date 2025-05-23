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
    internal abstract class GameObject
    {
        protected int VAO, VBO, EBO;
        protected uint[] indices;
        protected List<Vector3> vertices;
        protected List<Vector2> texCoords;

        public abstract void LoadTextures();
        public abstract void Render(Shader shader, SeasonManager.Season currentSeason);
        public abstract void Dispose();
    }
}
