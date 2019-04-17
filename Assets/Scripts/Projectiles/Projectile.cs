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
        protected Vector3 Direction;
        protected Equation Equation;

        protected Transform Transform;
        protected int Frame;
        protected float TimeLeft = 500;

        public bool Alive = true;
        public abstract void Die();
        public Guid Uuid;

        void Start()
        {
            Equation = new SinusEquation();
            gameObject.tag = "projectile";
            
            Transform = GetComponent<Transform>();
            Origin = transform.position;
            Uuid = Guid.NewGuid();
        }

        void Update()
        {
            if (Alive)
            {
                Transform.position = Origin + Equation.GetPosition(Frame) * Direction.x;
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

        public void SetDirection(Vector3 direction)
        {
            Direction = direction;
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

        protected void Destroy()
        {
            Alive = false;
            Destroy(gameObject, 5);
            Destroy(GetComponent<SphereCollider>());
            Destroy(GetComponent<Rigidbody>());
            Destroy(GetComponent<ParticleSystem>());
            GetComponentInChildren<TrailCollider>().Destroy();
        }
    }
}
