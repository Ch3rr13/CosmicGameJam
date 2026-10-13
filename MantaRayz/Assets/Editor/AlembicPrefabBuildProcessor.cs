using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Build;

// Alembic's scene processor packages scene instances, but not prefabs spawned at runtime.
// Those prefabs retain their Assets/... paths, relative to StreamingAssets in a player.
public class AlembicPrefabBuildProcessor : BuildPlayerProcessor
{
    public override void PrepareForBuild(BuildPlayerContext buildPlayerContext)
    {
        var scenes = buildPlayerContext.BuildPlayerOptions.scenes;
        foreach (string path in AssetDatabase.GetDependencies(scenes, true)
                     .Where(path => path.EndsWith(".abc", StringComparison.OrdinalIgnoreCase)))
        {
            if (!File.Exists(path)) throw new BuildFailedException($"Missing Alembic cache: {path}");
            buildPlayerContext.AddAdditionalPathToStreamingAssets(path, path);
        }
    }
}
