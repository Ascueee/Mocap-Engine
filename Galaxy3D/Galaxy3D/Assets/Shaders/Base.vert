#version 330
layout(location = 0) in vec3 pos;
layout(location = 1) in vec2 texCoords;
layout(location = 2) in vec3 normals;
layout(location = 3) in vec4 boneIds;
layout(location = 4) in vec4 boneWeights;

uniform mat4 model;
uniform mat4 view;
uniform mat4 projection;

out vec2 texCoord;
out vec3 normal;
void main() {
    texCoord = texCoords; 
    gl_Position = projection * view * model * vec4(pos, 1.0);
}