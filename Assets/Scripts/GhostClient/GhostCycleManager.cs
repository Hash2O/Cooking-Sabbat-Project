using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class GhostCycleManager : MonoBehaviour
{
    [Header("Points de déplacement")]
    public Transform ghostSpawnPoint;
    public Transform ghostWaitPoint;
    public Transform ghostExitPoint;

    [Header("Gestion des fantômes")]
    //public List<GameObject> ghostPrefabs;
    private List<GameObject> availableGhosts = new();
    private GhostClient activeGhost;

    [Header("Récompenses")]
    public GameObject coinPrefab;
    public GameObject keyPrefab;
    public Transform coinDeliveryPoint;
    public Transform keyDeliveryPoint;
    public float timeBetweenCoins = 0.5f;
    public float bonusTimeToCheck = 30f;

    [Header("Timers")]
    public float spawnDelay = 2f;
    public float exitDelay = 3f;
    public float timeBeforeVanish = 5f;

    [Tooltip("Temps maximum qu’un fantôme attend sa potion avant de partir (en secondes).")]
    public float maxWaitTime = 180f;

    [Header("Référence au compteur de citrouilles")]
    [SerializeField] private PumpkinCounter pumpkinCounter;

    [Header("Référence à la tirelire")]
    [SerializeField] private CoinTriggerDoor coinTrigger;

    [Header("Gestion Présence/Absence XR Rig dans la cuisine")]
    public bool isPlayerInside = false; // gère l'absence ou la présence du XR Rig dans la "cuisine"
    public bool isPaused = false;   // gel du timer patience si le joueur sort

    [Header("Patience penalties")]
    [Tooltip("Temps retiré à la patience du fantôme en cas de mauvaise potion")]
    public float wrongPotionPenalty = 30f;

    private bool isSpawning = false; 
    private float remainingWaitTime;    // Stockage du temps restant quand le fantôme est satisfait (gestion récompense bonus)

    private void Awake()
    {
        pumpkinCounter = FindFirstObjectByType<PumpkinCounter>();
        coinTrigger = FindFirstObjectByType<CoinTriggerDoor>();
    }

    private void Start()
    {
        StartCoroutine(GhostCycleLoop());
    }

    private IEnumerator GhostCycleLoop()
    {
        while (true)
        {
            // Tant que le joueur n’est PAS dans la zone → pause
            while (!isPlayerInside)
                yield return null;

            // Tant qu’on est en pause, SpawnGhost n’est pas lancé.
            if (!isSpawning && activeGhost == null)
            {
                yield return StartCoroutine(SpawnGhost());
            }

            yield return null;
        }
    }

    private IEnumerator SpawnGhost()
    {
        //isSpawning = true;
        //yield return new WaitForSeconds(spawnDelay);

        //// Spawn du fantôme
        //GameObject prefab = ghostPrefabs[Random.Range(0, ghostPrefabs.Count)];
        //GameObject ghostObj = Instantiate(prefab, ghostSpawnPoint.position, Quaternion.identity);
        //activeGhost = ghostObj.GetComponent<GhostClient>();

        isSpawning = true;
        yield return new WaitForSeconds(spawnDelay);

        availableGhosts = RecipeManager.Instance.GetKnownGhostPrefabs();

        if (availableGhosts.Count == 0)
        {
            Debug.LogWarning("Aucun fantôme disponible !");
            isSpawning = false;
            yield break;
        }

        GameObject prefab = availableGhosts[Random.Range(0, availableGhosts.Count)];
        GameObject ghostObj = Instantiate(prefab, ghostSpawnPoint.position, Quaternion.identity);
        activeGhost = ghostObj.GetComponent<GhostClient>();

        NavMeshAgent agent = ghostObj.GetComponent<NavMeshAgent>();

        // Aller au comptoir
        if (agent != null)
            agent.SetDestination(ghostWaitPoint.position);

        // Son d’alerte
        if(AudioManager.audioInstance != null)
            AudioManager.audioInstance.PlayTheGoodSound(1);

        // Attente que le fantôme arrive au point de patience
        yield return StartCoroutine(WaitForGhostToReach(agent, ghostWaitPoint.position));

        // Lancer le timer de patience
        bool potionDelivered = false;
        yield return StartCoroutine(GhostWaitTimer(() => potionDelivered = activeGhost.isSatisfied));

        // Si la potion a été donnée à temps
        if (activeGhost.isSatisfied)
        {
            yield return StartCoroutine(GiveReward());
            if (pumpkinCounter != null)
            {
                pumpkinCounter.RegisterSatisfiedClient();
            }
        }
        else
        {
            Debug.Log("Le fantôme est parti frustré (pas de potion à temps).");
            if (pumpkinCounter != null)
            {
                pumpkinCounter.RegisterUnsatisfiedClient();
            }
        }

        // Départ du fantôme
        if (agent != null)
            agent.SetDestination(ghostExitPoint.position);

        yield return new WaitForSeconds(exitDelay);
        yield return new WaitForSeconds(timeBeforeVanish);

        Destroy(ghostObj);
        activeGhost = null;
        isSpawning = false;
    }

    /// <summary>
    /// Attend que le fantôme soit proche de son point d’attente.
    /// </summary>
    private IEnumerator WaitForGhostToReach(NavMeshAgent agent, Vector3 target)
    {
        if (agent == null)
            yield break;

        while (Vector3.Distance(agent.transform.position, target) > agent.stoppingDistance + 0.2f)
        {
            //Debug.Log("Distance : " + Vector3.Distance(agent.transform.position, target));
            yield return null;
        }
    }

    private IEnumerator GhostWaitTimer(System.Func<bool> isSatisfiedCheck)
    {
        remainingWaitTime = maxWaitTime;
        if (activeGhost.ghostRenderer != null)
        {
            activeGhost.ghostRenderer.material.SetFloat("_Alpha", 1.0f);
        }
        while (remainingWaitTime > 0f)
        {
            if (isSatisfiedCheck())
            {
                if (activeGhost.patienceBar != null)
                    activeGhost.patienceBar.SetVisible(false);
                if (activeGhost.ghostRenderer != null)
                {

                    activeGhost.ghostRenderer.material.SetFloat("_Alpha",Mathf.Lerp(activeGhost.ghostRenderer.material.GetFloat("_Alpha"),1.0f, 0.75f));
                }

                yield break;
            }

            while (isPaused)
                yield return null;

            remainingWaitTime -= Time.deltaTime;

            if (activeGhost.patienceBar != null)
            {
                float remainingPercent = remainingWaitTime / maxWaitTime;
                activeGhost.patienceBar.SetFill(remainingPercent);
            }
            if (activeGhost.ghostRenderer != null)
            {
                float remainingPercent = remainingWaitTime / maxWaitTime;
                activeGhost.ghostRenderer.material.SetFloat("_Alpha",remainingPercent);
            }

            yield return null;
        }

        if (activeGhost.patienceBar != null)
            activeGhost.patienceBar.SetVisible(false);

        // Temps écoulé
        Debug.Log("⏰ Temps écoulé ! Le fantôme part sans potion, vidant en partie la tirelire en partant.");

        if (activeGhost.patienceBar != null) activeGhost.patienceBar.SetVisible(false); // Désactivation de la barre de patience
        if (coinTrigger != null) coinTrigger.CoinRemoving();    // Si pièce dans la tirelire, le fantôme en enlève une 
        if (AudioManager.audioInstance != null) AudioManager.audioInstance.PlayNotificationSound(1);    // Fail Notification Horror
        
    }

    public void ApplyWrongPotionPenalty()
    {
        remainingWaitTime -= wrongPotionPenalty;
        remainingWaitTime = Mathf.Max(remainingWaitTime, 0f);

        Debug.Log($"❌ Mauvaise potion ! -{wrongPotionPenalty} secondes de patience.");
    }

    private IEnumerator GiveReward()
    {
        if (coinPrefab == null || coinDeliveryPoint == null)
            yield break;

        int coinCount = Random.Range(1, 4); // 1 à 3 pièces de base
        int bonusCoins = Mathf.FloorToInt(remainingWaitTime / bonusTimeToCheck);    // Bonus si livraison rapide

        if(AudioManager.audioInstance != null)
            AudioManager.audioInstance.PlayTheGoodSound(11);    // Fairy Cartoon Success Voice

        coinCount += bonusCoins;    // calcul du nombre de pièces données par le client fantôme satisfait

        if (AudioManager.audioInstance != null)
            AudioManager.audioInstance.PlayTheGoodSound(0); // Cashing Sound

        for (int i = 0; i < coinCount; i++)
        {
            Instantiate(coinPrefab, coinDeliveryPoint.position, Quaternion.identity);
            yield return new WaitForSeconds(timeBetweenCoins);
        }

        int randomKey = Random.Range(1, 11);
        Debug.Log("Random Key Number : " + randomKey);

        if (randomKey < 4)
            Instantiate(keyPrefab, keyDeliveryPoint.position, Quaternion.identity);

        Debug.Log($"{coinCount} pièce(s) récompensent la sorcière !");
    }
}

