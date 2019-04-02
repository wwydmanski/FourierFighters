namespace Assets.Scripts.Equations
{
    public abstract class IEquation
    {
        public float freq = 20;
        public abstract UnityEngine.Vector3 GetPosition(int frame);
        public abstract float GetEnergy();

        public float GetDerivative(int frame)
        {
            float res = (GetPosition(frame + 1).y - GetPosition(frame - 1).y) / 2;
            return res;
        }
    }
}
