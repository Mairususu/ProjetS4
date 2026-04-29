// ============================================================
// EnemyAnimator.cs
// Gère les animations de marche et de mort de l'ennemi
// ============================================================
// Setup Unity :
//   1. Attacher sur le prefab ennemi (même GO que EnemyHealth)
//   2. Relier le composant Animator dans l'Inspector
//   3. Créer un Animator Controller avec les paramètres ci-dessous
//
// Paramètres requis dans l'Animator Controller :
//   • IsWalking  (Bool)   → active l'animation de marche
//   • IsDead     (Bool)   → active l'animation de mort
//   • SpeedMultiplier (Float) → optionnel, pour adapter la vitesse d'anim
//
// États recommandés dans l'Animator Controller :
//   [Idle] ──(IsWalking = true)──→ [Walk] ──(IsDead = true)──→ [Death]
//                                              ↑                    ↓
//                                    (IsWalking = false)       (fin anim)
// ============================================================
using System.Collections;
using UnityEngine;

namespace TowerDefense
{
    public class EnemyAnimator : MonoBehaviour
    {
        [Header("Références")]
        [Tooltip("L'Animator du modèle (peut être sur un enfant)")]
        [SerializeField] private Animator animator;

        [Header("Noms des paramètres Animator")]
        [SerializeField] private string paramIsWalking       = "IsWalking";
        [SerializeField] private string paramIsDead          = "IsDead";
        [SerializeField] private string paramSpeedMultiplier = "SpeedMultiplier";

        [Header("Mort")]
        [Tooltip("Durée de l'animation de mort avant destruction du GameObject")]
        [SerializeField] private float deathAnimDuration = 2f;

        private int idIsWalking;
        private int idIsDead;
        private int idSpeedMultiplier;
        private bool hasSpeedParam;

        private void Awake()
        {
            if (animator == null)
                animator = GetComponentInChildren<Animator>();

            if (animator == null)
            {
                Debug.LogWarning($"[EnemyAnimator] Pas d'Animator trouvé sur {gameObject.name}");
                return;
            }
            idIsWalking       = Animator.StringToHash(paramIsWalking);
            idIsDead          = Animator.StringToHash(paramIsDead);
            idSpeedMultiplier = Animator.StringToHash(paramSpeedMultiplier);
            hasSpeedParam = HasParameter(paramSpeedMultiplier);
        }

        public void SetWalking(bool walking)
        {
            if (animator == null) return;
            animator.SetBool("IsWalking", walking);
        }

        public void SetSpeedMultiplier(float speed)
        {
            if (animator == null || !hasSpeedParam) return;
            animator.SetFloat(idSpeedMultiplier, speed);
        }

        public void PlayDeath(System.Action onDeathComplete)
        {
            if (animator == null)
            {
                onDeathComplete?.Invoke();
                return;
            }

            animator.SetBool(idIsWalking, false);
            animator.SetBool(idIsDead,    true);

            StartCoroutine(WaitForDeathAnim(onDeathComplete));
        }

        private IEnumerator WaitForDeathAnim(System.Action onComplete)
        {
            yield return new WaitForSeconds(deathAnimDuration);
            onComplete?.Invoke();
        }

        private bool HasParameter(string paramName)
        {
            if (animator == null || animator.runtimeAnimatorController == null)
                return false;

            foreach (AnimatorControllerParameter p in animator.parameters)
                if (p.name == paramName) return true;

            return false;
        }
    }
}
