// ============================================================
// TowerSlot.cs
// Zone de placement d'une tour sur l'arène
// ============================================================
// Setup Unity :
//   • Créer des GameObjects "TowerSlot" le long du chemin
//   • Attacher ce script + un Collider (trigger ou non selon usage)
//   • Assigner le layer "PlacementArea"
// ============================================================

using System.Collections;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

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
            if (!IsAvailable) return;
            StartCoroutine(PlaceTowerAsync(data));
        }

        private IEnumerator PlaceTowerAsync(TowerData data)
        {
            IsAvailable = false;
            RefreshVisual();

            AsyncOperationHandle<GameObject> handle =
                data.prefabRef.LoadAssetAsync<GameObject>();

            yield return handle;

            if (handle.Status != AsyncOperationStatus.Succeeded)
            {
                Debug.LogError($"[TowerSlot] Échec chargement prefab tour : {data.towerName}");
                IsAvailable = true;
                RefreshVisual();
                EconomyManager.Instance.Earn(data.purchaseCost); // rembourse
                yield break;
            }

            GameObject go = Instantiate(handle.Result, transform.position, transform.rotation);
            PlacedTower   = go.GetComponent<TowerBase>();
            if (PlacedTower != null) PlacedTower.data = data;

            GameEvents.RaiseTowerPlaced(PlacedTower);
        }

        public void ClearTower()
        {
            if (PlacedTower != null)
            {
                Addressables.ReleaseInstance(PlacedTower.gameObject);
                PlacedTower = null;
            }
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
