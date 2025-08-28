using Godot;

namespace ChessLike.GenericGridStorage;

public struct Cell
{
	public static readonly Cell INVALID = new(){Valid = false, Name = "INVALID"};
	private bool Valid = true;

	public Vector3i Position;

	public string Name;

	public Godot.Color Color;

	public bool IntersectEnabled = true;

	private Dictionary<string, Variant> Metadata = new();

	public Cell(Vector3i position)
	{
		SetPosition(position);
		Name = "UNNAMED";
		Color = Colors.White;
	}

	public Cell(Vector3i position, string name, Godot.Color color) : this(position)
	{
		Name = name;
		Color = color;
	}

	public void SetPosition(Vector3i position)
	{
		Position = position;
	}

	public void SetMeta(string key, int value)
		=> Metadata[key] = value;

	public void SetMeta(string key, string value)
		=> Metadata[key] = value;

	public int GetMetaInt(string key)
		=> Metadata[key].As<int>();

	public string GetMetaString(string key)
		=> Metadata[key].As<string>();

	public void SetIntersect(bool enabled)
		=> Valid = enabled;

	public bool IsValid()
		=> !Valid;
}