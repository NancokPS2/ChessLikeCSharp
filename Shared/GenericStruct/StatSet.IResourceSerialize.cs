using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ExtendedXmlSerializer.ExtensionModel.Content;
using Godot;

namespace ChessLike.Shared;

public partial class StatSet<[MustBeVariant]TStatEnum> : IResourceSerialize<StatSet<TStatEnum>, StatSetResource<TStatEnum>>
{
        public StatSetResource<TStatEnum> ToResource()
        {
            StatSetResource<TStatEnum> output = new();
            foreach (var item in Contents)
            {
                output.Contents.Add(item.Key, item.Value.GetMax());
            }
            return output;
        }

        public static StatSet<TStatEnum> FromResource(StatSetResource<TStatEnum> resource)
        {
            StatSet<TStatEnum> output = new();
            output.Contents = new();

            foreach (var item in resource.Contents)
            {
                output.SetStat(item.Key, item.Value);
            }

            return output;
        }
}
