#version 330
layout(location = 0) in vec3 pos;
layout(location = 1) in vec2 texCoords;

uniform mat4 model;
uniform mat4 view;
uniform mat4 projection;

out vec2 texCoord;
void main() {
    texCoord = texCoords; 
    gl_Position = projection * view * model * vec4(pos, 1.0);
}