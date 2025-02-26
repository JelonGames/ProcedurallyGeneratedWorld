using Game.Managers;
using UnityEngine;

namespace Game.Player
{
    [RequireComponent(typeof(Rigidbody2D), typeof(Animator))]
    public class PlayerMovement : MonoBehaviour
    {
        [SerializeField] private float health = 10;
        [SerializeField] private float speed;

        private GameManager _gameManager => GameManager.Instance;
        private Rigidbody2D _body;
        private Animator _animator;
        private SpriteRenderer _spriteRenderer;

        private bool _isMoving = false;

        private string ANIMATOR_ISWALKING { get => "isWalking"; }

        private void OnEnable()
        {
            _gameManager.Inputs.Player.Enable();
            _gameManager.Player = this.gameObject;
            _body = GetComponent<Rigidbody2D>();
            _animator = GetComponent<Animator>();
            _spriteRenderer = GetComponent<SpriteRenderer>();
        }

        private void OnDisable()
        {
            _gameManager.Inputs.Player.Disable();
            _gameManager.Player = null;
        }

        private void Update()
        {
            if (_gameManager.Inputs.Player.Move.IsPressed())
                _isMoving = true;
            else
                _isMoving = false;
        }

        private void FixedUpdate()
        {
            if (_isMoving)
            {
                Move();
                _animator.SetBool(ANIMATOR_ISWALKING, true);
            }
            else
            {
                _animator.SetBool(ANIMATOR_ISWALKING, false);
            }

            FlipSprite();
        }

        private void Move()
        {
            Vector2 inputValue = _gameManager.Inputs.Player.Move.ReadValue<Vector2>();
            Vector2 newPosition = _body.position + inputValue * speed * Time.deltaTime;

            _body.MovePosition(newPosition);
        }

        private void FlipSprite()
        {
            Vector2 playerPositionInScreen = Camera.main.WorldToScreenPoint(transform.position);

            if (_gameManager.MousePosition.x < playerPositionInScreen.x)
                _spriteRenderer.flipX = false;
            else
                _spriteRenderer.flipX = true;
        }

        public void GetDamage(float damage)
        {
            health -= damage;
            if (health <= 0)
                gameObject.SetActive(false);
        }
    }
}
