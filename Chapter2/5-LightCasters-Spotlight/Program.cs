﻿using OpenTK.Mathematics;
using OpenTK.Windowing.Desktop;

namespace LearnOpenTK
{
    public static class Program
    {
        private static void Main()
        {
            var nativeWindowSettings = new NativeWindowSettings()
            {
                Size = new Vector2i(800, 600),
                Title = "LearnOpenTK - Light casters - spotlight",
            };

            using var window = new Window(GameWindowSettings.Default, nativeWindowSettings);
            window.Run();
        }
    }
}
