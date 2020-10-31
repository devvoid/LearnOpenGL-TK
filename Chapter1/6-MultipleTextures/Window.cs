using System;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Input;
using LearnOpenTK.Common;

namespace LearnOpenTK
{
    public class Window : GameWindow
    {
        private readonly float[] _vertices =
        {
            // Position         Texture coordinates
             0.5f,  0.5f, 0.0f, 1.0f, 1.0f, // top right
             0.5f, -0.5f, 0.0f, 1.0f, 0.0f, // bottom right
            -0.5f, -0.5f, 0.0f, 0.0f, 0.0f, // bottom left
            -0.5f,  0.5f, 0.0f, 0.0f, 1.0f  // top left
        };

        private readonly uint[] _indices =
        {
            0, 1, 3,
            1, 2, 3
        };

        private int _elementBufferObject;

        private int _vertexBufferObject;

        private int _vertexArrayObject;

        private Shader _shader;

        private Texture _texture;

        private Texture _texture2;

        public Window(int width, int height, string title)
            : base(width, height, GraphicsMode.Default, title)
        {
        }

        protected override void OnLoad(EventArgs e)
        {
            GL.ClearColor(0.2f, 0.3f, 0.3f, 1.0f);

            _vertexBufferObject = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, _vertexBufferObject);
            GL.BufferData(BufferTarget.ArrayBuffer, _vertices.Length * sizeof(float), _vertices, BufferUsageHint.StaticDraw);

            _elementBufferObject = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, _elementBufferObject);
            GL.BufferData(BufferTarget.ElementArrayBuffer, _indices.Length * sizeof(uint), _indices, BufferUsageHint.StaticDraw);

            // shader.frag has been modified yet again, take a look at it as well.
            _shader = new Shader("Shaders/shader.vert", "Shaders/shader.frag");
            _shader.Use();

            _texture = new Texture("Resources/container.png");
            // Texture units are explained in Texture.cs, at the Use function.
            // Texture.Use will implicitly fill in Texture0 if you pass no arguments.
            // First texture goes in texture unit 0.
            _texture.Use();

            // This is helpful because System.Drawing reads the pixels differently than OpenGL expects
            _texture2 = new Texture("Resources/awesomeface.png");
            // Then, the second goes in texture unit 1.
            _texture2.Use(TextureUnit.Texture1);

            // Next, we must setup the samplers in the shaders to use the right textures.
            // The int we send to the uniform is which texture unit the sampler should use.
            _shader.SetInt("texture0", 0);
            _shader.SetInt("texture1", 1);

            _vertexArrayObject = GL.GenVertexArray();
            GL.BindVertexArray(_vertexArrayObject);

            GL.BindBuffer(BufferTarget.ArrayBuffer, _vertexArrayObject);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, _elementBufferObject);

            var vertexLocation = _shader.GetAttribLocation("aPosition");
            GL.EnableVertexAttribArray(vertexLocation);
            GL.VertexAttribPointer(vertexLocation, 3, VertexAttribPointerType.Float, false, 5 * sizeof(float), 0);

            var texCoordLocation = _shader.GetAttribLocation("aTexCoord");
            GL.EnableVertexAttribArray(texCoordLocation);
            GL.VertexAttribPointer(texCoordLocation, 2, VertexAttribPointerType.Float, false, 5 * sizeof(float), 3 * sizeof(float));

            base.OnLoad(e);
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            GL.Clear(ClearBufferMask.ColorBufferBit);

            GL.BindVertexArray(_vertexArrayObject);

            _texture.Use();
            _texture2.Use(TextureUnit.Texture1);
            _shader.Use();

            GL.DrawElements(PrimitiveType.Triangles, _indices.Length, DrawElementsType.UnsignedInt, 0);

            SwapBuffers();

            base.OnRenderFrame(e);
        }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            var input = Keyboard.GetState();

            if (input.IsKeyDown(Key.Escape))
            {
                Exit();
            }

            base.OnUpdateFrame(e);
        }

        protected override void OnResize(EventArgs e)
        {
            GL.Viewport(0, 0, Width, Height);
            base.OnResize(e);
        }

        protected override void OnUnload(EventArgs e)
        {
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.BindVertexArray(0);
            GL.UseProgram(0);

            GL.DeleteBuffer(_vertexBufferObject);
            GL.DeleteVertexArray(_vertexArrayObject);

            GL.DeleteProgram(_shader.Handle);
            GL.DeleteTexture(_texture.Handle);
            GL.DeleteTexture(_texture2.Handle);
            base.OnUnload(e);
        }
    }
}