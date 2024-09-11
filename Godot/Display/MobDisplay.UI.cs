using ChessLike.Entity;
using Action = ChessLike.Entity.Action;

namespace Godot.Display;

public partial class MobDisplay : Godot.Node3D
{
    public partial class MobUI : Control, IGSceneAdapter
    {
        public delegate void ActionPressedEventHandler(Action action);
        public event ActionPressedEventHandler ActionPressed;

        const string NODE_STAT_CONTAINER = "STAT_CONTAINER"; //VBoxContainer    
        const string NODE_ACTION_CONTAINER = "ACTION_CONTAINER";//VBoxContainer
        const string NODE_TURN_CONTAINER = "TURN_CONTAINER";//HBoxContainer
        const string NODE_NAME = "NAME";
        const string NODE_DELAY = "DELAY";
        const string NODE_HEALTH = "HEALTH";
        const string NODE_ENERGY = "ENERGY";

        private Mob? mob;

        public MobUI()
        {
        }

        public void CreateAllNodes()
        {
            List<Control> nodes = new();
            //Create action container.
            VBoxContainer action_container = new VBoxContainer();
            action_container.SetAnchorsAndOffsetsPreset(LayoutPreset.FullRect);
            action_container.SetAnchor(Side.Left, 0.6f);
            action_container.SetAnchor(Side.Bottom, 0.65f);
            nodes.Add(action_container);

            //Create the nodes to show stats.
            VBoxContainer stat_container = new VBoxContainer();
            stat_container.SetAnchorsAndOffsetsPreset(LayoutPreset.FullRect);
            stat_container.SetAnchor(Side.Right, 0.4f);
            nodes.Add(stat_container);

            Label name = new Label();
            Label delay = new Label();

            ProgressBar health = new ProgressBar();
            ProgressBar energy = new ProgressBar();

            nodes.Add(name);
            nodes.Add(delay);
            nodes.Add(health);
            nodes.Add(energy);


            //Add the container
            AddChild(stat_container);

            //Add the rest of the nodes to the container.
            foreach (Control node in nodes)
            {
                //Skip the container.
                if(node == stat_container){continue;}

                //Set the sizing.
                node.SizeFlagsHorizontal = SizeFlags.ExpandFill;
                node.SizeFlagsVertical = SizeFlags.ExpandFill;

                //Add it to the container.
                stat_container.AddChild(node);
            }
        }

        public void SetMob(Mob mob)
        {
            this.mob = mob;
            //UpdateStatNodes();
            UpdateActionButtons();
        }

        public void UpdateStatNodes()
        {
            #pragma warning disable CS8602 // Dereference of a possibly null reference.
            this.RequiredNodeTryToGet<Label>(new(NODE_NAME))
                .Text = mob.DisplayedName;

            this.RequiredNodeTryToGet<Label>(new(NODE_DELAY))
                .Text = mob.Stats.GetValue(StatName.DELAY).ToString();

            this.RequiredNodeTryToGet<ProgressBar>(new(NODE_HEALTH))
                .Value = mob.Stats.GetValue(StatName.HEALTH);

            this.RequiredNodeTryToGet<ProgressBar>(new(NODE_HEALTH))
                .MaxValue = mob.Stats.GetMax(StatName.HEALTH);

            this.RequiredNodeTryToGet<ProgressBar>(new (NODE_ENERGY))
                .Value = mob.Stats.GetValue(StatName.ENERGY);

            this.RequiredNodeTryToGet<ProgressBar>(new(NODE_ENERGY))
                .MaxValue = mob.Stats.GetMax(StatName.ENERGY);

        }

        public void UpdateActionButtons()
        {
            foreach (Node node in this.RequiredNodeTryToGet<VBoxContainer>(new(NODE_ACTION_CONTAINER)).GetChildren())
            {
                node.QueueFree();
            }
            
            foreach (ChessLike.Entity.Action action in mob.Actions)
            {
                ActionButton button = new(action);
                this.RequiredNodeTryToGet<VBoxContainer>(new(NODE_ACTION_CONTAINER))
                    .AddChild(button);
                button.Text = action.name;
                Console.WriteLine(button.GetPath());
                button.Pressed += () => ActionPressed?.Invoke(action);
            }
        }

        public void EnableActionButtons(bool enable)
        {
            foreach (Node node in this.RequiredNodeTryToGet<VBoxContainer>(new(NODE_ACTION_CONTAINER)).GetChildren())
            {
                if (node is Button button)
                {
                    button.Disabled = !enable;
                }
            }
        }
        #pragma warning restore CS8602 // Dereference of a possibly null reference.


        public List<IGSceneAdapter.NodeDeclaration> NodesRequired { get; set; } = new()
        {
            //Container
            new (NODE_STAT_CONTAINER, typeof(Container)),
            new (NODE_ACTION_CONTAINER, typeof(Container)),
            new (NODE_TURN_CONTAINER, typeof(Container)),

            //Label
            new (NODE_NAME, typeof(Label)),
            new (NODE_NAME, typeof(Label)),

            //ProgressBar
            new (NODE_HEALTH, typeof(ProgressBar)),
            new (NODE_ENERGY, typeof(ProgressBar)),
        };
        public string ScenePath { get; set; } = "res://assets/PackedScene/MobUI.tscn";
        public Node? SceneTopNode { get; set; }

        private partial class ActionButton : Button
        {
            public Action action;

            public ActionButton(Action action)
            {
                this.action = action;
                Text = action.name;
            }
        }

}
}