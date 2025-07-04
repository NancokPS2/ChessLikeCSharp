using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChessLike.World.Encounter;
using Godot;
using Godot.NativeInterop;

public partial class SceneChanger : Node
{

    public override void _Ready()
    {
        base._Ready();
        Global.ConnectToWindow(GetWindow());
    }

    public CombatScene GetCombatScene(EncounterData encounter)
    {
        CombatScene combatScene = Readonly.Scenes.MAIN_COMBAT;
        combatScene.Setup(encounter);
        return combatScene;
    }

}
