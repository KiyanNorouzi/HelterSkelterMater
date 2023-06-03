using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum SoundType
{
    NONE = 0,
    Menu_Start,
    Menu_Exit,
    Menu_Back,
    Menu_Level,
    Menu_PopUp,

    Game_Knife,
    Game_PC,
    Game_Smoking,
    Game_ManDeath,
    Game_WomanDeath,
    Game_ManScream,
    Game_WomanScream,
    Game_PoliceEnter,
    Game_PoliceFreeze,
    Game_HideOut,
    Game_Explosion,
    Game_Electricity,
    Game_Ready,

    Ambient_Menu,
    Ambient_InGame
}

/// <summary>
/// Interface for our Audio manager class
/// </summary>
internal interface IAudioManager
{
    bool Play(SoundType type, bool loop);
    bool Pause(SoundType type);
    bool PauseAll();
    bool Stop(SoundType type);
    bool StopAll();
    bool Mute(SoundType type);
}

public class AudioManager : Singleton<AudioManager>, IAudioManager
{
    private readonly string _soundsPath = "Prefabs/Sounds";
    private Dictionary<SoundType, ISound> _sounds = new Dictionary<SoundType, ISound>();
    public float MusicVolume { get; private set; }
    public float VfxVolume { get; private set; }


    #region Unity

    void OnLevelWasLoaded()
    {

    }
    void Awake()
    {
        Load();
        InitVolume();
    }

    void Start()
    {
        Play(SoundType.Ambient_InGame, true);
    }

    void Update()
    {
    }
    #endregion

    #region Methods

    public bool Play(SoundType type, bool loop)
    {
        if (SoundType.NONE == type) return false;
        if (_sounds[type] is AmbientSound) // Play only one background music
        {
            StopAll<AmbientSound>();
        }
        return _sounds[type].Play(loop);
    }

    public bool Pause(SoundType type)
    {
        return _sounds[type].Pause();
    }

    public bool PauseAll()
    {
        foreach (var kvp in _sounds)
        {
            kvp.Value.Pause();
        }
        return true;
    }

    public bool Stop(SoundType type)
    {
        return _sounds[type].Pause();
    }

    public bool StopAll()
    {
        foreach (var kvp in _sounds)
        {
            kvp.Value.Stop();
        }
        return true;
    }

    public bool StopAll<T>() where T : ISound
    {
        foreach (var kvp in _sounds)
        {
            if (kvp.Value is T)
            {
                kvp.Value.Stop();
            }
        }
        return true;
    }

    public bool Mute(SoundType type)
    {
        return _sounds[type].Mute();
    }

    private void Load()
    {
        Sound[] soundsPrefabs = Resources.LoadAll<Sound>(_soundsPath);
        for (int i = 0; i < soundsPrefabs.Length; i++)
        {
            Sound sp = soundsPrefabs[i];
            Transform spTrans = Instantiate(sp.transform) as Transform;
            if (spTrans)
            {
                spTrans.SetParent(transform);
            }
            sp = spTrans.GetComponent<Sound>();
            _sounds[sp.Type] = sp;
        }
    }

    void InitVolume()
    {
        MusicVolume = Settings.musicVolume;
        VfxVolume = Settings.vfxVolume;
        SetVolume();
    }

    void SetVolume()
    {
        foreach (var kvp in _sounds)
        {
            if (kvp.Value is AmbientSound)
            {
                kvp.Value.SetVolume(MusicVolume);
            }
            else
            {
                kvp.Value.SetVolume(VfxVolume);
            }
        }
    }

    public AudioClip this[SoundType type]
    {
        get
        {
            return _sounds[type].Clip;
        }
    }
    #endregion
}
