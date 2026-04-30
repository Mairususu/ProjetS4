using System.Collections.Generic;
using UnityEngine;
namespace TowerDefense
{
    public class PlacementManager : MonoBehaviour
    {
        public static PlacementManager Instance { get; private set; }

        [Header("Tours disponibles à l'achat")]
        [SerializeField] private TowerData[] shopData;

        [Header("Preview")]
        [SerializeField] private GameObject previewPrefab;
        [SerializeField] private LayerMask  placementMask; 
        public TowerData SelectedTowerData;
        public bool IsPlacementPhase;
        public TowerData[] ShopData        => shopData;

        private GameObject  previewInstance;
        private Camera      mainCamera;
        private void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;
            mainCamera = Camera.main;
        }

        private void Update()
        {
            if (!IsPlacementPhase) return;

            if (SelectedTowerData != null) UpdatePreview();

            if (Input.GetMouseButtonDown(0) && !IsPointerOverUIElement())
            {
                if (SelectedTowerData == null) return;
                TryPlaceAtCursor();
            }
        }

        private bool IsPointerOverUIElement()
        {
            var results = new List<UnityEngine.EventSystems.RaycastResult>();
            var pointer = new UnityEngine.EventSystems.PointerEventData(
                UnityEngine.EventSystems.EventSystem.current)
            {
                position = Input.mousePosition
            };

            UnityEngine.EventSystems.EventSystem.current.RaycastAll(pointer, results);

            foreach (var r in results)
            {
                if (r.gameObject.GetComponentInParent<Canvas>() != null)
                    return true;
            }

            return false;
        }

        public void BeginPlacementPhase()
        {
            IsPlacementPhase = true;
        }

        public void EndPlacementPhase()
        {
            IsPlacementPhase = false;
            ClearSelection();
        }
        
        public void SelectTower(TowerData data)
        {
            if (!EconomyManager.Instance.CanAfford(data.purchaseCost)) return;

            SelectedTowerData = data;
            ShowPreview(data);
        }

        public void ClearSelection()
        {
            SelectedTowerData = null;
            if (previewInstance != null) Destroy(previewInstance);
        }

        private void TryPlaceAtCursor()
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);

            if (!Physics.Raycast(ray, out RaycastHit hit, 100f, placementMask)) return;

            if (!hit.collider.TryGetComponent<TowerSlot>(out var slot)) return;

            if (!slot.IsAvailable) return;

            if (!EconomyManager.Instance.TrySpend(SelectedTowerData.purchaseCost)) return;

            slot.PlaceTower(SelectedTowerData);
            ClearSelection();
        }

        private void UpdatePreview()
        {
            if (previewInstance == null) return;

            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, 100f, placementMask))
                previewInstance.transform.position = hit.point;
        }

        private void ShowPreview(TowerData data)
        {
            if (previewInstance != null) Destroy(previewInstance);
            if (previewPrefab != null)
                previewInstance = Instantiate(previewPrefab);
        }
    }
}
