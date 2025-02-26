using UnityEngine;
using UnityEngine.AI;

namespace Game.Enemy
{
    [RequireComponent(typeof(NavMeshAgent))]
    public abstract class Enemy : MonoBehaviour
    {
        [SerializeField] internal bool PrintLog = false;
        [SerializeField] internal float Health;
        [SerializeField] internal float Speed;

        [Header("Components")]
        [SerializeField] internal SpriteRenderer EnemySprite;

        [Header("Chase Variable")]
        [SerializeField] internal float ChaseDistanse;

        [Header("Attack Variable")]
        [SerializeField] internal float AttackDamage;
        [SerializeField] internal float AttackSpeed;
        [SerializeField] internal float AttackDistance;

        internal EnemyState State { get; private set; }
        internal float runtimeHealth { get; private set; }
        internal float runtimeSpeed { get; private set; }

        internal Color damegeColor { get => new Color(255, 134, 134); }
        internal Color baseColor;

        public NavMeshAgent agent { get; private set; }

        internal protected void SetHealth(float newHealth) => runtimeHealth = newHealth;
        internal protected void SetSpeed(float newSpeed) => runtimeSpeed = newSpeed;
        internal protected void SetState(EnemyState newState)
        {
            State = newState;

            if(PrintLog)
                Debug.Log($"<color=green>{gameObject.name}</color> Set State {State.ToString()}");
        }

        internal protected virtual void Start()
        {
            runtimeHealth = Health;
            runtimeSpeed = Speed;
            agent = GetComponent<NavMeshAgent>();
            agent.updateRotation = false;
            agent.updateUpAxis = false;
            baseColor = EnemySprite.color;
        }

        internal protected virtual void Update()
        {
            switch (State)
            {
                case EnemyState.Attack:
                    Attack();
                    break;
                case EnemyState.Chase:
                    Chase();
                    break;
                case EnemyState.Idle:
                    Idle();
                    break;
                case EnemyState.Patrol:
                    Patrol();
                    break;
            }
        }

        public abstract void GetDamage(float damege);
        internal protected virtual void Death() => Destroy(this.gameObject);
        public abstract void Idle();
        public abstract void Patrol();
        public abstract void Chase();
        public abstract void Attack();


        #region Debug Method
        private void GetDamage() => GetDamage(1f);
        #endregion
    }
}
