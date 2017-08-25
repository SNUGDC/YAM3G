using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum SoundType { Check, Refill, MoveBS, Swap, Rotate, Click, Finale }
public enum MusicType { Normal }
public class SoundManager : MonoBehaviour
{
    public GameObject soundPlayer;
    public MusicPlayer musicPlayer;
    public AudioClip[] musicClips;
    public AudioClip[] soundClips;
    static SoundManager instance;
    public static Queue<GameObject> soundPlayers;
    public static List<GameObject> usingPlayers;
    static float aniTime { get { return BoardController.aniTime;} }
    void Awake()
    {
        instance = this;
        soundPlayers = new Queue<GameObject>();
        usingPlayers = new List<GameObject>();
    }
    void Start()
    {
        instance.musicPlayer.PlayMusic(ChooseMusic(MusicType.Normal));
    }
    public static void Initiate()
    {
        instance.musicPlayer.FadeInMusic(2*aniTime);
        usingPlayers.ForEach(spgo => spgo.GetComponent<SoundPlayer>().FadeOutSound(2*aniTime));
    }
    public static void PlaySound(SoundType type)
    {
        var clip = ChooseSound(type);
        if (soundPlayers.Count > 0)
        {
            var soundPlayerGO = soundPlayers.Dequeue();
            usingPlayers.Add(soundPlayerGO);
            soundPlayerGO.GetComponent<SoundPlayer>().PlaySound(clip);
        }
        else
        {
            var soundPlayerGO = Instantiate(instance.soundPlayer, instance.gameObject.transform);
            soundPlayerGO.GetComponent<SoundPlayer>().PlaySound(clip);
        }
    }
    static AudioClip ChooseSound(SoundType type)
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
            case SoundType.Finale:
                clipNum = 12; break;
            default:
                clipNum = 11; break;
        }
        return instance.soundClips[clipNum];
    }
    AudioClip ChooseMusic(MusicType type)
    {
        int clipNum;
        switch(type)
        {
            default:
                clipNum = 0; break;
        }
        return instance.musicClips[clipNum];
    }
    public void PlayClickSound()
    {
        PlaySound(SoundType.Click);
    }
    public static void PlayFinale()
    {
        PlaySound(SoundType.Finale);
        instance.musicPlayer.FadeOutMusic(aniTime*8);
    }

}