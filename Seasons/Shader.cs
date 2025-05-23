using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using StbImageSharp;

namespace Seasons
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
}
