using UnityEngine;
using System.Collections.Generic;


// Le grimoire garde une liste de pages, pas de recettes directement.

// OnEnable/OnDisabel servent à :
//s’abonner / se désabonner à des events
// et à éviter :
// des abonnements multiples
// des callbacks vers des objets désactivés
// des fuites d’events (classique en C#)

// Ils ne sont pas obligatoires pour que l’event fonctionne, mais ils rendent le système robuste.
public class Grimoire : MonoBehaviour
{
    public List<GrimoireRecipePage> recipePages;

    //private void OnEnable()
    //{
    //    if (RecipeManager.Instance != null)
    //    {
    //        RecipeManager.Instance.OnRecipeDiscovered += OnRecipeDiscovered;
    //    }
    //}

    //private void OnDisable()
    //{
    //    if (RecipeManager.Instance != null)
    //    {
    //        RecipeManager.Instance.OnRecipeDiscovered -= OnRecipeDiscovered;
    //    }
    //}

    //private void Start()
    //{
    //    // Afficher les recettes connues au démarrage
    //    foreach (var page in recipePages)
    //    {
    //        if (RecipeManager.Instance.IsRecipeKnown(page.recipe))
    //        {
    //            page.Reveal();
    //        }
    //    }
    //}

    //private void OnRecipeDiscovered(RecipeData recipe)
    //{
    //    Debug.Log("OnRecipeDiscovered");

    //    foreach (var page in recipePages)
    //    {
    //        if (page.recipe == recipe)
    //        {
    //            page.Reveal();
    //            return;
    //        }
    //    }

    //    Debug.LogWarning($"Aucune page de grimoire trouvée pour {recipe.recipeName}");
    //}

    private void OnEnable()
    {
        if (RecipeManager.Instance == null)
        {
            Debug.LogWarning("RecipeManager non prêt au moment du OnEnable du Grimoire");
            return;
        }

        RecipeManager.Instance.OnRecipeDiscovered += OnRecipeDiscovered;
    }

    private void OnDisable()
    {
        if (RecipeManager.Instance != null)
            RecipeManager.Instance.OnRecipeDiscovered -= OnRecipeDiscovered;
    }

    private void Start()
    {
        // 🔑 Synchronisation initiale
        foreach (var recipe in RecipeManager.Instance.GetKnownRecipes())
        {
            ShowRecipe(recipe);
        }

        RecipeManager.Instance.OnRecipeDiscovered += OnRecipeDiscovered;
    }

    private void OnRecipeDiscovered(RecipeData recipe)
    {
        Debug.Log("OnRecipeDiscovered");

        ShowRecipe(recipe);
    }

    private void ShowRecipe(RecipeData recipe)
    {
        foreach (var page in recipePages)
        {
            if (page.recipe == recipe)
            {
                page.Reveal();
                return;
            }
        }

        Debug.LogWarning($"Aucune page de grimoire trouvée pour {recipe.recipeName}");
    }
}
