using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ScoreboardManager : MonoBehaviour
{
    [SerializeField] KeyCode ShowKEY = KeyCode.Tab;
    [SerializeField] GameObject ScoreBoardMain;
    [SerializeField] Transform PlayerList;
    [SerializeField] GameObject PlayerBoxUI;
    public enum TextMode {
        Kill,
        Death,
        Assist
    }

    static ScoreboardManager instance;
    static Dictionary<string, ScoreboardEntity> ScorePlayers;
    
    class ScoreboardEntity {
        GameObject Box;
        public TextMeshProUGUI Kill;
        public TextMeshProUGUI Death;
        public TextMeshProUGUI Assist;

        public ScoreboardEntity(string id, string name, bool my) {
            Box = Instantiate(instance.PlayerBoxUI, Vector3.zero, Quaternion.identity, instance.PlayerList);
            
            var InfoTransform = Box.transform.Find("Info");
            Kill = InfoTransform.Find("Kill").GetComponent<TextMeshProUGUI>();
            Death = InfoTransform.Find("Death").GetComponent<TextMeshProUGUI>();
            Assist = InfoTransform.Find("Assist").GetComponent<TextMeshProUGUI>();

            // 이름 수정
            Box.transform.Find("Name").GetComponent<TextMeshProUGUI>().text = name;
        }
    }

    private void Awake() {
        ScorePlayers = new();

        if (instance == null)
            instance = this;

        for (int i = 0; i < PlayerList.childCount; i++)
            Destroy(PlayerList.GetChild(i).gameObject);
    }

    private void Update() {
        if (Input.GetKeyDown(ShowKEY))
            ScoreBoardMain.SetActive(true);
        else if (Input.GetKeyUp(ShowKEY))
            ScoreBoardMain.SetActive(false);
    }

    public static void Create(string id, string name, bool my) {
        ScorePlayers[id] = new(id, name, my);
    }

    public static void EditText(TextMode mode, string id, string value) {
        if (!ScorePlayers.TryGetValue(id, out var ScoreEntity)) return; // 없으면 리턴

        TextMeshProUGUI TextUI;
        switch (mode)
        {
            case TextMode.Kill:
                TextUI = ScoreEntity.Kill;
                break;
            case TextMode.Death:
                TextUI = ScoreEntity.Death;
                break;
            case TextMode.Assist:
                TextUI = ScoreEntity.Assist;
                break;
            default:
                return;
        }

        TextUI.text = value;
    }
}
