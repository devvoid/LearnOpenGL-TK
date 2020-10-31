#version 330 core

layout(location = 0) in vec3 aPosition;

layout(location = 1) in vec2 aTexCoord;

out vec2 texCoord;

// Add a uniform for the transformation matrix.
uniform mat4 transform;

void main(void)
{
    texCoord = aTexCoord;

    // Then all you have to do is multiply the vertices by the transformation matrix, and you'll see your transformation in the scene!
    gl_Position = vec4(aPosition, 1.0) * transform;
}