// ============================================================
// HUDController.cs
// Mise à jour des éléments du HUD en temps réel
// ============================================================
// Setup Unity :
//   • Attacher sur un GameObject dans le Canvas
//   • Relier chaque Text (TMP_Text) dans l'Inspector
// ============================================================
using UnityEngine;
using TMPro;

namespace TowerDefense
{
    public class HUDController : MonoBehaviour
    {
        [Header("Textes HUD")]
        [SerializeField] private TMP_Text scoreText;
        [SerializeField] private TMP_Text multiplierText;
        [SerializeField] private TMP_Text currencyText;
        [SerializeField] private TMP_Text waveText;
        [SerializeField] private TMP_Text enemiesRemainingText;

        [Header("Couleurs du multiplicateur")]
        [SerializeField] private Color positiveColor = Color.green;
        [SerializeField] private Color negativeColor = Color.red;
        [SerializeField] private Color neutralColor  = Color.white;

        void Awake()
        {
            SetScore(0);
            SetMultiplier(0);
            SetWave(0);
            SetEnemiesRemaining(0);
        }
        private void OnEnable()
        {
            GameEvents.OnScoreChanged       += SetScore;
            GameEvents.OnMultiplierChanged  += SetMultiplier;
            GameEvents.OnCurrencyChanged    += SetCurrency;
            GameEvents.OnWaveStarted        += SetWave;
            GameEvents.OnEnemyCountChanged  += SetEnemiesRemaining;
        }

        private void OnDisable()
        {
            GameEvents.OnScoreChanged       -= SetScore;
            GameEvents.OnMultiplierChanged  -= SetMultiplier;
            GameEvents.OnCurrencyChanged    -= SetCurrency;
            GameEvents.OnWaveStarted        -= SetWave;
            GameEvents.OnEnemyCountChanged  -= SetEnemiesRemaining;
        }

        private void SetScore(int score)
        {
            if (scoreText != null) scoreText.text = score.ToString("N0")+" points";
        }

        private void SetMultiplier(int mult)
        {
            if (multiplierText == null) return;

            string sign = mult >= 0 ? "+" : "";
            multiplierText.text  = $"{sign}{mult}×";
            multiplierText.color = mult > 0 ? positiveColor :
                                   mult < 0 ? negativeColor : neutralColor;
        }

        private void SetCurrency(int amount)
        {
            if (currencyText != null) currencyText.text = $"{amount} ¤";
        }

        private void SetWave(int waveNumber)
        {
            if (waveText != null) waveText.text = $"Vague {waveNumber}";
        }

        private void SetEnemiesRemaining(int count)
        {
            if (enemiesRemainingText != null) enemiesRemainingText.text = count.ToString()+ " enemies remaining";
        }
    }
}
