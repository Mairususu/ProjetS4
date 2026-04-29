using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace TowerDefense
{
    public class TowerCardUI : MonoBehaviour
    {
        [SerializeField] private Image     iconImage;
        [SerializeField] private TMP_Text  nameText;
        [SerializeField] private TMP_Text  costText;
        [SerializeField] private TMP_Text  statsText;
        [SerializeField] private Button    selectButton;
        [SerializeField] private GameObject affordableOverlay;
        

        public TowerData Data { get; private set; }

        public void Setup(TowerData data)
        {
            Data = data;
            
            if (iconImage != null && data.icon != null) iconImage.sprite = data.icon;
            if (nameText  != null) nameText.text  = data.towerName;
            if (costText  != null) costText.text  = $"{data.purchaseCost} ¤";

            // Stats niveau 0
            if (statsText != null && data.levels != null && data.levels.Length > 0)
            {
                TowerLevel lvl = data.levels[0];
                statsText.text =
                    $"DMG: {lvl.damage}  |  CD: {lvl.fireRate:F1}s\n" +
                    $"Portée: {lvl.range}  |  Proj: {lvl.projectileCount}";
            }

            selectButton?.onClick.AddListener(() => PlacementManager.Instance.SelectTower(data));

            SetAffordable(EconomyManager.Instance.CanAfford(data.purchaseCost));
           

        }

        public void SetAffordable(bool affordable)
        {
            if (selectButton != null)   selectButton.interactable = affordable;
            if (affordableOverlay != null) affordableOverlay.SetActive(!affordable);
        }
    }
}

