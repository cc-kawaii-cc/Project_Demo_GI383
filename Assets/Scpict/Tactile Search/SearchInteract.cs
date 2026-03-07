using UnityEngine;
using UnityEngine.UI;
public class SearchInteract : MonoBehaviour
{
    [Header("UI References")]
    public Image handIcon;
    public Image progressBar;
    [Header("Settings")]
    public float searchTime = 2.0f;
    public float interactDistance = 3f;
    private float currentTimer = 0f;
    private Camera cam;
    void Start()
    {
        cam = Camera.main;
        handIcon.gameObject.SetActive(false);
        progressBar.fillAmount = 0;
    }
    void Update()
    {
        Ray ray = new Ray(cam.transform.position, cam.transform.forward);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, interactDistance))
        {
            if (hit.collider.CompareTag("Searchable"))
            {
                handIcon.gameObject.SetActive(true);
                if (Input.GetKey(KeyCode.E))
                {
                    currentTimer += Time.deltaTime;
                    progressBar.fillAmount = currentTimer / searchTime;
                    if (currentTimer >= searchTime)
                    {
                        CompleteSearch(hit.collider.gameObject);
                    }
                }
                else
                {
                    ResetSearch();
                }
                return;
            }
        }
        handIcon.gameObject.SetActive(false);
        ResetSearch();
    }
    void ResetSearch()
    {
        currentTimer = 0;
        progressBar.fillAmount = 0;
    }
    void CompleteSearch(GameObject target)
    {
        Debug.Log("Find Item!: " + target.name);
        Destroy(target);
        ResetSearch();
        handIcon.gameObject.SetActive(false);
    }
}