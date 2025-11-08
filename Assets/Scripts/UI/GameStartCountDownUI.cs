using TMPro;
using UnityEngine;

/// <summary>
///脚本：GameStartCountDownUI.cs
///时间：2023.9.19
///功能：
/// </summary>

public class GameStartCountDownUI : MonoBehaviour
{
    private const string NUMBER_POP_UP = "NumberPopUp";

    public static GameStartCountDownUI Instance { get; private set; }

    [SerializeField] private TextMeshProUGUI countDownText;

    private Animator animator;
    private int previousCountDownNumber;

    private void Awake()
    {
        Instance = this;
        animator = GetComponent<Animator>();
    }

    private void Start()
    {
        GameManager.Instance.OnStateChanged += GameManager_OnStateChanged;
        Hide();
    }

    private void GameManager_OnStateChanged(object sender, System.EventArgs e)
    {
        if (GameManager.Instance.IsCountDownToStartActive())
        {
            Show();
        }
        else
        {
            Hide();
        }
    }

    private void Update()
    {
        int countDownNumber = Mathf.CeilToInt(GameManager.Instance.GetCountDownToStartTimer());
        countDownText.text = countDownNumber.ToString();

        if (previousCountDownNumber != countDownNumber)
        {
            previousCountDownNumber = countDownNumber;
            animator.SetTrigger(NUMBER_POP_UP);
            SoundManager.Instance.PlayCountDownSound();
        }
    }

    private void Show()
    {
        gameObject.SetActive(true);
    }

    private void Hide()
    {
        gameObject.SetActive(false);
    }
}
