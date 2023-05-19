using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSyncMove : MonoBehaviour
{
    public float? LastMouseX = null;
    public float? LastMouseY = null;
    public Vector3? LastCoords = null;

    Vector3 CacheEuler;
    PlayerInfo EntityInfo;

    [SerializeField, Tooltip("팔이 회전할때 클수록 더 부드러워짐")]
    float rotationSpeed = 40f; // 회전 속도
    [SerializeField, Tooltip("캐릭터가 움직일때 클수록 더 반영이 빠르지만 너무 드드득 하지롱")]
    float MoveSpeed = 25f;

    private void Awake() {
        EntityInfo = GetComponent<PlayerInfo>();
    }

    private void Update() {
        // XXX
        if (LastMouseX != null) {
            CacheEuler = transform.localEulerAngles;
            CacheEuler.y = Mathf.LerpAngle(CacheEuler.y, LastMouseX.Value, Time.deltaTime * rotationSpeed);
            transform.localEulerAngles = CacheEuler;
        }

        // YYYY
        if (LastMouseY != null) {
            CacheEuler = EntityInfo.HandHandler.transform.localEulerAngles;
            CacheEuler.x = Mathf.LerpAngle(CacheEuler.x, LastMouseY.Value, Time.deltaTime * rotationSpeed);
            EntityInfo.HandHandler.transform.localEulerAngles = CacheEuler;
        }

        // 좝좝표 (여기는 오히려 더 부드럽게 해야함 [너무 빠르게 반영되면 드드드드드드드득])
        if (LastCoords != null) {
            transform.position = Vector3.Lerp(transform.position, LastCoords.Value, Time.deltaTime * MoveSpeed);
        }
    }
}
