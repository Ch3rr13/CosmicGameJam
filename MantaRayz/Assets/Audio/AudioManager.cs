using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [SerializeField] private AudioBank audioSFXBank;

    [SerializeField] private AudioMixerGroup sfxGroup;
    [SerializeField] private AudioMixerGroup musicGroup;
    [SerializeField] private AudioSource musicSourceA;
    [SerializeField] private AudioSource musicSourceB;
    [SerializeField] private MusicController musicController;

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
        //Debug.Log($"Playing cue: {eventKey}");
        AudioCue cue;
        if (audioSFXBank != null)
        {
            if (soundOverride != null )
            {
                cue = soundOverride;
            } else
            {
                cue = audioSFXBank.GetCue(eventKey);
            }
        } else
        {
            cue = null;
            Debug.LogWarning("Missing audio bank.");
        }

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
        source.spatialBlend = 0.0f;
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



    public void PlayMusic(AudioClip clip, float fadeDuration = 2f)
    {
        if (musicController != null)
            musicController.PlayMusic(clip, fadeDuration);
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
        //
        // Look up the configured music for scene
        // Crossfade to music
    }
}