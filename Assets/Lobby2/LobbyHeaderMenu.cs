using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class LobbyHeaderMenu : MonoBehaviour
{
    [SerializeField] Transform ButtonList;
    Dictionary<int, UnityAction> MenuCallback = new();
    int ActiveMenu = 0;

    public void MenuClick(int index) {
        if (index == ActiveMenu) return; // 이미 선택되어 있음

        ActiveMenu = index;
        Transform ButtonTrans = ButtonList.GetChild(index);

        for (int i = 0; i < ButtonList.childCount; i++)
            ButtonList.GetChild(i).GetComponent<LobbyHeaderButton>().Active(index == i);
    }
}
