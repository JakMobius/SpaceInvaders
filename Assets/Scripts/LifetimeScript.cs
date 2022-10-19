using UnityEngine;

public class LifetimeScript : MonoBehaviour
{
    public float lifetime = 0.3f;
    
    private void Update()
    {
        lifetime -= Time.deltaTime;
        if (lifetime <= 0)
        {
            Destroy(gameObject);
        }
    }
}
