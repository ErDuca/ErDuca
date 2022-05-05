using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SoundManager
{
    public enum Sound {

    }

    private static Dictionary<Sound, float> soundTimerDictionary;

    public static void Initialize() {
        soundTimerDictionary = new Dictionary<Sound, float>();
        // Set all the sounds
    }

    public static void PlaySound(Sound sound) {
        if (CanPlaySound(sound)) {
            GameObject soundGameObject = new GameObject("Sound");
            AudioSource audioSource = soundGameObject.AddComponent<AudioSource>();
            audioSource.PlayOneShot(GetAudioClip(sound));
        }
    }

    private static bool CanPlaySound(Sound sound) {
        switch (sound) {
            default:
                return true;

            /* time-sensitive case
             *  if (soundTimerDictionary.ContainsKey(sound)) {
             *      float lastTimePlayed = soundTimerDictionary[sound];
             *      float timerMax = [insert max];
             *      
             *      if (lastTimePLayed + timerMax < Time.time) {
             *          soundTimerDictionary[sound] = Time.time;
             *          return true
             *      }
             *      else {
             *          return false
             *      }
             *  }
             *  break;
            */
        }
    }

    private static AudioClip GetAudioClip(Sound sound) {
        foreach (SoundAssets.SoundAudioClip soundAudioClip in SoundAssets.i.soundAudioClipArray) {
            if (soundAudioClip.sound == sound) {
                return soundAudioClip.audioClip;
            }
        }

        Debug.LogError("Sound " + sound + " not found!");
        return null;
    }
}
