using UnityEngine;
using System;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;
    public Sound[] sounds;

    void Awake()
    {
        instance = this;
        foreach (Sound s in sounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;

            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            s.source.loop = s.loop;
        }

        Play("BattleTheme1");
    }

    public void Play(string soundName)
    {
        try
        {
            Sound sound = GetSound(soundName);
            sound.source.Play();
        } catch(Exception e)
        {
            Debug.LogWarning(e.Message);
        }
    }

    public void Stop(string soundName)
    {
        try
        {
            Sound sound = GetSound(soundName);
            sound.source.Stop();
        }
        catch (Exception e)
        {
            Debug.LogWarning(e.Message);
        }
    }

    Sound GetSound(string soundName)
    {
        Sound sound = Array.Find(sounds, s => s.name == soundName);
        if (sound == null)
        {
            throw new Exception("Sound with name " + soundName + " not found");
        }
        return sound;
    }
}
