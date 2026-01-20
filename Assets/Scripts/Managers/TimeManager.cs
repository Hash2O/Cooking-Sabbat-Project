using UnityEngine;
using System;

public class TimeManager : MonoBehaviour
{
    public static TimeManager Instance { get; private set; }

    [Header("Paramètres du temps de jeu")]
    [Tooltip("Temps total de la partie en secondes (ex : 7200 = 2 heures)")]
    [SerializeField] private float totalTimeInSeconds = 7200f;

    [Tooltip("Le temps s'écoule-t-il automatiquement ?")]
    [SerializeField] private bool timeFlows = true;

    [Header("Debug / Lecture seule")]
    [SerializeField] private float currentTime;
    public float CurrentTime => currentTime;

    public bool IsGameOver { get; private set; } = false;

    // 🔔 Événements (optionnels mais très utiles)
    public event Action<float> OnTimeChanged;
    public event Action OnTimeOver;

    private void Awake()
    {
        // Singleton
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        // Optionnel : persister entre les scènes
        // DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        currentTime = totalTimeInSeconds;
        OnTimeChanged?.Invoke(currentTime);
    }

    private void Update()
    {
        if (!timeFlows || IsGameOver)
            return;

        currentTime -= Time.deltaTime;

        if (currentTime <= 0f)
        {
            currentTime = 0f;
            TriggerGameOver();
        }

        OnTimeChanged?.Invoke(currentTime);
    }

    // ⏸️ Pause / reprise du temps
    public void PauseTime(bool pause)
    {
        timeFlows = !pause;
    }

    // ➕ Ajout de temps (récompenses, objets, potions)
    public void AddTime(float seconds)
    {
        if (IsGameOver) return;

        currentTime += seconds;
        OnTimeChanged?.Invoke(currentTime);
    }

    // ➖ Retrait de temps (pénalités)
    public void RemoveTime(float seconds)
    {
        if (IsGameOver) return;

        currentTime -= seconds;
        currentTime = Mathf.Max(currentTime, 0f);
        OnTimeChanged?.Invoke(currentTime);

        if (currentTime <= 0f)
            TriggerGameOver();
    }

    // ⏱️ Pour fixer le temps (debug, chargement, cheat, etc.)
    public void SetTime(float seconds)
    {
        currentTime = Mathf.Max(seconds, 0f);
        OnTimeChanged?.Invoke(currentTime);

        if (currentTime <= 0f)
            TriggerGameOver();
    }

    private void TriggerGameOver()
    {
        if (IsGameOver) return;

        IsGameOver = true;
        timeFlows = false;

        Debug.Log("🎃 La nuit d’Halloween est terminée !");

        OnTimeOver?.Invoke();

        // Exemple :
        // GameManager.Instance?.HandleGameOver();
    }

    // 🕰️ Utilitaire : format lisible HH:MM:SS
    public string GetFormattedTime()
    {
        TimeSpan t = TimeSpan.FromSeconds(currentTime);
        return $"{t.Hours:D2}:{t.Minutes:D2}:{t.Seconds:D2}";
    }
}

