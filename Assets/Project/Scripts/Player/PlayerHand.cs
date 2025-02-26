using Game.Managers;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Game
{
    public class PlayerHand : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer inHandSR;
        [SerializeField] private Sprite inHandSprite;
        [SerializeField] private GameObject Bullet;
        
        private SpriteRenderer _handSR;

        private void OnEnable()
        {
            GameManager.Instance.Inputs.Player.Attack.performed += Shoot;
        }

        private void Start()
        {
            inHandSR.sprite = inHandSprite;
            _handSR = GetComponent<SpriteRenderer>();
        }

        private void OnDisable()
        {
            GameManager.Instance.Inputs.Player.Attack.performed -= Shoot;
        }

        private void FixedUpdate()
        {
            LookAt();
            FlipSprite();
        }

        private void LookAt()
        {
            var mousePos = Camera.main.ScreenToWorldPoint(GameManager.Instance.MousePosition);
            var rotation = mousePos - transform.position;
            var angle = Mathf.Atan2(rotation.y, rotation.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, angle);
        }

        private void FlipSprite()
        {
            Vector2 playerPositionInScreen = Camera.main.WorldToScreenPoint(transform.position);

            if (GameManager.Instance.MousePosition.x < playerPositionInScreen.x)
            {
                _handSR.flipY = true;
                inHandSR.flipY = true;
            }
            else
            {
                _handSR.flipY = false;
                inHandSR.flipY = false;
            }
        }

        private void Shoot(InputAction.CallbackContext context)
        {
            GameObject bullet = Instantiate(Bullet, transform.position + (transform.right * 2), Quaternion.identity);
            bullet.transform.rotation = transform.rotation;
            // Debug.DrawLine(transform.right * 2, transform.right * 10, Color.red, 2f);
        }
    }
}
