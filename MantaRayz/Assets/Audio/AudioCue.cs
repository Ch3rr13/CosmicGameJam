using UnityEngine;

[CreateAssetMenu(menuName = "Jam Audio/Audio Cue")]
public class AudioCue : ScriptableObject
{
    public AudioClip[] clips;

    [Range(0f, 1f)]
    public float volume = 1f;

    [Range(0.5f, 1.5f)]
    public float minPitch = 0.95f;

    [Range(0.5f, 1.5f)]
    public float maxPitch = 1.05f;

    [Range(0f, 1f)]
    public float spatialBlend = 1f;

    public AudioClip GetRandomClip()
    {
        if (clips == null || clips.Length == 0)
            return null;

        int index = Random.Range(0, clips.Length);

        return clips[index];
    }
}