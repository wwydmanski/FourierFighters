using UnityEngine;

namespace Assets.Scripts.Projectiles
{
    public class ExplodingProjectile : Projectile
    {
        private const float RadiusBase = 15;
        private LineRenderer _lineDrawer;
        private Color? _explosionColor;
        private float _explosionPower = 1;

        // ReSharper disable once UnusedMember.Local
        private void OnCollisionEnter(Collision collision)
        {
            if (Alive)
            {
                Debug.Log(collision.collider.name);
                var colliderProjectile = collision.collider.gameObject.GetComponent<Projectile>();
                if (colliderProjectile == null)
                    colliderProjectile = collision.collider.gameObject.GetComponentInParent<Projectile>();

                if (colliderProjectile != null)
                    _explosionColor = Color.Lerp(Color, colliderProjectile.Color, 0.5f);

                if (colliderProjectile != null)
                {
                    if (colliderProjectile is ExplodingProjectile explodingProjectile)
                    {
                        explodingProjectile.Die(false);
                        _explosionPower = 2;
                    }
                    else
                        colliderProjectile.Die();
                }

                Die();
            }
        }

        public override void Die()
        {
            Die(true);
        }

        public void Die(bool explode)
        {
            if (Alive)
            {
                var realRadius = RadiusBase * Equation.GetEnergy() / 40;

                var explosion = new Explosion(gameObject.AddComponent<LineRenderer>(), transform);

                if (explode)
                {
                    Color expColor = Color.Lerp(_explosionColor ?? Color, Color.white, 0.5f);
                    explosion.DrawParticles(ExplosionEffect, Equation.GetEnergy() * _explosionPower, expColor);
                    explosion.Explode(realRadius, Equation.GetEnergy() * _explosionPower);
                }

                Destroy();
            }
        }
    }
}
