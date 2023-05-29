using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class PlayerDamagePacketForm {
    public string AttackID;
    public bool isDie;

    public PlayerDamagePacketForm(string _id, bool _isDie) {
        AttackID = _id;
        isDie = _isDie;
    }
}

public class BulletSys : MonoBehaviour
{
    public string CreatePlayer = null;
    public GameObject BloodPrefeb;
    public Vector3 Direction;
    public int Damage;

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

        // 총알 맞은 플레이어가 나
        if (SpawnManager.instance.MyEntity == other.gameObject && !PlayerHealth.instance.isDie) {
            PlayerHealth.instance.health -= Damage;
            if (PlayerHealth.instance.isDie) { // 죽었다!!
                print($"{CreatePlayer}님이 당신을 처치하였습다");
            }

            // 서버한테 피해 받았다고 알리면서 죽었는지도 알려줌
            NetworkCore.Send("Room.PlayerDamage", new PlayerDamagePacketForm(CreatePlayer, PlayerHealth.instance.isDie));
        }
    }
}
