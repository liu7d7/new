#version 430 core

in vec2 v_TexCoords;
in vec2 v_Pos;

uniform sampler2D _tex0;
uniform sampler2D _tex1;
uniform float _width;
uniform float _threshold;
uniform float _depthThreshold;
uniform vec4 _outlineColor;
uniform int _glow;
uniform vec4 _otherColor;
uniform vec2 _screenSize;

out vec4 fragColor;

float linearize_depth(float d, float zNear, float zFar) {
  return zNear * zFar / (zFar + d * (zNear - zFar));
}

float depthAt(vec2 pos) {
  return linearize_depth(texture(_tex1, pos).r, 0.1, 38.4);
}

const float sqrt2 = 1.0 / sqrt(2.);
float diag = _width * sqrt2;
vec2 oneTexel = 1. / _screenSize;

const vec2 corners[8] = {
  vec2(-diag, -diag),
  vec2(diag, -diag),
  vec2(-diag, diag),
  vec2(diag, diag),
  vec2(-_width, 0),
  vec2(_width, 0),
  vec2(0, -_width),
  vec2(0, _width)
};

bool shouldOutline(vec2 pos, vec4 center, float depth) {
  float diff;
  for (int i = 0; i < 8; i++) {
    vec2 corner = (corners[i] + pos) * oneTexel;
    vec4 col = texture(_tex0, corner);
    diff = abs(center.r - col.r) + abs(center.g - col.g) + abs(center.b - col.b);
    
    if (diff > _threshold || abs(depth - depthAt(corner)) > _depthThreshold) {
      return true;
    }
  }
  return false;
}

void main() {
  vec4 center = texture(_tex0, v_TexCoords);
  bool o = shouldOutline(v_Pos, center, depthAt(v_TexCoords));
  if (o) {
    fragColor = _outlineColor;
  } else {
    fragColor = _otherColor;
  }
}