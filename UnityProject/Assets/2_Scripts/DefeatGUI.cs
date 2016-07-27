using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class DefeatGUI : MonoBehaviour {

    public void TryAgain()
    {
        SceneManager.LoadScene("NightClub2_LargerRoom");
    }
}
