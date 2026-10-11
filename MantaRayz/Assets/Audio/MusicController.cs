using System.Collections;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class MusicController : MonoBehaviour
{
    [Header("Configuration")]
    [SerializeField] private MusicLibrary library;
    [SerializeField] private AudioMixerGroup musicGroup;

    [Header("Music Sources")]
    [SerializeField] private AudioSource sourceA;
    [SerializeField] private AudioSource sourceB;

    private AudioSource currentSource;
    private Coroutine fadeRoutine;

    private void Awake()
    {
        if (sourceA == null || sourceB == null || sourceA == sourceB)
        {
            Debug.LogError("MusicController requires two different AudioSources.");
            enabled = false;
            return;
        }

        ConfigureSource(sourceA);
        ConfigureSource(sourceB);
    }

    private void ConfigureSource(AudioSource source)
    {
        source.playOnAwake = false;
        source.loop = true;
        source.spatialBlend = 0f; // 2D audio
        source.volume = 0f;

        if (musicGroup != null)
            source.outputAudioMixerGroup = musicGroup;
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void Start()
    {
        // Handle the scene that was already open when
        // the MusicController was initialized.
        SetMusicForScene(SceneManager.GetActiveScene().name);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        SetMusicForScene(scene.name);
    }

    private void SetMusicForScene(string sceneName)
    {
        if (library == null)
            return;

        if (library.TryGet(sceneName, out SceneMusicEntry entry))
        {
            PlayMusic(entry.track, entry.fadeSeconds);
        }
    }

    public void PlayMusic(AudioClip clip, float fadeDuration = 2f)
    {
        // Don't restart music that's already playing.
        if (currentSource != null &&
            currentSource.clip == clip &&
            currentSource.isPlaying)
        {
            return;
        }

        // Handle a new request arriving during a crossfade.
        if (fadeRoutine != null)
        {
            StopCoroutine(fadeRoutine);
            fadeRoutine = null;

            AudioSource louder = sourceA.volume >= sourceB.volume
                ? sourceA : sourceB;

            AudioSource quieter = louder == sourceA
                ? sourceB : sourceA;

            quieter.Stop();
            quieter.volume = 0f;

            currentSource = louder.isPlaying ? louder : null;
        }

        if (currentSource != null &&
            currentSource.clip == clip &&
            currentSource.isPlaying)
        {
            currentSource.volume = 1f;
            return;
        }

        AudioSource outgoing = currentSource;

        // A null clip means fade out to silence.
        if (clip == null)
        {
            currentSource = null;

            if (outgoing != null)
                fadeRoutine = StartCoroutine(
                    Crossfade(outgoing, null, fadeDuration)
                );

            return;
        }

        AudioSource incoming = outgoing == sourceA
            ? sourceB : sourceA;

        incoming.Stop();
        incoming.clip = clip;
        incoming.volume = 0f;
        incoming.Play();

        currentSource = incoming;

        fadeRoutine = StartCoroutine(
            Crossfade(outgoing, incoming, fadeDuration)
        );
    }

    private IEnumerator Crossfade(
        AudioSource outgoing,
        AudioSource incoming,
        float duration)
    {
        float elapsed = 0f;
        float startingVolume = outgoing != null
            ? outgoing.volume : 0f;

        if (duration > 0f)
        {
            while (elapsed < duration)
            {
                elapsed += Time.unscaledDeltaTime;

                float t = Mathf.Clamp01(elapsed / duration);

                if (outgoing != null)
                    outgoing.volume = Mathf.Lerp(
                        startingVolume, 0f, t
                    );

                if (incoming != null)
                    incoming.volume = Mathf.Lerp(0f, 1f, t);

                yield return null;
            }
        }

        if (outgoing != null)
        {
            outgoing.Stop();
            outgoing.volume = 0f;
        }

        if (incoming != null)
            incoming.volume = 1f;

        fadeRoutine = null;
    }
}