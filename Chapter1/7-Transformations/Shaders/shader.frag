#version 330

out vec4 outputColor;

in vec2 texCoord;

uniform sampler2D texture0;
uniform sampler2D texture1;

void main()
{
    outputColor = mix(texture(texture0, texCoord), texture(texture1, texCoord), 0.2);
}