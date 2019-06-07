using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Assets.Scripts;
using Assets.Scripts.Equations;
using Assets.Scripts.Projectiles;
using UnityEngine;

class BasicProjectile : Projectile
{

    private float _energyCoeff = 1f;

    protected new void Start()
    {
        base.Start();
        CollisionPriority = 0;
    }

    // ReSharper disable once UnusedMember.Local
    private void OnCollisionEnter(Collision collision)
    {
        if (Alive)
        {
            var colliderProjectile = collision.collider.gameObject.GetComponent<Projectile>();
            if (colliderProjectile == null)
            {
                if(collision.collider.tag!="wall")
                    Debug.Log(collision.collider.gameObject.name);
                colliderProjectile = collision.collider.gameObject.GetComponentInParent<Projectile>();
            }

            var dir = Direction;
            dir.y = Math.Abs(dir.y) < 0.01 ? 100 : dir.y;

            var controller = collision.collider.gameObject.GetComponent<PlayerController.PlayerController>();
            if(controller != null)
                controller.ProjectileHit(Direction * this.Equation.GetEnergy() * _energyCoeff);
            else
                collision.rigidbody?.AddForce(Direction * this.Equation.GetEnergy() * _energyCoeff,
                ForceMode.Impulse);

            if (colliderProjectile != null)
                Explode(colliderProjectile);

            Die();
        }
    }

    private void Explode(Projectile colliderProjectile)
    {
        var explosion = new Explosion(transform);
        if (colliderProjectile != null)
            explosion.DrawParticles(ExplosionEffect, Equation.GetEnergy(),
                Color.Lerp(Color, colliderProjectile.Color, 0.5f));
    }

    public override void Die()
    {
        if (Alive)
        {
            Destroy();
        }
    }

    public override void ExternalCollide(Projectile collider, int order)
    {
        if (CollisionPriority > collider.CollisionPriority || ((CollisionPriority == collider.CollisionPriority) && order == 0))
            Explode(collider.GetComponent<Projectile>());
        Die();
    }
}
