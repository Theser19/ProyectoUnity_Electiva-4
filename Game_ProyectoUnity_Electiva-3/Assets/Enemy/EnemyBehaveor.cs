using UnityEngine;

public class EnemyBehaveor : MonoBehaviour
{
    public int rutina;
    public float cronometro;
    public Animator ani;
    public Quaternion angulo;
    public float grado;

    public GameObject target;
    public bool atacando;
    void Start()
    {
        ani = GetComponent<Animator>();
        target = GameObject.Find("Player");
    }

    public void comportamiento_enemigo()
    {

        if (Vector3.Distance(transform.position, target.transform.position) > 10)
        {
            ani.SetBool("Run", false);
            //cambio de rutina
            cronometro += 1 * Time.deltaTime;

            if (cronometro >= 4)
            {
                rutina = Random.Range(0, 2);
                cronometro = 0;
            }

            switch (rutina)
            {
                case 0:
                    ani.SetBool("Walk", false);
                    break;

                case 1:
                    grado = Random.Range(0, 360);
                    angulo = Quaternion.Euler(0, grado, 0);
                    rutina++;
                    break;

                case 2:
                    transform.rotation = Quaternion.RotateTowards(transform.rotation, angulo, 0.5f);
                    transform.Translate(Vector3.forward * 1 * Time.deltaTime);
                    ani.SetBool("Walk", true);
                    break;
            }
        }
        else
        {
            if (Vector3.Distance(transform.position, target.transform.position) > 2)
            {
                var lookPos = target.transform.position - transform.position;
                lookPos.y = 0;
                var rotation = Quaternion.LookRotation(lookPos);
                transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * 3);
                ani.SetBool("Walk", false);
                ani.SetBool("Run", true);
                transform.Translate(Vector3.forward * 2 * Time.deltaTime);

                ani.SetBool("Attack", false);
            }
            else
            {
                ani.SetBool("Walk", false);
                ani.SetBool("Run", false);

                ani.SetBool("Attack", true);
                atacando = true;
            }
        }
    }
    public void Final_Ani()
    {
        ani.SetBool("Attack", false);
        atacando = false;
    }

    void Update()
    {
        comportamiento_enemigo();
    }
}

