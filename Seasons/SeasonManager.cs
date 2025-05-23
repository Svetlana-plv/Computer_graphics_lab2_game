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
    internal class SeasonManager
    {
        public enum Season { Winter, Spring, Summer, Autumn }
        public Season CurrentSeason { get; private set; }

        private float timer = 0;
        private const float SeasonDuration = 10f; // 10 секунд на сезон

        public void Update(float deltaTime)
        {
            timer += deltaTime;
            if (timer >= SeasonDuration)
            {
                timer = 0;
                CurrentSeason = (Season)(((int)CurrentSeason + 1) % 4);
            }
        }
    }
}
