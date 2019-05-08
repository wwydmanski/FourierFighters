using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Scripts.Equations;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace Assets.Scripts.Projectiles
{
    public abstract class Projectile : MonoBehaviour
    {
        protected Vector3 Origin;
        protected Vector3 Direction;
        protected Equation Equation;

        protected Transform Transform;
        protected int Frame;
        protected float TimeLeft = 100;
        protected GameObject ExplosionEffect;

        public int CollisionPriority = -1;
        public bool Alive = true;
        public abstract void Die();
        public abstract void ExternalCollide(Projectile collider, int order);

        public Guid Uuid;

        private float _offset;
        public Color Color;

        protected void Start()
        {
            Equation = new SinusEquation();
            gameObject.tag = "projectile";
            
            Transform = GetComponent<Transform>();
            Origin = transform.position;
            Uuid = Guid.NewGuid();


            GetComponentInChildren<TrailRenderer>().startColor = Color;
            GetComponentInChildren<TrailRenderer>().endColor = Color;

            _offset = (float) (UnityEngine.Random.value*Math.PI);
            var main = GetComponent<ParticleSystem>().main;
            main.startColor = Color.Lerp(Color, Color.white, 0.5f);
        }

        void Update()
        {
            if (Alive)
            {
                var calculatedPosition = Equation.GetPosition(Frame, _offset);
                var newPosition = new Vector3
                {
                    x = calculatedPosition.x * Direction.x + calculatedPosition.y * Direction.y,
                    y = calculatedPosition.x * Direction.y - calculatedPosition.y * Direction.x
                };

                Transform.position = Origin +  newPosition;

                Transform.rotation =
                    Quaternion.AngleAxis(Equation.GetDerivative(Frame, _offset) * 45, Vector3.forward);
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

        public void AddExplosionEffect(GameObject obj)
        {
            ExplosionEffect = obj;
        }
    }
}
