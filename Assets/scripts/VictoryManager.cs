using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class VictoryManager : MonoBehaviour
{
    public static VictoryManager Instance;

    [Header("Painel")]
    public GameObject victoryPanel;
    public GameObject missaoConcluidaTexto;
    public string missaoConcluidaAnimationState = "vitoria";

    [Header("Botoes")]
    public Button nextLevelButton;
    public Button mainMenuButton;

    // Inicializa a instancia global, configura referencias e esconde a tela de vitoria.
    void Awake()
    {
        Instance = this;

        if (victoryPanel == gameObject)
        {
            Debug.LogError("VictoryManager.victoryPanel esta a apontar para o proprio VictoryManager. Usa o objeto VictoryPanel.");
            victoryPanel = null;
        }

        AutoConfigureMissionText();
        AutoConfigureButtons();

        SetVictoryVisible(false);
    }

    // Procura automaticamente o texto de missao concluida se ele nao foi atribuido.
    void AutoConfigureMissionText()
    {
        if (missaoConcluidaTexto != null) return;

        missaoConcluidaTexto = FindSceneObjectByName("missaoConcluidaTexto");
        if (missaoConcluidaTexto == null)
            missaoConcluidaTexto = FindSceneObjectByName("missaoConcluida");
        if (missaoConcluidaTexto == null)
            missaoConcluidaTexto = FindSceneObjectByName("missaoConsluida");
    }

    // Encontra um objeto da cena pelo nome, incluindo objetos desativados.
    GameObject FindSceneObjectByName(string objectName)
    {
        GameObject[] objects = Resources.FindObjectsOfTypeAll<GameObject>();
        foreach (GameObject sceneObject in objects)
        {
            if (sceneObject.name == objectName && sceneObject.scene.IsValid())
                return sceneObject;
        }

        return null;
    }

    // Procura botoes no painel de vitoria e liga suas funcoes de clique.
    void AutoConfigureButtons()
    {
        if (victoryPanel == null) return;

        Button[] buttons = victoryPanel.GetComponentsInChildren<Button>(true);
        foreach (Button button in buttons)
        {
            if (nextLevelButton == null && button.name.Contains("Restart"))
                nextLevelButton = button;

            if (mainMenuButton == null && button.name.Contains("Menu"))
                mainMenuButton = button;
        }

        if (nextLevelButton != null)
        {
            nextLevelButton.onClick.RemoveAllListeners();
            nextLevelButton.onClick.AddListener(NextLevel);
        }

        if (mainMenuButton != null)
        {
            mainMenuButton.onClick.RemoveAllListeners();
            mainMenuButton.onClick.AddListener(MainMenu);
        }
    }

    // Mostra a tela de nivel completo e ajusta o botao conforme exista proximo nivel.
    public void ShowLevelComplete(int completedLevel, int totalLevels, bool hasNextLevel)
    {
        SetButtonLabel(nextLevelButton, hasNextLevel ? "Proximo nivel" : "Jogar novamente");

        if (victoryPanel != null)
            SetVictoryVisible(true);
        else
            Debug.LogError("Painel de vitoria nao atribuido no VictoryManager.");

        Time.timeScale = 0f;
    }

    // Mostra uma tela simples de vitoria para compatibilidade com chamadas antigas.
    public void ShowVictory()
    {
        ShowLevelComplete(1, 1, false);
    }

    // Continua para o proximo nivel ou reinicia se nao houver outro nivel.
    public void NextLevel()
    {
        Time.timeScale = 1f;

        if (SpawnManager.Instance != null && SpawnManager.Instance.StartNextLevel())
        {
            SetVictoryVisible(false);

            return;
        }

        Restart();
    }

    // Recarrega a cena atual para jogar novamente.
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

    // Altera o texto visivel de um botao da UI.
    void SetButtonLabel(Button button, string label)
    {
        if (button == null) return;

        TextMeshProUGUI text = button.GetComponentInChildren<TextMeshProUGUI>(true);
        if (text != null)
            text.text = label;
    }

    // Mostra ou esconde os objetos da tela de vitoria.
    void SetVictoryVisible(bool visible)
    {
        if (victoryPanel != null)
            victoryPanel.SetActive(visible);

        if (missaoConcluidaTexto != null)
            missaoConcluidaTexto.SetActive(visible);

        if (visible)
            PlayMissionTextAnimation();
    }

    // Reinicia e toca a animacao do texto de missao concluida.
    void PlayMissionTextAnimation()
    {
        if (missaoConcluidaTexto == null) return;

        Animator animator = missaoConcluidaTexto.GetComponentInChildren<Animator>(true);
        if (animator == null) return;

        animator.enabled = true;
        animator.updateMode = AnimatorUpdateMode.UnscaledTime;
        animator.Rebind();
        animator.Update(0f);

        int stateHash = Animator.StringToHash(missaoConcluidaAnimationState);
        if (animator.HasState(0, stateHash))
            animator.Play(stateHash, 0, 0f);
        else if (animator.HasState(0, Animator.StringToHash("vitoria 0")))
            animator.Play("vitoria 0", 0, 0f);
    }
}
