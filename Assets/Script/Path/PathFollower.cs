using UnityEngine;
using UnityEngine.AI;

namespace TowerDefense
{
    [RequireComponent(typeof(NavMeshAgent))]
    [RequireComponent(typeof(EnemyAnimator))]
    public class PathFollower : MonoBehaviour
    {
        public static readonly System.Collections.Generic.HashSet<PathFollower> ActiveFollowers = new();

        [Tooltip("Distance en dessous de laquelle on considère l'ennemi arrivé au point B")]
        [SerializeField] private float arrivalTolerance = 0.5f;

        [Tooltip("Vitesse de rotation visuelle vers la direction de déplacement")]
        [SerializeField] private float rotationSpeed = 10f;

        [Header("Fix rotation")]
        [Tooltip("Offset Y appliqué au modèle si le mesh est orienté à l'envers. 180 si l'ennemi recule.")]
        [SerializeField] private float modelRotationOffsetY = 180f;

        [Tooltip("Transform du modèle visuel enfant. Si null, utilise ce GameObject.")]
        [SerializeField] private Transform modelRoot;

        public float Progress  { get; private set; }
        public bool  IsArrived { get; private set; }
        private NavMeshAgent  agent;
        private EnemyAnimator anim;
        private float         totalDistance;
        private bool          initialized;

        private void Awake()
        {
            agent = GetComponent<NavMeshAgent>();
            anim  = GetComponent<EnemyAnimator>();

            agent.updateRotation = false;
            agent.updateUpAxis   = false;
            if (modelRoot != null)
                modelRoot.localRotation = Quaternion.Euler(0f, modelRotationOffsetY, 0f);
        }

        private void OnEnable()  => ActiveFollowers.Add(this);
        private void OnDisable() => ActiveFollowers.Remove(this);
        private void OnDestroy() => ActiveFollowers.Remove(this);

        public void Initialize(float speed)
        {
            if (PathDefinition.Instance == null)
            {
                Debug.LogError("[PathFollower] PathDefinition introuvable !");
                return;
            }

            IsArrived   = false;
            initialized = false;
            agent.speed            = speed;
            agent.stoppingDistance = arrivalTolerance;

            bool warped = agent.Warp(PathDefinition.Instance.StartPoint);
            if (!warped)
            {
                Debug.LogWarning($"[PathFollower] {gameObject.name} hors NavMesh.");
                return;
            }

            agent.SetDestination(PathDefinition.Instance.EndPoint);
            totalDistance = Vector3.Distance(
                PathDefinition.Instance.StartPoint,
                PathDefinition.Instance.EndPoint);

            initialized = true;
            anim?.SetWalking(true);
        }

        private void Update()
        {
            if (!initialized || IsArrived) return;
            UpdateProgress();
            UpdateRotation();
            CheckArrival();
        }

        private void UpdateRotation()
        {
            if (agent.velocity.sqrMagnitude < 0.01f) return;

            Vector3    moveDir   = new Vector3(agent.velocity.x, 0f, agent.velocity.z).normalized;
            Quaternion targetRot = Quaternion.LookRotation(moveDir);
            transform.rotation   = Quaternion.Lerp(
                transform.rotation, targetRot, Time.deltaTime * rotationSpeed);
        }


        private void UpdateProgress()
        {
            if (agent.pathPending || totalDistance <= 0f) return;
            Progress = Mathf.Clamp01(1f - (agent.remainingDistance / totalDistance));
        }

        private void CheckArrival()
        {
            if (agent.pathPending) return;
            if (agent.remainingDistance > arrivalTolerance) return;
            if (agent.velocity.sqrMagnitude > 0.04f) return;

            OnReachedEnd();
        }

        private void OnReachedEnd()
        {
            IsArrived       = true;
            agent.isStopped = true;
            anim?.SetWalking(false);

            EnemyData data = GetComponent<EnemyHealth>()?.Data;
            if (data != null) GameEvents.RaiseEnemyReachedEnd(data);

            Destroy(gameObject);
        }

      
    }
}