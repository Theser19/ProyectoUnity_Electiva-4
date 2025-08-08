using UnityEngine;

public class Functions : MonoBehaviour
{
    public GameObject Cubo;

    // Rotación
    public void RotateLeft()
    {
        Cubo.transform.Rotate(0.0f, 10.0f, 0.0f, Space.Self);
    }

    public void RotateRight()
    {
        Cubo.transform.Rotate(0.0f, -10.0f, 0.0f, Space.Self);
    }

    // Traslación
    public void TranslateUp()
    {
        Cubo.transform.Translate(Vector3.up * Time.deltaTime * 120, Space.World);
    }

    public void TranslateDown()
    {
        Cubo.transform.Translate(Vector3.down * Time.deltaTime * 120, Space.World);
    }

    public void TranslateLeft()
    {
        Cubo.transform.Translate(Vector3.left * Time.deltaTime * 120, Space.World);
    }

    public void TranslateRight()
    {
        Cubo.transform.Translate(Vector3.right * Time.deltaTime * 120, Space.World);
       
    }

    // Escalado
    public void ScaleUP(float magnitud)
    {
        Vector3 changerscale = new Vector3(magnitud, magnitud, magnitud);
        Cubo.transform.localScale += changerscale;
    }

    public void ScaleDown(float magnitud)
    {
        Vector3 changerscale = new Vector3(magnitud, magnitud, magnitud);
        Cubo.transform.localScale -= changerscale;
    }
}
