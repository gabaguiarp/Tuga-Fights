using UnityEngine;

namespace MemeFight
{
    public class Rotator : MonoBehaviour
    {
        public enum Orientation { Clockwise = -1, Counterclockwise = 1 }

        public Orientation orientation = Orientation.Clockwise;
        public float rotationSpeed = 10f;
        public bool isActive = true;

        float _targetAngle = 0.0f;

        void Update()
        {
            if (isActive)
            {
                _targetAngle += rotationSpeed * (int)orientation * Time.deltaTime;
                transform.rotation = Quaternion.Euler(transform.forward * _targetAngle);
            }
        }
    }
}
