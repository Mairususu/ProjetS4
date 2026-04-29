using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class UIDebugger : MonoBehaviour
{
    void Update()
    {
        if (!Input.GetMouseButtonDown(0)) return;

        var results = new List<RaycastResult>();
        var pointer = new PointerEventData(EventSystem.current)
        {
            position = Input.mousePosition
        };

        EventSystem.current.RaycastAll(pointer, results);

        if (results.Count == 0)
        {
            Debug.Log("[UIDebugger] Aucun élément UI touché");
            return;
        }

        foreach (var r in results)
            Debug.Log($"[UIDebugger] UI touché : {r.gameObject.name} | Parent : {r.gameObject.transform.parent?.name}");
    }
}
