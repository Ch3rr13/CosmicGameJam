using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Build;
using UnityEngine;

// Put this in an Editor folder (or an Editor-only assembly).
// Alembic's own scene processor only packages .abc files used by scene instances.
// Prefabs that are spawned at runtime need their .abc copied to StreamingAssets
// under the same "Assets/..." path the stream descriptor stores.
public class AlembicPrefabBuildProcessor : BuildPlayerProcessor
{
    public override void PrepareForBuild(BuildPlayerContext buildPlayerContext)
    {
        var options = buildPlayerContext.BuildPlayerOptions;
        var scenes = options.scenes ?? Array.Empty<string>();
        Debug.Log($"[AlembicPrefabBuildProcessor] running. Target={options.target}, scenes={scenes.Length}");

        // 1) .abc files reachable from the build's scenes.
        var fromScenes = AssetDatabase.GetDependencies(scenes, true)
            .Where(p => p.EndsWith(".abc", StringComparison.OrdinalIgnoreCase));

        // 2) Every .abc in Assets, so prefabs loaded only at runtime
        //    (Resources, Addressables, code, a prefab nobody references) are covered too.
        var allInAssets = Directory.GetFiles("Assets", "*.abc", SearchOption.AllDirectories)
            .Select(p => p.Replace('\\', '/'));

        foreach (string path in fromScenes.Concat(allInAssets).Distinct())
        {
            if (!File.Exists(path))
                throw new BuildFailedException($"Missing Alembic cache: {path}");

            buildPlayerContext.AddAdditionalPathToStreamingAssets(path, path);
            Debug.Log($"[AlembicPrefabBuildProcessor] Added {path} -> StreamingAssets/{path}");
        }
    }
}