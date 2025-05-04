using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SpriteAnimatorUI : MonoBehaviour {
    public Image targetImage;       // UI Image component displaying the sprite animation.
    public Sprite[] frames;         // Array of sprites for the animation frames.
    public float frameRate = 10f;   // Frame rate for the animation.

    private bool isAnimating = false;

    // This public method triggers the shoot animation on demand.
    public void PlayShootAnimation() {
        if (isAnimating) {
            StopAllCoroutines();
        }
        StartCoroutine(Animate());
    }

    // Coroutine that runs the animation sequence.
    private IEnumerator Animate() {
        isAnimating = true;
        // Cycle through each frame once.
        for (int i = 0; i < frames.Length; i++) {
            targetImage.sprite = frames[i];
            yield return new WaitForSeconds(1f / frameRate);
        }
        // Optionally, you can reset to a default idle frame (for example, frames[0]) here.
        // targetImage.sprite = frames[0];
        isAnimating = false;
    }
}
