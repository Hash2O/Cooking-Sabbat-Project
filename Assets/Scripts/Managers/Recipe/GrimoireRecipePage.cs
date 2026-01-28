using UnityEngine;

// Chaque page du grimoire est un GameObject avec :
// une RecipeData
// un script qui sait s’afficher / se cacher

public class GrimoireRecipePage : MonoBehaviour
{
    public RecipeData recipe;

    private void Awake()
    {
        // Cachée par défaut
        gameObject.SetActive(false);
    }

    public void Reveal()
    {
        gameObject.SetActive(true);
        Debug.Log($"📖 Page de recette révélée : {recipe.recipeName}");
    }
}
