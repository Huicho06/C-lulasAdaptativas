using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ControladorJuego : MonoBehaviour
{
    public GameObject celulaPrefab;  // Prefab de la c�lula
    public int numeroDeCelulas = 5;  // N�mero de c�lulas a generar por ronda
    public float duracionDeRonda = 10f;  // Duraci�n de cada ronda en segundos
    public Image fondoImage;  // Referencia al componente Image que act�a como fondo
    public Text contadorPuntosTexto;  // Texto UI para mostrar los puntos
    public Text contadorVivosTexto;   // Texto UI para mostrar los sobrevivientes acumulados

    private int puntos = 0;  // Contador de puntos
    private int totalSobrevivientes = 0;  // Contador acumulativo de c�lulas sobrevivientes
    private Color colorSobrevivienteFinal = Color.clear;  // Color del �ltimo sobreviviente

    private float tiempoRestante;  // Tiempo restante de la ronda
    private int celulasRestantes;  // N�mero de c�lulas que no han sido eliminadas

    void Start()
    {
        IniciarRonda();
    }

    void Update()
    {
        // Actualizar el tiempo de la ronda
        tiempoRestante -= Time.deltaTime;

        // Si el tiempo se acaba, finalizar la ronda
        if (tiempoRestante <= 0)
        {
            FinalizarRonda();
            IniciarRonda();  // Iniciar una nueva ronda
        }
    }

    void IniciarRonda()
    {
        tiempoRestante = duracionDeRonda;  // Reiniciar el temporizador de la ronda
        celulasRestantes = numeroDeCelulas;  // Reiniciar las c�lulas restantes

        // Obtener el color del fondo del Image
        Color colorFondo = fondoImage.color;

        // Generar c�lulas en posiciones aleatorias
        for (int i = 0; i < numeroDeCelulas; i++)
        {
            Vector2 posicionAleatoria = new Vector2(Random.Range(-8f, 8f), Random.Range(-4f, 4f));
            GameObject nuevaCelula = Instantiate(celulaPrefab, posicionAleatoria, Quaternion.identity);

            // Pasamos el color del fondo a las nuevas c�lulas
            C�lula scriptCelula = nuevaCelula.GetComponent<C�lula>();
            if (scriptCelula != null)
            {
                scriptCelula.SetColorFondo(colorFondo);  // Pasar el color del fondo
                scriptCelula.SetColorSobreviviente(colorSobrevivienteFinal);  // Pasar el color del �ltimo sobreviviente
            }
        }

        ActualizarUI();  // Actualizar los textos UI al inicio de la ronda
    }

    void FinalizarRonda()
    {
        // Aqu� evaluamos las c�lulas que quedan vivas
        GameObject[] celulasRestantesObjetos = GameObject.FindGameObjectsWithTag("Celula");
        float maxTiempoDeVida = 0f;  // Reiniciar el m�ximo tiempo de vida para la ronda actual

        foreach (GameObject celula in celulasRestantesObjetos)
        {
            C�lula scriptCelula = celula.GetComponent<C�lula>();
            if (scriptCelula != null)
            {
                float tiempoDeVida = scriptCelula.ObtenerTiempoDeVida();
                if (tiempoDeVida > maxTiempoDeVida)
                {
                    maxTiempoDeVida = tiempoDeVida;
                    colorSobrevivienteFinal = scriptCelula.ObtenerColor();  // Guardamos el color del sobreviviente con m�s tiempo
                }

                // Aumentamos el contador de sobrevivientes
                totalSobrevivientes++;
            }

            // Destruimos todas las c�lulas al final de la ronda
            Destroy(celula);
        }

        ActualizarUI();  // Actualizar los textos UI al final de la ronda
    }

    // M�todo llamado por las c�lulas cuando son eliminadas
    public void CelulaEliminada(float tiempoDeVida, Color colorEliminado)
    {
        puntos++;  // Aumentar el puntaje       
        celulasRestantes--;  // Reducir el n�mero de c�lulas vivas

        ActualizarUI();  // Actualizar los textos UI tras eliminar una c�lula
    }

    void ActualizarUI()
    {
        // Actualizamos el texto de los puntos y el texto de los sobrevivientes acumulados
        contadorPuntosTexto.text = "Puntos: " + puntos;
        contadorVivosTexto.text = "Sobrevivientes: " + totalSobrevivientes;
    }
}
