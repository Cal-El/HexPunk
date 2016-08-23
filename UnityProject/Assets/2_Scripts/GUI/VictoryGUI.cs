using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class VictoryGUI : MonoBehaviour {

    public void ReturnToMenu()
    {
        SceneManager.LoadScene("Menu");
    }
}
