using ChessLike.Entity;
using ChessLike.Extension;
using Godot;
using System;

[GlobalClass]
public partial class MobListUI : BaseButtonMenu<MobListUI.MobTooltipButton, Mob>, ISceneDependency
{
	[Export]
	public EFaction FactionAssigned;

	public Mob? MobSelected;

	public Godot.Vector4 SelectBorderColor = new(0, 1, 0, 0.6f);

	public string SCENE_PATH { get; } = "res://Godot/Display/UI/Party/PartyMobListUI.tscn";

	[Export]
	public Label NodeFactionNameLabel;

	public MobListUI() : base()
	{
		ButtonVerticalFlags = SizeFlags.ExpandFill;
	}

	public override void _Ready()
	{
		base._Ready();
		EventBus.InputPauseOptionSelected += OnInputPauseOptionSelected;
	}

	public void Update(Faction faction)
	{
		NodeFactionNameLabel.Text = $"Faction: {faction.DisplayedName}";
		Update(faction.Mobs);
	}

	public void Update(List<Faction> factions)
	{
		List<Mob> mobList = new();
		foreach (Faction fac in factions)
		{
			mobList.AddRange(fac.Mobs);
		}
		NodeFactionNameLabel.Text = $"Factions: {factions.ToStringList()}";
		Update(mobList);
	}

	#region Event Handling - INTERNAL
	protected override void _ButtonCreated(MobTooltipButton button, Mob param)
	{
		button.MobReference = param;
		button.Text = param.ToString();
		button.Material = Global.ManagerMaterial.ResourceGet("CanvasShaderBorderColor");
		(button.Material as ShaderMaterial)?.SetShaderParameter("border_color", Colors.Transparent);
	}

	protected override void _ButtonPressed(MobTooltipButton button, Mob param)
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

		base._ButtonPressed(button, param);
		EventBus.MobSelected?.Invoke(MobSelected);
	}

	protected override void _ButtonHovered(MobTooltipButton button, Mob param, bool hovered)
	{
		base._ButtonHovered(button, param, hovered);
		//Do not affect the modulate if this is the selected button.
		if (param == MobSelected) { return; }

		if (hovered)
		{
			button.Modulate = Godot.Colors.White * 0.5f;
		}
		else
		{
			button.Modulate = Godot.Colors.White;
		}
	}
	#endregion

	#region Event Handling

	private void OnInputPauseOptionSelected(EPauseOption obj)
	{
		string? identifier = Global.ManagerFaction.FindIdentifier(FactionAssigned);

		if (identifier is null) throw new Exception();
		
		if (obj == EPauseOption.PARTY)
		{
			Update(Global.ManagerFaction.ResourceGet( identifier ));
		}
	}

	#endregion

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

		Godot.Vector2 ITooltip.GetRectSize() => new(240, 100);
	}

}
