using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class SoundManager : MonoBehaviour
{
    public static SoundManager inst;


    public List<AudioSource> allSfx = new List<AudioSource>();

    public List<AudioSource> backgroundSoundProfile = new List<AudioSource>();
    public List<AudioSource> backgroundSoundsPermanent = new List<AudioSource>();

    public AudioSource overlappedSound = null;


    [Range(0,1)]
    public float sfxVolume = 1;

    [Range(0, 1)]
    public float musicVolume = 1;
    
    [Range(0, 1)]
    public float totalVolume = 1;


    void Start()
    {
        if (inst != null)
        {
            Destroy(gameObject);
        }
        inst = this;
        allSfx = new List<AudioSource>();
    }


    public AudioSource PlaySfx(AudioClip c, float volume = 1, float minPitch = 1, float maxPitch = 1, bool isOverlapped = false)
    {
        GameObject o = new GameObject(c.name);
        o.transform.parent = this.transform;
        AudioSource src = o.AddComponent<AudioSource>();
        src.clip = c;
        src.volume = totalVolume * sfxVolume * volume;
        src.loop = false;
        src.pitch = Random.Range(minPitch, maxPitch);
        src.Play();
        Destroy(o, c.length);
        if (isOverlapped)
        {
            if (overlappedSound != null)
            {
                Destroy(overlappedSound);
            }
            overlappedSound = src;
        }
        return src;
    }

    public void AddBackground(AudioClip c, float volume = 1, float pitch = 1, bool isPermanent = false)
    {
        GameObject o = new GameObject(c.name);
        o.transform.parent = this.transform;
        AudioSource src = o.AddComponent<AudioSource>();
        src.clip = c;
        src.loop = true;
        src.pitch = pitch;
        src.volume = totalVolume * musicVolume * volume;
        src.Play();
        if (isPermanent)
        {
            backgroundSoundsPermanent.Add(src);
        }
        else
        {
            backgroundSoundProfile.Add(src);
        }
    }

    public IEnumerator ChangeBackgroundProfile(AudioClip[] c, float[] volume, float[] pitch, float fadingTime = 1f)
    {
        if (backgroundSoundProfile.Count != 0)
        {
            Tween tw = null;
            for (int i = 0; i < backgroundSoundProfile.Count; ++i)
            {
                tw = backgroundSoundProfile[0].DOFade(0, fadingTime).SetEase(Ease.InQuad);
            }
            yield return tw.WaitForCompletion();
        }
        for (int i = 0; i < backgroundSoundProfile.Count; ++i)
        {
            Destroy(backgroundSoundProfile[i]);
        }
        backgroundSoundProfile.Clear();
        for (int i = 0; i < c.Length; ++i)
        {
            AddBackground(c[i], 0, pitch[i]);
            backgroundSoundProfile[i].DOFade(volume[i] * musicVolume * totalVolume, fadingTime).SetEase(Ease.OutQuad);
        }
    }
}
