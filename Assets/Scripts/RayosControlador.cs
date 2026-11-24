using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.InputSystem;

public class RayosControlador : MonoBehaviour
{

    [SerializeField] private Transform pivoteCamara;
    [SerializeField] private float alcanceInteraccion = 1f;
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private float shootForce = 10f;
    PlayerInput input;

    InputAction disparar;
    InputAction interactuar;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    { 
        input = GetComponent<PlayerInput>();
        disparar = input.actions["Shoot"];
        interactuar = input.actions["Interact"];
    }

    // Update is called once per frame
    void Update()
    {

        if (disparar.WasPressedThisFrame())
        {
            Disparar();
        }
        if (interactuar.WasPressedThisFrame())
        {
            Interactuar();
        }
    }

    private void Disparar()
    {
        Ray rayo = new Ray(pivoteCamara.position, pivoteCamara.forward);

        Debug.DrawRay(rayo.origin, rayo.direction * alcanceInteraccion, Color.red, 2f);

        GameObject bullet = Instantiate(bulletPrefab, pivoteCamara.position, Quaternion.identity);

        bullet.GetComponent<Rigidbody>().AddForce(rayo.direction * shootForce, ForceMode.Impulse);
        /*
        RaycastHit hit;
        if (Physics.Raycast(rayo, out hit, alcanceInteraccion))
        {
            Debug.Log("Impacto");

            if (hit.collider.gameObject.tag == "Cubo")
            {

                Destroy(hit.collider.gameObject);
            }
        }*/
    }

    private void Interactuar()
    {
        Ray rayo = new Ray(pivoteCamara.position, pivoteCamara.forward);

        Debug.DrawRay(rayo.origin, rayo.direction * alcanceInteraccion, Color.red, 2f);

        RaycastHit hit;
        if(Physics.Raycast(rayo, out hit, alcanceInteraccion))
        {
            Debug.Log("Impacto");

            if(hit.collider.gameObject.tag == "Cubo")
            {

                Destroy(hit.collider.gameObject);
            }

            if(hit.collider.gameObject.tag == "door")
            {
                hit.collider.gameObject.GetComponentInParent<DoorScript>().Interactuar();
            }
        }
    }
}
