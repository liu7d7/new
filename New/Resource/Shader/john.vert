#version 430 core

layout(location = 0) in vec3 pos;
layout(location = 1) in vec4 color;
layout(location = 2) in vec2 texCoords;

uniform mat4 _proj;
uniform mat4 _look;
uniform int _rendering3d;

out vec4 v_Color;
out vec2 v_TexCoords;

void main() {
  vec4 final = _proj * _look * vec4(pos, 1.0);
  gl_Position = final;
  v_TexCoords = texCoords;
  v_Color = color;
}