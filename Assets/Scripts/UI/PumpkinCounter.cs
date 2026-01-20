//using UnityEngine;
//using System.Collections.Generic;

//public class PumpkinCounter : MonoBehaviour
//{
//    [Header("Liste des citrouilles (dans l'ordre)")]
//    public List<GameObject> pumpkins = new List<GameObject>();

//    [Header("Debug")]
//    [Tooltip("Nombre de clients satisfaits dans la série de 10 en cours")]
//    public int satisfiedClients = 0;
//    [Tooltip("Nombre total de clients satisfaits depuis le début du jeu")]
//    public int totalSatisfiendClients = 0;

//    // Appelée par GhostClient lorsqu'un fantôme est satisfait
//    public void RegisterSatisfiedClient()
//    {
//        // Trouver la première citrouille inactive
//        foreach (GameObject pumpkin in pumpkins)
//        {
//            if (!pumpkin.activeSelf)
//            {
//                pumpkin.SetActive(true);
//                satisfiedClients++;
//                // Tenir le compte du nombre total de clients satisfaits pendant le jeu
//                totalSatisfiendClients = totalSatisfiendClients + satisfiedClients;

//                // Audio 
//                if (AudioManager.audioInstance != null)
//                    AudioManager.audioInstance.PlayTheGoodSound(5); // Success Notification

//                Debug.Log($"Citrouille activée ! Total : {satisfiedClients}");

//                CheckForVictory();
//                return;
//            }
//        }

//        Debug.Log("Toutes les citrouilles sont déjà activées (clients supplémentaires ?)");
//    }

//    //public void ActivatePumpkins()
//    //{
//    //    // Trouver la première citrouille inactive
//    //    foreach (GameObject pumpkin in pumpkins)
//    //    {
//    //        if (!pumpkin.activeSelf)
//    //        {
//    //            pumpkin.SetActive(true);

//    //            // Audio 
//    //            if (AudioManager.audioInstance != null)
//    //                AudioManager.audioInstance.PlayTheGoodSound(5); // Success Notification

//    //            Debug.Log($"Citrouille activée ! Total : {satisfiedClients}");

//    //            //CheckForVictory();
//    //            return;
//    //        }
//    //    }

//    //    Debug.Log("Toutes les citrouilles sont déjà activées (clients supplémentaires ?)");
//    //}

//    // Appelée lorsqu'un fantôme repart frustré
//    public void RegisterUnsatisfiedClient()
//    {
//        // On ne descend pas en-dessous de zéro
//        if (satisfiedClients <= 0)
//        {
//            Debug.Log("Aucune citrouille à retirer.");
//            return;
//        }

//        // Trouver la dernière citrouille active
//        for (int i = pumpkins.Count - 1; i >= 0; i--)
//        {
//            if (pumpkins[i].activeSelf)
//            {
//                pumpkins[i].SetActive(false);
//                satisfiedClients--;

//                // Audio échec
//                if (AudioManager.audioInstance != null)
//                    AudioManager.audioInstance.PlayTheGoodSound(8); // Horror lose

//                Debug.Log($"Citrouille désactivée... Total : {satisfiedClients}");
//                return;
//            }
//        }
//    }

//    private void CheckForVictory()
//    {
//        if (satisfiedClients >= pumpkins.Count)
//        {
//            Debug.Log("VICTOIRE ! Les dix premiers clients sont satisfaits !");
//            // Désactivation des citrouilles allumées pour repartir sur une nouvelle série
//            foreach (GameObject pumpkin in pumpkins)
//            {
//                pumpkin.SetActive(false);
//            }
//            // Ajout de temps de jeu : ici dix minutes
//            if (TimeManager.Instance != null) TimeManager.Instance.AddTime(600);
//            // Audio 
//            if (AudioManager.audioInstance != null)
//                AudioManager.audioInstance.PlayTheGoodSound(5); // Success Notification
//            // Remise à zéro du nombre de clients satisfaits pour la nouvelle série
//            satisfiedClients = 0;
//        }
//    }
//}

