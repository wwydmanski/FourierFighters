using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Scripts.Equations;
using UnityEngine;

namespace Assets.Scripts.Projectiles
{
    public abstract class Projectile : MonoBehaviour
    {
        protected Vector3 Origin;
        protected bool Alive = true;
        protected Equation Equation;

        protected Transform Transform;
        protected int Frame;
        protected float TimeLeft = 500;

        public abstract void Die();

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
            if (Alive)
            {
                Frame++;
                TimeLeft -= Equation.Freq/15;
                if (TimeLeft <= 0)
                    Die();
            }
        }

        public void SetEquation(Equation eq)
        {
            Equation = eq;
            Debug.Log("Changed equation");
        }

        public Equation GetEquation()
        {
            return Equation;
        }

        public Projectile ChangeType<TLatter>() 
            where TLatter : Projectile
        {
            Destroy(GetComponent<Projectile>());
            return gameObject.AddComponent<TLatter>();
        }

        public void Attenuate(float db)
        {
            this.Equation.Attenuate(db);
        }

    }
}
