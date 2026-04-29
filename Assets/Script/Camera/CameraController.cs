using Cinemachine;
using UnityEngine;

namespace TowerDefense
{
    public class CameraController : MonoBehaviour
    {
        [Header("Cinemachine")]
        [Tooltip("La CinemachineVirtualCamera de la scène")]
        [SerializeField] private CinemachineVirtualCamera virtualCamera;

        [Header("Déplacement")]
        [SerializeField] private float keyboardSpeed   = 12f;
        [SerializeField] private float edgeScrollSpeed = 10f;

        [Header("Défilement aux bords")]
        [SerializeField] private float edgeThickness  = 20f;
        [SerializeField] private bool  enableEdgeScroll = true;

        [Header("Zoom (molette)")]
        [SerializeField] private bool  enableZoom    = true;
        [SerializeField] private float zoomSpeed     = 4f;
        [SerializeField] private float zoomMin       = 5f;    // hauteur min (FOV ou ortho size)
        [SerializeField] private float zoomMax       = 20f;   // hauteur max
        [SerializeField] private float zoomSmoothing = 6f;

        [Header("Limites de l'arène")]
        [Tooltip("Centre de l'arène en world space (X, Z)")]
        [SerializeField] private Vector2 arenaCenter = Vector2.zero;
        [Tooltip("Taille de l'arène en world units (largeur, hauteur)")]
        [SerializeField] private Vector2 arenaSize   = new Vector2(30f, 20f);

        private float targetZoom;
        private CinemachineTransposer   transposer;
        private bool useOrtho;


        private void Awake()
        {
            if (virtualCamera == null)
            {
                Debug.LogError("[CameraController] CinemachineVirtualCamera non assignée !");
                return;
            }

            CinemachineBrain brain = Camera.main?.GetComponent<CinemachineBrain>();
            useOrtho = Camera.main != null && Camera.main.orthographic;

            transposer        = virtualCamera.GetCinemachineComponent<CinemachineTransposer>();

            targetZoom = useOrtho
                ? virtualCamera.m_Lens.OrthographicSize
                : (transposer != null ? -transposer.m_FollowOffset.y : zoomMax * 0.5f);
        }

        private void Update()
        {
            HandleMovement();
            //if (enableZoom) HandleZoom();
        }

        private void HandleMovement()
        {
            Vector3 move = Vector3.zero;

            move += GetKeyboardInput();

            if (move == Vector3.zero) return;

            Vector3 newPos = transform.position + move * Time.deltaTime;
            transform.position = ClampToArena(newPos);
        }

        private Vector3 GetKeyboardInput()
        {
            Vector3 dir = Vector3.zero;

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


        private void HandleZoom()
        {
            float scroll = Input.GetAxis("Mouse ScrollWheel");
            if (Mathf.Abs(scroll) < 0.001f) return;

            targetZoom -= scroll * zoomSpeed;
            targetZoom  = Mathf.Clamp(targetZoom, zoomMin, zoomMax);

            if (useOrtho)
            {
                float current = virtualCamera.m_Lens.OrthographicSize;
                virtualCamera.m_Lens.OrthographicSize =
                    Mathf.Lerp(current, targetZoom, Time.deltaTime * zoomSmoothing);
            }
            else if (transposer != null)
            {
                Vector3 offset = transposer.m_FollowOffset;
                offset.y = Mathf.Lerp(offset.y, -targetZoom, Time.deltaTime * zoomSmoothing);
                transposer.m_FollowOffset = offset;
            }
        }

        private Vector3 ClampToArena(Vector3 pos)
        {
            float halfW = arenaSize.x * 0.5f;
            float halfH = arenaSize.y * 0.5f;

            pos.x = Mathf.Clamp(pos.x, arenaCenter.x - halfW, arenaCenter.x + halfW);
            pos.z = Mathf.Clamp(pos.z, arenaCenter.y - halfH, arenaCenter.y + halfH);

            return pos;
        }


    }
}
