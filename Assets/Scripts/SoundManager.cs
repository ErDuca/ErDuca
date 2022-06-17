using System.Collections;
using UnityEngine;

public enum Sound
{
    #region Menu Sounds
    click,
    enemyFound,
    menuWhoosh,
    titleEntrance,
    pageLeft,
    #endregion

    #region Gameplay Sounds
    startingTurn,
    coinFlip,
    unitDraw,
    unitPlaced,
    unitWalk,
    unitJump,
    unitSlide,
    gameOver,
    openInfoBox,
    closeInfoBox,
    
    attackMelee,
    attackRanged,
    death,
    playerPick
    #endregion
}

[System.Serializable]
public struct SoundAudioClip
{
    public Sound sound;
    public AudioClip audioClip;
}

public class SoundManager : MonoBehaviour
{
    public SoundAudioClip[] soundAudioClipArray;
    public AudioSource sfxAudioSource;

    public void PlaySound(Sound sound) 
    {
        GameObject audioGO = new GameObject("Sound");
        AudioSource audioSource =  audioGO.AddComponent<AudioSource>();
        audioSource.volume = PlayerPrefs.GetFloat("SFXVolume");
        audioSource.PlayOneShot(GetAudioClip(sound));
        StartCoroutine(WaitUntilSoundEnds(audioGO, audioSource));
    }   

    private AudioClip GetAudioClip(Sound sound) {
        foreach (SoundAudioClip soundAudioClip in soundAudioClipArray) {
            if (soundAudioClip.sound == sound) {
                return soundAudioClip.audioClip;
            }
        }
        Debug.LogError("Sound " + sound + " not found!");
        return null;
    }

    private IEnumerator WaitUntilSoundEnds(GameObject audio, AudioSource audioSource) {

        yield return new WaitUntil(() => !audioSource.isPlaying);
        Destroy(audio);
    }

    public void playClickSound() {
        PlaySound(Sound.click);
    }

    public void PlayNextPageSound() {
        PlaySound(Sound.pageLeft);
    }
}
