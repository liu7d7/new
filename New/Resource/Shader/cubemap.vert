#version 460
layout (location = 0) in vec3 aPos;
layout (location = 1) in vec2 texCoord;

out vec2 TexCoords;

uniform mat4 _proj;
uniform mat4 _look;

void main()
{
    TexCoords = texCoord;
    gl_Position = _proj * _look * vec4(aPos, 1.0);
}