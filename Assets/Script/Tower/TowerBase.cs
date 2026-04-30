using System.Collections.Generic;
using UnityEngine;

namespace TowerDefense
{
    [RequireComponent(typeof(TowerAttack))]
    public class TowerBase : MonoBehaviour
    {
        [Header("Données")]
        public TowerData data;

        [Header("Visuel")]
        [SerializeField] private Transform rotationPoint;

        [SerializeField] private List<GameObject> towerlevel;

        public int CurrentLevel { get; private set; } = 0;

        private TowerAttack attackComponent;
        private Transform   currentTarget;
        private bool        configured = false;

        private void Awake()
        {
            attackComponent = GetComponent<TowerAttack>();
            TryApplyLevel();
        }

        private void Start()
        {
            TryApplyLevel();
        }

        private void TryApplyLevel()
        {
            if (configured) return;
            if (data == null || attackComponent == null) return;
            configured = true;
            ApplyLevel();
        }

        private void Update()
        {
            FindTarget();
            RotateToTarget();
        }

        private void FindTarget()
        {
            if (data == null) return;

            TowerLevel cfg  = data.GetLevel(CurrentLevel);
            float      best = cfg.range * cfg.range;

            Transform found = null;
            foreach (PathFollower follower in PathFollower.ActiveFollowers)
            {
                if (follower == null) continue;
                float dist = (follower.transform.position - transform.position).sqrMagnitude;
                if (dist <= best)
                {
                    best  = dist;
                    found = follower.transform;
                }
            }

            if (found != null)
                currentTarget = found;
        }

        private void RotateToTarget()
        {
            if (rotationPoint == null || currentTarget == null) return;

            Vector3 dir = currentTarget.position - rotationPoint.position;
            if (dir == Vector3.zero) return;

            Quaternion targetRot = Quaternion.LookRotation(new Vector3(dir.x, 0f, dir.z));
            rotationPoint.rotation = Quaternion.RotateTowards(
                rotationPoint.rotation, targetRot, 360f * Time.deltaTime);
        }

        public Transform GetCurrentTarget() => currentTarget;

        public bool TryUpgrade()
        {
            int cost = data.GetUpgradeCost(CurrentLevel);
            if (cost < 0) return false;
            if (!EconomyManager.Instance.TrySpend(cost)) return false;

            CurrentLevel++;
            ApplyLevel();
            GameEvents.RaiseTowerUpgraded(this);
            return true;
        }

        public bool IsMaxLevel()        => CurrentLevel >= data.MaxLevel;
        public int  GetNextUpgradeCost() => data.GetUpgradeCost(CurrentLevel);

        private void ApplyLevel()
        {
            attackComponent.Configure(data.GetLevel(CurrentLevel));

            if (towerlevel == null || towerlevel.Count == 0) return;

            for (int i = 0; i < towerlevel.Count; i++)
            {
                if (towerlevel[i] != null)
                    towerlevel[i].SetActive(i == CurrentLevel);
            }
        }

        private void OnDrawGizmosSelected()
        {
            if (data == null) return;
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(transform.position, data.GetLevel(CurrentLevel).range);
        }
        
        public void SetTargetForTest(Transform target)
        {
            currentTarget = target;
        }
    }
}