//private IEnumerator GhostWaitTimer(System.Func<bool> isSatisfiedCheck)
//{
//    float timer = 0f;

//    // Active la barre au début
//    if (activeGhost.patienceBar != null)
//        activeGhost.patienceBar.SetVisible(true);

//    while (timer < maxWaitTime)
//    {
//        // Si la potion est donnée : on stoppe tout immédiatement
//        if (isSatisfiedCheck())
//        {
//            // Capturer le temps restant pour déterminer si récompense bonus (voir GiveReward())
//            lastRemainingTime = maxWaitTime - timer;

//            if (activeGhost.patienceBar != null)
//                activeGhost.patienceBar.SetVisible(false);
//            yield break;
//        }

//        // PAUSE : tant que le joueur est hors de la zone
//        while (isPaused)
//        {
//            // On gèle tout : pas d’incrément du timer, pas de changement de fill
//            yield return null;
//        }

//        // Mise à jour de la barre (pendant que le timer avance)
//        if (activeGhost.patienceBar != null)
//        {
//            float remainingPercent = 1f - (timer / maxWaitTime);
//            activeGhost.patienceBar.SetFill(remainingPercent);
//        }

//        // Le timer n’avance que lorsque la zone est active
//        timer += Time.deltaTime;
//        yield return null;
//    }

