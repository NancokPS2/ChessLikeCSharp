using Godot;
using System;

public partial class PopupText : CpuParticles3D
{
	public Godot.Font Font {set => SetFont(value); get => GetFont();}
	public string Text {set;get;}


	private void SetFont(Godot.Font font)
	{
		TextMesh mesh_ref = (TextMesh)Mesh ?? throw new Exception("No TextMesh assigned.");
		mesh_ref.Font = font;
	}

	private Godot.Font GetFont()
	{
		TextMesh mesh_ref = (TextMesh)Mesh ?? throw new Exception("No TextMesh assigned.");
		return mesh_ref.Font;
	}

}
