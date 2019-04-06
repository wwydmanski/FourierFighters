using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controls : MonoBehaviour
{
    private Rigidbody2D _rb;

    private const float MovementMultiplier = 10.0f;
    private const float JumpMultiplier = 1.0f;

    // Start is called before the first frame update
    void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        Move();
    }

    
    private void Move()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        float x = h * MovementMultiplier;
        float y = v * MovementMultiplier;

        Vector2 move = new Vector2(x, y);

        _rb.AddForce(move);

        if (Input.GetKey("space"))
        {
            Debug.Log("Spacebar");
            _rb.AddForce(Vector2.up*JumpMultiplier, ForceMode2D.Impulse);
        }
    }
}
