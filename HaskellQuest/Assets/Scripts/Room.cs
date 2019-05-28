using UnityEngine;
using UnityEngine.UI;
using UnityStandardAssets.Characters.FirstPerson;

public class Room : MonoBehaviour {

    [SerializeField] Text moneyText;
    [SerializeField] Text educationText;
    [SerializeField] Text progressText;
    [SerializeField] Slider educationSlider;
    [SerializeField] GameObject sliderFill;
    [SerializeField] GameObject[] beds;
    [SerializeField] GameObject exitGame;
    //The date text box
    [SerializeField] private Text dateText;
    private FirstPersonController personController;

    private void Start(){
        personController = FindObjectOfType<FirstPersonController>();
        GameManager gm = FindObjectOfType<GameManager>();
        moneyText.text = "£" + gm.GetMoney().ToString();
        int educationLevel = gm.GetEducationLevel();
        educationText.text = educationLevel.ToString();
        dateText.text = gm.GetDate();
        //The total amount of education you have to get to level up is (educationLevel+1)*10
        int totalProgress = (educationLevel + 1) * 10;
        int educationProgress = gm.GetEducationPorgress();
        progressText.text = educationProgress.ToString() + "/" + totalProgress.ToString();
        //If the user currently has no progress the fill should not appear
        if (educationProgress == 0){
            sliderFill.gameObject.SetActive(false);
        }
        //If the player has bought the new bed then activate the new bed and deactivate the old bed
        if (gm.UnlockedBed()){
            beds[0].SetActive(false);
            beds[1].SetActive(true);
        }
        educationSlider.value = (float)educationProgress / totalProgress;
    }

    private void FixedUpdate(){
        if (Input.GetKeyDown(KeyCode.Escape)){
            //Open the exit game panel
            exitGame.SetActive(true);
            //Stop time
            Time.timeScale = 0;
            //Unlock the cursor and stop player rotation
            personController.SetSensitivityAndMouse(0, 0, false);
        }
    }

    //Called when the user presses yes to exit the game
    public void ExitGame(){
        Application.Quit();
    }

    //Called when the user presses no to exit the game
    public void ResumeGame(){
        //Close the exit game panel
        exitGame.SetActive(false);
        //Restart time
        Time.timeScale = 1;
        //Lock the cursor and restart player rotation
        personController.SetSensitivityAndMouse(2, 2, true);
    }
}
