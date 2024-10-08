using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ControladorJuego : MonoBehaviour
{
    public GameObject celulaPrefab;  // Prefab de la célula
    public int numeroDeCelulas = 5;  // Número de células a generar por ronda
    public float duracionDeRonda = 10f;  // Duración de cada ronda en segundos

    public Text contadorPuntosTexto;  // Texto UI para mostrar los puntos
    public Text contadorVivosTexto;   // Texto UI para mostrar los sobrevivientes acumulados

    private int puntos = 0;  // Contador de puntos
    private int totalSobrevivientes = 0;  // Contador acumulativo de células sobrevivientes
    private Color colorSobrevivienteFinal = Color.clear;  // Color del último sobreviviente
    private Color colorUltimaCelulaEliminada = Color.clear;  // Color de la última célula eliminada
    private float maxTiempoDeVida = 0f;  // Máximo tiempo de vida de la célula sobreviviente

    private float tiempoRestante;  // Tiempo restante de la ronda
    private int celulasRestantes;  // Número de células que no han sido eliminadas

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
            // Iniciar una nueva ronda
        }

    }

    void IniciarRonda()
    {
        tiempoRestante = duracionDeRonda;  // Reiniciar el temporizador de la ronda
        celulasRestantes = numeroDeCelulas;  // Reiniciar las células restantes

        // Generar células en posiciones aleatorias
        for (int i = 0; i < numeroDeCelulas; i++)
        {
            Vector2 posicionAleatoria = new Vector2(Random.Range(-8f, 8f), Random.Range(-4f, 4f));
            GameObject nuevaCelula = Instantiate(celulaPrefab, posicionAleatoria, Quaternion.identity);

            // Pasamos el color del fondo a las nuevas células
            Célula scriptCelula = nuevaCelula.GetComponent<Célula>();
            if (scriptCelula != null)
            {
                scriptCelula.SetColorSobreviviente(colorSobrevivienteFinal);  // Pasar el color del último sobreviviente
            }
        }

        ActualizarUI();  // Actualizar los textos UI al inicio de la ronda
    }

    void FinalizarRonda()
    {
        // Aquí evaluamos las células que quedan vivas
        GameObject[] celulasRestantesObjetos = GameObject.FindGameObjectsWithTag("Celula");
        maxTiempoDeVida = 0f;  // Reiniciar el máximo tiempo de vida para la ronda actual

        if (celulasRestantesObjetos.Length > 0)
        {
            // Si hay células vivas, evaluarlas
            foreach (GameObject celula in celulasRestantesObjetos)
            {
                Célula scriptCelula = celula.GetComponent<Célula>();
                if (scriptCelula != null)
                {
                    float tiempoDeVida = scriptCelula.ObtenerTiempoDeVida();
                    if (tiempoDeVida > maxTiempoDeVida)
                    {
                        maxTiempoDeVida = tiempoDeVida;
                        colorSobrevivienteFinal = scriptCelula.ObtenerColor();  // Guardamos el color del sobreviviente con más tiempo
                    }

                    // Aumentamos el contador de sobrevivientes
                    totalSobrevivientes++;
                }

                // Destruimos todas las células al final de la ronda
                Destroy(celula);
            }
        }
        else
        {
            // Si no hay células restantes, usar el color de la última célula eliminada
            Debug.Log("No hay células vivas, usando el color de la última célula eliminada: " + colorUltimaCelulaEliminada);
            colorSobrevivienteFinal = colorUltimaCelulaEliminada;  // Adaptar al color de la última célula eliminada
        }

        ActualizarUI();  // Actualizar los textos UI al final de la ronda
        IniciarRonda();  // Iniciar una nueva ronda
    }

    void AdaptarCeldasAlSobreviviente()
    {
        // Lógica para adaptar las nuevas células al color del sobreviviente
        // Puedes implementar la lógica para cambiar el color del prefab o crear nuevas células adaptadas
        Debug.Log("Adaptando nuevas células al color del sobreviviente: " + colorSobrevivienteFinal);
        // Aquí puedes implementar la lógica para crear nuevas células con el color sobreviviente
    }

    // Método llamado por las células cuando son eliminadas
    public void CelulaEliminada(float tiempoDeVida, Color colorEliminado)
    {
        puntos++;  // Aumentar el puntaje       
        celulasRestantes--;  // Reducir el número de células vivas

        // Almacenar el color de la última célula eliminada
        colorUltimaCelulaEliminada = colorEliminado;

        ActualizarUI();  // Actualizar los textos UI tras eliminar una célula
    }

    void ActualizarUI()
    {
        // Actualizamos el texto de los puntos y el texto de los sobrevivientes acumulados
        contadorPuntosTexto.text = "Puntos: " + puntos;
        contadorVivosTexto.text = "Sobrevivientes: " + totalSobrevivientes;
    }
}
