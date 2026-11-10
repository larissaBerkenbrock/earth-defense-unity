using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameOverManager : MonoBehaviour
{
    public GameObject gameOverPanel; // arrasta o GameOverPanel aqui
    public Button restartButton;     // arrasta o RestartButton aqui

    // Prepara o painel de game over e liga o botao de reiniciar.
    void Start()
    {
        // Garante que o painel começa desativado
        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);

        // Liga o botão à função de reiniciar
        if (restartButton != null)
            restartButton.onClick.AddListener(RestartGame);
    }

    // Mostra o painel de game over e pausa o jogo.
    public void ShowGameOver()
    {
        if (gameOverPanel != null)
            gameOverPanel.SetActive(true);

        // Pausa o jogo
        Time.timeScale = 0f;
    }

    // Despausa e recarrega a cena atual para recomecar a partida.
    public void RestartGame()
    {
        // Despausa antes de recarregar
        Time.timeScale = 1f;
        // Recarrega a cena atual
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
