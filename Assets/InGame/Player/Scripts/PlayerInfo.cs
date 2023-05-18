using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInfo : MonoBehaviour
{
    [Tooltip("카메라 설치 위치")]
    public Vector3 MaincameraCoords;
    [Tooltip("손손")]
    public GameObject HandHandler;
    [Tooltip("1인칭이면 손 위치 변경할 좌표")]
    public Vector3 MyHandCoords;
}
