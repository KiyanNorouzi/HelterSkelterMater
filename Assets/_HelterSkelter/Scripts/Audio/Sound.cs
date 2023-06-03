using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public interface ISound
{
    bool Play(bool loop);
    bool Play(SoundType type, bool loop);
    bool Pause();
    bool Stop();
    bool Mute();
    void SetVolume(float vol);
    AudioClip Clip { get; }
}
[RequireComponent(typeof(AudioSource))]
public class Sound : MonoBehaviour, ISound
{
    private static Dictionary<SoundType, AudioClip> _clips = new Dictionary<SoundType, AudioClip>();
    private AudioSource _audio;

    protected AudioSource Audio
    {
        get
        {
            if (_audio == null)
            {
                _audio = GetComponent<AudioSource>();
            }
            return _audio;
        }
    }

    public AudioClip Clip
    {
        get { return _audio.clip; }
    }

    public SoundType Type; // Don't convert it to auto property FFS, Unity editor won't serialize it!

    protected virtual void Start()
    {
    }

    public bool Play(bool loop = false)
    {
//        print("Play() " + Type);
        if (SoundType.NONE == Type) return false;
        if (Audio.isPlaying) return false;
        Audio.loop = loop;
        Audio.Play();
        return true;
    }

    public bool Play(SoundType type, bool loop)
    {
        if (SoundType.NONE == Type) return false;
        if (Audio.isPlaying) { Audio.Stop(); }
        SetClip(type);
        return Play(loop);
    }

    public bool Pause()
    {
        if (!Audio.isPlaying) return true;
        Audio.Pause();
        return true;
    }

    public bool Stop()
    {
        Audio.Stop();
        return true;
    }

    public bool Mute()
    {
        Audio.mute = Audio.isPlaying;
        return Audio.mute;
    }

    public void SetVolume(float vol)
    {
        Audio.volume = Mathf.Clamp01(vol);
    }

    private void SetClip(SoundType type)
    {
        if (!_clips.ContainsKey(type))
        {
            AudioClip clip = AudioManager.Instance[type];
            if (clip == null)
            {
                Debug.LogError(string.Format("[Sound][Error] No such sound type ({0}) is assigned.", type));
            }
            _clips[type] = clip;
        }
        Type = type;
        Audio.clip = _clips[type];
    }
}
