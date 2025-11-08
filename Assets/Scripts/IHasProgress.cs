using System;
using UnityEngine;

/// <summary>
///脚本：IHasProgress.cs
///时间：2023.9.13
///功能：
/// </summary>

public interface IHasProgress
{
    public event EventHandler<OnProgressChangeEventArgs> OnProgressChanged;
    public class OnProgressChangeEventArgs : EventArgs
    {
        public float progressNormalized;
    }

}
