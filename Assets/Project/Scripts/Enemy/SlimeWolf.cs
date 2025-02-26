using Game.Managers;
using Game.Player;
using System.Collections;
using UnityEngine;

namespace Game.Enemy
{
    public class SlimeWolf : Enemy
    {
        private GameObject player;
        
        private bool canMove = true;
        private bool canAttack = true;
        
        internal protected override void Start()
        {
            base.Start();

            SetState(EnemyState.Idle);
            player = GameManager.Instance.Player;
        }

        internal protected override void Update()
        {
            base.Update();
        }

        public override void Attack()
        {
            float distanseToPlayer = Vector3.Distance(transform.position, player.transform.position);
            if (distanseToPlayer > AttackDistance)
            {
                SetState(EnemyState.Chase);
                return;
            }

            if (canAttack)
                StartCoroutine(AttackE());
        }

        public override void Chase()
        {
            float distanseToPlayer = Vector3.Distance(transform.position, player.transform.position);
            if (distanseToPlayer < AttackDistance)
            {
                SetState(EnemyState.Attack);
                agent.SetDestination(transform.position);
                return;
            }
            else if (distanseToPlayer > ChaseDistanse)
            {
                SetState(EnemyState.Idle);
                agent.SetDestination(transform.position);
                return;
            }

            agent.SetDestination(player.transform.position);
        }
        public override void Idle()
        {
            if (!canMove) return;

            float distanseToPlayer = Vector3.Distance(transform.position, player.transform.position);
            if (distanseToPlayer < ChaseDistanse)
            {
                SetState(EnemyState.Chase);
                agent.SetDestination(transform.position);
                return;
            }

            StartCoroutine(IdleE());
        }

        public override void Patrol()
        {
            throw new System.NotImplementedException();
        }

        public override void GetDamage(float damege)
        {
            StartCoroutine(GetDamageAnim());

            SetHealth(runtimeHealth - damege);
            if (runtimeHealth <= 0)
                Death();
        }

        internal protected override void Death()
        {
            StartCoroutine(DeathAnim());
        }

        private IEnumerator IdleE()
        {
            canMove = false;
            float posX = Random.Range(-5f, 5f);
            float posY = Random.Range(-5f, 5f);
            Vector3 newPos = new Vector3(posX, posY);

            agent.SetDestination(transform.position + newPos);
            yield return new WaitForSeconds(5f);
            canMove = true;

            yield return null;
        }

        private IEnumerator GetDamageAnim()
        {
            EnemySprite.color = Color.Lerp(baseColor, damegeColor, .5f);
            yield return new WaitForSeconds(.01f);
            EnemySprite.color = Color.Lerp(damegeColor, baseColor, .5f);
        }

        private IEnumerator DeathAnim()
        {
            EnemySprite.color = Color.Lerp(baseColor, Color.red, 1f);
            yield return new WaitForSeconds(1.5f);
            Destroy(this.gameObject);
        }

        private IEnumerator AttackE()
        {
            canAttack = false;
            if(PrintLog)
                Debug.Log($"<color=orange>{gameObject.name}</color> Attack Player");

            player.GetComponent<PlayerMovement>().GetDamage(AttackDamage);
            yield return new WaitForSeconds(AttackSpeed);
            canAttack = true;
        }

    }
}