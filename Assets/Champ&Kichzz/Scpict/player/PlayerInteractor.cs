using UnityEngine;
using TMPro; 
using System.Collections; 
using UnityEngine.InputSystem; // เพิ่มไลบรารี Input System

public class PlayerInteractor : MonoBehaviour
{
    public float interactRange = 3f;
    public LayerMask interactableLayer;

    [Header("Input Actions")]
    public InputActionReference interactAction; // รับค่าปุ่มกดสำรวจ (เช่น ปุ่ม E)

    [Header("Subtitle Settings")]
    public TextMeshProUGUI subtitleText;
    public float subtitleDuration = 4f;

    private Coroutine activeSubtitle;
    private Camera mainCamera;

    void Start()
    {
        mainCamera = Camera.main; 
    }

    void Update()
    {
        if (mainCamera == null) return; 
        Ray ray = new Ray(mainCamera.transform.position, mainCamera.transform.forward);
        Debug.DrawRay(ray.origin, ray.direction * interactRange, Color.red);

        if (Physics.Raycast(ray, out RaycastHit hit, interactRange, interactableLayer))
        {
            StoryInteractable interactable = hit.collider.GetComponent<StoryInteractable>();
            
            if (interactable != null && !interactable.hasInteracted)
            {
                // ตรวจสอบว่าปุ่มถูกกดในเฟรมนี้หรือไม่
                if (interactAction.action.WasPressedThisFrame()) 
                {
                    interactable.DoInteract();
                    
                    if (subtitleText != null)
                    {
                        if (activeSubtitle != null) StopCoroutine(activeSubtitle);
                        activeSubtitle = StartCoroutine(ShowSubtitle(interactable.inspectText));
                    }
                }
            }
        }
    }

    IEnumerator ShowSubtitle(string text)
    {
        subtitleText.text = text;
        subtitleText.gameObject.SetActive(true);
        yield return new WaitForSeconds(subtitleDuration); 
        subtitleText.gameObject.SetActive(false);
    }

    // อย่าลืม Enable/Disable Action
    private void OnEnable() => interactAction?.action.Enable();
    private void OnDisable() => interactAction?.action.Disable();
}