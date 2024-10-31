shader_type canvas_item;
render_mode blend_mix;
#include "Includes/Canvas.gdshaderinc"

uniform float border_width : hint_range(0, 0.5) = 0.1f;

uniform vec4 border_color : source_color = vec4(0,1,0,0.65);

void fragment() {
	COLOR.rgb += (border_color.rgb * border_color.a) * border_invert(UV, border_width);
}