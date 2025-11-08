using System;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
///脚本：ProgressBarUI.cs
///时间：2023.9.12
///功能：进度条UI视觉表现
/// </summary>

public class ProgressBarUI : MonoBehaviour
{
    [SerializeField] private GameObject hasProgressGameObject;
    [SerializeField] private Image barImage;

    private IHasProgress hasProgress;

    private void Start()
    {
        try
        {
            hasProgress = hasProgressGameObject.GetComponent<IHasProgress>();

            hasProgress.OnProgressChanged += HasProgress_OnProgressChanged;

            barImage.fillAmount = 0f;

            Hide();
        }
        catch (NullReferenceException ex)
        {
            Debug.LogError("GameObject" + hasProgressGameObject +
                           "does not have a component that implements IHasProgress!" + ex.Message);
            throw new NullReferenceException();
        }
        catch (Exception ex)
        {
            Debug.LogError(ex.ToString());
            throw new Exception();
        }
    }

    private void HasProgress_OnProgressChanged(object sender, IHasProgress.OnProgressChangeEventArgs e)
    {
        barImage.fillAmount = e.progressNormalized;

        if (e.progressNormalized == 0f || Mathf.Approximately(e.progressNormalized, 1f))
        {
            Hide();
        }
        else
        {
            Show();
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
