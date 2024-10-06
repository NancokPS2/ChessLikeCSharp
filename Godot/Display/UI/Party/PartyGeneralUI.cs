using Godot;
using System;

public partial class PartyGeneralUI : Control, ISceneDependency
{
    public string SCENE_PATH { get; } = "res://Godot/Display/UI/Party/PartyGeneralUI.tscn";

    [Export]
    public PartyMobListUI? NodePartyListUI;

	public void Update()
	{

	}
}
