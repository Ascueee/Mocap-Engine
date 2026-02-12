#version 330 core
struct Material {
    sampler2D texture0;
    vec3 specular;
    float shine;
};

struct DirLight {
    vec3 color;
    vec3 dir;
    vec3 ambient;
    vec3 diffuse;
    vec3 specular;
};

#define MAX_DIRLIGHTS 1

out vec4 fragColor;
in vec2 texCoord;
in vec3 normal;
in vec3 fragPos;

uniform Material mat;
uniform DirLight dirLight[MAX_DIRLIGHTS];
uniform vec3 camPos;

vec3 CalcDirLight(DirLight light, vec3 normal, vec3 viewDir);

void main()
{
    vec3 norm = normalize(normal);
    vec3 viewDir = normalize(camPos - fragPos);

    vec3 result = vec3(0.0);
    for(int i = 0; i < MAX_DIRLIGHTS; i++) {
        result += CalcDirLight(dirLight[i], norm, viewDir);
    }

    fragColor = vec4(result, 1.0);
}

vec3 CalcDirLight(DirLight light, vec3 normal, vec3 viewDir) {
    vec3 lightDir = normalize(-light.dir);


    float diff = max(dot(normal, lightDir), 0.0);
    vec3 halfwayDir = normalize(lightDir + viewDir);
    float spec = pow(max(dot(normal, halfwayDir), 0.0), mat.shine);
    
    vec3 texColor = vec3(texture(mat.texture0, texCoord));
    
    vec3 ambient  = light.ambient  * texColor;
    vec3 diffuse  = light.diffuse  * diff * texColor;
    vec3 specular = light.specular * spec * mat.specular;

    return light.color * (ambient + diffuse + specular);
}