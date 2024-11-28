using UnityEngine;
using DG.Tweening;

public class SpiralRotation : MonoBehaviour
{
    public RectTransform uiElement;
    public float duration = 2f;
    public int loops = -1; // -1 for infinite loops
    public float scaleMultiplier = 0.5f; // Adjust this to change the spiral size
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            OnEnable();
        }
    }
    private void OnEnable()
    {
        SoundManager.Instance.PlayAudioClip(SoundManager.Instance.AllSounds.levelUpSound);
        // Scale down to create a spiral effect
        uiElement.localScale = Vector3.one * scaleMultiplier;

        // Create a spiral effect by rotating and scaling back up
        uiElement.DOScale(Vector3.one, duration).SetEase(Ease.InOutSine).SetLoops(loops, LoopType.Yoyo);
        uiElement.DORotate(Vector3.forward * (360f * 2), duration, RotateMode.FastBeyond360)
            .SetEase(Ease.Linear)
            .SetLoops(loops, LoopType.Restart);

    }
}
