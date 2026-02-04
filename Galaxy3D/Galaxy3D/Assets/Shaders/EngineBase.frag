#version 330
#define MAX_DIRLIGHTS 6
#define MAX_POINTLIGHTS 1
struct Material {
    vec3 ambient;
    vec3 diffuse;
    vec3 specular;
    
    float shine;
};
struct PointLight{
    vec3 position;
    vec3 color;

    float constant;
    float linear;
    float quadratic;

    vec3 ambient;
    vec3 diffuse;
    vec3 specular;
};


struct DirLight {
    vec3 color;
    vec3 dir;
    vec3 ambient;
    vec3 diffuse;
    vec3 specular;
};

out vec4 fragColor;
in vec2 texCoord;
in vec3 normal;
in vec3 fragPos;

uniform Material mat;
uniform DirLight dirLights[MAX_DIRLIGHTS];
uniform PointLight pointLights[MAX_POINTLIGHTS];

uniform vec3 camPos;
uniform sampler2D texture0;

uniform int amountDirLights;
uniform int amountPointLights;

vec3 CalcDirLight(DirLight light, vec3 normal, vec3 viewDir);
vec3 CalcPointLights(PointLight light, vec3 normal, vec3 fragPos, vec3 viewDir);

void main()
{
    vec3 norm = normalize(normal);
    vec3 viewDir = normalize(camPos - fragPos);
    vec3 lightingResult = vec3(0.0f);
    vec3 baseAmbient = mat.ambient * 0.2; 
    
    for(int i = 0; i < amountDirLights; i++){
        lightingResult += CalcDirLight(dirLights[i], norm, viewDir);
    }
    
    for(int i = 0; i < amountPointLights; i++){
        lightingResult += CalcPointLights(pointLights[i], norm, fragPos, viewDir);
    }

    vec4 texColor = texture(texture0, texCoord);
    //fragColor = vec4(texColor.rgb * lightingResult, texColor.a);
    //fragColor = vec4(texColor.rgb * (baseAmbient + lightingResult), texColor.a);
    fragColor = vec4(lightingResult, 1.0);
    
//    vec3 n = normalize(normal);
//    fragColor = vec4(n * 0.5 + 0.5, 1.0);
}

vec3 CalcDirLight(DirLight light, vec3 normal, vec3 viewDir){
    
    vec3 lightDir = normalize(-light.dir);
    float diff = max(dot(normal, lightDir), 0.0);
    vec3 reflectDir = reflect(-lightDir, normal);
    float spec = pow(max(dot(viewDir, reflectDir), 0.0), mat.shine);

    vec3 ambient  = light.color * light.ambient * mat.ambient;
    vec3 diffuse  = light.color * light.diffuse * diff * mat.diffuse;
    vec3 specular = light.color * light.specular * spec * mat.specular;
    return (ambient + diffuse + specular);
}


vec3 CalcPointLights(PointLight light, vec3 normal, vec3 fragPos, vec3 viewDir){
    
    vec3 lightDir = normalize(light.position - fragPos);
    float diff = max(dot(normal, lightDir), 0.0);
    //specular
    vec3 reflectDir = reflect(-lightDir, normal);
    float spec = pow(max(dot(viewDir, reflectDir), 0.0), mat.shine);
    //attenuation to lessen the light as objects are farther
    float distance    = length(light.position - fragPos);
    float attenuation = 1.0 / (light.constant + light.linear * distance +
    light.quadratic * (distance * distance));
    
    vec3 ambient  = light.color * light.ambient * mat.ambient;
    vec3 diffuse  = light.color * light.diffuse * diff * mat.diffuse;
    vec3 specular = light.color * light.specular * spec * mat.specular;
    
    ambient  *= attenuation;
    diffuse  *= attenuation;
    specular *= attenuation;
    return (ambient + diffuse + specular);
}