using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundTest : MonoBehaviour
{
    public SoundManager soundManager;
    public Sound sound;

    public void TestSound() {
        soundManager.PlaySound(sound);
    }
}
