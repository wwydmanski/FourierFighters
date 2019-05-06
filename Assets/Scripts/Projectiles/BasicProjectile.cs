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

    // ReSharper disable once UnusedMember.Local
    private void OnCollisionEnter(Collision collision)
    {
        if (Alive)
        {
            Debug.Log(collision.collider.name);
            var colliderProjectile = collision.collider.gameObject.GetComponent<Projectile>();

            var dir = Direction;
            dir.y = Math.Abs(dir.y) < 0.01 ? 100 : dir.y;

            var controller = collision.collider.gameObject.GetComponent<PlayerController.PlayerController>();
            if(controller != null)
                controller.ProjectileHit(Direction * this.Equation.GetEnergy() * _energyCoeff);
            else
                collision.rigidbody?.AddForce(Direction * this.Equation.GetEnergy() * _energyCoeff,
                ForceMode.Impulse);
            Die();
        }
    }

    public override void Die()
    {
        if (Alive)
        {
            Destroy();
        }
    }
}
