#version 460

layout(location = 0) in vec3 aPos;
layout(location = 1) in vec4 color;

out vec4 vtColor;

uniform mat4 _proj;
uniform mat4 _look;

#define MAX 1024

layout(binding = 0, packed) uniform _instanceInfo {
  vec4 _translation[MAX];
};

void main() {
  vtColor = color;
  gl_Position = _proj * _look * vec4(aPos + _translation[gl_InstanceID].xyz, 1.0);
}