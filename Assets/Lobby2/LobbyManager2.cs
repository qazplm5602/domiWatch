using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LobbyManager2 : MonoBehaviour
{
    public static string MyName;
    public static string MyID;

    [SerializeField] TextMeshProUGUI NameText;

    private void Start() {
        NameText.text = MyName;
        print(MyName);
        print(MyID);
    }
}
