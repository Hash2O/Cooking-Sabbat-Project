using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RecipeManager : MonoBehaviour
{
    public static RecipeManager Instance { get; private set; }

    // A garder si on veut déplacer la gestion des recettes depuis le chaudron vers le RecipeManager
    [Header("Toutes les recettes disponibles dans le jeu")]
    public List<RecipeData> allRecipes = new List<RecipeData>();

    [Header("Recettes découvertes par le joueur")]
    public List<RecipeData> discoveredRecipes = new List<RecipeData>();

    [SerializeField] private TextMeshProUGUI newRecipeText;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public bool IsRecipeDiscovered(RecipeData recipe)
    {
        return discoveredRecipes.Contains(recipe);
    }

    public void DiscoverRecipe(RecipeData recipe)
    {
        if (!discoveredRecipes.Contains(recipe))
        {
            discoveredRecipes.Add(recipe);
            Debug.Log($"Nouvelle recette découverte : {recipe.recipeName}");
            // A faire : notifier ici le grimoire / livre de recettes (UI Canvas)
            newRecipeText.text = "New Recipe Discovered.";
        }
    }

    public List<RecipeData> GetAvailableRecipes()
    {
        return discoveredRecipes;
    }

    public void ResetDiscoveredRecipes()
    {
        discoveredRecipes.Clear();
    }
}

