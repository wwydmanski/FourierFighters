using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Projectiles
{
    public class ExplodingProjectile : Projectile
    {
        private GameObject _explosionEffect;
        public void AddExplosionEffect(GameObject obj)
        {
            _explosionEffect = obj;
        }

        public override void Die()
        {
            var explosionPos = transform.position;
            Collider[] colliders = Physics.OverlapSphere(explosionPos, 15);
            foreach (Collider hit in colliders)
            {
                var rb = hit.GetComponent<Rigidbody>();

                if (rb != null)
                    rb.AddExplosionForce(Equation.GetEnergy() * 10, explosionPos, 15);
            }

            Destroy(gameObject, 5);
            Destroy(GetComponent<BoxCollider>());
            Destroy(GetComponent<Rigidbody>());
            Destroy(GetComponent<ParticleSystem>());
            ParticleSystem expl = Instantiate(_explosionEffect, transform.position, Quaternion.identity).GetComponent<ParticleSystem>();
            var main = expl.main;
            main.startSpeed = Equation.GetEnergy() / 10;

            Alive = false;
        }

        void OnCollisionEnter(Collision collision)
        {
            if (Alive)
            {
                Debug.Log(collision.collider.name);
                var colliderProjectile = collision.collider.gameObject.GetComponent<Projectile>();
                colliderProjectile?.Die();
                Die();
            }
        }
    }
}
