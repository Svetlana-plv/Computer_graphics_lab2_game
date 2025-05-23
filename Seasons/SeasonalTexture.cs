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
    internal class SeasonalTexture
    {
        private readonly int[] textureIds = new int[4];

        public SeasonalTexture(SeasonManager seasonManager,
                             string winter, string spring,
                             string summer, string autumn)
        {
            textureIds[0] = LoadTexture(winter);
            textureIds[1] = LoadTexture(spring);
            textureIds[2] = LoadTexture(summer);
            textureIds[3] = LoadTexture(autumn);
        }

        public int GetTexture(SeasonManager.Season season)
        {
            return textureIds[(int)season];
        }

        private int LoadTexture(string path)
        {
            // 4. Загрузка текстуры
            int textureID = GL.GenTexture();
            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, textureID);

            // Параметры текстуры
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);

            // Загрузка изображения
            StbImage.stbi_set_flip_vertically_on_load(1);
            ImageResult boxTexture = ImageResult.FromStream(File.OpenRead(path), ColorComponents.RedGreenBlueAlpha);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, boxTexture.Width, boxTexture.Height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, boxTexture.Data);

            // Отвязка текстуры
            GL.BindTexture(TextureTarget.Texture2D, 0);

            return textureID;
        }
    }
}
