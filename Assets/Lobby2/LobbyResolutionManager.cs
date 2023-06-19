using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class domiScreenList {
    public string Name;
    public FullScreenMode Mode;

    public domiScreenList(string name, FullScreenMode mode) {
        Name = name;
        Mode = mode;
    }
}

public class LobbyResolutionManager : MonoBehaviour
{
    [SerializeField] TMPro.TextMeshProUGUI Text;
    static bool Already = false;
    domiScreenList[] Screens = {
        new("전체화면", FullScreenMode.FullScreenWindow),
        new("창모드", FullScreenMode.Windowed)
    };
    void Start()
    {
        int index = PlayerPrefs.GetInt("Screen.IndexMode", 0);
        if (Text != null)
            Text.text = Screens[index].Name;

        if (Already) return;
        Already = true;
        Screen.fullScreenMode = Screens[index].Mode;
    }

    public void ChangeResolution() {
        int index = PlayerPrefs.GetInt("Screen.IndexMode", 0);
        index ++;
        if (index >= Screens.Length) index = 0;

        domiScreenList Info = Screens[index];
        PlayerPrefs.SetInt("Screen.IndexMode", index);

        Text.text = Info.Name;
        Screen.fullScreenMode = Info.Mode;
    }
}
