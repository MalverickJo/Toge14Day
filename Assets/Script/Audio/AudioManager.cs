using UnityEngine;
using System.Collections.Generic;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Sources")]
    public AudioSource sfxSource;
    public AudioSource musicSource;
    public AudioSource footstepSource;

    [Header("Global SFX")]
    public AudioClip buttonClickSound;
    public AudioClip encounterSound;
    public AudioClip levelUpSound;

    [Header("Footstep by Tile Tag")]
    public List<TileSound> footstepSounds = new List<TileSound>();

    [Header("Background Music")]
    public AudioClip overworldMusic;
    public AudioClip battleMusic;

    [Range(0f, 1f)] public float sfxVolume = 1f;
    [Range(0f, 1f)] public float musicVolume = 0.6f;

    [System.Serializable]
    public class TileSound
    {
        public string tileTag;
        public AudioClip clip;
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        if (sfxSource == null) sfxSource = gameObject.AddComponent<AudioSource>();
        if (footstepSource == null) footstepSource = gameObject.AddComponent<AudioSource>();
        if (musicSource == null)
        {
            musicSource = gameObject.AddComponent<AudioSource>();
            musicSource.loop = true;
        }
    }

    // ─── SFX ───────────────────────────────────────────

    public void PlaySFX(AudioClip clip)
    {
        if (clip == null || sfxSource == null) return;
        sfxSource.PlayOneShot(clip, sfxVolume);
    }

    public void PlayButtonClick() => PlaySFX(buttonClickSound);
    public void PlayEncounter() => PlaySFX(encounterSound);
    public void PlayLevelUp() => PlaySFX(levelUpSound);

    // ─── Footstep ──────────────────────────────────────

    public void PlayFootstep(string tileTag)
    {
        if (footstepSource == null) return;
        if (footstepSource.isPlaying) return;

        AudioClip clip = footstepSounds.Find(t => t.tileTag == tileTag)?.clip;
        if (clip != null)
            footstepSource.PlayOneShot(clip, sfxVolume * 0.7f);
    }

    // ─── Music ─────────────────────────────────────────

    public void PlayMusic(AudioClip clip, bool loop = true)
    {
        if (clip == null || musicSource == null) return;
        if (musicSource.clip == clip && musicSource.isPlaying) return;

        musicSource.clip = clip;
        musicSource.loop = loop;
        musicSource.volume = musicVolume;
        musicSource.Play();
    }

    public void PlayOverworldMusic() => PlayMusic(overworldMusic);
    public void PlayBattleMusic() => PlayMusic(battleMusic);

    public void StopMusic() => musicSource?.Stop();
}
