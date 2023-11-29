using System;
using System.Collections.Generic;
using Game.UI;
using UnityEngine;
using UnityEngine.VFX;

namespace Game
{
    public class PlayerController : MonoBehaviour
    {
        public event Action OnBubbleAddedCallback;
        public event Action<bool> OnBubbleRemovedCallback;
        public event Action OnBucketAddedCallback;
        public event Action<bool> OnBucketRemovedCallback;
        public event Action OnIngotAddedCallback;
        public event Action<bool> OnIngotRemovedCallback;
        public event Action OnGoldAddedCallback;
        public event Action OnGoldRemovedCallback;
        
        [Header("Prefabs")]
        [SerializeField] private GameObject bucketPrefab;
        [SerializeField] private GameObject ingotPrefab;

        [Header("References")] 
        [SerializeField] private GameController gameController;
        [SerializeField] private Transform handTransform;
        [SerializeField] private VisualEffect bubbleVFX;
        [SerializeField] private InteractButton interactButton;
        
        [Header("Settings")] 
        public bool isInteracting;
        
        // Is serialized for debugging purposes.
        [SerializeField] private int _bubbles;
        public int Bubbles {
            get => _bubbles;
            set {
                if (_bubbles == value) return;
                if (_bubbles < value) {
                    _bubbles = value;
                    OnBubbleAddedCallback?.Invoke();
                }
                else {
                    _bubbles = value;
                    if (_bubbles == 0)
                        OnBubbleRemovedCallback?.Invoke(true);
                    else
                        OnBubbleRemovedCallback?.Invoke(false);
                }
            }
        }
        
        // Is serialized for debugging purposes.
        [SerializeField] private int _buckets;
        public int Buckets {
            get => _buckets;
            set {
                if (_buckets == value) return;
                if (_buckets < value) {
                    _buckets = value;
                    OnBucketAddedCallback?.Invoke();
                }
                else {
                    _buckets = value;
                    if(_buckets == 0)
                        OnBucketRemovedCallback?.Invoke(true);
                    else
                        OnBucketRemovedCallback?.Invoke(false);
                }
            }
        }
        
        // Is serialized for debugging purposes.
        [SerializeField] private int _ingots;
        public int Ingots {
            get => _ingots;
            set {
                if (_ingots == value) return;
                if (_ingots < value) {
                    _ingots = value;
                    OnIngotAddedCallback?.Invoke();
                }
                else {
                    _ingots = value;
                    if (_ingots == 0)
                        OnIngotRemovedCallback?.Invoke(true);
                    else
                        OnIngotRemovedCallback?.Invoke(false);
                }
            }
        }
        
        // Is serialized for debugging purposes.
        [SerializeField] private int _golds;
        public int Golds {
            get => _golds;
            set {
                if (_golds == value) return;
                if (_golds < value) {
                    _golds = value;
                    OnGoldAddedCallback?.Invoke();
                }
                else {
                    _golds = value;
                    OnGoldRemovedCallback?.Invoke();
                }
            }
        }
        public int totalGainedGold;
        public int maxBubbleAtHand;

        private List<GameObject> buckets;
        private List<GameObject> ingots;
        
        private void Awake() {
            buckets = new List<GameObject>();
            ingots = new List<GameObject>();
        }

        private void OnEnable() {
            OnBubbleAddedCallback += RefreshVFX;
            OnBubbleRemovedCallback += RefreshVFX;
            OnBucketAddedCallback += AddBucket;
            OnBucketRemovedCallback += RemoveBucket;
            OnIngotAddedCallback += AddIngot;
            OnIngotRemovedCallback += RemoveIngot;
        }

        private void OnDisable() {
            OnBubbleAddedCallback -= RefreshVFX;
            OnBubbleRemovedCallback -= RefreshVFX;
            OnBucketAddedCallback -= AddBucket;
            OnBucketRemovedCallback -= RemoveBucket;
            OnIngotAddedCallback -= AddIngot;
            OnIngotRemovedCallback -= RemoveIngot;
        }

        private void Update() {
            isInteracting = interactButton.isInteracting;
        }

        private void RefreshVFX() {
            bubbleVFX.SetInt("Count", Bubbles);
            bubbleVFX.Reinit();
        }
        private void RefreshVFX(bool _) {
            bubbleVFX.SetInt("Count", Bubbles);
            bubbleVFX.Reinit();
        }
        
        private void AddBucket() {
            Vector3 spawnPos = new Vector3(0, 0.003f * buckets.Count, 0.003f);
            GameObject bucketGO = Instantiate(bucketPrefab, handTransform);
            bucketGO.transform.localPosition = spawnPos;
            bucketGO.transform.localScale = new Vector3(0.01f, 0.01f, 0.01f);
            buckets.Add(bucketGO);
        }
        private void RemoveBucket(bool removedAll) {
            if (removedAll) {
                foreach (var bucket in buckets) {
                    Destroy(bucket);
                }
                buckets.Clear();
            }
            else {
                Destroy(buckets[^1]);
                buckets.RemoveAt(buckets.Count - 1); 
            }
        }
        
        private void AddIngot() {
            Vector3 spawnPos = new Vector3(0, 0.001f * ingots.Count, 0.003f);
            GameObject ingotGO = Instantiate(ingotPrefab, handTransform);
            Transform ingotTr = ingotGO.transform;
            ingotTr.localPosition = spawnPos;
            ingotTr.rotation = Quaternion.Euler(0,90,0);
            ingotTr.localScale = new Vector3(0.0004f, 0.0004f, 0.0004f);
            ingots.Add(ingotGO);
        }
        private void RemoveIngot(bool removedAll) {
            if (removedAll) {
                foreach (var ingot in ingots) {
                    Destroy(ingot);
                }
                ingots.Clear();
            }
            else {
                Destroy(ingots[^1]);
                ingots.RemoveAt(ingots.Count - 1); 
            }
        }
    }
}
