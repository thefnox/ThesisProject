using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundController : MonoBehaviour
{
    public AudioSource player1;
    public AudioSource player2;
    public AudioClip hit1;
    public AudioClip hit2;
    public AudioClip hit3;
    public AudioClip block;
    public float volume = 0.25f;

    public AudioClip GetClip(string sound)
    {
        switch (sound)
        {
            case "hit1":
                return hit1;
            case "hit2":
                return hit2;
            case "hit3":
                return hit3;
            default:
                return block;
        }
    }
    
    public void PlayerOneHit(string sound)
    {
        player1.PlayOneShot(GetClip(sound), volume);
    }
    public void PlayerTwoHit(string sound)
    {
        player2.PlayOneShot(GetClip(sound), volume);
    }
}
