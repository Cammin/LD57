using CamLib.Editor;
using UnityEngine;

public class AssetWindow : CentralizedAssetWindowImplementation
{
    public override string[] SceneFolders => new []{"Assets/_Game/Scenes"};

    public override IEditorPrefInstance[] Prefs => base.Prefs;

    public override string[] AssetDisplayPaths => new []
    {
        "Assets/_Game/Prefabs/Player.prefab",
        "Assets/_Game/LDtk/LD57.ldtk",
        "Assets/_Game/Prefabs/Battery.prefab",
        "Assets/_Game/Prefabs/Checkpoint.prefab",
    };
}
