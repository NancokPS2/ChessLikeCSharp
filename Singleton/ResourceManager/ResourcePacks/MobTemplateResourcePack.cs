using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChessLike.Entity;
using Godot;

public class MobTemplateResourcePack : ResourcePack<MobTemplate>
{
	public TTemplate GetResource<TTemplate>(EPackIDMobTemplate templateID) where TTemplate : MobTemplate
		=> (TTemplate)ResourceGet(templateID.ToString());
}
