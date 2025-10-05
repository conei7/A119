using UnityEngine;

public class PlanetInfo : MonoBehaviour
{
    [Tooltip("この重力場に入った時の速度倍率")]
    public float speedMultiplier = 1.5f;

    [Tooltip("重力場捕獲時に最低でも確保したい速度")]
    public float minOrbitSpeed = 1f;
}