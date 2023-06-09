﻿#version 460

layout(location = 0) in vec3 aPos;
layout(location = 1) in vec4 color;

out vec4 vtColor;

uniform mat4 _proj;
uniform mat4 _look;

void main() {
  gl_Position = _proj * _look * vec4(aPos, 1.0);
  vtColor = color;
}