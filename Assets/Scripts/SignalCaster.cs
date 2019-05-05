using System;
using System.Collections;
using Assets.Scripts.Equations;
using Assets.Scripts.Projectiles;
using UnityEngine;

namespace Assets.Scripts
{
    public class SignalCaster : MonoBehaviour
    {
        public GameObject Projectile;
        public GameObject ExplosionEffect;

        public bool Rotating;
        private Projectile _currentProjectile;
        private Transform _transform;
        private Vector3 _direction;
        private float _lastRotation;
        private float _offsetMult = 1;

        // ReSharper disable once UnusedMember.Local
        private void Start()
        {
            _transform = GetComponent<Transform>();
            _direction = Vector3.right;
            _direction.x = (float) (Math.Cos(_transform.rotation.y * Math.PI));

//            StartCoroutine(Cast(30, 0, _direction, false));
//            StartCoroutine(Cast(60, 2, _direction, true));
            CastRight(30, true, 0);
            _lastRotation = Time.time;
            _offsetMult = GetComponent<Collider>().bounds.size.magnitude;
        }

        // ReSharper disable once UnusedMember.Local
        private void Update()
        {
            _offsetMult = GetComponent<Collider>().bounds.size.magnitude;
            Debug.Log("Offset mult: "+_offsetMult);
            if (Rotating)
            {
                if (Time.time - _lastRotation > 1)
                {
                    _lastRotation = Time.time;
                    _direction = Quaternion.Euler(0, 0, 90) * _direction;
                    Debug.DrawRay(_transform.position, _direction,
                        Color.yellow, 0.5f);

                    StartCoroutine(Cast(20, 0, _direction, false));
                }
            }
        }

        public void CastRight(float freq, bool exploding=false, float waitTime=0)
        {
            StartCoroutine(Cast(freq, waitTime, Vector3.right, exploding));
        }

        public void CastLeft(float freq, bool exploding = false, float waitTime = 0)
        {
            StartCoroutine(Cast(freq, waitTime, Vector3.left, exploding));
        }

        public void CastUp(float freq, bool exploding = false, float waitTime = 0)
        {
            StartCoroutine(Cast(freq, waitTime, Vector3.up, exploding));
        }

        public void CastDown(float freq, bool exploding = false, float waitTime = 0)
        {
            StartCoroutine(Cast(freq, waitTime, Vector3.down, exploding));
        }

        private IEnumerator Cast(float freq, float waitTime, Vector3 direction, bool exploding)
        {
            yield return new WaitForSecondsRealtime(waitTime);

            _currentProjectile = Instantiate(Projectile, transform.position + direction*_offsetMult, Quaternion.identity).GetComponent<Projectile>();

            if(exploding)
                _currentProjectile = _currentProjectile.ChangeType<ExplodingProjectile>();

            yield return new WaitForSecondsRealtime(0.01f);
            _currentProjectile.SetEquation(new SinusEquation(freq));
            _currentProjectile.SetDirection(direction);

            if (exploding)
                (_currentProjectile as ExplodingProjectile)?.AddExplosionEffect(ExplosionEffect);
        }
    }
}
