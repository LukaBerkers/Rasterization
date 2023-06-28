#version 330
 
// shader inputs
in vec4 positionWorld;              // fragment position in World Space
in vec4 normalWorld;                // fragment normal in World Space
in vec2 uv;                         // fragment uv texture coordinates
uniform sampler2D diffuseTexture;	// texture sampler
uniform vec4 lightColor;            // for the ambient light color?
uniform vec3 lightPosition;          // world space
uniform vec4 specularLight;
uniform vec3 cameraPosition;       
uniform vec3 viewD;


// shader output
out vec4 outputColor;

// fragment shader
void main()
{
    vec3 L = lightPosition - positionWorld.xyz; 
    float attenuation = 1.0 / dot(L, L);
    float NdotL = max(0, dot(normalize(normalWorld.xyz), normalize(L))); 
    vec3 diffuseColor = texture(diffuseTexture, uv).rgb; 

    //specular light
    vec3 R = lightPosition - 2*dot(lightPosition, normalWorld.xyz) * normalWorld.xyz;
    float VdotR = pow(max(dot(normalize(viewD), normalize(R)),0),25);
    float specualarStrenght = 0.005f;
    //vec3 specularColor = lightColor.rgb * specularLight.rgb * specualarStrenght;
    //outputColor = vec4(lightColor.rgb * diffuseColor * attenuation * NdotL, 1.0) + (VdotR * specualarStrenght * specularColor, 1 );

    //phong illumination
    outputColor = vec4(lightColor.rgb * diffuseColor * attenuation * NdotL, 1.0) + (VdotR * lightColor * specualarStrenght);

    

}