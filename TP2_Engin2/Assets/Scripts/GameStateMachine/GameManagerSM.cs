using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManagerSM : GM_BaseStateMachine<GM_IState>
{
    [SerializeField]
    private GameObject m_mainMenu;
    [SerializeField]
    private GameObject m_optionsMenu;

    private void Awake() 
    {
        DontDestroyOnLoad(this.gameObject);
        CreatePossibleStates();
    }
    protected override void CreatePossibleStates()
    {
        m_possibleStates = new List<GM_IState>();
        m_possibleStates.Add(new MainMenuState());
        m_possibleStates.Add(new LobbyState());
        m_possibleStates.Add(new TutorialState());
        m_possibleStates.Add(new MainLevelState());
    }
    public void PlayGame()
    {
        SceneManager.LoadScene(1);
    }
    public void PlayTutorial()
    {
        SceneManager.LoadScene(3);
    }
    public void OpenOptionsMenu()
    {
        m_mainMenu.SetActive(false);
        m_optionsMenu.SetActive(true);
    }
    public void CloseOptionsMenu()
    {
        m_mainMenu.SetActive(true);
        m_optionsMenu.SetActive(false);
    }
    public void QuitGame()
    {
        Application.Quit();
    }
}
