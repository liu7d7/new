#version 460

in vec3 pos;
in vec4 col;

out vec4 color;

uniform mat4 _proj;
uniform mat4 _look;

void main() {
  gl_Position = _proj * _look * vec4(pos, 1.0);
  color = col;
}