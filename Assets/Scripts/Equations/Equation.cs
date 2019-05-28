namespace Assets.Scripts.Equations
{
    public abstract class Equation
    {
        public float Freq = 20;
        public abstract UnityEngine.Vector3 GetPosition(int frame, float offset);
        public abstract float GetEnergy();

        protected readonly float CalibrationFreq = 30;

        public abstract void Attenuate(float db);

        public float GetDerivative(int frame, float offset)
        {
            float res = (GetPosition(frame + 1, offset).y - GetPosition(frame - 1, offset).y) / 2;
            return res;
        }

    }
}
