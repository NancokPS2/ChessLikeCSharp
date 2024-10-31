using ChessLike.Entity;
using Godot;
using System;

public partial class PartyMobListUI : BaseButtonMenu<Button, Mob>, ISceneDependency
{
	public Mob? MobSelected;

    public Godot.Vector4 SelectBorderColor = new(0,1,0,0.6f);

    public string SCENE_PATH { get; } = "res://Godot/Display/UI/Party/PartyMobListUI.tscn";

    public PartyMobListUI() : base()
    {
        ButtonVerticalFlags = SizeFlags.ExpandFill;
    }

    public override void _Ready()
    {
        base._Ready();
		Container ??= (Control)FindChild("PortraitContainer");
    }

    public void Update(EFaction faction)
	{
		var list = Global.ManagerMob.GetFromFaction(faction);
		Update(list);
	}

    public void Update(List<EFaction> factions)
    {
        List<Mob> list = new();
        foreach (EFaction fac in factions)
        {
            list.AddRange(Global.ManagerMob.GetFromFaction(fac));
        }
        Update(list);
    }

    protected override void OnButtonCreated(Button button, Mob param)
    {
		button.Text = param.DisplayedName;
        button.Material = (Material)Global.Readonly.SHADER_BORDER_CANVAS.Duplicate();
    }

    protected override void OnButtonPressed(Button button, Mob param)
    {
        MobSelected = param;     
        foreach (var item in ButtonInstances)
        {
            if (item.NodeReference.Material is ShaderMaterial other_shader)
            {
                other_shader.SetShaderParameter("border_color", Godot.Colors.Transparent);
            }
        }
        if (button.Material is ShaderMaterial shader)
        {
            shader.SetShaderParameter("border_color", SelectBorderColor);
        }

        base.OnButtonPressed(button, param);
    }

    protected override void OnButtonHovered(Button button, Mob param, bool hovered)
    {
        base.OnButtonHovered(button, param, hovered);
        //Do not affect the modulate if this is the selected button.
        if (param == MobSelected){return;}

        if (hovered)
		{
			button.Modulate = Godot.Colors.White * 0.5f;
		}
		else
		{
			button.Modulate = Godot.Colors.White;
		}
    }

}
