using System;
using System.IO;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class ControladorJugador : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private Transform pivoteCamara;
    [SerializeField] private Camera camara;
    [SerializeField] private GameObject modelo;

    [Header("Movimiento")]
    [SerializeField] private float velocidad = 5f;
    [SerializeField] private float alturaSalto = 1.5f;
    [SerializeField] private float gravedad = -20f;
    [SerializeField] private float sensibilidadMouse = 0.12f;

    [Header("Camara")]
    [SerializeField] private Vector3 posicionTercera = new Vector3(0.65f, 0.15f, -3f);
    [SerializeField] private Vector3 posicionApuntado = new Vector3(0.55f, 0.1f, -1.7f);
    [SerializeField] private float suavizado = 10f;

    private float velocidadVertical;

    private float inclinacion = 0f;

    private bool primeraPersona = false;


    //Componentes que vamos a manipular
    private CharacterController controlador;
    private PlayerInput input;
    private InputAction mover;
    private InputAction saltar;
    private InputAction mirar;
    private InputAction apuntar;
    private InputAction cambiarVista;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        controlador = GetComponent<CharacterController>();
        input = GetComponent<PlayerInput>();

        mover = input.actions["Move"];
        saltar = input.actions["Jump"];
        mirar = input.actions["Look"];
        apuntar = input.actions["Point"];
        cambiarVista = input.actions["ChangeView"];

        //Bloqueamos el cursor y lo hacemos invisible
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

    }

    // Update is called once per frame
    void Update()
    {
        if (cambiarVista.WasPressedThisFrame())
        {
            primeraPersona = !primeraPersona;
            modelo.SetActive(!primeraPersona);
        }

        GirarCamara();
        MoverPersonaje();
        ActualizarCamara();
    }

    void GirarCamara()
    {
        //1. Obtener el delta del ratón
        Vector2 deltaMouse = mirar.ReadValue<Vector2>();

        //2. Rotamos le gameobject (jugador) en el eje Y
        transform.Rotate(Vector3.up * deltaMouse.x * sensibilidadMouse);

        //3. Obtenemos la inclinación de la cámara según el desplazamiento del ratón en Y
        inclinacion -= deltaMouse.y * sensibilidadMouse;

        //4. Limitamos el ángulo de inclinación
        inclinacion = Mathf.Clamp(inclinacion, -80f, 80f);

        //5. Aplicamos la rotación al pivote (LOCAL)
        pivoteCamara.localRotation = Quaternion.Euler(inclinacion, 0f, 0f);
        
    }

    void MoverPersonaje()
    {
        //1. Leer el input
        Vector2 entrada = mover.ReadValue<Vector2>();

        //2. Calcular dirección de movimiento
        Vector3 direccion = transform.right * entrada.x + transform.forward * entrada.y;

        direccion = Vector3.ClampMagnitude(direccion, 1f);

        //Gestionar la velocidad en Y e implementar el salto
        if(controlador.isGrounded && saltar.WasPressedThisFrame())
        {
            velocidadVertical = Mathf.Sqrt(alturaSalto * -2f * gravedad);
        }

        velocidadVertical += gravedad * Time.deltaTime;
        
        //3. Calcular vector desplazamiento final para utilizar con el Move
        Vector3 desplazamiento = direccion * velocidad;

        desplazamiento.y = velocidadVertical;

        //4. Mover al personaje con el CharacterController
        controlador.Move(desplazamiento * Time.deltaTime);

    }

    void ActualizarCamara()
    {
        //1. Destino de la camara
        Vector3 destino = Vector3.zero;
        // Cambia según FPS o TPS
        if (!primeraPersona)
        {
            //Posición de tercera definida
            destino = posicionTercera;
        }

        //Interpolamos entre donde está ahora y donde estará
        camara.transform.localPosition = Vector3.Lerp(
            camara.transform.localPosition,
            destino,
            suavizado * Time.deltaTime);

    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {        

        if(hit.gameObject.tag == "Cubo")
        {

            Destroy(hit.gameObject);
        }
    }
}
