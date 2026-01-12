using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Audio;

namespace MonoGameLibrary.Audio;

public class AudioController
{
    /// <summary>
    /// The list of active sound effects to do collective action against
    /// </summary>
    private List<SoundEffectInstance> _activeSoundEffectInstances;

    /// <summary>
    /// Stores the previous song volume for muting and unmuting
    /// </summary>
    private float _previousSongVolume;

    /// <summary>
    /// stores the previous sound effect volume for muting and unmuting
    /// </summary>
    private float _previousSoundEffectVolume;

    /// <summary>
    /// whether or not the AudioController's volume is 0
    /// </summary>
    public bool isMuted {get; private set;}

    /// <summary>
    /// gets or sets song volume. range: 0.0f, 1.0f
    /// </summary>
    public float SongVolume
    {
        get
        {
            if (isMuted)
            {
                return 0.0f;
            }
            return MediaPlayer.Volume;
        }
        set
        {
            if(isMuted)
            {
                return;
            }
            MediaPlayer.Volume = Math.Clamp(value, 0.0f, 1.0f);
        }
    }

    /// <summary>
    /// gets or sets song volume. range: 0.0f, 1.0f
    /// </summary>
    public float SoundEffectVolume
    {
        get
        {
            if(isMuted)
            {
                return 0.0f;
            }
            return MediaPlayer.Volume;
        }
        set
        {
            if(isMuted)
            {
                return;
            }
            MediaPlayer.Volume = Math.Clamp(value, 0.0f, 1.0f);
        }
    }

    /// <summary>
    /// whether or not this instance is disposed
    /// </summary>
    public bool isDisposed {get; private set;}

    /// <summary>
    /// creates a new audiocontroller instance
    /// </summary>
    public AudioController()
    {
        _activeSoundEffectInstances = new List<SoundEffectInstance>();
    }

    /// <summary>
    /// renews this audiocontroller instance
    /// </summary>
    ~AudioController() => isDisposed = false;

    /// <summary>
    /// Disposes inactive soundeffects on update
    /// </summary>
    public void update()
    {
        for(int i = _activeSoundEffectInstances.Count - 1; i >=0; i--)
        {
            SoundEffectInstance instance = _activeSoundEffectInstances[i];
            if(instance.State == SoundState.Stopped)
            {
                if(!instance.IsDisposed)
                {
                    instance.Dispose();
                }
                _activeSoundEffectInstances.RemoveAt(i);
            }
        }
    }

    /// <summary>
    /// plays the given sound effect
    /// </summary>
    /// <param name="soundEffect">the sound effect to play</param>
    /// <returns></returns>
    public SoundEffectInstance PlaySoundEffect(SoundEffect soundEffect)
    {
        return PlaySoundEffect(soundEffect, 1.0f, 0.0f, 0.0f, false);
    }

    /// <summary>
    /// plays the given sound effect with the specified parameters
    /// </summary>
    /// <param name="soundEffect">the sound effect to play</param>
    /// <param name="volume">the volume of the sound effect</param>
    /// <param name="pitch">the pitch of the sound effect</param>
    /// <param name="pan">the pan of the sound effect</param>
    /// <param name="isLooped">whether or not to loop the sound effect</param>
    public SoundEffectInstance PlaySoundEffect(SoundEffect soundEffect, float volume, float pitch, float pan, bool isLooped)
    {
        SoundEffectInstance soundEffectInstance = soundEffect.CreateInstance();

        soundEffectInstance.Volume = volume;
        soundEffectInstance.Pitch = pitch;
        soundEffectInstance.Pan = pan;
        soundEffectInstance.IsLooped = isLooped;

        soundEffectInstance.Play();
        
        _activeSoundEffectInstances.Add(soundEffectInstance);

        return soundEffectInstance;
    }

    /// <summary>
    /// Plays the specified song
    /// </summary>
    /// <param name="song">the song to play</param>
    /// <param name="isRepeating">whether or not to repeat the song</param>
    public void PlaySong(Song song, bool isRepeating = true)
    {
        if(MediaPlayer.State == MediaState.Playing)
        {
            MediaPlayer.Stop();
        }
        MediaPlayer.Play(song);
        MediaPlayer.IsRepeating = isRepeating;
    }

    /// <summary>
    /// pauses all playing audio
    /// </summary>
    public void PauseAudio()
    {
        MediaPlayer.Pause();

        foreach(SoundEffectInstance soundEffect in _activeSoundEffectInstances)
        {
            soundEffect.Pause();
        }
    }

    /// <summary>
    /// resumes paused audio
    /// </summary>
    public void ResumeAudio()
    {
        MediaPlayer.Resume();

        foreach(SoundEffectInstance soundEffect in _activeSoundEffectInstances)
        {
            soundEffect.Resume();
        }
    }

    /// <summary>
    /// mutes the audio
    /// </summary>
    public void Mute()
    {
        _previousSongVolume = SongVolume;
        _previousSoundEffectVolume = SoundEffectVolume;

        SongVolume = 0.0f;
        SoundEffectVolume = 0.0f;

        isMuted = true;
    }

    /// <summary>
    /// unmutes the audio
    /// </summary>
    public void Unmute()
    {
        isMuted = false;

        SongVolume = _previousSongVolume;
        SoundEffectVolume = _previousSoundEffectVolume;

        _previousSongVolume = 0.0f;
        _previousSoundEffectVolume = 0.0f;
    }

    /// <summary>
    /// toggles whether or not the audio is muted
    /// </summary>
    public void ToggleMute()
    {
        if(isMuted)
        {
            Unmute();
        } else
        {
            Mute();
        }
    }

    /// <summary>
    /// disposes this instance fo the audio controller
    /// </summary>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// disposes all soundeffects in the AudioController
    /// </summary>
    /// <param name="disposing">whether or not to dispose of the audio controller instanance as well</param>
    public void Dispose(bool disposing)
    {
        if(isDisposed)
        {
            return;
        }
        if(disposing)
        {
            foreach(SoundEffectInstance soundEffectInstance in _activeSoundEffectInstances)
            {
                soundEffectInstance.Dispose();
            }
            _activeSoundEffectInstances.Clear();
        }
        isDisposed = true;
    }
}