using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BloodAutoRemove : MonoBehaviour
{
    float LastTime = 0;

    // Update is called once per frame
    void Update()
    {
        LastTime += Time.deltaTime;
        if (LastTime <= 1) return;

        Destroy(gameObject);
    }
}
