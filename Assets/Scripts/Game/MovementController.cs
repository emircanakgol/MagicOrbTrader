using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace Game
{
    public class MovementController : MonoBehaviour
    {        
        [Header("References")]
        [SerializeField] private AnimationController animationController;
        [SerializeField] private FixedJoystick joystick;
        private CharacterController _characterController;
        private PlayerController _playerController;
        
        [Header("Fields")]
        [SerializeField] private Vector3 movementInput;
        [SerializeField] private float movementSpeed;
        [SerializeField] private Transform manaBubbles;

        private bool _ismoving;
        public bool isMoving {
            get => _ismoving;
            set {
                if (_ismoving != value) {
                    _ismoving = value;
                    animationController.Walk(value);
                }
            }
        }

        public float movementSpeedProgress;  //{ get; private set; }
        public float movementSpeedMultiplier = 1;
        
        private Vector3 _rotatedInput;
        private Transform _cameraTransform;
        
        private void Awake() {
            _characterController = GetComponent<CharacterController>();
            _playerController = GetComponent<PlayerController>();
            
            if (Camera.main != null) 
                _cameraTransform = Camera.main.transform;
        }

        private void OnEnable() {
            _playerController.OnBucketAddedCallback += SetMovementSpeedMultiplier;
            _playerController.OnBucketRemovedCallback += SetMovementSpeedMultiplier;
            _playerController.OnIngotAddedCallback += SetMovementSpeedMultiplier;
            _playerController.OnIngotRemovedCallback += SetMovementSpeedMultiplier;
        }
        
        private void OnDisable() {
            _playerController.OnBucketAddedCallback -= SetMovementSpeedMultiplier;
            _playerController.OnBucketRemovedCallback -= SetMovementSpeedMultiplier;
            _playerController.OnIngotAddedCallback -= SetMovementSpeedMultiplier;
            _playerController.OnIngotRemovedCallback -= SetMovementSpeedMultiplier;
        }

        private void Update() {
            HandleInput();
        }

        private void HandleInput() {
            if(animationController.isPutting) return;
            
            movementInput = new Vector3(joystick.Horizontal, 0,joystick.Vertical);
            
            movementInput = Vector3.ClampMagnitude(movementInput, 1f);
            _rotatedInput = Quaternion.AngleAxis(_cameraTransform.eulerAngles.y, Vector3.up) * movementInput;
            _characterController.Move(Time.deltaTime * movementSpeed * movementSpeedMultiplier * _rotatedInput);
            if (movementInput != Vector3.zero) {
                isMoving = true;
                movementSpeedProgress = _characterController.velocity.magnitude / movementSpeed;
                transform.forward = _rotatedInput;
                manaBubbles.rotation = Quaternion.Euler (0.0f, 0.0f, transform.rotation.z * -1.0f);
            }
            else {
                isMoving = false;
                movementSpeedProgress = 0;
            }
        }

        private void SetMovementSpeedMultiplier() {
            if(_playerController.Ingots > 0)
                movementSpeedMultiplier = 1 - 0.16f * _playerController.Ingots;
            else
                movementSpeedMultiplier = 1 - 0.16f * _playerController.Buckets;
            
        }
        
        private void SetMovementSpeedMultiplier(bool _) {
            if(_playerController.Ingots > 0)
                movementSpeedMultiplier = 1 - 0.16f * _playerController.Ingots;
            else
                movementSpeedMultiplier = 1 - 0.16f * _playerController.Buckets;
            
        }
    }
}
