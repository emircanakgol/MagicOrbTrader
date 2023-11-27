using System;
using UnityEngine;

namespace Game
{
    public class Billboard : MonoBehaviour
    {
        private Transform _camTransform;

        private void Awake() {
            if (Camera.main != null) _camTransform = Camera.main.transform;
        }

        private void LateUpdate() {
            transform.LookAt(transform.position + _camTransform.forward);
        }
    }
}
