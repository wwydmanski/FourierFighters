using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Projectiles;
using UnityEngine;

public class SignalCaster : MonoBehaviour
{
    public GameObject Projectile;
    public GameObject ExplosionEffect;
    private Projectile _currentProjectile;

    // Start is called before the first frame update
    void Start()
    {
        _currentProjectile = Instantiate(Projectile, transform.position+Vector3.right, Quaternion.identity).GetComponent<Projectile>();
        StartCoroutine(Cast(100));
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator Cast(float freq)
    {
        yield return new WaitForSecondsRealtime(1);

        _currentProjectile = Instantiate(Projectile, transform.position + Vector3.right, Quaternion.identity).GetComponent<Projectile>();
        _currentProjectile = _currentProjectile.ChangeType<ExplodingProjectile>();
        yield return new WaitForSecondsRealtime(0.01f);
        _currentProjectile.SetEquation(new SinusEquation(freq));
        (_currentProjectile as ExplodingProjectile).AddExplosionEffect(ExplosionEffect);
    }
}
