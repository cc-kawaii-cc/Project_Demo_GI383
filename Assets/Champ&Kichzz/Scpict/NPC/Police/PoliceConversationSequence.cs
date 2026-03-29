using System.Collections;
using UnityEngine;
using TMPro;

public class PoliceConversationSequence : MonoBehaviour
{
    [Header("References")]
    public GameObject player;
    public PlayerMovement playerMovement;
    public MouseLook mouseLook;
    public CharacterController controller;
    public Camera mainCamera;

    [Header("Police NPC")]
    public GameObject policeNPC;
    public Transform policeTargetPoint; 
    public float policeSpeed = 4f;
    public Animator policeAnimator; 

    [Header("Dialog UI")]
    public GameObject dialogPanel;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI dialogText;
    public string[] speakerNames;
    [TextArea(3, 5)] public string[] dialogLines;

    private int currentLine = 0;
    private bool isSequenceStarted = false;
    private bool canAdvanceDialog = false;

    
    private Vector3 originalPosition;
    private Quaternion originalRotation;

    void Start()
    {
        
        if (policeNPC != null)
        {
            originalPosition = policeNPC.transform.position;
            originalRotation = policeNPC.transform.rotation;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !isSequenceStarted)
        {
            StartCoroutine(StartPoliceSequence());
        }
    }

    IEnumerator StartPoliceSequence()
    {
        isSequenceStarted = true;

      
        playerMovement.enabled = false;
        mouseLook.enabled = false;
        controller.enabled = false;

        
        if (policeAnimator) policeAnimator.SetBool("isRunning", true);
        
        while (Vector3.Distance(policeNPC.transform.position, policeTargetPoint.position) > 0.1f)
        {
            
            policeNPC.transform.position = Vector3.MoveTowards(policeNPC.transform.position, policeTargetPoint.position, policeSpeed * Time.deltaTime);
            
         
            Vector3 lookPos = player.transform.position;
            lookPos.y = policeNPC.transform.position.y;
            policeNPC.transform.LookAt(lookPos);
            
           
            Vector3 dir = (policeNPC.transform.position + Vector3.up * 1.5f) - mainCamera.transform.position;
            mainCamera.transform.rotation = Quaternion.Slerp(mainCamera.transform.rotation, Quaternion.LookRotation(dir), Time.deltaTime * 5f);
            
            yield return null;
        }
        if (policeAnimator) policeAnimator.SetBool("isRunning", false);

       
        dialogPanel.SetActive(true);
        ShowNextLine();
    }

    void Update()
    {
        if (canAdvanceDialog && dialogPanel.activeSelf && Input.GetMouseButtonDown(0))
        {
            currentLine++;
            if (currentLine < dialogLines.Length)
            {
                ShowNextLine();
            }
            else
            {
                EndSequence();
            }
        }
    }

    void ShowNextLine()
    {
        nameText.text = speakerNames[currentLine];
        dialogText.text = dialogLines[currentLine];
        
        canAdvanceDialog = true; 
    }

    void EndSequence()
    {
       
        dialogPanel.SetActive(false);
        playerMovement.enabled = true;
        mouseLook.enabled = true;
        controller.enabled = true;
        canAdvanceDialog = false;

        
        StartCoroutine(PoliceReturnSequence());
    }

    IEnumerator PoliceReturnSequence()
    {
        
        if (policeAnimator) policeAnimator.SetBool("isRunning", true);
        
        while (Vector3.Distance(policeNPC.transform.position, originalPosition) > 0.1f)
        {
          
            policeNPC.transform.position = Vector3.MoveTowards(policeNPC.transform.position, originalPosition, policeSpeed * Time.deltaTime);
            
         
            Vector3 dir = (originalPosition - policeNPC.transform.position).normalized;
            if (dir != Vector3.zero)
            {
                policeNPC.transform.rotation = Quaternion.Slerp(policeNPC.transform.rotation, Quaternion.LookRotation(dir), Time.deltaTime * 8f);
            }
            
            yield return null;
        }
        
        
        policeNPC.transform.rotation = originalRotation;
        if (policeAnimator) policeAnimator.SetBool("isRunning", false);
        
       
        this.enabled = false; 
    }
}