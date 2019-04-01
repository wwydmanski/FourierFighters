using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SignalCaster : MonoBehaviour
{
    public GameObject Projectile;
    private BasicProjectile _currentProjectile;

    // Start is called before the first frame update
    void Start()
    {
        _currentProjectile = Instantiate(Projectile, transform.position+Vector3.right, Quaternion.identity).GetComponent<BasicProjectile>();
        StartCoroutine(Cast(20));
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator Cast(float freq)
    {
        yield return new WaitForSecondsRealtime(1);
        _currentProjectile = Instantiate(Projectile, transform.position + Vector3.right, Quaternion.identity).GetComponent<BasicProjectile>();
        yield return new WaitForSecondsRealtime(0.01f);
        _currentProjectile.SetEquation(new SinusEquation(freq));
    }
}
