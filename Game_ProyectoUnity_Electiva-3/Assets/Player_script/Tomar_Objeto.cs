using UnityEngine;
using SimpleInputNamespace;

public class Tomar_Objeto : MonoBehaviour
{
    [Header("Referencias")]
    public GameObject handPoint;

    [Header("Configuración")]
    public string objectTag = "Objeto";
    public string inputButton = "e";

    private GameObject pickedObject = null;
    private GameObject objectInRange = null;
    private ItemUseZone currentUseZone = null;
    private CarRepairSystem currentCarZone = null;

    private void Update()
    {
        if (SimpleInput.GetButtonDown(inputButton))
        {
            // Prioridad 1: Si estamos en una zona de uso normal (puertas, etc)
            if (currentUseZone != null)
            {
                currentUseZone.IntentarUsarItem();
            }
            // Prioridad 2: Si tenemos un objeto tomado, lo soltamos
            else if (pickedObject != null)
            {
                DropObject();
            }
            // Prioridad 3: Si hay un objeto en rango, lo tomamos
            else if (objectInRange != null)
            {
                PickUpObject(objectInRange);
            }
            // El carro ya no necesita E, las llantas se instalan automáticamente
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Detectar zona de reparación de carro
        CarRepairSystem carZone = other.GetComponent<CarRepairSystem>();
        if (carZone != null)
        {
            currentCarZone = carZone;
            Debug.Log("Entraste a zona de reparación del carro");
            return;
        }

        // Detectar zona de uso de items (ItemUseZone)
        ItemUseZone useZone = other.GetComponent<ItemUseZone>();
        if (useZone != null)
        {
            currentUseZone = useZone;
            Debug.Log("Entraste a zona de uso: " + useZone.itemRequerido);
            return;
        }

        // Detectar objetos normales para tomar
        if (other.gameObject.CompareTag(objectTag) && pickedObject == null)
        {
            objectInRange = other.gameObject;
            Debug.Log("Objeto disponible para tomar: " + other.name);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // Salir de zona de reparación de carro
        CarRepairSystem carZone = other.GetComponent<CarRepairSystem>();
        if (carZone != null && carZone == currentCarZone)
        {
            currentCarZone = null;
            Debug.Log("Saliste de zona de reparación del carro");
            return;
        }

        // Salir de zona de uso
        ItemUseZone useZone = other.GetComponent<ItemUseZone>();
        if (useZone != null && useZone == currentUseZone)
        {
            currentUseZone = null;
            Debug.Log("Saliste de zona de uso");
            return;
        }

        // Objeto sale del rango
        if (other.gameObject.CompareTag(objectTag) && other.gameObject == objectInRange)
        {
            objectInRange = null;
            Debug.Log("Objeto fuera de alcance: " + other.name);
        }
    }

    private void PickUpObject(GameObject obj)
    {
        if (obj == null || handPoint == null) return;

        Rigidbody rb = obj.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = true;
            rb.useGravity = false;
        }

        obj.transform.position = handPoint.transform.position;
        obj.transform.rotation = handPoint.transform.rotation;
        obj.transform.SetParent(handPoint.transform);

        Collider objCollider = obj.GetComponent<Collider>();
        if (objCollider != null)
        {
            objCollider.enabled = false;
        }

        pickedObject = obj;
        objectInRange = null;

        Debug.Log("Objeto tomado: " + obj.name);
    }

    private void DropObject()
    {
        if (pickedObject == null) return;

        Rigidbody rb = pickedObject.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = false;
            rb.useGravity = true;
        }

        Collider objCollider = pickedObject.GetComponent<Collider>();
        if (objCollider != null)
        {
            objCollider.enabled = true;
        }

        pickedObject.transform.SetParent(null);

        if (rb != null)
        {
            Vector3 throwForce = transform.forward * 2f;
            rb.AddForce(throwForce, ForceMode.Impulse);
        }

        Debug.Log("Objeto soltado: " + pickedObject.name);
        pickedObject = null;
    }

    public bool HasObjectPickedUp()
    {
        return pickedObject != null;
    }

    public GameObject GetPickedObject()
    {
        return pickedObject;
    }

    public bool HasObjectAvailable()
    {
        return objectInRange != null && pickedObject == null;
    }

    public GameObject GetAvailableObject()
    {
        return objectInRange;
    }

    public bool IsInUseZone()
    {
        return currentUseZone != null;
    }

    public bool IsInCarZone()
    {
        return currentCarZone != null;
    }
}