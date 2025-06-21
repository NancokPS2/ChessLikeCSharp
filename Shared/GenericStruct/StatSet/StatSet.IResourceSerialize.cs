using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChessLike.Entity;
using ExtendedXmlSerializer.ExtensionModel.Content;
using Godot;

namespace ChessLike.Shared;

public partial class StatSet<[MustBeVariant]TStatEnum>
{
        public StatSetResource<TStatEnum> ToResource()
        {
            StatSetResource<TStatEnum> output = new();
            foreach (var item in MaxDict)
            {
                output.Contents.Add(item.Key, item.Value);
            }
            return output;
        }

        public static StatSet<TStatEnum> FromResource(StatSetResource<TStatEnum> resource)
        {
            StatSet<TStatEnum> output = new();
            output.MaxDict = new();

            foreach (var item in resource.Contents)
            {
                output.SetStat(item.Key, item.Value);
            }

            return output;
        }
}
