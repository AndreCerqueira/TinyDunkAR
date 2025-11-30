using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Project.Runtime
{
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(Collider))]
    public class BallLauncher : MonoBehaviour
    {
        [Header("Charge Settings")]
        [SerializeField] private float _minThrowDistance = 0.05f;
        [SerializeField] private float _maxThrowDistance = 0.4f;
        [SerializeField] private float _minArcHeight = 0.025f;
        [SerializeField] private float _maxArcHeight = 0.2f;
        [SerializeField] private float _maxChargeTime = 1.5f;

        [Header("Physics Settings")]
        [SerializeField] private float _gravityValue = 3.0f;
        [SerializeField] private float _respawnDelay = 2f;

        private Rigidbody _rb;
        private Camera _mainCamera;
        private Vector3 _initialLocalPosition;
        private Quaternion _initialLocalRotation;
        
        private InputAction _pressAction;
        private InputAction _positionAction;
        
        private bool _isCharging;
        private bool _isLaunched;
        private float _currentChargeTime;
        
        private const string PRESS_BINDING = "<Pointer>/press";
        private const string POSITION_BINDING = "<Pointer>/position";

        private void Awake()
        {
            _rb = GetComponent<Rigidbody>();
            _mainCamera = Camera.main;

            _initialLocalPosition = transform.localPosition;
            _initialLocalRotation = transform.localRotation;

            _rb.useGravity = false; 
            _rb.isKinematic = true; 

            _rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;

            _pressAction = new InputAction(type: InputActionType.Button, binding: PRESS_BINDING);
            _positionAction = new InputAction(type: InputActionType.Value, binding: POSITION_BINDING);

            _pressAction.started += _ => OnPressStarted();
            _pressAction.canceled += _ => OnPressReleased();
        }

        private void FixedUpdate()
        {
            if (_isLaunched)
            {
                _rb.AddForce(Vector3.down * _gravityValue, ForceMode.Acceleration);
            }
        }

        private void Update()
        {
            if (_isCharging)
            {
                _currentChargeTime += Time.deltaTime;
            }
        }

        private void OnEnable()
        {
            _pressAction.Enable();
            _positionAction.Enable();
        }

        private void OnDisable()
        {
            _pressAction.Disable();
            _positionAction.Disable();
        }

        private void OnPressStarted()
        {
            if (_isLaunched) return;

            var screenPosition = _positionAction.ReadValue<Vector2>();
            CheckAndStartCharge(screenPosition);
        }

        private void OnPressReleased()
        {
            if (_isCharging)
            {
                PerformPhysicsThrow();
            }
        }

        private void CheckAndStartCharge(Vector2 screenPosition)
        {
            var ray = _mainCamera.ScreenPointToRay(screenPosition);

            if (Physics.Raycast(ray, out var hit))
            {
                if (hit.transform == transform)
                {
                    Debug.Log("Toque detetado na bola! A carregar...");
                    _isCharging = true;
                    _currentChargeTime = 0f;
                }
            }
        }

        private void PerformPhysicsThrow()
        {
            Debug.Log("Lançamento iniciado!");
            
            _isCharging = false;
            _isLaunched = true;
            _pressAction.Disable();

            _rb.isKinematic = false; 
            
            var chargeRatio = Mathf.Clamp01(_currentChargeTime / _maxChargeTime);
            
            var targetDistance = Mathf.Lerp(_minThrowDistance, _maxThrowDistance, chargeRatio);
            var targetHeight = Mathf.Lerp(_minArcHeight, _maxArcHeight, chargeRatio);

            var direction = transform.position - _mainCamera.transform.position;
            direction.y = 0;
            direction.Normalize();

            var velocity = CalculateLaunchVelocity(direction, targetDistance, targetHeight);
            
            _rb.linearVelocity = velocity;

            StartCoroutine(RespawnRoutine());
        }

        private Vector3 CalculateLaunchVelocity(Vector3 forwardDir, float distance, float height)
        {
            var gravity = _gravityValue;
            
            var velocityY = Mathf.Sqrt(2 * gravity * height);
            var timeToApex = velocityY / gravity;
            var totalFlightTime = timeToApex * 2;

            var velocityForward = distance / totalFlightTime;

            return (forwardDir * velocityForward) + (Vector3.up * velocityY);
        }

        private IEnumerator RespawnRoutine()
        {
            yield return new WaitForSeconds(_respawnDelay);
            ResetBall();
        }

        private void ResetBall()
        {
            _isLaunched = false;
            _rb.isKinematic = true;
            
            #if UNITY_6000_0_OR_NEWER
            _rb.linearVelocity = Vector3.zero;
            #else
            _rb.velocity = Vector3.zero;
            #endif
            
            _rb.angularVelocity = Vector3.zero;

            transform.localPosition = _initialLocalPosition;
            transform.localRotation = _initialLocalRotation;

            _pressAction.Enable();
        }
    }
}