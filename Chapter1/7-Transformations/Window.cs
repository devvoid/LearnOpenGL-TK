using System;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Input;
using LearnOpenTK.Common;

namespace LearnOpenTK
{
    // So you can setup OpenGL, you can draw basic shapes without wasting vertices, and you can texture them.
    // There's one big thing left, though: moving the shapes.
    // To do this, we use linear algebra to move the vertices in the vertex shader.

    // Just as a disclaimer: this tutorial will NOT explain linear algebra or matrices; those topics are wayyyyy too complex to do with comments.
    // If you want a more detailed understanding of what's going on here, look at the web version of this tutorial instead.
    // A deep understanding of linear algebra won't be necessary for this tutorial as OpenTK includes built-in matrix types that abstract over the actual math.

    // Head down to RenderFrame to see how we can apply transformations to our shape.
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
            _texture.Use();

            _texture2 = new Texture("Resources/awesomeface.png");
            _texture2.Use(TextureUnit.Texture1);

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

            // Note: The matrices we'll use for transformations are all 4x4.

            // We start with an identity matrix. This is just a simple matrix that doesn't move the vertices at all.
            var transform = Matrix4.Identity;

            // The next few steps just show how to use OpenTK's matrix functions, and aren't necessary for the transform matrix to actually work.
            // If you want, you can just pass the identity matrix to the shader, though it won't affect the vertices at all.

            // To combine two matrices, you multiply them. Here, we combine the transform matrix with another one created by OpenTK to rotate it by 20 degrees.
            // Note that all Matrix4.CreateRotation functions take radians, not degrees. Use MathHelper.DegreesToRadians() to convert to radians, if you want to use degrees.
            transform *= Matrix4.CreateRotationZ(MathHelper.DegreesToRadians(20f));

            // Next, we scale the matrix. This will make the rectangle slightly larger.
            transform *= Matrix4.CreateScale(1.1f);

            // Then, we translate the matrix, which will move it slightly towards the top-right.
            // Note that we aren't using a full coordinate system yet, so the translation is in normalized device coordinates.
            // The next tutorial will be about how to set one up so we can use more human-readable numbers.
            transform *= Matrix4.CreateTranslation(0.1f, 0.1f, 0.0f);

            _texture.Use();
            _texture2.Use(TextureUnit.Texture1);
            _shader.Use();

            // Now that the matrix is finished, pass it to the vertex shader.
            // Go over to shader.vert to see how we finally apply this to the vertices
            _shader.SetMatrix4("transform", transform);

            // And that's it for now! In the next tutorial, I'll show you how to setup a full coordinates system.

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