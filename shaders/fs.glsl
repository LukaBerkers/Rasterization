#version 330
 
// shader inputs
in vec4 positionWorld;              // fragment position in World Space
in vec4 normalWorld;                // fragment normal in World Space
in vec2 uv;                         // fragment uv texture coordinates
uniform sampler2D diffuseTexture;	// texture sampler
uniform vec4 lightColor;            // for the ambient light color?
uniform vec3 lightPosition;          // world space
uniform vec3 cameraPosition;       

out vec3 fragPos;

// shader output
out vec4 outputColor;

// fragment shader
void main()
{
    //outputColor = texture(diffuseTexture, uv) * lightColor + 0.5 * normalWorld;
    vec3 L = lightPosition - positionWorld.xyz; 
    float attenuation = 1.0 / dot(L, L);
    float NdotL = max(0, dot(normalize(normalWorld.xyz), normalize(L))); 
    vec3 diffuseColor = texture(diffuseTexture, uv).rgb; 
    outputColor = vec4(lightColor.rgb * diffuseColor * attenuation * NdotL, 1.0);

    float specularStrength = 0.5f;

/*    // Calculate lighting for pointlights
   vec3 CalcPointLight(vec3 lightPosition, vec3 normalWorld, vec4 positionWorld, vec3 cameraPosition, vec4 lightColor) 
   {
    
        // Calculate the colour using phong illumination

        // Diffuse component calculation
        vec3 norm = normalize(normalWorld);
        vec3 lightDir = normalize(lightPosition - positionWorld.xyz);

        float diff = max(dot(norm, lightDir), 0.0);
        vec3 diffuse = diff * ObjectMaterial.diffuse * light.diffuse;

        // Specular component calculation

        ...


        // Calculate attenuation
        float distance = length(lightPosition - positionWorld.xyz);
        float attenuation = 1.0 / dot(distance, distance);

        diffuse *= attenuation;

        return (diffuse + specular); 
    }*/
}