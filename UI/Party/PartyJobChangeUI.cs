using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChessLike.Entity;
using ChessLike.Extension;
using Godot;

[GlobalClass]
public partial class PartyJobChangeUI : Control
{
	[Export]
	protected BaseButtonMenuMobTemplateJob CurrentJobMenu = null!;

	[Export]
	protected BaseButtonMenuMobTemplateJob AvailableJobMenu = null!;

	[Export]
	protected Godot.Collections.Array<string> JobAvailableFilters = new();


	public override void _Ready()
	{
		base._Ready();
		EventBus.MobSelected += OnMobSelected;
	}

	public void Update(Mob mob)
	{
		CurrentJobMenu.Update(mob.TemplateGet<MobTemplateJob>());
		AvailableJobMenu.Update(GetAvailableJobs());
	}

	protected List<MobTemplateJob> GetAvailableJobs()
	{
		if (JobAvailableFilters.IsEmpty())
		{
			return Global.ManagerMobTemplate.ResourceGetAll(false).FilterByType<MobTemplateJob>();
		}
		else
		{
			List<MobTemplateJob> output = new();
			foreach (var item in JobAvailableFilters)
			{
				output.AddRange(
					Global.ManagerMobTemplate.ResourcesGetWithTag(item)
					.Where(x => !output.Contains(x))
					.FilterByType<MobTemplateJob>());
			}
			return output;
		}
	}

	#region Event Handling
	private void OnMobSelected(Mob obj)
	{
		Update(obj);
	}
	#endregion
}
