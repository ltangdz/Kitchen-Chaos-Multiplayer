using UnityEngine;
using UnityEngine.UI;

/// <summary>
///脚本：GamePlayingClockUI.cs
///时间：2023.9.19
///功能：
/// </summary>

public class GamePlayingClockUI : MonoBehaviour
{
    public static GamePlayingClockUI Instance { get; private set; }

    [SerializeField] private Image timerImage;

    private void Awake()
    {
        Instance = this;
    }

    private void Update()
    {
        timerImage.fillAmount = GameManager.Instance.GetGamePlayingTimerNormalized();
    }
}
