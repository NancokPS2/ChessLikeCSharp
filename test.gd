extends Node

var player: Node


func load_file(path: String):
	var config := ConfigFile.new()
	if FileAccess.file_exists(path):
		config.save(path)
	else:
		config.load(path)
	
