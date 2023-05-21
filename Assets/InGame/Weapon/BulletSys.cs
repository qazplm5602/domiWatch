using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletSys : MonoBehaviour
{
    public string CreatePlayer = null;
    public Vector3 Direction;

    private void Update() {
        // 이동
        transform.position += Direction * 50 * Time.deltaTime;

        // 회전
        transform.rotation = Quaternion.LookRotation(Direction);
    }
}
