// ============================================================
// TowerSlot.cs
// Zone de placement d'une tour sur l'arène
// ============================================================
// Setup Unity :
//   • Créer des GameObjects "TowerSlot" le long du chemin
//   • Attacher ce script + un Collider (trigger ou non selon usage)
//   • Assigner le layer "PlacementArea"
// ============================================================
using UnityEngine;

namespace TowerDefense
{
    public class TowerSlot : MonoBehaviour
    {
        [Header("Visuel")]
        [SerializeField] private GameObject availableVisual;   // indicateur vert
        [SerializeField] private GameObject occupiedVisual;    // indicateur gris
        public bool      IsAvailable   { get; private set; } = true;
        public TowerBase PlacedTower   { get; private set; }
        
        private void Start() => RefreshVisual();

        public void PlaceTower(TowerData data)
        {
            if (!IsAvailable || data?.prefab == null) return;

            GameObject go = Instantiate(data.prefab, transform.position, transform.rotation);
            PlacedTower  = go.GetComponent<TowerBase>();

            if (PlacedTower != null) PlacedTower.data = data;

            IsAvailable = false;
            RefreshVisual();
            GameEvents.RaiseTowerPlaced(PlacedTower);
        }

        public void ClearTower()
        {
            if (PlacedTower != null) Destroy(PlacedTower.gameObject);
            PlacedTower = null;
            IsAvailable = true;
            RefreshVisual();
        }

        private void RefreshVisual()
        {
            if (availableVisual != null) availableVisual.SetActive(IsAvailable);
            if (occupiedVisual  != null) occupiedVisual.SetActive(!IsAvailable);
        }

        private void SelectPlacedTower()
        {
            if (PlacedTower != null) GameEvents.RaiseTowerSelected(PlacedTower);
        }
    }
}
