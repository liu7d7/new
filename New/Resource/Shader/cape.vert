#version 460

layout (location = 0) in vec3 tier;

out vec4 vtColor;

uniform mat4 _proj;
uniform mat4 _look;
uniform vec4 _color;
uniform vec3 _translation;
uniform float _time;
uniform float _yaw;

const float segments = 16.;
const float inc = 40. / segments;
const float rad = 0.33f;

float curve(float f) {
  return pow(f, 1.55);
}

float f(float x) {
  return (sin(x) + sin(2. * x)) / 2.5;
}

void main() {
  vec3 final = vec3(cos(radians(tier.x * inc + _yaw)) * rad, tier.y, sin(radians(tier.x * inc + _yaw)) * rad);
  float x = (tier.x * 0.25 + _time * (sin(_time) * 0.5 + 4.));

  float mul = f(x) * 0.25 + 1.25;
  float h = 5.33;
  final.xz *= abs(curve((tier.y - h) / h) * 10.) * mul + 0.45;

  float muly = f(x) * 0.375 + 1.;
  final.y += abs(curve((tier.y - h) / h) * 3.) * muly;

  final += _translation;
  gl_Position = _proj * _look * vec4(final, 1.);
  vtColor = vec4(_color.rgb + tier.x * (h - tier.y) / h * 0.03, 1.);
}