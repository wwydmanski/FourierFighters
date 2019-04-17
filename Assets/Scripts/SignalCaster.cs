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
        private Quaternion _direction;
        private Vector3 _offset;
        private float lastRotation;

        // Start is called before the first frame update
        void Start()
        {
            _transform = GetComponent<Transform>();
            StartCoroutine(Cast(30, 0));

            StartCoroutine(CastExploding(60, 2));
            //_direction = Vector3.right * (float)(Math.Cos(transform.rotation.y * Math.PI));
            _direction = Quaternion.identity;
            _direction.x = (float) (Math.Cos(_transform.rotation.y * Math.PI));
            _offset = Vector3.right * (float)(Math.Cos(_transform.rotation.y * Math.PI));
            lastRotation = Time.time;
        }

        // Update is called once per frame
        void Update()
        {
            if (Rotating)
            {
                if (Time.time - lastRotation > 1)
                {
                    lastRotation = Time.time;
                    _direction = Quaternion.Euler(0, 0, 90) * _direction;
                    gameObject.transform.rotation = _direction;
                    StartCoroutine(Cast(20, 0));
                }
            }
        }

        IEnumerator Cast(float freq, int waitTime)
        {
            yield return new WaitForSecondsRealtime(waitTime);
            Debug.Log(_direction);

            _currentProjectile = Instantiate(Projectile, transform.position + _offset, Quaternion.identity).GetComponent<Projectile>();
            yield return new WaitForSecondsRealtime(0.01f);
            _currentProjectile.SetEquation(new SinusEquation(freq));
            _currentProjectile.SetDirection(_direction);
        }

        IEnumerator CastExploding(float freq, int waitTime)
        {
            yield return new WaitForSecondsRealtime(waitTime);

            _currentProjectile = Instantiate(Projectile, transform.position + _offset, Quaternion.identity).GetComponent<Projectile>();
            _currentProjectile = _currentProjectile.ChangeType<ExplodingProjectile>();
            yield return new WaitForSecondsRealtime(0.01f);
            _currentProjectile.SetEquation(new SinusEquation(freq));
            _currentProjectile.SetDirection(_direction);

            (_currentProjectile as ExplodingProjectile)?.AddExplosionEffect(ExplosionEffect);
        }
    }
}
