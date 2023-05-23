using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletSys : MonoBehaviour
{
    public string CreatePlayer = null;
    public GameObject BloodPrefeb;
    public Vector3 Direction;

    private void Update() {
        // 이동
        transform.position += Direction * 50 * Time.deltaTime;

        // 회전
        transform.rotation = Quaternion.LookRotation(Direction);
    }

    private void OnTriggerEnter(Collider other) {
        if (!other.gameObject.CompareTag("Player")) return;

        // 내가 쏜 총알이고 자기 자신이 맞은거면
        if (CreatePlayer == null && SpawnManager.instance.MyEntity == other.gameObject) return;
        // 총알 소환한 플레이어가 맞았다면
        if (CreatePlayer != null && SyncManager.PlayerEntity.TryGetValue(CreatePlayer, out var ownerPlayer) && ownerPlayer == other.gameObject) {
            return;
        }

        print($"총알맞았다 크크크 맞은놈: {other.gameObject.name} / 생성한놈 : {CreatePlayer}");

        //  피 튀기는 이펙
        GameObject BloodEffect = Instantiate(BloodPrefeb, transform.position, Quaternion.identity);

        // 피 튀기는 방향 설정 (얘는 튀기는쪽이 반대여야함)
        BloodEffect.transform.rotation = Quaternion.LookRotation(-Direction);

        // 자동 삭제ㅔㅔㅔ
        BloodEffect.AddComponent<AutoRemoveEntity>().DelayRemove = 1;
    }
}
