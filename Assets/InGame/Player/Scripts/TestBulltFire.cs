using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestBulltFire : MonoBehaviour
{
    [SerializeField] GameObject BulletEntity;
    Transform firePos;
    Vector3 dir;
    Ray ray;
    private void Start() {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    private void Update() {
        if (Input.GetMouseButtonDown(0)) {
            Shot();
        }

        // Debug.DrawRay(firePos.position, dir, Color.red);
        Debug.DrawRay(firePos.position, ray.direction * 5,  Color.red);
    }

    void Shot() {
// 총구 위치 찾기
        firePos = GameObject.Find("ShotgunFirePos").transform;
        dir = Camera.main.ScreenPointToRay(Input.mousePosition).direction;
        ray = new Ray(firePos.position, dir);
        
        StartCoroutine(GOgogo(ray.direction, firePos.position));
    }

    IEnumerator GOgogo(Vector3 direction, Vector3 adadad) {
        GameObject Bullet = Instantiate(BulletEntity, adadad, Quaternion.identity);
        
        while (true) {
            yield return null;

            Bullet.transform.position += direction * 50 * Time.deltaTime;
        }
    }
}
