using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class DefeatGUI : MonoBehaviour {

    private Button tryAgain;

    // Use this for initialization
    void Start()
    {
        tryAgain = GetComponentInChildren<Button>();
    }

    public void TryAgain()
    {
        SceneManager.LoadScene("NightClub2_LargerRoom");
    }
}
