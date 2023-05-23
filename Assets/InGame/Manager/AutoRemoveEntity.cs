using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoRemoveEntity : MonoBehaviour
{
    public float DelayRemove;

    private void Start() => StartCoroutine(Removeeeeee());

    IEnumerator Removeeeeee() {
        yield return new WaitForSeconds(DelayRemove);
        Destroy(gameObject);
    }
}
