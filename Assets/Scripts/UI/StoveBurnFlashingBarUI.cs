using UnityEngine;

/// <summary>
///脚本：StoveBurnFlashingBarUI.cs
///时间：2023.9.26
///功能：
/// </summary>

public class StoveBurnFlashingBarUI : MonoBehaviour
{
    private const string IS_FLASHING = "IsFlashing";

    [SerializeField] private StoveCounter stoveCounter;

    private Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void Start()
    {
        stoveCounter.OnProgressChanged += StoveCounter_OnProgressChanged;

        animator.SetBool(IS_FLASHING, false);
    }

    private void StoveCounter_OnProgressChanged(object sender, IHasProgress.OnProgressChangeEventArgs e)
    {
        float burnShowProgressAmount = .5f;
        bool isFlashing = stoveCounter.IsFried && e.progressNormalized >= burnShowProgressAmount;

        animator.SetBool(IS_FLASHING, isFlashing);
    }
}
