using UnityEngine;

public class Fireball : MonoBehaviour
{
    public float speed = 10f;
    private GameObject target;

    public void SetTarget(GameObject newTarget)
    {
        target = newTarget;
    }

    void Update()
    {
        if (target != null)
        {
            Vector3 direction = (target.transform.position - transform.position).normalized;
            transform.Translate(direction * speed * Time.deltaTime, Space.World);

            if (Vector3.Distance(transform.position, target.transform.position) < 1f)
            {

                Destroy(gameObject);
            }
        }
    }
}
