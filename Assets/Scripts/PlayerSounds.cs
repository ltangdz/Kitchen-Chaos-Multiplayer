using UnityEngine;

/// <summary>
///脚本：PlayerSounds.cs
///时间：2023.9.14
///功能：脚步声
/// </summary>

public class PlayerSounds : MonoBehaviour
{
    private Player player;
    private float footStepTimer;
    private float footStepTimerMax = .1f;

    private void Awake()
    {
        player = GetComponent<Player>();
        footStepTimer = footStepTimerMax;
    }

    private void Update()
    {
        footStepTimer -= Time.deltaTime;
        if (footStepTimer < 0f)
        {
            footStepTimer = footStepTimerMax;

            if (player.IsWalking)
            {
                float volume = 1f;
                SoundManager.Instance.PlayFootStepsSound(player.transform.position, volume);
            }
        }
    }
}