//    // Timer écoulé : cacher la barre
//    if (activeGhost.patienceBar != null)
//        activeGhost.patienceBar.SetVisible(false);

//    Debug.Log("Temps écoulé ! Le fantôme part sans potion.");
//}

/// <summary>
/// Timer d’attente du fantôme : si le timer expire avant qu’il soit satisfait → il part.
/// </summary>
//private IEnumerator GhostWaitTimer(System.Func<bool> isSatisfiedCheck)
//{
//    float timer = 0f;

//    Debug.Log("Ghost Wait Timer"); 

//    // Active la barre au début
//    if (activeGhost.patienceBar != null)
//    {
//        Debug.Log("patienceBar.SetVisible(true)");
//        activeGhost.patienceBar.SetVisible(true);
//    }


//    while (timer < maxWaitTime)
//    {
//        if (isSatisfiedCheck())
//        {
//            // Potion reçue → cacher la barre
//            if (activeGhost.patienceBar != null)
//                activeGhost.patienceBar.SetVisible(false);

//            yield break;
//        }

//        // Remplit la barre visuelle
//        if (activeGhost.patienceBar != null)
//        {
//            float remainingPercent = 1f - (timer / maxWaitTime);
//            activeGhost.patienceBar.SetFill(remainingPercent);
//        }

//        timer += Time.deltaTime;
//        Debug.Log("Timer : " + timer);
//        yield return null;
//    }

//    // Timer écoulé → cacher la barre
//    if (activeGhost.patienceBar != null)
//    {
//        Debug.Log("patienceBar.SetVisible(false)");
//        activeGhost.patienceBar.SetVisible(false);
//    }

//    Debug.Log("Temps écoulé ! Le fantôme part sans potion.");
//}
