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
    public GameObject virtualCamera; // ตัว Follow Camera ที่ต้องสั่งปิดไม่ให้มันดึงกล้องกลับ

    [Header("Ghost Elements")]
    public GameObject ghostEntity;

    [Header("The Inevitable Car (รถพุ่งชน)")]
    public GameObject fakeCar;
    public Transform carSpawnPoint;
    public float carSpeed = 25f;
    public AudioSource carRushAudio;
    public AudioSource jumpscareAudio;

    [Header("The Key Evidence (หลักฐานชิ้นสำคัญ)")]
    public GameObject finalEvidence;

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
        if (playerController) playerController.enabled = false;
        if (virtualCamera) virtualCamera.SetActive(false);
        float startRainVol = rainAudio != null ? rainAudio.volume : 1f;
        if (rainAudio) rainAudio.volume = 0f;
        if (glitchVolume) glitchVolume.weight = 1f;
        if (playerFlashlight) playerFlashlight.StartGlitchFlicker(3.5f);
        yield return new WaitForSeconds(1.0f);
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
        yield return new WaitForSeconds(1.5f);
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
        if (playerController) playerController.enabled = false;
        if (jumpscareAudio) jumpscareAudio.Play();
        if (glitchVolume) glitchVolume.weight = 1f; 
        if (fakeCar) fakeCar.SetActive(false);
        if (ghostEntity) ghostEntity.SetActive(false);
        yield return new WaitForSeconds(0.8f); 
        if (glitchVolume) glitchVolume.weight = 0f;
        if (rainAudio) rainAudio.volume = startRainVol;
        if (playerController) playerController.enabled = true; 
        Debug.Log("เข้ม: เมื่อกี้...เหี้ยไรวะ...");
        if (finalEvidence != null) 
        {
            finalEvidence.SetActive(true);
            Debug.Log("[ระบบ] หลักฐานชิ้นสำคัญโผล่มาที่จุดเกิดแล้ว");
        }
    }
}