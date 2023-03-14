﻿#version 460

in vec2 Position;

uniform vec2 _screenSize;

uniform float SubPixelShift;// 0.25

out vec2 texCoord;
out vec4 posPos;

void main() {
    gl_Position = vec4(Position, -0.2, 1.0);

    texCoord = Position * 0.5 + 0.5;
    posPos.xy = texCoord.xy;
    posPos.zw = texCoord.xy - (1.0 / _screenSize * vec2(0.5 + SubPixelShift));
}
