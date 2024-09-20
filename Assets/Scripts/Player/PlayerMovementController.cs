using UnityEngine;

namespace Player
{
    public class PlayerMovementController : MonoBehaviour
    {
        [SerializeField] private float _speed = 250;
        [SerializeField] private FixedJoystick _variableJoystick;
        [SerializeField] private Rigidbody _playerRb;
        [SerializeField] private GameObject _playerVisuals;
    
        private void FixedUpdate()
        {
            Vector3 direction = Vector3.forward * _variableJoystick.Vertical + Vector3.right * _variableJoystick.Horizontal;
        
            if (direction.magnitude >= 0.1f)
            {
                _playerRb.velocity = direction * _speed * Time.fixedDeltaTime;
                _playerVisuals.transform.rotation = Quaternion.LookRotation(direction);
            }
            else _playerRb.velocity = Vector3.zero;
        }
    }
}