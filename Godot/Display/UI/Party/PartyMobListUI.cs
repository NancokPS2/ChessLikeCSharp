using ChessLike.Entity;
using ChessLike.Extension;
using Godot;
using System;

public partial class PartyMobListUI : BaseButtonMenu<PartyMobListUI.MobTooltipButton, Mob>, ISceneDependency
{
	public Mob? MobSelected;

	public Godot.Vector4 SelectBorderColor = new(0,1,0,0.6f);

	public string SCENE_PATH { get; } = "res://Godot/Display/UI/Party/PartyMobListUI.tscn";

	[Export]
	public Label NodeFactionNameLabel;

	public PartyMobListUI() : base()
	{
		ButtonVerticalFlags = SizeFlags.ExpandFill;
	}

	public override void _Ready()
	{
		base._Ready();
		Container ??= (Control)FindChild("PortraitContainer");
		NodeFactionNameLabel ??= (Label)FindChild("FactionNameLabel");
	}

	public void Update(EFaction faction)
	{
		var list = Global.ManagerMob.GetFromFaction(faction);
		NodeFactionNameLabel.Text = $"Faction: {faction}";
		Update(list);
	}

	public void Update(List<EFaction> factions)
	{
		List<Mob> list = new();
		foreach (EFaction fac in factions)
		{
			list.AddRange(Global.ManagerMob.GetFromFaction(fac));
		}
		NodeFactionNameLabel.Text = $"Factions: {factions.ToStringList()}";
		Update(list);
	}

	protected override void OnButtonCreated(MobTooltipButton button, Mob param)
	{
		button.MobReference = param;
		button.Text = param.ToString();
		button.Material = (ShaderMaterial)Global.Readonly.SHADER_BORDER_CANVAS.Duplicate();
		(button.Material as ShaderMaterial)?.SetShaderParameter("border_color", Colors.Transparent);
	}

	protected override void OnButtonPressed(MobTooltipButton button, Mob param)
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

	protected override void OnButtonHovered(MobTooltipButton button, Mob param, bool hovered)
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

	public partial class MobTooltipButton : Button, ITooltip
	{
		public Mob? MobReference;

        public MobTooltipButton()
        {
			MobReference = null;
        }

        public MobTooltipButton(Mob mobReference)
        {
            MobReference = mobReference;
        }

        string ITooltip.GetText() => MobReference?.ToStringStats() ?? "UNDEFINED";

		Godot.Vector2 ITooltip.GetRectSize() => new (240,100);
	}

}
