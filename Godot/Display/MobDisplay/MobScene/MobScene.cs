using ChessLike.Entity;
using ChessLike.Entity.Action;
using Godot;
using System;

[GlobalClass]
public partial class MobScene : Node3D
{
    public Mob MobUsing;

    protected PackedScene FloatingIconScene = GD.Load<PackedScene>("uid://bmm3h2202bdkq");

    public Node3D FloatingIconParent { get => floatingIconParent ?? throw new Exception(); set => floatingIconParent = value; }
    [Export]
    private Node3D? floatingIconParent;

    public Node3D ModelParent { get => modelParent ?? throw new Exception(); set => modelParent = value; }
    [Export]
    private Node3D? modelParent;

    private List<FloatingIcon3D> FloatingIcons = new(){};

    public MobScene(Mob mobUsing)
    {
        MobUsing = mobUsing;
        EventBus.ActionUsed += OnActionUsed;
    }

    private void OnActionUsed(UsageParameters parameters)
    {
        throw new NotImplementedException();
    }


    protected void Update(List<ActionEvent> actionEvents)
    {
        FloatingIcons.ForEach(x => x.QueueFree());

        foreach (var action in actionEvents)
        {
            if (!action.IsPassive()) continue;
            
            if (action.GetFloatingTexture() is Texture2D texture)
            {
                var floatingIcon = FloatingIconScene.Instantiate<FloatingIcon3D>();
                FloatingIcons.Add(floatingIcon);

                floatingIcon.SetTexture(texture);
                floatingIcon.SetCount(action.GetActivationsLeft());
            }
            else continue;
        }
    }

    protected void SetFloatingIconsAnimationSpeed(float fps)
    {

    }
}
