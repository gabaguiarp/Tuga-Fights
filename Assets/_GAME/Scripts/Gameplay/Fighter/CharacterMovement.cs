using UnityEngine;

namespace MemeFight.Characters
{
    [RequireComponent(typeof(Rigidbody2D), typeof(CapsuleCollider2D))]
    public class CharacterMovement : MonoBehaviour
    {
        [Header("Info")]
        [SerializeField, ReadOnly] Vector2 _movement;

        Rigidbody2D _rb;
        CapsuleCollider2D _collider;

        void Awake()
        {
            _rb = GetComponent<Rigidbody2D>();
            _collider = GetComponent<CapsuleCollider2D>();
        }

        void Start()
        {
            SetEnabled(enabled);
        }

        /// <summary>
        /// Moves the rigidbody in the desired <paramref name="direction"/>, at a physics update rate.
        /// </summary>
        public void MoveRigidbody(Vector2 direction)
        {
            if (!enabled)
            {
                Debug.LogWarning(name + " cannot be moved because the CharacterMovement component is disabled!");
                return;
            }

            _movement = direction;
            _rb.MovePosition(_rb.position + _movement);
        }

        /// <summary>
        /// Moves the transform in the desired <paramref name="direction"/>, at a regular frame dependant update rate.
        /// </summary>
        public void MoveTransform(Vector2 direction)
        {
            if (!enabled)
            {
                Debug.LogWarning(name + " cannot be moved because the CharacterMovement component is not disabled!");
                return;
            }

            _movement = direction;
            transform.position += (Vector3)_movement;
        }

        public void SetEnabled(bool enabled)
        {
            _rb.bodyType = enabled ? RigidbodyType2D.Dynamic : RigidbodyType2D.Kinematic;
            _rb.velocity = Vector2.zero;
            _rb.angularVelocity = 0.0f;
            _movement = Vector2.zero;
            _collider.enabled = enabled;
            this.enabled = enabled;
        }
    }
}
