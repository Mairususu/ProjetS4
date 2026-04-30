using System.Collections;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace TowerDefense
{
    public class TowerSlot : MonoBehaviour
    {
        [Header("Visuel")]
        [SerializeField] private GameObject availableVisual;
        [SerializeField] private GameObject occupiedVisual;    
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
            if (data.prefabRef != null && data.prefabRef.RuntimeKeyIsValid())
            {
                var handle = data.prefabRef.InstantiateAsync(transform.position, transform.rotation);
                yield return handle;
                if (handle.Status != AsyncOperationStatus.Succeeded)
                {
                    IsAvailable = true;
                    RefreshVisual();
                    EconomyManager.Instance.Earn(data.purchaseCost);
                    yield break;
                }

                GameObject go = Instantiate(handle.Result, transform.position, transform.rotation);
                PlacedTower   = go.GetComponent<TowerBase>();
                if (PlacedTower != null) PlacedTower.data = data;
                GameEvents.RaiseTowerPlaced(PlacedTower);
            }
            else 
            {
                Debug.LogWarning("Le prefabRef est invalide. Construction ignorée pour le test.");
            }
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
