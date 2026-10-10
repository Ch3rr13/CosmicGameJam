using System;
using UnityEngine;

[Serializable]
public class AudioEntry
{
    public string eventKey;
    public AudioCue cue;
}

[CreateAssetMenu(menuName = "Jam Audio/Audio Bank")]
public class AudioBank : ScriptableObject
{
    [SerializeField]
    private AudioEntry[] entries;

    public AudioCue GetCue(string eventKey)
    {
        if (entries == null)
            return null;

        foreach (AudioEntry entry in entries)
        {
            if (entry.eventKey == eventKey)
                return entry.cue;
        }

        return null;
    }
}