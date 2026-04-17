using UnityEngine;

public class Obstacle : MonoBehaviour
{
    public float minSize = 0.5f;
    public float maxSize = 2.5f;
    public float minSpeed = 80f;
    public float maxSpeed = 280f;
    public float maxSpinSpeed = 10f;

    Rigidbody2D rb;

    void Start()
    {
        float randomSize = Random.Range(minSize, maxSize);
        transform.localScale = new Vector3(randomSize, randomSize, 1);

        float randomSpeed = Random.Range(minSpeed, maxSpeed) / randomSize;
        Vector2 randomDirection = Random.insideUnitCircle;

        float randomTorque = Random.Range(-maxSpinSpeed, maxSpinSpeed);

        rb = GetComponent<Rigidbody2D>();
        rb.AddForce(randomDirection * randomSpeed);
        rb.AddTorque(randomTorque);
    }
}