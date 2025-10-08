using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Firebase.Auth;
using System.Threading.Tasks;
using UnityEngine.SceneManagement;

public class FirebaseLogin : MonoBehaviour
{
    [Header("Referencias de UI")]
    public TMP_InputField emailInput;
    public TMP_InputField passwordInput;
    public TMP_Text statusText;

    private void Start()
    {
        if (!FirebaseInit.IsReady())
        {
            statusText.text = "Inicializando Firebase...";
            FirebaseInit.OnFirebaseReady += OnFirebaseReady;
        }
    }

    private void OnFirebaseReady()
    {
        statusText.text = "Firebase listo.";
    }

    public async void RegisterUser()
    {
        string email = emailInput.text;
        string password = passwordInput.text;

        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
        {
            statusText.text = "Completa todos los campos.";
            return;
        }

        if (password.Length < 6)
        {
            statusText.text = "La contraseña debe tener al menos 6 caracteres.";
            return;
        }

        try
        {
            await FirebaseInit.Auth.CreateUserWithEmailAndPasswordAsync(email, password);
            statusText.text = "Usuario registrado correctamente.";
            await Task.Delay(1000);
            SceneManager.LoadScene(1);
        }
        catch (System.Exception e)
        {
            statusText.text = "Error al registrar: " + e.Message;
        }
    }

    public async void LoginUser()
    {
        string email = emailInput.text;
        string password = passwordInput.text;

        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
        {
            statusText.text = "Completa todos los campos.";
            return;
        }

        try
        {
            var result = await FirebaseInit.Auth.SignInWithEmailAndPasswordAsync(email, password);
            statusText.text = "Bienvenido " + result.User.Email;
            await Task.Delay(1000);
            SceneManager.LoadScene(1);
        }
        catch (System.Exception e)
        {
            statusText.text = "Error al iniciar sesión: " + e.Message;
        }
    }
}
