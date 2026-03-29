using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;

public class GhostEncounterManager : MonoBehaviour
{
    [Header("Sequence Components")]
    public AudioSource rainAudio;
    public Volume glitchVolume;
    public FlashlightController playerFlashlight;
    public Transform playerBody;

    [Header("Player Control & Camera Lock")]
    public MonoBehaviour playerController; 
    public Camera mainCamera; 
    public GameObject virtualCamera; 

    [Header("Ghost Elements")]
    public GameObject ghostEntity;

    [Header("The Inevitable Car")]
    public GameObject fakeCar;
    public Transform carSpawnPoint;
    public float carSpeed = 25f;
    public AudioSource carRushAudio;
    public AudioSource jumpscareAudio;

    [Header("The Key Evidence")]
    public GameObject finalEvidence; // หลักฐานจริงที่จะโผล่ตอนจบ

    private bool hasTriggered = false;

    void Start()
    {
        if (ghostEntity) ghostEntity.SetActive(false);
        if (fakeCar) fakeCar.SetActive(false);
        if (glitchVolume) glitchVolume.weight = 0f;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !hasTriggered)
        {
            hasTriggered = true;
            StartCoroutine(PlayGhostSequence());
        }
    }

    private IEnumerator PlayGhostSequence()
    {
        // 1. ล็อกขา & ปิดกล้องอัจฉริยะชั่วคราว
        if (playerController) playerController.enabled = false;
        if (virtualCamera) virtualCamera.SetActive(false); 

        float startRainVol = rainAudio != null ? rainAudio.volume : 1f;
        if (rainAudio) rainAudio.volume = 0f;
        if (glitchVolume) glitchVolume.weight = 1f;
        if (playerFlashlight) playerFlashlight.StartGlitchFlicker(3.5f);

        yield return new WaitForSeconds(1.0f);

        // 2. ผีโผล่ & บังคับกล้องมองผี
        if (ghostEntity) ghostEntity.SetActive(true);
        float lookDuration = 0.8f;
        float elapsed = 0f;
        Vector3 ghostFacePos = ghostEntity.transform.position + (Vector3.up * 1.5f);
        Quaternion startBodyRot = playerBody.rotation;
        Quaternion startCamRot = mainCamera != null ? mainCamera.transform.rotation : Quaternion.identity;

        while (elapsed < lookDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.SmoothStep(0f, 1f, elapsed / lookDuration);
            Vector3 bodyDir = (ghostEntity.transform.position - playerBody.position).normalized;
            bodyDir.y = 0; 
            if (bodyDir != Vector3.zero) playerBody.rotation = Quaternion.Slerp(startBodyRot, Quaternion.LookRotation(bodyDir), t);

            if (mainCamera != null)
            {
                Vector3 camDir = (ghostFacePos - mainCamera.transform.position).normalized;
                if (camDir != Vector3.zero) mainCamera.transform.rotation = Quaternion.Slerp(startCamRot, Quaternion.LookRotation(camDir), t);
            }
            yield return null; 
        }

        yield return new WaitForSeconds(1.5f); // แช่ภาพผี 1.5 วิ

        // 3. รถวิ่งชน! ปล่อยให้วิ่งหนีได้
        if (virtualCamera) virtualCamera.SetActive(true); 
        if (playerController) playerController.enabled = true;

        if (fakeCar && carSpawnPoint)
        {
            fakeCar.transform.position = carSpawnPoint.position;
            fakeCar.SetActive(true);
            if (carRushAudio) carRushAudio.Play();

            while (Vector3.Distance(fakeCar.transform.position, playerBody.position) > 2.5f)
            {
                Vector3 moveDir = (playerBody.position - fakeCar.transform.position).normalized;
                fakeCar.transform.position += moveDir * carSpeed * Time.deltaTime;
                fakeCar.transform.LookAt(playerBody);
                yield return null;
            }
        }

        // 4. จอมืด Jumpscare
        if (playerController) playerController.enabled = false;
        if (jumpscareAudio) jumpscareAudio.Play();
        if (glitchVolume) glitchVolume.weight = 1f; 
        if (fakeCar) fakeCar.SetActive(false);
        if (ghostEntity) ghostEntity.SetActive(false);

        yield return new WaitForSeconds(0.8f); 

        // 5. คืนสติ และ เปิดหลักฐานของจริง!
        if (glitchVolume) glitchVolume.weight = 0f;
        if (rainAudio) rainAudio.volume = startRainVol;
        if (playerController) playerController.enabled = true; 
        
        Debug.Log("เข้ม: เมื่อกี้...เหี้ยไรวะ...");

        if (finalEvidence != null) 
        {
            finalEvidence.SetActive(true); // <--- โผล่มาให้เก็บแล้ว!
        }
    }
}