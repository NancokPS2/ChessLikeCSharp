using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChessLike.Entity;
using Godot;

[GlobalClass]
public partial class BaseButtonMenuMobTemplateJob : BaseButtonMenu<Button, MobTemplateJob?>
{

	[Export]
	protected int MinimumSlots = 0;

	protected override void _Update(List<MobTemplateJob?> parameterList)
	{
		base._Update(parameterList);
		int total = parameterList.Count;

		int missing = Math.Clamp(MinimumSlots - total, 0, int.MaxValue);
		for (int i = 0; i < missing; i++)
		{
			ButtonCreate(null);
		}
	}

	protected override void _ButtonCreated(Button button, MobTemplateJob? param)
	{
		base._ButtonCreated(button, param);
		if (param is null) return;

		button.Text = param.TemplateName;
	}
}
