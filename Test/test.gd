extends Label
class_name DialogueDisplay

@export var audio_stream_player: AudioStreamPlayer

@export var dialogues: Array[DialogueLine]

var dialogue_index: int = 0

func show_dialogue():
	#Simple failsafe for when there's no dialogues
	if dialogues.is_empty(): return
	
	var current: DialogueLine = dialogues[dialogue_index]
	text = current.text
	audio_stream_player.steam = current.sfx
	
	
func next_dialogue():
	dialogue_index += 1
	if dialogue_index >= dialogues.size():
		dialogue_index = 0
