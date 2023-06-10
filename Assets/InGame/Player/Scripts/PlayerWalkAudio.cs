using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWalkAudio : MonoBehaviour
{
    [SerializeField] AudioClip[] WalkSounds;
    AudioSource audioSource;

    bool _isWalk = false;
    public bool isWalk {
        get => _isWalk;
        set {
            _isWalk = value;
            if (value) StartSound();
        }
    }
    
    int PlayIndex = 0;
    // int PlayTime = 0;

    private void Awake() {
        audioSource = transform.Find("AudioBlock")?.GetComponent<AudioSource>();
    }

    void StartSound() {
        if (audioSource == null || audioSource.isPlaying) return; // 이미 재생중이라면 안함 (겹쳐서 재생되면 안됨ㅁㅁㅁ)

        audioSource.PlayOneShot(WalkSounds[PlayIndex]);

        // index 올리기
        PlayIndex ++;
        if (PlayIndex >= WalkSounds.Length) PlayIndex = 0;

        StartCoroutine(RepeatSound());
    }

    IEnumerator RepeatSound() {
        yield return new WaitUntil(() => !audioSource.isPlaying);
        ///////// 발소리 끝남 /////////

        if (isWalk)  // 아직도 걷는 중이면
            StartSound();
    }
}
