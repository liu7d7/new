#version 460

in vec2 v_TexCoords;
in vec2 v_Pos;

out vec4 fragColor;

uniform sampler2D u_Texture;

void main()
{
  fragColor = texture(u_Texture, v_TexCoords);
}