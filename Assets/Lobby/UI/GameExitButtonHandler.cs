using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameExitButtonHandler : MonoBehaviour
{
    public void GameExit() {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false; // 꺼
        #else
            Application.Quit();
        #endif
    }
}
