// ============================================================
// CameraController.cs
// Caméra vue de dessus avec ZQSD + glissement aux bords
// ============================================================
// Setup Unity :
//   • Attacher sur la Camera principale (ou son parent)
//   • Définir arenaBounds (centre + taille de l'arène)
//   • La caméra doit être orientée vers le bas (rotation X = 90)
// ============================================================
using UnityEngine;

namespace TowerDefense
{
    public class CameraController : MonoBehaviour
    {
        [Header("Vitesse")]
        [SerializeField] private float keyboardSpeed  = 10f;
        [SerializeField] private float edgeScrollSpeed = 8f;

        [Header("Défilement aux bords")]
        [Tooltip("Épaisseur de la zone de bord en pixels")]
        [SerializeField] private float edgeThickness = 20f;
        [SerializeField] private bool  enableEdgeScroll = true;

        [Header("Limites de l'arène")]
        [Tooltip("Centre de l'arène en world space")]
        [SerializeField] private Vector2 arenaCenter = Vector2.zero;
        [Tooltip("Taille de l'arène (largeur, hauteur) en world space")]
        [SerializeField] private Vector2 arenaSize   = new Vector2(30f, 20f);

        // ── Lifecycle ──────────────────────────────────────────

        private void Update()
        {
            Vector3 move = Vector3.zero;

            move += HandleKeyboard();

            if (enableEdgeScroll)
                move += HandleEdgeScroll();

            if (move == Vector3.zero) return;

            Vector3 newPos = transform.position + move * Time.deltaTime;
            transform.position = ClampToArena(newPos);
        }

        // ── Input ──────────────────────────────────────────────

        private Vector3 HandleKeyboard()
        {
            Vector3 dir = Vector3.zero;

            // ZQSD (disposition AZERTY) + WASD fallback
            if (Input.GetKey(KeyCode.Z) || Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
                dir += Vector3.forward;
            if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
                dir += Vector3.back;
            if (Input.GetKey(KeyCode.Q) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
                dir += Vector3.left;
            if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
                dir += Vector3.right;

            return dir * keyboardSpeed;
        }

        private Vector3 HandleEdgeScroll()
        {
            Vector3 dir   = Vector3.zero;
            Vector2 mouse = Input.mousePosition;

            if (mouse.x < edgeThickness)                         dir += Vector3.left;
            if (mouse.x > Screen.width  - edgeThickness)        dir += Vector3.right;
            if (mouse.y < edgeThickness)                         dir += Vector3.back;
            if (mouse.y > Screen.height - edgeThickness)        dir += Vector3.forward;

            return dir * edgeScrollSpeed;
        }

        // ── Contrainte aux bords de l'arène ───────────────────

        private Vector3 ClampToArena(Vector3 pos)
        {
            float halfW = arenaSize.x * 0.5f;
            float halfH = arenaSize.y * 0.5f;

            pos.x = Mathf.Clamp(pos.x, arenaCenter.x - halfW, arenaCenter.x + halfW);
            pos.z = Mathf.Clamp(pos.z, arenaCenter.y - halfH, arenaCenter.y + halfH);

            return pos;
        }

        // ── Gizmos ─────────────────────────────────────────────

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireCube(
                new Vector3(arenaCenter.x, 0f, arenaCenter.y),
                new Vector3(arenaSize.x, 0.1f, arenaSize.y));
        }
    }
}
