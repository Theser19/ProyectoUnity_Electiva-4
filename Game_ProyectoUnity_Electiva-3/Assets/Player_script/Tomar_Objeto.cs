using UnityEngine;
using SimpleInputNamespace;

public class Tomar_Objeto : MonoBehaviour
{
    [Header("Referencias")]
    public GameObject handPoint; // El punto donde se posicionar� el objeto tomado

    [Header("Configuraci�n")]
    public string objectTag = "Objeto"; // Tag de los objetos que se pueden tomar
    public string inputButton = "e"; // Nombre del bot�n de input

    private GameObject pickedObject = null; // Objeto actualmente tomado
    private GameObject objectInRange = null; // Objeto dentro del rango de interacci�n

    private void Update()
    {
        // Si tenemos un objeto tomado y presionamos E, lo soltamos
        if (pickedObject != null)
        {
            if (SimpleInput.GetButtonDown(inputButton))
            {
                DropObject();
            }
        }
        // Si hay un objeto en rango y no tenemos nada tomado, lo tomamos
        else if (objectInRange != null)
        {
            if (SimpleInput.GetButtonDown(inputButton))
            {
                PickUpObject(objectInRange);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Cuando un objeto entra en el trigger
        if (other.gameObject.CompareTag(objectTag) && pickedObject == null)
        {
            objectInRange = other.gameObject;
            // Aqu� podr�as agregar un indicador visual de que se puede tomar el objeto
            Debug.Log("Objeto disponible para tomar: " + other.name);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // Cuando un objeto sale del trigger
        if (other.gameObject.CompareTag(objectTag) && other.gameObject == objectInRange)
        {
            objectInRange = null;
            // Remover indicador visual
            Debug.Log("Objeto fuera de alcance: " + other.name);
        }
    }

    private void PickUpObject(GameObject obj)
    {
        if (obj == null || handPoint == null) return;

        // Desactivar f�sica del objeto
        Rigidbody rb = obj.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = true;
            rb.useGravity = false;
        }

        // Posicionar el objeto en la mano
        obj.transform.position = handPoint.transform.position;
        obj.transform.rotation = handPoint.transform.rotation;

        // Hacer que el objeto sea hijo del punto de la mano
        obj.transform.SetParent(handPoint.transform);

        // Desactivar el collider del objeto para evitar interferencias
        Collider objCollider = obj.GetComponent<Collider>();
        if (objCollider != null)
        {
            objCollider.enabled = false;
        }

        // Asignar como objeto tomado
        pickedObject = obj;
        objectInRange = null;

        Debug.Log("Objeto tomado: " + obj.name);
    }

    private void DropObject()
    {
        if (pickedObject == null) return;

        // Reactivar f�sica del objeto
        Rigidbody rb = pickedObject.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = false;
            rb.useGravity = true;
        }

        // Reactivar el collider
        Collider objCollider = pickedObject.GetComponent<Collider>();
        if (objCollider != null)
        {
            objCollider.enabled = true;
        }

        // Quitar el objeto como hijo del punto de la mano
        pickedObject.transform.SetParent(null);

        // Opcional: Agregar una peque�a fuerza hacia adelante al soltar
        if (rb != null)
        {
            Vector3 throwForce = transform.forward * 2f; // Ajusta la fuerza seg�n necesites
            rb.AddForce(throwForce, ForceMode.Impulse);
        }

        Debug.Log("Objeto soltado: " + pickedObject.name);

        // Limpiar referencia
        pickedObject = null;
    }

    // M�todo p�blico para verificar si tenemos un objeto tomado
    public bool HasObjectPickedUp()
    {
        return pickedObject != null;
    }

    // M�todo p�blico para obtener el objeto tomado
    public GameObject GetPickedObject()
    {
        return pickedObject;
    }

    // M�todo p�blico para verificar si hay un objeto disponible para tomar
    public bool HasObjectAvailable()
    {
        return objectInRange != null && pickedObject == null;
    }

    // M�todo p�blico para obtener el objeto disponible
    public GameObject GetAvailableObject()
    {
        return objectInRange;
    }
}