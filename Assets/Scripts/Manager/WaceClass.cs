using UnityEngine;

[System.Serializable] // ეს აუცილებელია, რომ ინსპექტორში გამოჩნდეს
public class Wave
{
    public string waveName;
    public GameObject enemyPrefab; // რომელი მტერი გაჩნდეს
    public int count;              // რამდენი მტერი გაჩნდეს ჯამში
    public float rate;             // რა ინტერვალით (წამებში)
}