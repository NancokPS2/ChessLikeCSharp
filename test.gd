##Item.gd
extends Node2D


func _physics_process(delta):
	var raycast: RayCast2D = $RayCastChild

	
	var distance_to_floor: float = raycast.position.distance_to(raycast.get_collision_point()) 
	position += Vector2.DOWN * distance_to_floor
	
	var fifteen: int = 15
	var fifteen_str: String = fifteen as String
	var first_digit: int = fifteen_str[0] as int



class Upgrade extends Resource:
	pass
@export var player: Node

func load_file(path: String):
	var config := ConfigFile.new()
	if FileAccess.file_exists(path):
		config.save(path)
		breakpoint
	else:
		config.load(path)
	
