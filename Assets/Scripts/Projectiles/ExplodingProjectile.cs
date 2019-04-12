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
        private float RadiusBase = 15;
        private LineRenderer _lineDrawer;


        public void AddExplosionEffect(GameObject obj)
        {
            _explosionEffect = obj;
        }

        private Vector3 GetRcastCollision(Vector3 pos2)
        {
            if (Physics.Linecast(transform.position, pos2, out var hit))
            {
                if (hit.transform.tag == "wall")
                {
                    Debug.DrawLine(transform.position, hit.point,
                        Color.yellow);
                    return hit.point;
                }
                else
                {
                    RaycastHit[] hits;
                    hits = Physics.RaycastAll(transform.position, hit.point - transform.position, (pos2-transform.position).magnitude);
                    foreach (var raycastHit in hits)
                    {
                        if (raycastHit.transform.tag == "wall")
                            return raycastHit.point;
                    }
                }
            }

            Debug.DrawLine(transform.position, pos2, Color.white);
            return pos2;
        }

        private void DrawRadius(float radius)
        {
            float theta = 0f;
            float thetaScale = 0.01f;

            int size = (int)((1f / thetaScale) + 1f);
            _lineDrawer = gameObject.AddComponent<LineRenderer>();
            _lineDrawer.widthMultiplier = 0.1f;

            _lineDrawer.positionCount = size;
            for (int i = 0; i < size; i++)
            {
                theta += (2.0f * Mathf.PI * thetaScale);
                float x = radius * Mathf.Cos(theta);
                float y = radius * Mathf.Sin(theta);

                Vector3 obstruction =
                    GetRcastCollision(new Vector3(transform.position.x + x, transform.position.y + y, 0));

                _lineDrawer.SetPosition(i, obstruction);
            }
        }

        public override void Die()
        {
            var explosionPos = transform.position;
            var realRadius = RadiusBase * Equation.GetEnergy() / 40;
            DrawRadius(realRadius);

            Collider[] colliders = Physics.OverlapSphere(explosionPos, realRadius);
            foreach (Collider hit in colliders)
            {
                var rb = hit.GetComponent<Rigidbody>();
                if (rb != null && Physics.Linecast(transform.position, rb.position, out var lineOfSightObstructor))
                {
                    if (lineOfSightObstructor.transform.tag != "wall")
                    {
                        rb.AddExplosionForce(Equation.GetEnergy() * 10, explosionPos, realRadius);
                    }
                }
            }

            Destroy(gameObject, 5);
            Destroy(GetComponent<BoxCollider>());
            Destroy(GetComponent<Rigidbody>());
            Destroy(GetComponent<ParticleSystem>());

            ParticleSystem expl = Instantiate(_explosionEffect, transform.position, Quaternion.identity).GetComponent<ParticleSystem>();
            var main = expl.main;
            main.maxParticles = (int) (main.maxParticles * Equation.GetEnergy() * 0.01);
            main.startSpeed = Equation.GetEnergy();

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
