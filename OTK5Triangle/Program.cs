using System;
using OpenTK.Graphics.OpenGL;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;

namespace OTK5Triangle
{
    class Program
    {
        //Vertex shader source.
        public static readonly string VertexSrc = @"
#version 330 core
layout(location = 0) in vec2 vPosition;

void main()
{
    gl_Position = vec4(vPosition, 0, 1);
}
";

        //Fragment shader source.
        public static readonly string FragmentSrc = @"
#version 330 core
out vec4 Color;

void main()
{
    Color = vec4(0.2f, 0.2f, 0.8f, 1);
}
";

        private static readonly float[] Vertices = new[]
        {
            0f, 0.5f,
            0.5f, 0f,
            -0.5f, 0f,
        };

        private static GameWindow _window;
        
        static void Main(string[] args)
        {
            _window = new GameWindow(GameWindowSettings.Default, NativeWindowSettings.Default);
            
            _window.Load += WindowOnLoad;
            _window.RenderFrame += WindowOnRenderFrame;
            _window.Unload += WindowOnUnload;
            
            _window.Run();
            _window.Dispose();
            Console.ReadKey();
        }

        private static uint _vbo, _vao, _program;

        private static void WindowOnLoad()
        {
            unsafe
            {
                GL.ClearColor(1, 0, 1, 1);

                _vao = GL.CreateVertexArrays();
            
                _vbo = GL.GenBuffers();
                GL.BindBuffer(BufferTargetARB.ArrayBuffer, _vbo);
                GL.BufferData(BufferTargetARB.ArrayBuffer, Vertices, BufferUsageARB.StaticDraw);

                GL.BindVertexArray(_vao);
                GL.VertexAttribPointer(0, 2, VertexAttribPointerType.Float, 0, 2 * sizeof(float), null);
                GL.EnableVertexAttribArray(0);

                uint CreateShader(ShaderType type, string src)
                {
                    uint shader = GL.CreateShader(type);
                    GL.ShaderSource(shader, src);
                    GL.CompileShader(shader);
                    int length = 0;
                    string infoLog = GL.GetShaderInfoLog(shader, 255, ref length);
                    if (!string.IsNullOrWhiteSpace(infoLog))
                    {
                        throw new Exception($"{type} failed to compile: {infoLog}");
                    }

                    return shader;
                }

                uint vertexShader = CreateShader(ShaderType.VertexShader, VertexSrc);
                uint fragmentShader = CreateShader(ShaderType.FragmentShader, FragmentSrc);

                _program = GL.CreateProgram();
                
                GL.AttachShader(_program, vertexShader);
                GL.AttachShader(_program, fragmentShader);
                
                GL.LinkProgram(_program);
                int length = 0;
                string infoLog = GL.GetProgramInfoLog(_program, 255, ref length);
                if (!string.IsNullOrWhiteSpace(infoLog))
                {
                    throw new Exception($"Program failed to link {infoLog}");
                }
                
                GL.DetachShader(_program, vertexShader);
                GL.DetachShader(_program, fragmentShader);
                GL.DeleteShader(vertexShader);
                GL.DeleteShader(fragmentShader);
            }
        }

        private static void WindowOnRenderFrame(FrameEventArgs obj)
        {
            GL.Clear(ClearBufferMask.ColorBufferBit);
            
            GL.UseProgram(_program);
            GL.BindVertexArray(_vao);
            GL.DrawArrays(PrimitiveType.Triangles, 0, 3);
            
            _window.SwapBuffers();
        }

        private static void WindowOnUnload()
        {
            GL.DeleteProgram(_program);
            GL.DeleteVertexArrays(1, ref _vao);
            GL.DeleteBuffers(1, ref _vbo);
        }
    }
}