extends Node
class_name ASS

const scene: PackedScene = preload("res://Test.tscn")
const scrip: Script = ASS

@export var player: Node

func load_file(path: String):
	var config := ConfigFile.new()
	if FileAccess.file_exists(path):
		config.save(path)
		breakpoint
	else:
		config.load(path)
	
signal pressed
	
func _ready():
	var node: Node = get_parent()
	pressed.connect(node.draw)
	
