using MoreMountains.NiceVibrations;
using UnityEngine;

public partial class SFX : SingletonBehaviourResourcesDontDestroy<SFX>
{
    [SerializeField] private AudioSource audioSourceBMG;
    [SerializeField] private AudioSource audioSourceSFX;

    protected override void OnAwake()
    {
        PlayBgMusic();
    }

    public bool MusicEnable
    {
        get => PlayerPrefs.GetInt("MusicEnable", 1) == 1;
        set
        {
            PlayerPrefs.SetInt("MusicEnable", value ? 1 : 0);
            if (value) PlayBgMusic();
            else PauseBgMusic();
        }
    }

    public bool SoundEnable
    {
        get => PlayerPrefs.GetInt("SoundEnable", 1) == 1;
        set => PlayerPrefs.SetInt("SoundEnable", value ? 1 : 0);
    }

    public bool VibrateEnable
    {
        get => PlayerPrefs.GetInt("VibrateEnable", 1) == 1;
        set => PlayerPrefs.SetInt("VibrateEnable", value ? 1 : 0);
    }

    public void VibrateSelection()
    {
#if UNITY_EDITOR
        return;
#endif
        if (!VibrateEnable) return;
        MMVibrationManager.Haptic(HapticTypes.Selection);
    }

    public void Vibrate(bool heavy = false)
    {
#if UNITY_EDITOR
        return;
#endif
        if (!VibrateEnable) return;
        if (!heavy) MMVibrationManager.Vibrate();
        else MMVibrationManager.Haptic(HapticTypes.HeavyImpact);
    }

    private void PlayBgMusic()
    {
        if (audioSourceBMG.isPlaying) return;
        PlayBgMusic(audioSourceBMG.clip, audioSourceBMG.volume, true);
    }

    public void PlayBgMusic(AudioClip bgm, float volume = 1, bool loop = false)
    {
        if (!MusicEnable || audioSourceBMG == null || bgm == null) return;

        audioSourceBMG.clip = bgm;
        audioSourceBMG.volume = volume;
        audioSourceBMG.loop = loop;
        audioSourceBMG.Play();
    }

    public void PauseBgMusic()
    {
        if (audioSourceBMG != null) audioSourceBMG.Pause();
    }

    public void UnPauseBgMusic()
    {
        if (audioSourceBMG != null) audioSourceBMG.UnPause();
    }

    public void PlaySound(AudioClip clip, float volume = 1, bool loop = false, bool overrideCurrent = false)
    {
        if (!SoundEnable || clip == null || audioSourceSFX == null) return;
        if (loop || overrideCurrent)
        {
            audioSourceSFX.Stop();
            audioSourceSFX.clip = clip;
            audioSourceSFX.loop = loop;
            audioSourceSFX.Play();
        }
        else
            audioSourceSFX.PlayOneShot(clip, volume);
    }

    public void StopCurrentSound()
    {
        if (audioSourceSFX == null) return;
        audioSourceSFX.Stop();
        audioSourceSFX.clip = null;
        audioSourceSFX.loop = false;
    }

    public void PauseCurrentSound()
    {
        if (audioSourceSFX == null) return;
        audioSourceSFX.Pause();
    }

    public void StopCurrentBgMusic()
    {
        if (audioSourceBMG == null) return;
        audioSourceBMG.Stop();
        audioSourceBMG.clip = null;
        audioSourceBMG.loop = false;
    }

    public bool IsAudioSourceBGM()
    {
        return audioSourceBMG.clip != null;
    }


}
