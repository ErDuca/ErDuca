using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundAssets : MonoBehaviour
{
    private static SoundAssets _i;
    
    public static SoundAssets i {
        get {
            if (i == null) _i = Instantiate(Resources.Load<SoundAssets>("SoundAssets"));

            return _i;
        }

    }

    public SoundAudioClip[] soundAudioClipArray;

    [System.Serializable]
    public class SoundAudioClip {
        public SoundManager.Sound sound;
        public AudioClip audioClip;
    }

    
}
