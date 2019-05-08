using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Projectiles;
using TMPro;
using UnityEngine;

public class Filter : MonoBehaviour
{
    public int BandStart = 10;
    public int Attenuation = 10;

    private int _bandWidth = 20;

    // Start is called before the first frame update
    void Start()
    {
        GetComponentInChildren<TextMeshPro>().text = $"{BandStart}-{BandStart + _bandWidth} Hz";
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter(Collider other)
    {
        Projectile proc = other.GetComponent<Projectile>();

        if (proc != null)
        {
            float freq = proc.GetEquation().Freq;
            if (BandStart <= freq && freq < (BandStart + _bandWidth))
                proc.Attenuate(Attenuation);
        }
    }
}
