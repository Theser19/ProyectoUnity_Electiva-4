using UnityEngine;
using SimpleInputNamespace;
using UnityEngine.Rendering;

public class MiradaCamara : MonoBehaviour
{
    public float sensibilidad = 100f;
    float RotacionX = 0f;
    public Transform Player;
    
    void Start()
    {
        
    }

    void Update()
    {
    
        float mouseX = SimpleInput.GetAxis("Mouse X")* sensibilidad * Time.deltaTime;
        float mouseY = SimpleInput.GetAxis("Mouse Y")* sensibilidad * Time.deltaTime;
        
        RotacionX-= mouseY;
        RotacionX = Mathf.Clamp(RotacionX, -90f, 90f); // Limita la rotación vertical para evitar voltear el personaje


        transform.localRotation = Quaternion.Euler(RotacionX, 0f, 0f);
        Player.Rotate(Vector3.up * mouseX);

    }

}