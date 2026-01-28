using UnityEngine;

public class RecipePickup : MonoBehaviour
{
    public RecipeData recipe;
    public float dissolveDuration = 1.5f;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("RecipeBook")) return;

        RecipeManager.Instance.DiscoverRecipe(recipe);

        // TODO : jouer dissolve shader ici
        Destroy(gameObject, dissolveDuration);
    }
}
