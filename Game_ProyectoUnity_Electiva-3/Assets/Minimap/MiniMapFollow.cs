using UnityEngine;

public class MiniMapFollow : MonoBehaviour
{
    public Transform player;
    public float offsetHeight = 30f; // Altura sobre el terreno

    void LateUpdate()
    {
        RaycastHit hit;
        float height = player.position.y + offsetHeight;

        // Detecta el terreno bajo el jugador
        if (Physics.Raycast(player.position + Vector3.up * 10f, Vector3.down, out hit, 100f))
        {
            height = hit.point.y + offsetHeight;
        }

        Vector3 newPosition = new Vector3(player.position.x, height, player.position.z);
        transform.position = newPosition;
        transform.rotation = Quaternion.Euler(90f, player.eulerAngles.y, 0f);
    }
}
