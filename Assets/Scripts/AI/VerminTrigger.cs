using UnityEngine;

public class VerminTrigger : MonoBehaviour
{
    [SerializeField]
    private float speed;

    private VerminMovement currentManager;

    void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "SimiliNavMesh")
        {
            currentManager = other.gameObject.GetComponent<VerminMovement>();
            other.gameObject.GetComponent<VerminMovement>().VerminEnter(this.gameObject.transform, speed);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if(other.gameObject.CompareTag("SimiliNavMesh"))
        {
            other.gameObject.GetComponent<VerminMovement>().VerminExit();
            currentManager = null;
        }
    }
    public void HitByWeapon()
    {
        if (currentManager != null)
        {
            currentManager.KillVermin();
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
