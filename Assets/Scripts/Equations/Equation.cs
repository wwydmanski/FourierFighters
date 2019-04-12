namespace Assets.Scripts.Equations
{
    public abstract class Equation
    {
        public float Freq = 20;
        public abstract UnityEngine.Vector3 GetPosition(int frame);
        public abstract float GetEnergy();

        protected readonly float CalibrationFreq = 30;

        public abstract void Attenuate(float db);

        public float GetDerivative(int frame)
        {
            float res = (GetPosition(frame + 1).y - GetPosition(frame - 1).y) / 2;
            return res;
        }

    }
}
