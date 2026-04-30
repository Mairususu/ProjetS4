using UnityEngine;

namespace TowerDefense
{
    public class TowerClick : MonoBehaviour
    {
        private TowerBase towerBase;

        private void Awake()
        {
            towerBase = GetComponent<TowerBase>() ?? GetComponentInParent<TowerBase>();
        }

        private void OnMouseDown()
        {
            if (PlacementManager.Instance.SelectedTowerData != null) return;
            if (towerBase != null)
            {
                Debug.Log($"[TowerClick] Tour sélectionnée : {towerBase.data.towerName}");
                GameEvents.RaiseTowerSelected(towerBase);
            }
        }
    }
}
