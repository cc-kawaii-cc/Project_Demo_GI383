using UnityEngine;
using TMPro;

public class DialogSystem : MonoBehaviour
{
    
    public static DialogSystem instance;

    [Header("Player Control")]
    public PlayerMovement playerMovement;
    public MouseLook mouseLook;

    [Header("UI References")]
    public GameObject dialogPanel; 
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI dialogText;

    private string[] currentNames;
    private string[] currentLines;
    private int currentLineIndex = 0;
    
    private bool isTalking = false;
    private NPCWalker currentNPC;

    private void Awake()
    {
        if (instance == null) instance = this;
    }

    
    public void StartDialog(NPCWalker npc, string[] names, string[] lines)
    {
        if (isTalking) return;
        
        isTalking = true;
        currentNPC = npc;
        currentNames = names;
        currentLines = lines;
        currentLineIndex = 0;

        
        if (playerMovement) playerMovement.enabled = false;
        if (mouseLook) mouseLook.enabled = false;

        
        dialogPanel.SetActive(true);
        ShowNextLine();
    }

    void Update()
    {
        
        if (isTalking && Input.GetMouseButtonDown(0))
        {
            currentLineIndex++;
            if (currentLineIndex < currentLines.Length)
            {
                ShowNextLine();
            }
            else
            {
                EndDialog();
            }
        }
    }

    void ShowNextLine()
    {
        nameText.text = currentNames[currentLineIndex];
        dialogText.text = currentLines[currentLineIndex];
    }

    void EndDialog()
    {
        isTalking = false;
        dialogPanel.SetActive(false);
        
        // ปลดล็อกผู้เล่น
        if (playerMovement) playerMovement.enabled = true;
        if (mouseLook) mouseLook.enabled = true;

        // สั่งให้ NPC เดินต่อ
        if (currentNPC) currentNPC.EndTalking();
    }
}