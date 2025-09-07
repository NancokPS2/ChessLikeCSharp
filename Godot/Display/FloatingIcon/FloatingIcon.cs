using ChessLike.Entity.Action;
using Godot;
using System;
using System.ComponentModel;
using System.Runtime.Serialization;

[GlobalClass]
public partial class FloatingIcon3D : Node3D
{
	/* 	
	[Export]
    protected Sprite3D? Sprite;

    [Export]
    protected Label3D? Label; 
	*/
	[Export]
	protected float PixelSizeDefault = 0.005f;

	protected Dictionary<uint, VisualInstance3D> NodeDict = new();

	public virtual Sprite3D GetNewSpriteInstance() => new Sprite3D()
	{
		Billboard = BaseMaterial3D.BillboardModeEnum.Enabled,
		TextureFilter = BaseMaterial3D.TextureFilterEnum.Nearest,
		PixelSize = PixelSizeDefault,
		NoDepthTest = true,
		FixedSize = true,
	};

	public VisualInstance3D? GetVisualInstance(uint layer)
	{
		NodeDict.TryGetValue(layer, out VisualInstance3D? existingInstance);
		return existingInstance;
	}

	public virtual Label3D GetNewLabelInstance() => new Label3D()
	{
		Billboard = BaseMaterial3D.BillboardModeEnum.Enabled,
		FontSize = 16,
		OutlineSize = 4,
		RenderPriority = 1,
		OutlineRenderPriority = 0,
		Font = Readonly.Fonts.Joystix,
		PixelSize = PixelSizeDefault,
		NoDepthTest = true,
		FixedSize = true,
	};

	public void SetTexture(Texture2D texture, uint layer)
	{
		ClearInstance(layer);

		int index = GetNewIndex(layer);

		Sprite3D node = GetNewSpriteInstance();
		node.Texture = texture;
		node.SortingOffset = index;

		NodeDict[layer] = node;
		AddChild(node);
		MoveChild(node, index);
	}

	public void SetText(string text, uint layer)
	{
		ClearInstance(layer);

		int index = GetNewIndex(layer);

		Label3D node = GetNewLabelInstance();
		node.Text = text;
		node.SortingOffset = index;

		NodeDict[layer] = node;
		AddChild(node);
		MoveChild(node, index);
	}

	public void SetModulation(Godot.Color color, uint layer)
	{
		NodeDict.TryGetValue(layer, out VisualInstance3D? instance);
		if (instance is Sprite3D sprite)
		{
			sprite.Modulate = color;
		}
		else if (instance is Label3D label)
		{
			label.Modulate = color;
		}
	}

	private int GetNewIndex(uint layer)
	{
		return (int)Math.Clamp(layer, 0, GetChildren().Count());
	}

	public void ClearInstances()
	{
		foreach (var item in NodeDict.Keys)
		{
			ClearInstance(item);
		}
	}

	protected void ClearInstance(uint layer)
	{
		VisualInstance3D? existingInstance = GetVisualInstance(layer);
		NodeDict.Remove(layer);
		existingInstance?.QueueFree();
	}
}
