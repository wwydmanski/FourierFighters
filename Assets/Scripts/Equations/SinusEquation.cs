using System;
using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Equations;
using UnityEngine;

public class SinusEquation : IEquation
{
    private float amplitude = 1.0f;
    //public float freq = 60;
    private float speed = 0.2f;

    private float freqReduction = 80;

    public SinusEquation()
    {

    }

    public SinusEquation(float freq)
    {
        this.freq = freq;
    }

    public override Vector3 GetPosition(int frame)
    {
        float x = (float)(frame * speed);
        float y = (float)(amplitude * Math.Sin(x * freq * Math.PI / freqReduction));

        return new Vector3(x, y);
    }

    public override float GetEnergy()
    {
        return freq;
    }
}
