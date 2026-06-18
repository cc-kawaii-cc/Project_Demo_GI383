using UnityEngine;
using UnityEngine.Playables;
using System.Collections;
using TMPro;

public class OfficeSequenceManager : MonoBehaviour
{
    public enum OfficeState { Exploring, Sitting, Eating, AuntCutscene, EmotionalQTE, Completed }
    public OfficeState currentState = OfficeState.Exploring;

    [Header("Player Spawner Settings")]
    public GameObject playerPrefab;    // ลาก Prefab ของผู้เล่นมาใส่ที่นี่
    public Transform spawnPoint;       // ลากจุด PlayerSpawnPoint มาใส่ที่นี่

    [Header("Dynamic Player References (Auto Assigned)")]
    public PlayerMovement playerMovement; // ตัวระบบจะค้นหาและใส่ให้เองตอนเริ่มเกม
    public MouseLook mouseLook;           // ตัวระบบจะค้นหาและใส่ให้เองตอนเริ่มเกม

    [Header("Cutscene References")]
    public PlayableDirector auntCutsceneTimeline; 
    public GameObject qteUI; 
    public TextMeshProUGUI subtitleText;

    [Header("Scene Objects")]
    public GameObject doorExitCollider; 
    public GameObject noodleBowl; 
    public Transform sitPosition; 

    void Start()
    {
        // 1. เรียกใช้งานฟังก์ชันสปอว์นผู้เล่นก่อนระบบอื่นทำงาน
        SpawnPlayerFromPrefab();

        qteUI.SetActive(false);
        if (noodleBowl != null) noodleBowl.GetComponent<Collider>().enabled = false; 
        if (doorExitCollider != null) doorExitCollider.SetActive(false); 
    }

    void SpawnPlayerFromPrefab()
    {
        if (playerPrefab != null && spawnPoint != null)
        {
            // ทำการเกิดออบเจ็กต์ผู้เล่นจาก Prefab ตามตำแหน่งและทิศทางของจุดสปอว์น
            GameObject spawnedPlayer = Instantiate(playerPrefab, spawnPoint.position, spawnPoint.rotation);
            
            // ใช้ระบบสแกนหา Component เพื่อผูกค่าแบบ Dynamic แก้ปัญหา References หลุด
            playerMovement = spawnedPlayer.GetComponent<PlayerMovement>();
            
            // ค้นหา MouseLook (ตรวจสอบว่าติดอยู่ที่ตัว Player หรือลูกของมัน เช่น Main Camera)
            mouseLook = spawnedPlayer.GetComponentInChildren<MouseLook>();

            // [คำแนะนำเพิ่มเติม] ถ้าในฉากมีกล้อง Cinemachine Virtual Camera 
            // จำเป็นต้องอัปเดตเป้าหมาย Follow และ LookAt ให้หันตามตัวที่สปอว์นใหม่ด้วย:
            /*
            var vcam = FindObjectOfType<Cinemachine.CinemachineVirtualCamera>();
            if (vcam != null)
            {
                vcam.Follow = spawnedPlayer.transform;
                vcam.LookAt = spawnedPlayer.transform;
            }
            */
            
            Debug.Log("[Spawn System]: Player ถูกสร้างสำเร็จและผูกระบบตรวจจับเรียบร้อย");
        }
        else
        {
            Debug.LogError("[Spawn System]: ลืมใส่ Player Prefab หรือ Spawn Point ใน Inspector!");
        }
    }

    public void SitAtDesk()
    {
        if (currentState != OfficeState.Exploring || playerMovement == null) return;

        currentState = OfficeState.Sitting;
        playerMovement.enabled = false; 
        
        // ย้ายตัวผู้เล่นไปที่เก้าอี้
        playerMovement.transform.position = sitPosition.position;
        playerMovement.transform.rotation = sitPosition.rotation;

        if (noodleBowl != null) noodleBowl.GetComponent<Collider>().enabled = true; 
    }

    public void EatNoodles()
    {
        if (currentState != OfficeState.Sitting) return;

        currentState = OfficeState.Eating;
        if (noodleBowl != null) noodleBowl.GetComponent<Collider>().enabled = false;
        StartCoroutine(NoodleSequence());
    }

    private IEnumerator NoodleSequence()
    {
        if (subtitleText != null)
        {
            subtitleText.text = "วันนี้เพิ่งจะได้นั่งกินมาม่าดีๆ บ้าง ปกติแทบไม่มีเวลาได้นั่งกินเลย";
            subtitleText.gameObject.SetActive(true);
        }
        yield return new WaitForSeconds(4f);
        if (subtitleText != null) subtitleText.gameObject.SetActive(false);

        currentState = OfficeState.AuntCutscene;
        if (mouseLook != null) mouseLook.enabled = false; 
        if (auntCutsceneTimeline != null) auntCutsceneTimeline.Play(); 
        
        if (auntCutsceneTimeline != null)
        {
            yield return new WaitForSeconds((float)auntCutsceneTimeline.duration);
        }
        else
        {
            yield return new WaitForSeconds(5f); // ค่าเผื่อกรณีไม่มี Timeline
        }

        StartEmotionalQTE();
    }

    private void StartEmotionalQTE()
    {
        currentState = OfficeState.EmotionalQTE;
        if (qteUI != null) qteUI.SetActive(true); 
    }

    public void OnQTECompleted()
    {
        if (currentState != OfficeState.EmotionalQTE) return;

        if (qteUI != null) qteUI.SetActive(false);
        StartCoroutine(ResolutionSequence());
    }

    private IEnumerator ResolutionSequence()
    {
        if (subtitleText != null)
        {
            subtitleText.text = "เก็บเงินไว้ทำศพน้องเถอะป้า คดีนี้ผมทำให้ฟรี... ผมสัญญาว่าจะลากคอมันมาเข้าคุกให้ได้";
            subtitleText.gameObject.SetActive(true);
        }
        yield return new WaitForSeconds(5f);
        if (subtitleText != null) subtitleText.gameObject.SetActive(false);

        currentState = OfficeState.Completed;
        if (playerMovement != null) playerMovement.enabled = true;
        if (mouseLook != null) mouseLook.enabled = true;
        if (doorExitCollider != null) doorExitCollider.SetActive(true); 
    }
}