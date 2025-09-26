using UnityEngine;

public class MoonController : MonoBehaviour
{
    [SerializeField] private float rotationSpeed = 10f;

    void Update()
    {
        // Z軸を中心に回転させる
        transform.Rotate(0, 0, rotationSpeed * Time.deltaTime);
    }
}