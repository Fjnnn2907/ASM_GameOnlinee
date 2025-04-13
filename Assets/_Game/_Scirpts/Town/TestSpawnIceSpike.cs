using UnityEngine;

public class TestSpawnIceSpike : MonoBehaviour
{
    public GameObject[] enemy; // Gán nhiều enemy trong Inspector
    public IceSpikeSpawner spikeSpawner; // Gán component IceSpikeSpawner
    public KeyCode triggerKey = KeyCode.Space;

    void Update()
    {
        if (Input.GetKeyDown(triggerKey))
        {
            if (enemy != null && spikeSpawner != null)
            {
                Vector2 playerPos = transform.position;

                foreach (GameObject e in enemy)
                {
                    if (e != null)
                    {
                        Vector2 enemyPos = e.transform.position;
                        spikeSpawner.SpawnIceSpikes(playerPos, enemyPos);
                    }
                }
            }
        }
    }
}
