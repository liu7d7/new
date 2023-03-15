#version 460

layout(location = 0) in vec3 aPos;

out vec4 vtColor;

uniform mat4 _proj;
uniform mat4 _look;

void main() {
  gl_Position = _proj * _look * vec4(aPos, 1.0);
  vtColor = vec4(vec3(0.5), 1.0);
}