using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [SerializeField] private AudioBank audioBank;
    [SerializeField] private AudioMixerGroup sfxGroup;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void PlaySound(
        string eventKey,
        Vector3 position,
        AudioCue soundOverride = null)
    {
        AudioCue cue = soundOverride != null
            ? soundOverride
            : audioBank != null
                ? audioBank.GetCue(eventKey)
                : null;

        if (cue == null)
        {
            Debug.LogWarning($"Missing audio cue: {eventKey}");
            return;
        }

        AudioClip clip = cue.GetRandomClip();

        if (clip == null)
            return;

        GameObject emitter = new GameObject("SFX: " + eventKey);

        emitter.transform.position = position;

        AudioSource source = emitter.AddComponent<AudioSource>();

        source.playOnAwake = false;
        source.clip = clip;

        source.outputAudioMixerGroup = sfxGroup;

        source.volume = cue.volume;

        source.pitch = Random.Range(
            Mathf.Min(cue.minPitch, cue.maxPitch),
            Mathf.Max(cue.minPitch, cue.maxPitch)
        );

        source.spatialBlend = cue.spatialBlend;

        source.Play();

        Destroy(emitter, clip.length / source.pitch + 0.1f);
    }
    private void OnEnable()
    {   
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Look up the configured music for this scene.
        // Crossfade to that track if necessary.
    }
}