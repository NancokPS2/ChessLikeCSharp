extends Node
class_name ASS

const scene: PackedScene = preload("res://Test.tscn")
const scrip: Script = ASS

@export var player: Node


func _ready():
	var light := DirectionalLight3D.new()
	light.light_specular

func load_file(path: String):
	var config := ConfigFile.new()
	if FileAccess.file_exists(path):
		config.save(path)
		breakpoint
	else:
		config.load(path)
	
