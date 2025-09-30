using Firebase.Firestore;
using UnityEngine;

public class CatalogManager : MonoBehaviour
{
    FirebaseFirestore db;

    void Start()
    {
        db = FirebaseFirestore.DefaultInstance;
        CrearCatalogo();
    }

    async void CrearCatalogo()
    {
        DocumentReference llanta = db.Collection("Objetos").Document("1");
        await llanta.SetAsync(new { id = 1, nombre = "Llanta", imagen = "Llanta.jpg" });

        DocumentReference llave = db.Collection("Objetos").Document("2");
        await llave.SetAsync(new { id = 2, nombre = "Llave", imagen = "llave.png" });

        Debug.Log("Catálogo creado");
    }
}
