using System;
using UnityEngine;

namespace Game
{
    public class Floating : MonoBehaviour
    {
        private Vector3 startPos;
        private float _y;

        [SerializeField] private float floatHeight;
        [SerializeField] private float floatSpeed;

        private void Start() {
            startPos = transform.position;
        }

        private void Update() {
            _y = Mathf.Cos(Time.time * floatSpeed) * floatHeight;
            transform.position = _y * Vector3.up + startPos;
        }
    }
}
