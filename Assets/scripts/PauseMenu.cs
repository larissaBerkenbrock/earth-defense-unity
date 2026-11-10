using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public GameObject pausePanel;
    public static bool isPaused = false;

    // Garante que o jogo comeca despausado e com o painel de pausa escondido.
    void Start()
    {
        pausePanel.SetActive(false);
        Time.timeScale = 1f;
    }

    // Escuta a tecla Escape para alternar entre pausar e continuar.
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused) Resume();
            else Pause();
        }
    }

    // Mostra o painel de pausa e congela o tempo do jogo.
    public void Pause()
    {
        pausePanel.SetActive(true);
        Time.timeScale = 0f;
        isPaused = true;
    }

    // Esconde o painel de pausa e volta o tempo do jogo ao normal.
    public void Resume()
    {
        pausePanel.SetActive(false);
        Time.timeScale = 1f;
        isPaused = false;
    }

    // Reinicia a cena atual a partir do menu de pausa.
    public void Restart()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    // Volta para a cena do menu principal.
    public void MainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }
}
