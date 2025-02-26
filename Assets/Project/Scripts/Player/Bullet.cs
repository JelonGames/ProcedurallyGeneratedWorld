using UnityEngine;

namespace Game.Player
{
    [RequireComponent(typeof(BoxCollider2D))]
    public class Bullet : MonoBehaviour
    {
        [SerializeField] private float LiveTime = 1f;
        [SerializeField] private float Speed = 1f;
        [SerializeField] private float Damage = 1f;

        private void Start()
        {
            Destroy(this.gameObject, LiveTime);
        }

        private void FixedUpdate()
        {
            transform.Translate(Vector3.right * Speed * Time.deltaTime, Space.Self);
        }

        private void OnTriggerEnter2D(Collider2D coll)
        {
            if(coll.CompareTag("Enemy"))
            {
                coll.GetComponent<Game.Enemy.Enemy>().GetDamage(Damage);
                Destroy(this.gameObject);
            }
            else if(coll.CompareTag("WorldObject"))
            {
                Destroy(this.gameObject);
            }
        }
    }
}
