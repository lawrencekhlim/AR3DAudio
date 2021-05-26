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

        AudioSeekManager.Instance.setTracks (song);

    }
}
