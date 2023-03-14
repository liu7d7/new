#version 460

layout(location = 0) in vec3 pos;
layout(location = 1) in float width;
layout(location = 2) in vec4 col;

layout(location = 0) uniform mat4 _proj;
layout(location = 3) uniform mat4 _look;

out vec4 v_col;
out noperspective float v_line_width;

void main() {
    v_col = col;
    v_line_width = width;
    gl_Position = _proj * _look * vec4(pos.xyz, 1.0);
}