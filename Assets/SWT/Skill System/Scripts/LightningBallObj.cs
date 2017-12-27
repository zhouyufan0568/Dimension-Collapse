using UnityEngine;

public class LightningBallObj : MonoBehaviour {

    public float velocity = 5f;

    public GameObject owner;

    private new Rigidbody rigidbody;

    private void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        transform.position += transform.forward * (Time.deltaTime * velocity);
    }
}
