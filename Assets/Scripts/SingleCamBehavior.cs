using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingleCamBehavior : MonoBehaviour
{
    private WeaponController gun;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (gun == null) {
            gun = GameObject.FindObjectOfType<WeaponController>();
        } else {
            transform.rotation = Quaternion.Euler(transform.rotation.x, gun.gameObject.transform.rotation.y, transform.rotation.z);
        }
    }
}
