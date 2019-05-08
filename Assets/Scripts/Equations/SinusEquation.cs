using System;
using UnityEngine;

namespace Assets.Scripts.Equations
{
    public class SinusEquation : Equation
    {
        private float _amplitude;

        private float _displayAmplitudeCoeff = 0.2f;
        //public float freq = 60;
        private float speed = 0.3f;

        private float freqReduction = 40;
        private float energyMultiplier = 40;

        public SinusEquation()
        {

        }

        public SinusEquation(float freq)
        {
            this.Freq = freq;
            this._amplitude = freq / CalibrationFreq;
        }

        public override Vector3 GetPosition(int frame, float offset)
        {
            var x = frame * speed;
            var y = (float)(_displayAmplitudeCoeff*_amplitude * Math.Sin(offset - x * Freq * Math.PI / freqReduction));

            return new Vector3(x, y);
        }

        public override float GetEnergy()
        {
            return _amplitude*_amplitude/2 * energyMultiplier;
        }

        public override void Attenuate(float db)
        {
            _amplitude /= (float)Math.Pow(10, db/10);
        }
    }
}
