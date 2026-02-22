using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager inst;


    public List<AudioSource> allSfx = new List<AudioSource>();


    public AudioClip SFX_PRODUCE_CUBO;
    public AudioClip SFX_PRODUCE_BUBIL;

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


    public AudioSource PlaySfx(AudioClip c, float volume = 1)
    {
        GameObject o = new GameObject(c.name);
        AudioSource src = o.AddComponent<AudioSource>();
        src.clip = c;
        src.volume = totalVolume * sfxVolume * volume;
        src.loop = false;
        src.Play();
        Destroy(o, c.length);
        return src;
    }
}
