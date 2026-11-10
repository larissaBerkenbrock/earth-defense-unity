using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    // Carrega a cena principal do jogo quando o jogador aperta Play.
    public void PlayGame()
    {
        SceneManager.LoadScene("SampleScene");
    }

    // Fecha o jogo quando o jogador escolhe sair.
    public void QuitGame()
    {
        Debug.Log("Sair do jogo");
        Application.Quit();
    }
}
