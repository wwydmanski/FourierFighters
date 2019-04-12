using System;
using UnityEngine;

namespace Assets.Scripts.Equations
{
    public class SinusEquation : Equation
    {
        private float amplitude = 1.0f;
        //public float freq = 60;
        private float speed = 0.2f;

        private float freqReduction = 80;
        private float energyMultiplier = 30;

        public SinusEquation()
        {

        }

        public SinusEquation(float freq)
        {
            this.Freq = freq;
            this.amplitude = freq / CalibrationFreq;
        }

        public override Vector3 GetPosition(int frame)
        {
            float x = (float)(frame * speed);
            float y = (float)(amplitude * Math.Sin(x * Freq * Math.PI / freqReduction));

            return new Vector3(x, y);
        }

        public override float GetEnergy()
        {
            return amplitude*amplitude/2 * energyMultiplier;
        }

        public override void Attenuate(float db)
        {
            amplitude /= (float)Math.Pow(10, db/10);
        }
    }
}
