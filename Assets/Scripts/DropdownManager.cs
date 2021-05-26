using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropdownManager : MonoBehaviour
{
    //public string song = "(Look Out) She's America - Otis McDonald";
    public string song = "Gemini Robot - Bird Creek";
    //public AudioSeekManager audioSeekManager;


    public void Changed(int val)
    {
        Debug.Log(val);
        AudioSeekManager.Instance.setSongPosition(0);
        // code below kind of works, needs try except in case instrument is not out, and clip seems to have issues
        // however, if all instruments are present it can pause all of them at once
        if(val == 0)
        {
            song = "(Look Out) She's America - Otis McDonald";
        }
        else if(val == 1)
        {
            song = "Blood, Sweat, No Tears - Jeremy Korpas";
        }
        else if(val == 2)
        {
            song = "Do or Die - Dougie Wood";
        }
        else if(val == 3)
        {
            song = "Ever Felt pt.3 - Otis McDonald";
        }
        else if(val == 4)
        {
            song = "Game Set Match - Riot";
        }
        else if(val == 5)
        {
            song = "Gemini Robot - Bird Creek";
        }
        else if(val == 6)
        {
            song = "Here, If You're Going - Otis McDonald";
        }
        else if(val == 7)
        {
            song = "La, La, La - Otis McDonald";
        }
        else if(val == 8)
        {
            song = "Life is Good - Magic In The Other";
        }
        else if(val == 9)
        {
            song = "Mariachiando - Doug Maxwell_Jimmy Fontanez";
        }
        else if(val == 10)
        {
            song = "O Christmas Tree (Vocals) - Jingle Punks";
        }
        else if(val == 11)
        {
            song = "Poured - Lauren Duski";
        }
        else if(val == 12)
        {
            song = "Road Tripzzz - Ofshane";
        }
        var clip = Resources.Load(song) as AudioClip;

        string[] instrument_names = new string[] { "bass", "piano", "drums", "vocals", "other" };
        for(int i=0; i < 5; i++)
        {
            string instrument_tag = "Instrument" + (i+1).ToString();
            string instrument_name = instrument_names[i];
            if (GameObject.FindGameObjectsWithTag(instrument_tag).Length != 0)
            {
                AudioSource audio_left = GameObject.FindGameObjectWithTag(instrument_tag).GetComponents<AudioSource>()[0];
                AudioSource audio_right = GameObject.FindGameObjectWithTag(instrument_tag).GetComponents<AudioSource>()[1];
                audio_left.Pause();
                audio_right.Pause();

                audio_left.clip = Resources.Load("allOutput/" + song + "/" + instrument_name + "_left") as AudioClip;
                audio_right.clip = Resources.Load("allOutput/" + song + "/" + instrument_name + "_right") as AudioClip;

                if (audio_left.clip == null)
                {
                    Debug.Log("Audio" + (i+1).ToString() + "_left was null");
                    audio_left.clip = clip;
                }
                if (audio_right.clip == null)
                {
                    Debug.Log("Audio" + (i + 1).ToString() + "_right was null");
                    audio_right.clip = clip;
                }
                audio_left.Play();
                audio_right.Play();
            }
        }
    }
}
