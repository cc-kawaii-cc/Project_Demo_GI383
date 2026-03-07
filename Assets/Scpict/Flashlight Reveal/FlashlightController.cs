using UnityEngine;
using UnityEngine.InputSystem;

public class FlashlightController : MonoBehaviour
{
    [Header("Components")]
    public Light flashlightSource;
    public AudioSource clickSoundSource;
    [Header("Settings")]
    public bool isFlashlightOn = false;

    void Start()
    {
        UpdateFlashlightState();
    }
    public void OnFlashlight(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            isFlashlightOn = !isFlashlightOn;
            UpdateFlashlightState();
            if (clickSoundSource != null && clickSoundSource.clip != null)
            {
                clickSoundSource.PlayOneShot(clickSoundSource.clip);
            }
        }
    }
    void UpdateFlashlightState()
    {
        if (flashlightSource != null)
        {
            flashlightSource.enabled = isFlashlightOn;
        }
    }
}