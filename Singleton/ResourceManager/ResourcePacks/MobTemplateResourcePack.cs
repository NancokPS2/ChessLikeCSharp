using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChessLike.Entity;
using Godot;

public class MobTemplateResourcePack : ResourcePack<MobTemplate>
{
	public MobTemplate GetResource(EPackIDMobTemplate templateID)
		=> GetResource(templateID.ToString());
}
