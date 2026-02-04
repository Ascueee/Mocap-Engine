#version 330
//Going to be the shader for bling-phong lighting(this lighting model will use lighting textures and different 
//lightSources Dir, Spotlight, pointlights each light type will serve a different lighting type 
layout(location = 0) in vec3 pos;
layout(location = 1) in vec2 texCoords;
layout(location = 2) in vec3 normals;

uniform mat4 model;
uniform mat4 view;
uniform mat4 projection;

out vec2 texCoord;
out vec3 normal;
out vec3 fragPos;

void main() {
    texCoord = texCoords;
    normal = normalize(mat3(transpose(inverse(model))) * normals);
    fragPos = vec3(model * vec4(pos, 1.0));
    gl_Position = projection * view * model * vec4(pos, 1.0);
}