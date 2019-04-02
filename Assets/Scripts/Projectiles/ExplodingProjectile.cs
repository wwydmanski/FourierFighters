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

        void OnCollisionEnter(Collision collision)
        {
            if (Alive)
            {
                Debug.Log(collision.collider.name);
                Vector3 explosionPos = transform.position;
                Collider[] colliders = Physics.OverlapSphere(explosionPos, 5);
                foreach (Collider hit in colliders)
                {
                    Rigidbody rb = hit.GetComponent<Rigidbody>();

                    if (rb != null)
                        rb.AddExplosionForce(Equation.GetEnergy() * 10, explosionPos, 15);
                }

                Destroy(gameObject, 5);
                Destroy(GetComponent<BoxCollider>());
                Destroy(GetComponent<Rigidbody>());
                Destroy(GetComponent<ParticleSystem>());
                Alive = false;
                Instantiate(_explosionEffect, transform.position, Quaternion.identity);
            }
        }
    }
}
