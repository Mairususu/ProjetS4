using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace TowerDefense
{
    public class PlacementPhaseUI : MonoBehaviour
    {
        [SerializeField] private Button   launchButton;
        [SerializeField] private TMP_Text wavePreviewText;

        private void Awake()
        {
            launchButton?.onClick.AddListener(() => GameManager.Instance.LaunchWave());
        }

        private void OnEnable()
        {
            int next = WaveManager.Instance != null
                ? WaveManager.Instance.CurrentWaveIndex + 2 
                : 1;

            if (wavePreviewText != null)
                wavePreviewText.text = $"Lancer la vague {next}";

        }
    }
}
