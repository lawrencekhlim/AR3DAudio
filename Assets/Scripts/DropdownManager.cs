using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropdownManager : MonoBehaviour
{
    //private bool justChanged = false;

    public void Changed(int val)
    {
        Debug.Log(val);
        // code below kind of works, needs try except in case instrument is not out, and clip seems to have issues
        // however, if all instruments are present it can pause all of them at once
        // AudioSource audio1 = GameObject.FindGameObjectWithTag("Instrument1").GetComponent<AudioSource>();
        // Debug.Log(audio1.ToString());
        // AudioSource audio2 = GameObject.FindGameObjectWithTag("Instrument2").GetComponent<AudioSource>();
        // Debug.Log(audio2.ToString());
        // AudioSource audio3 = GameObject.FindGameObjectWithTag("Instrument3").GetComponent<AudioSource>();
        // Debug.Log(audio3.ToString());
        // AudioSource audio4 = GameObject.FindGameObjectWithTag("Instrument4").GetComponent<AudioSource>();
        // Debug.Log(audio4.ToString());
        // AudioSource audio5 = GameObject.FindGameObjectWithTag("Instrument5").GetComponent<AudioSource>();
        // Debug.Log(audio5.ToString());
        // var clip = Resources.Load("Do or Die - Dougie Wood.mp3") as AudioClip;
        // audio1.Pause();
        // audio2.Pause();
        // audio3.Pause();
        // audio4.Pause();
        // audio5.Pause();
        // audio1.clip = clip;
        // audio2.clip = clip;
        // audio3.clip = clip;
        // audio4.clip = clip;
        // audio5.clip = clip;
        // audio1.Play();
        // audio2.Play();
        // audio3.Play();
        // audio4.Play();
        // audio5.Play();
    }
}
