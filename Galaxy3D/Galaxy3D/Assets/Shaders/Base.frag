#version 330
out vec4 fragColor;
in vec2 texCoord;
uniform vec4 color;

uniform sampler2D texture0;

void main()
{
    
    fragColor = texture(texture0, texCoord) * color;
}