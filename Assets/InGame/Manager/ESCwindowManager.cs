using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
}
