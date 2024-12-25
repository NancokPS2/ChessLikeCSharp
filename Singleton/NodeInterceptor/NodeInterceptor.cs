using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ExtendedXmlSerializer.Core.Sources;
using Godot;

public partial class NodeInterceptor : Node
{
    private Dictionary<Type, List<Action<Node>>> InterceptorFunctionDict = new();
    static public NodeInterceptor Instance;

    public override void _Ready()
    {
        base._Ready();
        Instance = this;
        GetTree().NodeAdded += Intercept;

        AddInterceptorFunction<CpuParticles3D>(InterceptParticles);
        AddInterceptorFunction<AudioStreamPlayer>(InterceptAudioStreamPlayer);
    }

    private void Intercept(Node node)
    {
        if (InterceptorFunctionDict.ContainsKey(node.GetType()))
        {
            List<Action<Node>> functions = InterceptorFunctionDict[node.GetType()];
            foreach (var item in functions)
            {
                item(node);
            }
        }
    }

    private void InterceptParticles(Node node)
    {
        if (node is CpuParticles3D particles)
        {
            const string COUNT_META_KEY = "default_particle_count";
            particles.SetMeta(COUNT_META_KEY, particles.Amount);
            particles.Amount = (int)((float)particles.GetMeta(COUNT_META_KEY, 1f) * Settings.Get(Settings.KeyFloat.PARTICLE_RATIO));
            particles.Finished += particles.QueueFree;
        }
    }

    private void InterceptAudioStreamPlayer(Node node)
    {
        if (node is AudioStreamPlayer player)
        {
            player.VolumeDb = Mathf.LinearToDb(Settings.Get(Settings.KeyFloat.VOLUME));
        }
    }

    public static void AddInterceptorFunction<TNodeType>(Action<Node> function) where TNodeType : notnull, Node
    {
        if (!Instance.InterceptorFunctionDict.ContainsKey(typeof(TNodeType)))
        {
            Instance.InterceptorFunctionDict[typeof(TNodeType)] = new();
        }
        Instance.InterceptorFunctionDict[typeof(TNodeType)].Add(function);
    }
    public void ClearInterceptorFunction<TNodeType>() where TNodeType : notnull, Node
    {
        InterceptorFunctionDict.Remove(typeof(TNodeType));
    } 
}
