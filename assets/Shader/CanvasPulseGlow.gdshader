shader_type canvas_item;
render_mode blend_mix;
#include "Includes/Time.gdshaderinc"
#include "Includes/UVConsts.gdshaderinc"

uniform float border_width : hint_range(0, 0.5) = 0.5f;

uniform vec4 border_color : source_color = vec4(0,1,0,0.65);

void fragment() {
	vec4 color = border_color;
	color *= (get_milis_sin());
	
	if(UV.x > 0.5)
	{ 
		color *= 1.0 - distance(UV.x, 0.5 + border_width);
	}
	else
	{
		color *= distance(UV.x, 0.5 - border_width);
	}
	COLOR += color;
}

