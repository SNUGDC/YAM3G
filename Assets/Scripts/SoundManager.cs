using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SoundType {Check,Refill,MoveBS,Swap,Rotate,Click}
public class SoundManager : MonoBehaviour
{
    public GameObject soundPlayer;
    public AudioClip[] soundClips;
    static SoundManager instance;
    public static Queue<GameObject> soundPlayers;
    void Awake()
    {
        instance = this;
        soundPlayers = new Queue<GameObject>();
    }
    public static void PlaySound(SoundType type)
    {
        instance.PlaySoundNonstatic(type);
    }
    public void PlaySoundNonstatic(SoundType type)
    {
        var clip = ChooseClip(type);
        if (soundPlayers.Count > 0)
        {
            var soundPlayer = soundPlayers.Dequeue().GetComponent<SoundPlayer>();
            soundPlayer.PlaySound(clip);
        }
        else
        {
            var soundPlayerGO = Instantiate(instance.soundPlayer, instance.gameObject.transform);
            soundPlayerGO.GetComponent<SoundPlayer>().PlaySound(clip);
        }
    }
    AudioClip ChooseClip(SoundType type)
    {
        int clipNum;
        switch(type)
        {
            case SoundType.Check:
                clipNum = (int)(UnityEngine.Random.value*3); break;
            case SoundType.Refill:
                clipNum = (int)(UnityEngine.Random.value*4)+3; break;
            case SoundType.MoveBS:
                clipNum = 7; break;
            case SoundType.Swap:
                clipNum = (int)(UnityEngine.Random.value*2)+8; break;
            case SoundType.Rotate:
                clipNum = 10; break;
            default:
                clipNum = 11; break;
        }
        return instance.soundClips[clipNum];
    }
    public void PlayClickSound()
    {
        SoundManager.PlaySound(SoundType.Click);
    }
}