using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Projectiles;
using UnityEngine;

public class Filter : MonoBehaviour
{
    public int BandStart;

    private int _bandWidth = 20;

    private int decibels = 10;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter(Collider other)
    {
        Projectile proc = other.GetComponent<Projectile>();
        Debug.Log(other.name);

        if (proc != null)
        {
            float freq = proc.GetEquation().Freq;
            if (BandStart <= freq && freq < (BandStart + _bandWidth))
                proc.Attenuate(decibels);
        }
    }
}
