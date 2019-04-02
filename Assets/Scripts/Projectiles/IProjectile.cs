using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Scripts.Equations;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Assets.Scripts.Projectiles
{
    public abstract class Projectile : MonoBehaviour
    {
        protected Vector3 Origin;
        protected bool Alive = true;
        protected IEquation Equation;

        protected Transform Transform;
        protected int Frame;

        void Start()
        {
            Equation = new SinusEquation();
            Transform = GetComponent<Transform>();
            Origin = transform.position;
        }

        void Update()
        {
            if (Alive)
            {
                Transform.position = Origin + Equation.GetPosition(Frame);
                Transform.rotation =
                    Quaternion.AngleAxis(Equation.GetDerivative(Frame) * 45, Vector3.forward);
            }
        }

        void FixedUpdate()
        {
            Frame++;
        }

        public void SetEquation(IEquation eq)
        {
            Equation = eq;
            Debug.Log("Changed equation");
        }

        public Projectile ChangeType<TLatter>() 
            where TLatter : Projectile
        {
            Destroy(GetComponent<Projectile>());
            return gameObject.AddComponent<TLatter>();
        }
    }
}
