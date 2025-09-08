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

public static class MobTemplateResourcePackExtension
{
	public static List<TTemplate> FilterByType<TTemplate>(this IEnumerable<MobTemplate> collection) where TTemplate : MobTemplate
	{
		return collection.OfType<TTemplate>().ToList();
	}
}
