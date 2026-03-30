using UnityEngine;
using TMPro; 
using System.Collections; 

public class PlayerInteractor : MonoBehaviour
{
    public float interactRange = 3f;
    public LayerMask interactableLayer;

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
                if (Input.GetKeyDown(KeyCode.E))
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
}