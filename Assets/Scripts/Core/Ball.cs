using UnityEngine;

namespace Core
{
    public class Ball : MonoBehaviour
    {
        public delegate void CollisionEventHandler();

        public static event CollisionEventHandler HitEvent;
        public static event CollisionEventHandler MissEvent;

        private Vector3 _initialPosition;
        private Rigidbody2D _rigidbody2D;
        private Color _myColor;

        // Start is called before the first frame update
        private void Awake()
        {
            _initialPosition = transform.position;
            _rigidbody2D = GetComponent<Rigidbody2D>();
            DisableMovement();
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            Color otherColor = other.GetComponent<SpriteRenderer>().color;

            if (_myColor == otherColor)
                OnHit();
            else
                OnMiss();

            MoveBall(_initialPosition);

            DisableMovement();
        }

        private static void OnHit()
        {
            HitEvent?.Invoke();
        }

        private static void OnMiss()
        {
            MissEvent?.Invoke();
        }

        public void SetBallColor(Color color)
        {
            _myColor = color;
            GetComponent<SpriteRenderer>().color = color;
        }

        public void EnableMovement() => _rigidbody2D.isKinematic = false;

        public void DisableMovement()
        {
            _rigidbody2D.velocity = Vector2.zero;
            _rigidbody2D.isKinematic = true;
        }

        public void MoveBall(Vector3 position) => transform.position = position;
    }
}