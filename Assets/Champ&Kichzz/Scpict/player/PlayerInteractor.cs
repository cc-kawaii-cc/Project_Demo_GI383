using UnityEngine;

public class PlayerInteractor : MonoBehaviour
{
    public float interactRange = 3f;
    public LayerMask interactableLayer;

    void Update()
    {
        Ray ray = new Ray(transform.position, transform.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, interactRange, interactableLayer))
        {
            StoryInteractable interactable = hit.collider.GetComponent<StoryInteractable>();
            if (interactable != null)
            {
                // TODO: A UI appears in the center of the screen saying "Press [E] to explore"
                if (Input.GetKeyDown(KeyCode.E))
                {
                    interactable.DoInteract();
                }
            }
        }
    }
}
