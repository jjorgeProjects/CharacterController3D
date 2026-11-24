using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class DoorScript : MonoBehaviour
{
    [SerializeField] private float anguloApertura = 90f;
    [SerializeField] private float velocidadGiro = 120f;

    private bool abierta = false;
    private Quaternion rotacionCerrada;
    private Quaternion rotacionObjetivo;
    private bool cooldown = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rotacionCerrada = transform.localRotation;
        rotacionObjetivo = rotacionCerrada;
    }

    // Update is called once per frame
    void Update()
    {
        transform.localRotation = Quaternion.RotateTowards(
            transform.localRotation, 
            rotacionObjetivo, 
            velocidadGiro * Time.deltaTime
            );
    }

    IEnumerator CambiarEstado()
    {
        rotacionObjetivo = abierta ? rotacionCerrada * Quaternion.Euler(0, anguloApertura, 0) : rotacionCerrada;

        yield return new WaitForSeconds(2f);

        cooldown = false;
    }

    public void Interactuar()
    {
       

        if (!cooldown) {
            if (!abierta)
            {
                Debug.Log("Abriendo");
            }
            else
            {
                Debug.Log("Cerrando");
            }
            cooldown = true;
            abierta = !abierta;
            StartCoroutine(nameof(CambiarEstado));
        }


}
}
