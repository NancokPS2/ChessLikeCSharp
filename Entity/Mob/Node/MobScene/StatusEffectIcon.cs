using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChessLike.Entity.Action;
using ChessLike.StatusEffect;
using Godot;

[GlobalClass]
public partial class StatusEffectIcon : FloatingIcon3D
{
	public enum EBackgroundType {NEGATIVE, NEUTRAL, POSITIVE}
	protected enum ELayer {BACKGROUND, ICON, TEXT}

	protected static readonly Dictionary<EBackgroundType, Texture2D> BackgroundIcons =
		new()   {
			{EBackgroundType.NEGATIVE, GD.Load<Texture2D>("uid://b4bjgaqnxfu7p")},
			{EBackgroundType.NEUTRAL, GD.Load<Texture2D>("uid://b4bjgaqnxfu7p")},
			{EBackgroundType.POSITIVE, GD.Load<Texture2D>("uid://b4bjgaqnxfu7p")}
		};

	[Export]
    double SwapInterval = 1.2;

    public List<Status> StatusEffects = new();

    int currentIndex;

    double timeSinceLast;

	Status? previousStatus;

    [Obsolete("Missing a placeholder.")]
    public override void _Process(double delta)
	{
		base._Process(delta);
		if (timeSinceLast > SwapInterval)
		{
			currentIndex++;
		}
		if (currentIndex >= StatusEffects.Count)
		{
			currentIndex = 0;

			if (StatusEffects.Count == 0)
			{
				ClearInstances();
				return;
			}
		}

		Status statusEffect = StatusEffects[currentIndex];

		SetStatusEffect(statusEffect);

		if (GetVisualInstance((uint)ELayer.TEXT) is Label3D label)
		{
			label.Text = statusEffect.ActivationsLeft.ToString();
		}
		
		timeSinceLast += delta;
	}

	public override Label3D GetNewLabelInstance()
	{
		var instance = base.GetNewLabelInstance();
		instance.Offset = new(0, BackgroundIcons[EBackgroundType.NEUTRAL].GetHeight() + 1);
		return instance;
	}

	private void SetStatusEffect(Status statusEffect)
	{
		//Don't update if it is the same Status object.
		if (previousStatus == statusEffect) return;

		//Decide on background and modulation.
		Texture2D backgroundTexture;
		Godot.Color modulation;
		if (statusEffect.IsBuff())
		{
			backgroundTexture = BackgroundIcons[EBackgroundType.POSITIVE];
			modulation = Colors.Green;
		}
		else if (statusEffect.IsDebuff())
		{
			backgroundTexture = BackgroundIcons[EBackgroundType.NEGATIVE];
			modulation = Colors.Red;
		}
		else
		{
			backgroundTexture = BackgroundIcons[EBackgroundType.NEUTRAL];
			modulation = Colors.White;
		}

		//Background
		SetTexture(backgroundTexture, (uint)ELayer.BACKGROUND);

		//Icon
		SetTexture(
			statusEffect.Icon ?? new PlaceholderTexture2D(){Size = new(8,8)},
			(uint)ELayer.ICON);
		SetModulation(modulation, (uint)ELayer.ICON);

		//Text
		SetText(statusEffect.ActivationsLeft.ToString(), (uint)ELayer.TEXT);

		previousStatus = statusEffect;
	}
}
