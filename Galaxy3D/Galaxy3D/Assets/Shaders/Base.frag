#version 330
struct Material {
    vec3 color;
};

out vec4 fragColor;
in vec2 texCoord;

uniform Material mat;
uniform sampler2D texture0;

void main()
{
    fragColor = texture(texture0, texCoord) * vec4(mat.color, 1.0);
}