using ChessLike.WorldMap;
using Godot;

public partial class Readonly
{
	public static class Scenes
	{
		public static MobScene SCENE_MOB
		{
			get => GD.Load<PackedScene>("uid://k16lil2fu57n").Instantiate<MobScene>();
		}

		public static PopupText3D SCENE_PARTICLE_POPUP_TEXT
		{
			get => GD.Load<PackedScene>("uid://ia3s65kbmt7u").Instantiate<PopupText3D>();
		}

		public static CombatScene MAIN_COMBAT
		{
			get => GD.Load<PackedScene>("uid://b60n65v58uy2o").Instantiate<CombatScene>();
		}

		public static TravelMapScene MAIN_TRAVEL_MAP
		{
			get => GD.Load<PackedScene>("uid://ct6gl8ha8xn84").Instantiate<TravelMapScene>();
		}

		public static MainMenuScene MAIN_MENU
		{
			get => GD.Load<PackedScene>("uid://c84de8msjqd3c").Instantiate<MainMenuScene>();
		}

		public static DialogueBubble DIALOGUE_BUBBLE
		{
			get => GD.Load<PackedScene>("uid://bs3m3jeeg1cd2").Instantiate<DialogueBubble>();
		}
	}
}
