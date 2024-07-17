using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    Rigidbody _rb;
    // Start is called before the first frame update
    void Awake()
    {
        _rb = GetComponent<Rigidbody>();
       
    }

    void SetOff()
    {
        gameObject.SetActive(false);
        _rb.velocity = Vector3.zero;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.CompareTag("Finish"))
        {
            FindObjectOfType<Target>().SetEnemy(collision.gameObject);
          
        }

        SetOff();
    }
    private void OnTriggerEnter(Collider other)
    {
        SetOff();
    }

    public void Fire(Vector3 v, float Speed)
    {
        Vector3 direction = (v-transform.position).normalized;

        if (v == Vector3.zero) direction = Vector3.forward;

        _rb.AddForce(direction*Speed, ForceMode.Impulse);
         Invoke(nameof(SetOff), 2);
    }

    
}
