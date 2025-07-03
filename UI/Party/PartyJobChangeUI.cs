using ChessLike.Entity;
using ChessLike.Extension;
using Godot;

[GlobalClass]
public partial class PartyJobChangeUI : Control, ISceneDependency
{
    public string SCENE_PATH { get; } = "res://Godot/Display/UI/Party/PartyJobChangeUI.tscn";

    [Export]    
    public HBoxContainer? NodeUsedJobs;
    [Export]    
    public VBoxContainer? NodeAvailableJobs;
    [Export]
    public Godot.Collections.Array<EJob> AvailableJobs = new();

    [Export]
    public bool CanChangeJob = true;
    [Export]
    public int MaxJobs = 2;

    private JobButton? UsedSelected;

    public override void _Ready()
    {
        base._Ready();
        NodeUsedJobs ??= (HBoxContainer)FindChild("SelectedJobs");
        NodeAvailableJobs ??= (VBoxContainer)FindChild("AvailableJobs");

        //Fill with all jobs if no jobs have been defined as available.
        if (AvailableJobs.Count == 0)
        {
            AvailableJobs.AddRange(Enum.GetValues<EJob>());
        }

    }

    private Mob? MobCurrent;

    public void Update(Mob mob)
    {
        NodeUsedJobs?.FreeChildren();
        NodeAvailableJobs?.FreeChildren();

        foreach (var item in AvailableJobs)
        {
            //list_of_pickable_jobs.Add(Job.CreatePrototype(item));
            JobButton button = new(this, Job.CreatePrototype(item));
            NodeAvailableJobs?.AddChild(button);
            button.JobSelected += OnAvailablePressed;
        }

        List<Job> mob_jobs = mob.GetJobs();
        foreach (var item in mob_jobs)
        {
            JobButton button = new(this, item);
            button.SizeFlagsHorizontal = SizeFlags.ExpandFill;
            NodeUsedJobs?.AddChild(button);
            button.JobSelected += OnUsedPressed;
        }
        while (NodeUsedJobs.GetChildren().Count < MaxJobs)
        {
            JobButton button = new(this, null);
            button.SizeFlagsHorizontal = SizeFlags.ExpandFill;
            NodeUsedJobs?.AddChild(button);
            button.JobSelected += OnUsedPressed;
        } 

        MobCurrent = mob;
    }

    /// <summary>
    /// Fired when a job from the list is selected.
    /// </summary>
    /// <param name="button"></param>
    /// <exception cref="Exception"></exception>
    private void OnAvailablePressed(JobButton button)
    {
        if (MobCurrent is null){ throw new Exception();}
        if (!CanChangeJob)
        {
            return;
        }
        //Ignore if no Used is already selected.
        if (UsedSelected is null)
        {
            MessageQueue.AddMessage("One of mob's slots must be chosen first.");
            return;
        }

        UsedSelected.Job = button.Job;
        UpdateMobJobs(MobCurrent, (from btn in NodeUsedJobs?.GetChildren<JobButton>() select btn.Job).ToList() );
        UsedSelected = null;
        Update(MobCurrent);
    }

    private void OnUsedPressed(JobButton button)
    {
        if (!CanChangeJob)
        {
            return;
        }
        UsedSelected = button;
        SetToGlow(button, NodeUsedJobs?.GetChildren<CanvasItem>().ToList() ?? new());
    }

    private void UpdateMobJobs(Mob mob, List<Job> jobs)
    {
        mob.ChainJob(jobs);
    }

    public void SetToGlow(CanvasItem? node, List<CanvasItem> siblings)
    {
        foreach (var item in siblings)
        {
            item.AnimateIntermitentGlowStop();
        }
        node?.AnimateIntermitentGlow(1, Colors.Gray);
    }

    private partial class JobButton : Button
    {
        public delegate void JobPressed(JobButton button);
        public event JobPressed JobSelected;
        public PartyJobChangeUI UI;
        public Job? Job;

        public JobButton(PartyJobChangeUI ui, Job? job)
        {
            Job = job;
            UI = ui;
            if (Job is not null)
            {
                Text = Enum.GetName<EJob>(Job.Identifier);
            }else
            {
                Text = "EMPTY";
            }
        }

        public override void _Pressed()
        {
            base._Pressed();
            JobSelected?.Invoke(this);
        }
    }

}
