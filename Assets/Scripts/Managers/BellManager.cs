using UnityEngine;

public class BellManager : MonoBehaviour
{
    [SerializeField] private GhostCycleManager ghostManager;
    [SerializeField] private TimeManager timeManager;
    [SerializeField] private ParticleSystem startNotificationVFX;

    private bool isBellActivated = false;

    private void OnCollisionEnter(Collision collision)
    {
        if(AudioManager.audioInstance != null)
            AudioManager.audioInstance.PlayTheGoodSound(7);     // Bell ringing
        Debug.Log("Ring my bell !");

        if(isBellActivated == false)
        {
            Debug.Log("Les clients fantômes sont invités à venir passer commande !");
            isBellActivated = true;
            startNotificationVFX.Play();
            if(ghostManager != null) ghostManager.gameObject.SetActive(true);
            if(timeManager != null) timeManager.gameObject.SetActive(true);
        }
    }
}
