using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInfoData {
    public string Id = null;
    public double[] Coords = new double[3];
    public double MouseX;
    public double MouseY;

    public PlayerInfoData(Vector3 _coord, double x, double y) {
        Coords[0] = _coord.x;
        Coords[1] = _coord.y;
        Coords[2] = _coord.z;

        MouseX = x;
        MouseY = y;
    }
}

public class PlayerMovement : MonoBehaviour
{
    public float speed;      // 캐릭터 움직임 스피드.
    public float jumpSpeed; // 캐릭터 점프 힘.
    public float gravity;    // 캐릭터에게 작용하는 중력.
    public float mouseSensitivity = 3f;
 
    private CharacterController controller; // 현재 캐릭터가 가지고있는 캐릭터 컨트롤러 콜라이더.
    PlayerWalkAudio walkAudio;
    private Vector3 MoveDir;                // 캐릭터의 움직이는 방향.
    private float verticalRotation = 0f;

    // 서버 업데이트 기록
    Vector3 SaveCoords;
    float SaveX;
    float SaveY;
    
    private void Awake() {
        controller = GetComponent<CharacterController>();
        walkAudio = GetComponent<PlayerWalkAudio>();
        speed     = 6.0f;
        jumpSpeed = 8.0f;
        gravity   = 20.0f;
 
        MoveDir = Vector3.zero;
    }
    
    void Update()
    {
        // 현재 캐릭터가 땅에 있는가?
        if (controller.isGrounded)
        {
            // 위, 아래 움직임 셋팅. 
            MoveDir = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
            if (PlayerHealth.instance.isDie || Cursor.lockState == CursorLockMode.None)
                MoveDir = Vector3.zero;
 
            // 벡터를 로컬 좌표계 기준에서 월드 좌표계 기준으로 변환한다.
            MoveDir = transform.TransformDirection(MoveDir);
 
            // 스피드 증가.
            MoveDir *= speed;
 
            // 캐릭터 점프 / 안죽어있으면
            if (Input.GetButton("Jump") && !PlayerHealth.instance.isDie && Cursor.lockState != CursorLockMode.None)
                MoveDir.y = jumpSpeed;
 
        }
 
        // 캐릭터에 중력 적용.
        MoveDir.y -= gravity * Time.deltaTime;

        if (walkAudio != null)
            walkAudio.isWalk = MoveDir.x != 0 || MoveDir.z != 0;
 
        // 캐릭터 움직임.
        controller.Move(MoveDir * Time.deltaTime);

        if (PlayerHealth.instance.isDie || Cursor.lockState == CursorLockMode.None) return; // 죽어있으면 아래 코드는 실행하지 않음 (채팅 키고 있을떄도)

        // Get mouse movement inputs
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        // Rotate the camera vertically
        verticalRotation -= mouseY;
        verticalRotation = Mathf.Clamp(verticalRotation, -90f, 90f);
        Camera.main.transform.localRotation = Quaternion.Euler(verticalRotation, 0f, 0f);

        // Rotate the character horizontally
        transform.Rotate(Vector3.up * mouseX);
    }

    public void SetCoords(Vector3 coords) {
        controller.enabled = false;
        transform.position = coords;
        controller.enabled = true;
    }

    [SerializeField, Range(0, 10f)]
    float SyncBetween = 2;
    private void FixedUpdate() {
        // 트래픽 최적화좀..
        if (
            Mathf.Abs(SaveX - transform.localEulerAngles.y) >= SyncBetween
            || Mathf.Abs(SaveY - verticalRotation) >= SyncBetween
            || Vector3.Distance(SaveCoords, transform.position) >= .1f
        ) {
            SaveX = transform.localEulerAngles.y;
            SaveY = verticalRotation;
            SaveCoords = transform.position;

            // 동기화 해줭
            NetworkCore.Send("Room.RequestPlayerUpdate", new PlayerInfoData(SaveCoords, SaveX, SaveY));
        }
    }
}
