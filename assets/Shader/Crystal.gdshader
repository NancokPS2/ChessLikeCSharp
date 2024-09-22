shader_type spatial;
#include "Includes/Fresnel.gdshaderinc"

uniform float fresnel_intensity : hint_range(0,2) = 0.5;
uniform vec3 base_color = vec3(1,1,1);

void fragment() {
	ALBEDO = base_color.rgb;
	float basic_fresnel = fresnel(fresnel_intensity, NORMAL, VIEW);
	ALPHA = basic_fresnel;
}

//void light() {
	// Called for every pixel for every light affecting the material.
	// Uncomment to replace the default light processing function with this one.
//}
