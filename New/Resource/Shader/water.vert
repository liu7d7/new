#version 460

in vec3 position;
in vec4 color;

out vec3 v_pos;

uniform mat4 _proj;
uniform mat4 _look;

void main() {
  gl_Position = _proj * _look * vec4(position.x, 0., position.z, 1.0);
  v_pos = position;
}