using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Célula : MonoBehaviour
{
    public AudioClip sonidoEliminacion;  // Clip de sonido que se reproducirá al eliminar la célula
    private AudioSource audioSource;
    private ControladorJuego controladorJuego;  // Referencia al controlador del juego

    private Color colorSobreviviente;  // Color de las células que sobrevivieron más tiempo
    private Color colorActual;  // Color actual de la célula
    private float tiempoDeVida = 0f;  // Cuánto tiempo ha sobrevivido la célula
    private bool clickeado = false;  // Controla si la célula ya ha sido clickeada
    private float adaptacionFactor = 0.95f;  // **Factor de adaptación aún más rápido (incrementado)**

    void Start()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.clip = sonidoEliminacion;

        controladorJuego = GameObject.FindObjectOfType<ControladorJuego>();
        CambiarColor();  // Cambia el color al inicio
    }

    void Update()
    {
        // Incrementamos el tiempo de vida de la célula
        tiempoDeVida += Time.deltaTime;
    }

    public void SetColorSobreviviente(Color nuevoColorSobreviviente)
    {
        colorSobreviviente = nuevoColorSobreviviente;  // Establecer el color de las células sobrevivientes
    }

    void CambiarColor()
    {
        // Cambia el color al inicio en un rango cercano al color sobreviviente (si existe) o aleatorio
        if (colorSobreviviente != Color.clear)
        {
            // Genera un color cercano al sobreviviente variando ligeramente dentro del rango de ese color
            colorActual = GenerarColorCercano(colorSobreviviente);
        }
        else
        {
            colorActual = new Color(Random.value, Random.value, Random.value);  // Color aleatorio si no hay sobreviviente
        }

        GetComponent<SpriteRenderer>().color = colorActual;
    }

    Color GenerarColorCercano(Color baseColor)
    {
        // Generamos una pequeña variación en el color, manteniéndolo dentro del rango del color sobreviviente
        float variacion = 0.15f;  // Rango de variación del color (más ajustado para mejor adaptación)
        float r = Mathf.Clamp(baseColor.r + Random.Range(-variacion, variacion), 0f, 1f);
        float g = Mathf.Clamp(baseColor.g + Random.Range(-variacion, variacion), 0f, 1f);
        float b = Mathf.Clamp(baseColor.b + Random.Range(-variacion, variacion), 0f, 1f);

        return new Color(r, g, b);
    }

    void OnMouseDown()
    {
        // Solo permitimos un clic por célula
        if (!clickeado)
        {
            EliminarCelula();
            clickeado = true;  // Evitar más clics en la misma célula
        }
    }

    public void EliminarCelula()
    {
        audioSource.Play();

        if (controladorJuego != null)
        {
            // Notificamos al controlador del juego que esta célula fue eliminada
            controladorJuego.CelulaEliminada(tiempoDeVida, colorActual);
        }

        Destroy(gameObject, sonidoEliminacion.length);  // Destruir tras reproducir el sonido
    }

    public float ObtenerTiempoDeVida()
    {
        return tiempoDeVida;
    }

    public Color ObtenerColor()
    {
        return colorActual;  // Devuelve el color actual de la célula
    }
}