using UnityEngine;
using System.Collections.Generic;

public class PumpkinCounter : MonoBehaviour
{
    [Header("Liste des citrouilles (dans l'ordre)")]
    public List<GameObject> pumpkins = new List<GameObject>();

    [Header("Progression")]
    [Tooltip("Nombre de clients satisfaits dans la série en cours")]
    public int satisfiedClients = 0;

    [Tooltip("Nombre total de clients satisfaits depuis le début du jeu")]
    public int totalSatisfiedClients = 0;

    [Header("Récompense")]
    [Tooltip("Temps ajouté (en secondes) lorsqu'une série complète est validée")]
    public float bonusTimeOnSeriesComplete = 600f; // 10 minutes

    /// <summary>
    /// Appelée lorsqu'un fantôme est satisfait
    /// </summary>
    public void RegisterSatisfiedClient()
    {
        // Sécurité
        if (pumpkins.Count == 0)
        {
            Debug.LogWarning("PumpkinCounter : aucune citrouille assignée.");
            return;
        }

        // Activer la prochaine citrouille
        if (satisfiedClients < pumpkins.Count)
        {
            pumpkins[satisfiedClients].SetActive(true);
            satisfiedClients++;
            totalSatisfiedClients++;

            // Audio succès
            if (AudioManager.audioInstance != null)
                AudioManager.audioInstance.PlayTheGoodSound(5);

            Debug.Log($"Citrouille activée ({satisfiedClients}/{pumpkins.Count})");

            CheckForSeriesCompletion();
        }
        else
        {
            Debug.Log("Toutes les citrouilles sont déjà activées.");
        }
    }

    /// <summary>
    /// Appelée lorsqu'un fantôme repart frustré
    /// </summary>
    public void RegisterUnsatisfiedClient()
    {
        if (satisfiedClients <= 0)
        {
            Debug.Log("Aucune citrouille à retirer.");
            return;
        }

        satisfiedClients--;

        pumpkins[satisfiedClients].SetActive(false);

        // Audio échec
        if (AudioManager.audioInstance != null)
            AudioManager.audioInstance.PlayTheGoodSound(8);

        Debug.Log($"Citrouille retirée ({satisfiedClients}/{pumpkins.Count})");
    }

    /// <summary>
    /// Vérifie si la série est complète et applique la récompense
    /// </summary>
    private void CheckForSeriesCompletion()
    {
        if (satisfiedClients < pumpkins.Count)
            return;

        Debug.Log("🎃 Série complète ! Bonus de temps accordé.");

        // Réinitialisation visuelle des citrouilles
        foreach (GameObject pumpkin in pumpkins)
        {
            pumpkin.SetActive(false);
        }

        // Bonus de temps
        if (TimeManager.Instance != null)
        {
            TimeManager.Instance.AddTime(bonusTimeOnSeriesComplete);
        }
        else
        {
            Debug.LogWarning("TimeManager introuvable : bonus de temps non appliqué.");
        }

        // Audio de victoire
        if (AudioManager.audioInstance != null)
            AudioManager.audioInstance.PlayTheGoodSound(5);

        // Réinitialisation de la série
        satisfiedClients = 0;
    }

    /// <summary>
    /// Permet de réactiver les citrouilles en cas de chargement des données de la précédente partie
    /// </summary>
    public void ActivatePumpkins()
    {
        // Trouver la première citrouille inactive
        foreach (GameObject pumpkin in pumpkins)
        {
            if (!pumpkin.activeSelf)
            {
                pumpkin.SetActive(true);

                // Audio 
                if (AudioManager.audioInstance != null)
                    AudioManager.audioInstance.PlayTheGoodSound(5); // Success Notification

                Debug.Log($"Citrouille activée ! Total : {satisfiedClients}");

                //CheckForVictory();
                return;
            }
        }

        Debug.Log("Toutes les citrouilles sont déjà activées (clients supplémentaires ?)");
    }
}

