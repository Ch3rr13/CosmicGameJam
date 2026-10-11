using System;
using UnityEngine;

[Serializable]
public class SceneMusicEntry
{
    public string sceneName;
    public AudioClip track;

    [Min(0f)]
    public float fadeSeconds = 2f;
}

[CreateAssetMenu(menuName = "Jam Audio/Music Library")]
public class MusicLibrary : ScriptableObject
{
    [SerializeField]
    private SceneMusicEntry[] entries;

    public bool TryGet(string sceneName, out SceneMusicEntry result)
    {
        if (entries != null)
        {
            foreach (var entry in entries)
            {
                if (entry.sceneName == sceneName)
                {
                    result = entry;
                    return true;
                }
            }
        }

        result = null;
        return false;
    }
}