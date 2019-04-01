using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SignalCaster : MonoBehaviour
{
    public GameObject Projectile;
    private GameObject _currentProjectile;

    // Start is called before the first frame update
    void Start()
    {
        _currentProjectile = Instantiate(Projectile, transform.position, Quaternion.identity);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
