using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class VictoryGUI : MonoBehaviour {

    private Button backToHQ;

    // Use this for initialization
    void Start()
    {
        backToHQ = GetComponentInChildren<Button>();
    }

    public void ReturnToMenu()
    {
        SceneManager.LoadScene("Menu");
    }
}
