extends Node
class_name ASS

const scene: PackedScene = preload("res://Test.tscn")
const scrip: Script = ASS

var player: Node


func load_file(path: String):
	var config := ConfigFile.new()
	if FileAccess.file_exists(path):
		config.save(path)
		breakpoint
	else:
		config.load(path)
	
