//using System.Collections.Generic;
//using System.Linq;
//using TMPro;
//using UnityEngine;
//using UnityEngine.UI;
//using System;

//// Ce script sert à :
//// gérer le démarrage avec 3 recettes
//// débloquer dynamiquement des recettes en jeu
//// alimenter directement le GhostCycleManager

//public class RecipeManager : MonoBehaviour
//{
//    public static RecipeManager Instance { get; private set; }

//    // Gestion des recettes depuis le chaudron vers le RecipeManager
//    [Header("Toutes les recettes disponibles dans le jeu")]
//    public List<RecipeData> allRecipes = new List<RecipeData>();

//    // Liste des recettes connues par la sorcière pendant le jeu
//    [Header("Recettes découvertes par le joueur")]
//    public List<RecipeData> discoveredRecipes = new List<RecipeData>();

//    private HashSet<RecipeData> knownRecipes = new();

//    [SerializeField] private TextMeshProUGUI newRecipeText;

//    public Action<RecipeData> OnRecipeDiscovered;

//    private void Awake()
//    {
//        if (Instance != null && Instance != this)
//        {
//            Destroy(gameObject);
//            return;
//        }

//        Instance = this;
//        DontDestroyOnLoad(gameObject);

//        InitializeKnownRecipes();
//    }

//    private void InitializeKnownRecipes()
//    {
//        foreach (var recipe in allRecipes)
//        {
//            if (recipe.isKnownAtStart)
//            {
//                knownRecipes.Add(recipe);
//            }
//        }
//    }

//    public bool IsRecipeKnown(RecipeData recipe)
//    {
//        return knownRecipes.Contains(recipe);
//    }

//    public bool IsRecipeDiscovered(RecipeData recipe)
//    {
//        return discoveredRecipes.Contains(recipe);
//    }

//    public void DiscoverRecipe(RecipeData recipe)
//    {
//        if (knownRecipes.Contains(recipe)) return;
//        knownRecipes.Add(recipe);
//        Debug.Log($"📖 Nouvelle recette découverte : {recipe.recipeName}");
//        OnRecipeDiscovered?.Invoke(recipe);
//    }

//    public List<RecipeData> GetAvailableRecipes()
//    {
//        return discoveredRecipes;
//    }

//    public void ResetDiscoveredRecipes()
//    {
//        discoveredRecipes.Clear();
//    }

//    public List<RecipeData> GetKnownRecipes()
//    {
//        return new List<RecipeData>(knownRecipes);
//    }

//    public List<GameObject> GetKnownGhostPrefabs()
//    {
//        return knownRecipes
//            .Where(r => r.ghostPrefab != null)
//            .Select(r => r.ghostPrefab)
//            .ToList();
//    }
//}

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

//public class RecipeManager : MonoBehaviour
//{
//    public static RecipeManager Instance { get; private set; }

//    [Header("Toutes les recettes disponibles dans le jeu")]
//    public List<RecipeData> allRecipes = new List<RecipeData>();

//    private HashSet<RecipeData> knownRecipes = new();

//    public event Action<RecipeData> OnRecipeDiscovered;

//    private void Awake()
//    {
//        if (Instance != null && Instance != this)
//        {
//            Destroy(gameObject);
//            return;
//        }

//        Instance = this;
//        DontDestroyOnLoad(gameObject);

//        InitializeKnownRecipes();
//    }

//    private void InitializeKnownRecipes()
//    {
//        foreach (var recipe in allRecipes)
//        {
//            if (recipe.isKnownAtStart)
//            {
//                knownRecipes.Add(recipe);
//            }
//        }
//    }

//    public bool IsRecipeKnown(RecipeData recipe)
//    {
//        return knownRecipes.Contains(recipe);
//    }

//    public void DiscoverRecipe(RecipeData recipe)
//    {
//        if (recipe == null)
//        {
//            Debug.LogWarning("Tentative de découverte d'une recette qui n'existe pas.");
//            return;
//        }

//        if (knownRecipes.Contains(recipe))
//        {
//            Debug.Log($"Recette déjà connue : {recipe.recipeName}");
//            return;
//        }

//        knownRecipes.Add(recipe);

//        Debug.Log($"📖 Nouvelle recette découverte : {recipe.recipeName}");

//        OnRecipeDiscovered?.Invoke(recipe);

//        Debug.Log($"Listeners OnRecipeDiscovered: {OnRecipeDiscovered?.GetInvocationList().Length ?? 0}");
//    }

//    public List<RecipeData> GetKnownRecipes()
//    {
//        return knownRecipes.ToList();
//    }

//    public List<GameObject> GetKnownGhostPrefabs()
//    {
//        return knownRecipes
//            .Where(r => r.ghostPrefab != null)
//            .Select(r => r.ghostPrefab)
//            .ToList();
//    }
//}

public class RecipeManager : MonoBehaviour
{
    public static RecipeManager Instance { get; private set; }

    [Header("Toutes les recettes du jeu")]
    public List<RecipeData> allRecipes = new();

    //[Header("Recettes connues au démarrage")]
    //public List<RecipeData> startingRecipes = new();

    [Header("Recettes connues et/ou découvertes en jeu (DEBUG / SCORE)")]
    [SerializeField] private List<RecipeData> discoveredRecipes = new();

    private HashSet<RecipeData> knownRecipes = new();

    public Action<RecipeData> OnRecipeDiscovered;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        InitializeKnownRecipes();
    }

    private void InitializeKnownRecipes()
    {
        knownRecipes.Clear();
        discoveredRecipes.Clear();

        foreach (var recipe in allRecipes)
        {
            if (recipe == null) continue;

            if (recipe.isKnownAtStart)
            {
                knownRecipes.Add(recipe);
                discoveredRecipes.Add(recipe);
            }
        }

        Debug.Log($"📚 Recettes connues au départ : {knownRecipes.Count}");
    }

    public void DiscoverRecipe(RecipeData recipe)
    {
        if (recipe == null) return;
        if (knownRecipes.Contains(recipe)) return;

        knownRecipes.Add(recipe);
        discoveredRecipes.Add(recipe);

        Debug.Log($"📖 Nouvelle recette découverte : {recipe.recipeName}");
        OnRecipeDiscovered?.Invoke(recipe);
    }

    public bool IsRecipeKnown(RecipeData recipe)
    {
        return knownRecipes.Contains(recipe);
    }

    public List<RecipeData> GetKnownRecipes()
    {
        return new List<RecipeData>(knownRecipes);
    }

    public List<RecipeData> GetDiscoveredRecipes()
    {
        return discoveredRecipes;
    }

    public List<GameObject> GetKnownGhostPrefabs()
    {
        return knownRecipes
            .Where(r => r.ghostPrefab != null)
            .Select(r => r.ghostPrefab)
            .ToList();
    }
}
