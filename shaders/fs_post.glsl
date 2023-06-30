#version 330

// shader inputs
in vec2 uv;						// fragment uv texture coordinates
in vec2 positionFromBottomLeft;
uniform sampler2D pixels;		// input texture (1st pass render target)
uniform vec3 cameraPosition;       
uniform vec3 WhiteCol;

// shader output
out vec3 outputColor;


// fragment shader
void main()
{
	// retrieve input pixel and for Chrome Abberation
	float r = texture(pixels, uv + vec2(0.009f,0.003)).r;
	float g = texture(pixels, uv).g;
	float b = texture(pixels, uv - vec2(0.009f,0)).b;
	outputColor = vec3(r,g,b);

	// apply dummy postprocessing effect
	float dist = length(positionFromBottomLeft);
	float vignetStrength = 2.1f;
	//for 1 - everything to invert it aka vignetting
	outputColor *= 1.0 - vignetStrength * WhiteCol.rgb * dist;

}