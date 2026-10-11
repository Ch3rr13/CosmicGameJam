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

    private SceneMusicEntry activePlaylist;
    private int lastTrackIndex = -1;

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
        source.loop = false;
        source.spatialBlend = 0f;
        source.volume = 0f;
        source.pitch = 1f;

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

        if (!library.TryGet(sceneName, out var entry))
            return;

        activePlaylist = entry;
        lastTrackIndex = -1;

        if (entry.tracks == null || entry.tracks.Length == 0)
        {
            activePlaylist = null;
            SwitchTo(null, entry.fadeSeconds, false);
            return;
        }

        PlayNextPlaylistTrack();
    }

    private void Update()
    {
        if (activePlaylist == null ||
            activePlaylist.playbackMode == MusicPlaybackMode.Single ||
            activePlaylist.tracks.Length <= 1 ||
            fadeRoutine != null ||
            currentSource == null ||
            currentSource.clip == null ||
            AudioListener.pause)
        {
            return;
        }

        // Begin transitioning before the current track ends.
        // Limit overlap for unusually short clips.
        float overlap = Mathf.Min(
            activePlaylist.fadeSeconds,
            currentSource.clip.length * 0.4f
        );

        if (!currentSource.isPlaying ||
            currentSource.time >= currentSource.clip.length - overlap)
        {
            PlayNextPlaylistTrack(overlap);
        }
    }

    private void PlayNextPlaylistTrack(float? fadeOverride = null)
    {
        if (activePlaylist == null)
            return;

        int count = activePlaylist.tracks.Length;

        if (count == 0)
            return;

        int nextIndex;

        switch (activePlaylist.playbackMode)
        {
            case MusicPlaybackMode.Sequential:
                nextIndex = (lastTrackIndex + 1) % count;
                break;

            case MusicPlaybackMode.Single:
                nextIndex = Random.Range(0, count);
                break;

            default:
                // Random selection without immediate index repeats.
                if (count == 1 || lastTrackIndex < 0)
                {
                    nextIndex = Random.Range(0, count);
                }
                else
                {
                    nextIndex = Random.Range(0, count - 1);

                    if (nextIndex >= lastTrackIndex)
                        nextIndex++;
                }
                break;
        }

        lastTrackIndex = nextIndex;

        AudioClip nextClip = activePlaylist.tracks[nextIndex];

        if (nextClip == null)
        {
            Debug.LogWarning("Music playlist contains an empty track.");
            float fade = activePlaylist.fadeSeconds;
            activePlaylist = null;
            SwitchTo(null, fade, false);
            return;
        }

        bool loop =
            activePlaylist.playbackMode == MusicPlaybackMode.Single ||
            count == 1;

        SwitchTo(
            nextClip,
            fadeOverride ?? activePlaylist.fadeSeconds,
            loop,
            true
        );
    }

    // Public API for manually overriding scene music.
    public void PlayMusic(AudioClip clip, float fadeDuration = 2f)
    {
        activePlaylist = null;
        SwitchTo(clip, fadeDuration, true);
    }

    private void SwitchTo(
        AudioClip clip,
        float fadeDuration,
        bool loop,
        bool forceRestart = false)
    {
        if (!forceRestart &&
            currentSource != null &&
            currentSource.clip == clip &&
            currentSource.isPlaying)
        {
            currentSource.loop = loop;
            return;
        }

        // Resolve an interrupted crossfade.
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

        AudioSource outgoing = currentSource;

        if (clip == null)
        {
            currentSource = null;

            if (outgoing != null)
            {
                if (fadeDuration <= 0f)
                {
                    outgoing.Stop();
                    outgoing.volume = 0f;
                }
                else
                {
                    fadeRoutine = StartCoroutine(
                        Crossfade(outgoing, null, fadeDuration)
                    );
                }
            }

            return;
        }

        AudioSource incoming = outgoing == sourceA
            ? sourceB : sourceA;

        incoming.Stop();
        incoming.clip = clip;
        incoming.loop = loop;
        incoming.volume = 0f;
        incoming.Play();

        currentSource = incoming;

        if (fadeDuration <= 0f)
        {
            if (outgoing != null)
            {
                outgoing.Stop();
                outgoing.volume = 0f;
            }

            incoming.volume = 1f;
        }
        else
        {
            fadeRoutine = StartCoroutine(
                Crossfade(outgoing, incoming, fadeDuration)
            );
        }
    }

    private IEnumerator Crossfade(
        AudioSource outgoing,
        AudioSource incoming,
        float duration)
    {
        float elapsed = 0f;

        float startingVolume = outgoing != null
            ? outgoing.volume : 0f;

        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;

            float t = Mathf.Clamp01(elapsed / duration);

            if (outgoing != null)
                outgoing.volume = Mathf.Lerp(startingVolume, 0f, t);

            if (incoming != null)
                incoming.volume = Mathf.Lerp(0f, 1f, t);

            yield return null;
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