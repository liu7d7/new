#version 430 core

layout(location = 0) in vec3 pos;
layout(location = 1) in vec4 color;

out vec4 v_Color;
out vec2 v_TexCoords;

#define MAX 1024

layout(binding = 0, packed) uniform _instanceInfo {
  mat4 _model[MAX];
};
uniform mat4 _proj;
uniform mat4 _look;

void main() {
  mat4 model = _model[gl_InstanceID];
  vec4 final = model * vec4(pos, 1.0);
  gl_Position = _proj * _look * final;
  v_Color = color;
}