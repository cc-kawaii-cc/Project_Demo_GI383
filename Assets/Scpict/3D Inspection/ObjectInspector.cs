using UnityEngine;

public class ObjectInspector : MonoBehaviour
{
    [Header("References")]
    public Transform inspectPoint;
    public MonoBehaviour playerMoveScript;
    public MonoBehaviour mouseLookScript;

    private GameObject currentModel;
    private bool isInspecting = false;
    public float rotateSpeed = 500f;

    void Update()
    {
        if (isInspecting && currentModel != null)
        {
            float rotX = Input.GetAxis("Mouse X") * rotateSpeed * Time.deltaTime;
            float rotY = Input.GetAxis("Mouse Y") * rotateSpeed * Time.deltaTime;
            currentModel.transform.Rotate(Vector3.up, -rotX, Space.World);
            currentModel.transform.Rotate(Vector3.right, rotY, Space.World);
            if (Input.GetKeyDown(KeyCode.Escape) || Input.GetMouseButtonDown(1))
            {
                CloseInspection();
            }
        }
    }
    public void ShowItem(GameObject itemPrefab)
    {
        if (isInspecting) return;
        isInspecting = true;
        if(playerMoveScript != null) playerMoveScript.enabled = false;
        if(mouseLookScript != null) mouseLookScript.enabled = false;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        currentModel = Instantiate(itemPrefab, inspectPoint.position, inspectPoint.rotation);
        currentModel.transform.parent = inspectPoint;
    }
    public void CloseInspection()
    {
        isInspecting = false;
        if (currentModel != null) Destroy(currentModel);
        if(playerMoveScript != null) playerMoveScript.enabled = true;
        if(mouseLookScript != null) mouseLookScript.enabled = true;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}
