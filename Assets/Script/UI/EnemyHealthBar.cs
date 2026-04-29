// ============================================================
// EnemyHealthBar.cs
// Barre de vie en world space au-dessus de chaque ennemi
// ============================================================
// Setup Unity :
//   • Créer un Canvas (World Space) enfant du prefab ennemi
//   • Ajouter un Slider ou deux Images (fond + remplissage)
//   • Attacher ce script sur ce Canvas enfant
//   • Relier fillImage (l'image de remplissage)
// ============================================================
using UnityEngine;
using UnityEngine.UI;

namespace TowerDefense
{
    public class EnemyHealthBar : MonoBehaviour
    {
        [SerializeField] private Image fillImage; 
        [SerializeField] private Vector3 offset = new Vector3(0f, 1.5f, 0f);

        private EnemyHealth health;
        private Transform   cam;

        private void Awake()
        {
            health = GetComponentInParent<EnemyHealth>();
            cam    = Camera.main?.transform;
        }

        private void LateUpdate()
        {
            if (cam != null)
                transform.LookAt(transform.position + cam.forward);

            if (health != null)
            {
                transform.position = health.transform.position + offset;

                if (fillImage != null)
                    fillImage.fillAmount = health.HealthNormalized;
            }
        }
    }
}
