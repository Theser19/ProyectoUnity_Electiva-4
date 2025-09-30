using UnityEngine;
using UnityEngine.UI;
using Firebase;
using Firebase.Extensions;
using Firebase.Storage;

public class ItemLoader : MonoBehaviour
{
    public string imagePath; // Nombre del archivo en Firebase (ej: "Llanta.jpg")
    public RawImage rawImage; // Imagen UI donde se mostrará

    void Start()
    {
        LoadImageFromFirebase();
    }

    void LoadImageFromFirebase()
    {
        FirebaseStorage storage = FirebaseStorage.DefaultInstance;
        StorageReference imageRef = storage.GetReference(imagePath);

        const long maxAllowedSize = 5 * 1024 * 1024; // 5 MB
        imageRef.GetBytesAsync(maxAllowedSize).ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted || task.IsCanceled)
            {
                Debug.LogError("Error al descargar imagen: " + task.Exception);
                return;
            }

            byte[] fileContents = task.Result;

            // Crear textura
            Texture2D tex = new Texture2D(2, 2);
            tex.LoadImage(fileContents);

            // Asignar a UI (RawImage)
            rawImage.texture = tex;
        });
    }

}
