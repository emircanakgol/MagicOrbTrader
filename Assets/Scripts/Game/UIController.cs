using System.Collections.Generic;
using DG.Tweening;
using Game.Areas;
using Game.UI;
using TMPro;
using UnityEngine;

namespace Game
{
    public class UIController : MonoBehaviour
    {
        public static UIController Instance;
        
        [Header("Prefabs")]
        [SerializeField] private GameObject orderPanelPrefab;
        
        [Header("References")]
        [Header("Gameplay")] 
        [SerializeField] private House house;
        [SerializeField] private Runestone runestone;
        [SerializeField] private PlayerController player;
        [SerializeField] private GameController gameController;
        
        [Header("UI")] 
        [SerializeField] private TextMeshProUGUI infoTMP;
        [SerializeField] private RectTransform levelValue;
        [SerializeField] private TextMeshProUGUI levelTMP;
        [SerializeField] private GameObject gameOverPanel;
        [SerializeField] private TextMeshProUGUI gameOverTMP;
        [SerializeField] private StartButton startButton;

        [Header("Bottom Panel")]
        public GameObject manaBar;
        [SerializeField] private RectTransform manaValue;
        [SerializeField] private TextMeshProUGUI manaBarTMP;
        [SerializeField] private TextMeshProUGUI maxManaTMP;
        public GameObject bubbleBar;
        [SerializeField] private RectTransform bubbleValue;
        [SerializeField] private TextMeshProUGUI bubbleBarTMP;
        [SerializeField] private TextMeshProUGUI maxBubbleTMP;
        public GameObject gemstoneBar;
        [SerializeField] private RectTransform gemstoneValue;
        [SerializeField] private TextMeshProUGUI gemstoneBarTMP;
        [SerializeField] private TextMeshProUGUI maxGemstoneTMP;
        [SerializeField] private TextMeshProUGUI goldTMP;
        
        
        [Header("Right Panel")]
        [SerializeField] private GameObject oneTextGO;
        [SerializeField] private GameObject twoTextGO;
        [SerializeField] private GameObject threeTextGO;
        [SerializeField] private Transform orderPanelParent;

        private int _oldMana;
        private int _oldMaxMana;
        private int _oldBubble;
        private int _oldMaxBubble;
        private int _oldGemstone;
        private int _oldMaxGemstone;
        private int _oldGold;

        private List<GameObject> _orderPanels;

        private void Awake() {
            if (Instance != null && Instance != this)
                Destroy(gameObject);
            else
                Instance ??= this;
            
            _orderPanels = new List<GameObject>();
        }

        private void OnEnable() {
            house.OnManaIncreasedCallback += UpdateManaBar;
            house.OnManaDecreasedCallback += UpdateManaBar;
            house.OnMaxManaChangedCallback += UpdateMaxManaUI;
            house.OnMaxManaChangedCallback += UpdateManaBar;
            runestone.OnBubbleIncreasedCallback += UpdateBubbleBar;
            runestone.OnBubbleDecreasedCallback += UpdateBubbleBar;
            runestone.OnMaxBubbleChangedCallback += UpdateMaxBubbleUI;
            runestone.OnMaxBubbleChangedCallback += UpdateBubbleBar;
            runestone.OnGemstoneIncreasedCallback += UpdateGemstoneBar;
            runestone.OnGemstoneDecreasedCallback += UpdateGemstoneBar;
            runestone.OnMaxGemstoneChangedCallback += UpdateMaxGemstoneUI;
            runestone.OnMaxGemstoneChangedCallback += UpdateGemstoneBar;
            player.OnGoldAddedCallback += UpdateGoldText;
            player.OnGoldRemovedCallback += UpdateGoldText;
            gameController.OnShopStartCallback += OnShopStart;
            gameController.OnShopStopCallback += OnShopStop;
        }

        private void OnDisable() {
            house.OnManaIncreasedCallback -= UpdateManaBar;
            house.OnManaDecreasedCallback -= UpdateManaBar;
            house.OnMaxManaChangedCallback -= UpdateMaxManaUI;
            house.OnMaxManaChangedCallback -= UpdateManaBar;
            runestone.OnBubbleIncreasedCallback -= UpdateBubbleBar;
            runestone.OnBubbleDecreasedCallback -= UpdateBubbleBar;
            runestone.OnMaxBubbleChangedCallback -= UpdateMaxBubbleUI;
            runestone.OnMaxBubbleChangedCallback -= UpdateBubbleBar;
            runestone.OnGemstoneIncreasedCallback -= UpdateGemstoneBar;
            runestone.OnGemstoneDecreasedCallback -= UpdateGemstoneBar;
            runestone.OnMaxGemstoneChangedCallback -= UpdateMaxGemstoneUI;
            runestone.OnMaxGemstoneChangedCallback -= UpdateGemstoneBar;
            player.OnGoldAddedCallback -= UpdateGoldText;
            player.OnGoldRemovedCallback -= UpdateGoldText;
            gameController.OnShopStartCallback -= OnShopStart;
            gameController.OnShopStopCallback -= OnShopStop;
        }

