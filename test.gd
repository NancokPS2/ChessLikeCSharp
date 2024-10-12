extends Button
class_name UpgradeButton



func _physics_process(delta):
	var raycast: RayCast2D = $RayCastChild
	
	var distance_to_floor: float = raycast.position.distance_to(raycast.get_collision_point()) 
	position += Vector2.DOWN * distance_to_floor
	
	var fifteen: int = 15
	var fifteen_str: String = fifteen as String
	var first_digit: int = fifteen_str[0] as int







signal upgrade_selected(upgrade: Upgrade)

@export var stored_upgrade: Upgrade

func _init():
	pressed.connect(emit_upgrade)

func emit_upgrade():
	upgrade_selected.emit(stored_upgrade)


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
	
	
func _ready():
	var node: Node = get_parent()
	pressed.connect(node.draw)
	
