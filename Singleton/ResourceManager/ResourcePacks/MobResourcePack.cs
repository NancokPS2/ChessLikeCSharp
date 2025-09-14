using System;
using ChessLike.Entity;
using Godot;

public class MobResourcePack : ResourcePack<Mob>
{
	public MobResourcePack() : base()
	{
		ContentLoaded += OnContentLoaded;
	}

	public override Mob ResourceGet(string identifier, bool getBase = false, bool duplicate = true)
	{
		Mob output = base.ResourceGet(identifier, getBase, duplicate);
		output.ResetForCombat();
		return output;
	}

	#region Event Handling
	public void OnContentLoaded(object? sender, EventArgs e)
	{
		foreach (var item in ContentRuntime.Values)
		{
			item.ResetForCombat();
		}
	}
	#endregion


    public Mob GetResource(EPackIDMob enu)
	{
		string? output = Enum.GetName<EPackIDMob>(enu);
		return ResourceGet(output ?? throw new Exception("Could not get name from enum."));
	}

    [Obsolete("This is not safe enough.")]
    public override string GetResourceIdentifier(Mob resource)
    {
        string output = base.GetResourceIdentifier(resource);
        if (output == "") output = resource.DisplayedName;
        return output;
    }

}