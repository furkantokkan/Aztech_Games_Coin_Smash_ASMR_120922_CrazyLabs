using DG.Tweening;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeButton : MonoBehaviour
{
    [Header("Item")]
    [SerializeField] private UpgradeItemData item;
    [Header("Animations")]
    [SerializeField] private Vector2 punch;
    [SerializeField] private int punchVibrato;
    [SerializeField] private float punchDuration;
    [SerializeField] private float inactiveAlpha;
    [Header("UI")]
    [SerializeField] private TextMeshProUGUI price;

    private IInteractable interaction;

    private CanvasGroup group;

    private Tween punchTween;
    private void Awake()
    {
        interaction = GetComponent<IInteractable>();
    }
    private void Start()
    {
        Initialize();
    }
    private void OnDisable()
    {
        EconomyManager.Instance.onSpendMoney -= UpdateButton;
        GameManager.Instance.updateUI -= UpdateButton;
        BallMovement.onBallsStartToMove -= UpdateButton;
    }
    public void Initialize()
    {
        group = GetComponent<CanvasGroup>();
        EconomyManager.Instance.onSpendMoney += UpdateButton;
        BallMovement.onBallsStartToMove += UpdateButton;
        GameManager.Instance.updateUI += UpdateButton;
        UpdateButton();
    }

    public void UpdateButton()
    {
        SetupUI();
    }

    public void SetupUI()
    {
        bool interactable = EconomyManager.Instance.GetCurrentMoney() >= item.CurrentPrice && item.currentLevel < item.maxLevel && interaction.CanInteract();
        group.interactable = interactable;
        group.alpha = interactable ? 1f : inactiveAlpha;
        price.text = item.currentLevel < item.maxLevel ? ((int)item.CurrentPrice).ToString() : "Max";
    }

    public void Upgrade()
    {
        if (item.Upgrade())
        {
            punchTween?.Kill(true);
            punchTween = transform.DOPunchScale(punch, punchDuration, punchVibrato);
            SetupUI();
        }
    }
}


