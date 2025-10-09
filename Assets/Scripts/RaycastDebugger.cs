using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class RaycastDebugger : MonoBehaviour
{
    void Update()
    {
        if (Input.GetMouseButtonDown(0)) // 左クリック
        {
            PointerEventData pointerData = new PointerEventData(EventSystem.current)
            {
                position = Input.mousePosition
            };

            List<RaycastResult> results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(pointerData, results);

            Debug.Log($"=== クリック位置: {Input.mousePosition} ===");
            if (results.Count == 0)
            {
                Debug.LogWarning("何もヒットしませんでした");
            }
            else
            {
                foreach (RaycastResult result in results)
                {
                    Debug.Log($"ヒット: {result.gameObject.name} (レイヤー: {result.gameObject.layer})");
                }
            }
        }
    }
}
