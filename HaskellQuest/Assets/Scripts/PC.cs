using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PC : MonoBehaviour {

    //The gameobject of the quiz button
    [SerializeField] Button quiz;
    //The worksheet button
    [SerializeField] Button worksheet;
    //The space invaders button
    [SerializeField] Button spaceInvaders;
    //The quiz panel
    [SerializeField] GameObject quizPanel;
    //An array of arrays of the quiz buttons
    //Each array contains quizzes of the same level
    [SerializeField] Buttons[] levels;
    //The sprites for the completed and active quizzes
    [SerializeField] Sprite activeQuiz;
    [SerializeField] Sprite completedQuiz;

    private void Start(){
        //Unlock the mouse and make it visible
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        GameManager gm = FindObjectOfType<GameManager>();
        //The quizzes are only available after day 2
        if (gm.GetDay() < 3){
            quiz.gameObject.SetActive(false);
        }
        if (gm.GetTime() != 0){
            quiz.interactable = false;
        }
        if (gm.GetTime() == 1){
            worksheet.interactable = true;
        }
        if (gm.UnlockedSpaceInvaders()){
            spaceInvaders.gameObject.SetActive(true);
        }
    }

    public void Worksheet(){
        SceneManager.LoadScene(Scenes.worksheet);
    }

    public void Quiz(){
        UnlockCurrentQuiz();
        quizPanel.SetActive(true);
    }

    public void PowerOff(){
        SceneManager.LoadScene(Scenes.room);
    }

    public void SpaceInvaders(){
        SceneManager.LoadScene(Scenes.spaceInvaders_preload);
    }

    //Called when one of the quiz buttons is pressed
    public void OpenQuiz(){
        Debug.Log(Scenes.quiz);
        SceneManager.LoadScene(Scenes.quiz);
    }

    //Called when the cross is pressed
    public void Exit(){
        quizPanel.SetActive(false);
    }

    //Called when the Quiz panel is opened to set the current quiz button to clickable and change the sprites of the completed quizzes
    private void UnlockCurrentQuiz(){
        int currentQuiz = FindObjectOfType<GameManager>().GetQuiz();
        //The number buttons visited so far
        int numQuizzes = 0;
        for(int i = 0; i < levels.Length; i++){
            Button[] buttons = levels[i].buttons;
            for(int j = 0; j < buttons.Length; j++){
                //We have found the current quiz so make it clickable
                if (numQuizzes == currentQuiz){
                    //The player has to be level i+2 in order to do this quiz
                    int educationLevel = FindObjectOfType<GameManager>().GetEducationLevel();
                    if (educationLevel >= i + 2){
                        buttons[j].GetComponent<Image>().sprite = activeQuiz;
                        buttons[j].interactable = true;
                        buttons[j].GetComponentInChildren<Text>().color = new Color32(255, 255, 255, 255);
                        return;
                    }
                    else{
                        return;
                    }
                }
                else{
                    buttons[j].GetComponent<Image>().sprite = completedQuiz;
                    numQuizzes++;
                }
            }
        }
    }

    //Called when the pointer enter the help icon
    public void ShowHelp(Transform help){
        //Helps child is its text
        help.GetChild(0).gameObject.SetActive(true);
    }

    //Called when the pointer enter the help icon
    public void HideHelp(Transform help){
        //Helps child is its text
        help.GetChild(0).gameObject.SetActive(false);
    }
}
