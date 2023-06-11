using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ESCwindowManager : MonoBehaviour
{
    [SerializeField] GameObject ESC_Main;

    void Update()
    {
        if (Input.GetKeyUp(KeyCode.Escape)) {
            bool isShow = ESC_Main.activeSelf;
            ESC_Main.SetActive(!isShow);

            // 커서 설정
            if (!isShow) {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            } else {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
        }
    }

    ////////// 버튼 hanlder... //////////
    public void RoomLeave() {
        NetworkCore.Send("Room.Left", null);
        SceneManager.LoadScene(1);
    }
    public void GameExit() {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false; // 꺼
        #else
            Application.Quit();
        #endif
    }
}
