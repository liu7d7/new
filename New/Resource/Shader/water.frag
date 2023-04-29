#version 460

in vec3 v_pos;

out vec4 f_color;

uniform float _time;
uniform sampler2D _noise;
uniform sampler2D _distortion;

const float PI = 3.1415926535897932384626433832795;

const float surfaceNoiseCutoff = 0.77;
const vec4 shallow = vec4(1.0, 0.0, 0.0, 1.0);
const vec4 deep = vec4(0.0, 1.0, 0.0, 1.0);

vec4 get() {
  float depth = -v_pos.y;
  vec2 uv = v_pos.xz / 120.;
  vec2 distortion = texture(_distortion, uv + _time * 0.03).rg * .25;
  float noise = texture(_noise, uv + distortion).r;
  
  if (noise > surfaceNoiseCutoff * depth * 1.1) {
    return shallow;
  } else {
    return deep;
  }
}

void main() {
  f_color = get();
}