        private void Start() {
            _oldMana = house.Mana;
            _oldMaxMana = house.MaxMana;
            _oldGold = player.Golds;
        }
        
        private void UpdateManaBar() {
            var mana = house.Mana;
            var maxMana = house.MaxMana;
            
            manaValue.DOSizeDelta(new Vector2(mana * 400f/maxMana, 40), 0.5f);
            DOVirtual.Int(_oldMana,
                mana,
                0.5f,
                (x) => {
                    manaBarTMP.text = x.ToString();
                }).OnComplete(
                () => _oldMana = mana);
        }

        private void UpdateMaxManaUI() {
            var maxMana = house.MaxMana;
            DOVirtual.Int(_oldMaxMana,
                maxMana,
                0.5f,
                (x) => {
                    maxManaTMP.text = x.ToString();
                }).OnComplete(
                () => _oldMaxMana = maxMana);
        }
        
        private void UpdateBubbleBar() {
            var bubble = runestone.Bubbles;
            var maxBubble = runestone.MaxBubbles;
            
            bubbleValue.DOSizeDelta(new Vector2(bubble * 200f/maxBubble, 40), 0.5f);
            DOVirtual.Int(_oldBubble,
                bubble,
                0.5f,
                (x) => {
                    bubbleBarTMP.text = x.ToString();
                }).OnComplete(
                () => _oldBubble = bubble);
        }

        private void UpdateMaxBubbleUI() {
            var maxBubble = runestone.MaxBubbles;
            DOVirtual.Int(_oldMaxBubble,
                maxBubble,
                0.5f,
                (x) => {
                    maxBubbleTMP.text = x.ToString();
                }).OnComplete(
                () => _oldMaxBubble = maxBubble);
        }
        
        private void UpdateGemstoneBar() {
            var gemstone = runestone.Gemstones;
            var maxGemstone = runestone.MaxGemstones;
            
            gemstoneValue.DOSizeDelta(new Vector2(gemstone * 200f/maxGemstone, 40), 0.5f);
            DOVirtual.Int(_oldGemstone,
                gemstone,
                0.5f,
                (x) => {
                    gemstoneBarTMP.text = x.ToString();
                }).OnComplete(
                () => _oldGemstone = gemstone);
        }

        private void UpdateMaxGemstoneUI() {
            var maxGemstone = runestone.MaxGemstones;
            DOVirtual.Int(_oldMaxGemstone,
                maxGemstone,
                0.5f,
                (x) => {
                    maxGemstoneTMP.text = x.ToString();
                }).OnComplete(
                () => _oldMaxGemstone = maxGemstone);
        }

        private void UpdateGoldText() {
            var gold = player.Golds;
            DOVirtual.Int(_oldGold,
                gold,
                0.5f,
                (x) => {
                    goldTMP.text = x + "g";
                }).OnComplete(
                () => _oldGold = gold);
        }

        public void UpdateRightPanel() {
            foreach (var opGO in _orderPanels) {
                Destroy(opGO);
            }
            _orderPanels.Clear();

            var topIndexes = gameController.Customers.Count < 3 ? gameController.Customers.Count : 3;
            
            for (int i = 0; i < topIndexes; i++) {
                var orderPanelGO = Instantiate(orderPanelPrefab, orderPanelParent);
                var orderPanel = orderPanelGO.GetComponent<OrderPanel>();
                orderPanel.Customer = gameController.Customers[i];
                _orderPanels.Add(orderPanelGO);
            }

            switch (topIndexes) {
                case 0:
                    oneTextGO.SetActive(false);
                    twoTextGO.SetActive(false);
                    threeTextGO.SetActive(false);
                    break;
                case 1:
                    oneTextGO.SetActive(true);
                    twoTextGO.SetActive(false);
                    threeTextGO.SetActive(false);
                    break;
                case 2:
                    oneTextGO.SetActive(true);
                    twoTextGO.SetActive(true);
                    threeTextGO.SetActive(false);
                    break;
                default:
                    oneTextGO.SetActive(true);
                    twoTextGO.SetActive(true);
                    threeTextGO.SetActive(true);
                    break;
            }
        }

        public void UpdateLevelBar() {
            if (gameController.difficulty == 4) {
                levelValue.sizeDelta = Vector2.zero;
                levelTMP.text = "MAX";
                return;
            }
            var neededExp = gameController.DifficultyIncreaseGold[gameController.difficulty];
            var exp = player.totalGainedGold % neededExp;
            
            levelValue.DOSizeDelta(new Vector2(exp * 400f/neededExp, 20), 0.5f);
            levelTMP.text = (gameController.difficulty + 1).ToString();
        }

        public void GameOver(string reason) {
            gameOverPanel.SetActive(true);
            gameOverTMP.text = "GAME OVER\n"+reason;
        }

        private void OnShopStart() {
            infoTMP.gameObject.SetActive(false);
            startButton.gameObject.SetActive(false);
        }

        private void OnShopStop(string reason) {
            infoTMP.gameObject.SetActive(true);
            infoTMP.text = reason;
            startButton.gameObject.SetActive(true);
        }
    }
}
