using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField] TMP_InputField playerNameInputField;
    [SerializeField] TextMeshProUGUI leaderboardText;

    public void PlayGame()
    {
        if (playerNameInputField.text.Length > 0)
        {
            PlayerPrefs.SetString("CurrentPlayer", playerNameInputField.text);
        }
        else
        {
            PlayerPrefs.SetString("CurrentPlayer", "Player");
        }
        PlayerPrefs.Save();

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
            // todo handle WebGL
            Application.Quit();
#endif
    }

    private void Start()
    {
        if (PlayerPrefs.HasKey("LeaderboardData"))
        {
    
            UpdateLeaderboardText();
        }
    }

    private void UpdateLeaderboardText()
    {
        string[] scoreStrings = PlayerPrefs.GetString("LeaderboardData").Split(';');

        for (int i = 1; i < scoreStrings.Length; i++)
        {
            leaderboardText.text += $"{i}. {scoreStrings[i-1]}\n";
        }
    }

    

}
