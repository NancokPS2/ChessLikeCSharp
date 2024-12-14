using ChessLike.Entity;
using ExtendedXmlSerializer;
using Godot;
using System;

[GlobalClass]
public partial class MobStatsUI : Control, ISceneDependency
{
    public string SCENE_PATH { get; } = "res://Godot/Display/UI/Mob/MobStatsUI.tscn";


	[Export]
	Control? NodeStatContainer;

    public override void _Ready()
    {
        base._Ready();
		NodeStatContainer ??= (Control?)FindChild("StatsContainer");
    }

    public void Update(Mob mob)
	{
		if(NodeStatContainer is null) {throw new Exception("Null NodeStatContainer");}

		NodeStatContainer.FreeChildren();
		foreach (var item in mob.Stats.GetMaxStatDictionary().Keys)
		{
			float current = mob.Stats.GetValue(item);
			float max = mob.Stats.GetMax(item);

			string text = item.ToString() + ": ";
			if (current == max){text += max.ToString();}
			else {text += current.ToString() + "/" + max.ToString();}

			Label label = new(){Text = text, SizeFlagsHorizontal = SizeFlags.ExpandFill};
			NodeStatContainer.AddChild(label);

		}

	}
}
