using UnityEngine;
using SimpleInputNamespace;

public class Player_Controller : MonoBehaviour
{
    public float speed = 5f;
    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        float h = SimpleInput.GetAxis("Horizontal");
        float v = SimpleInput.GetAxis("Vertical");

        Vector3 movement = new Vector3(h, 0, v) * speed;
        rb.linearVelocity = movement + new Vector3(0, rb.linearVelocity.y, 0); // mantiene velocidad vertical (salto)
    }
}
