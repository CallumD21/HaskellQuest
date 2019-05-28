using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Hints : MonoBehaviour {

    //The confirmation panel
    [SerializeField] GameObject confirmationPanel;
    //Where the hint is written to the screen
    [SerializeField] Text hint;
    //List of hints for the current questions
    private List<string> hints = new List<string>();
    //The index of hints of the current question (-1 if no hints unlocked)
    private int currentHint = -1;
    //The index of hints of the next question to be unlocked
    private int nextHint = 0;
    //The quiz manager
    private QuizManager quizManager;

    private void Start(){
        quizManager = GetComponentInParent<QuizManager>();
    }

    //Updates the required information for the new question
    public void ChangeQuestion(List<string> h){
        hints = h;
        currentHint = -1;
        nextHint = 0;
        hint.text = "You currently have no hints, press the <color=#00ff00ff><b>+</b></color> to buy a new hint!";
    }

    //Called when the hints button is pressed
    public void DisplayHints(){
        gameObject.SetActive(true);
    }

    //Called when the left arrow is pressed
    public void Left(){
        //Only move left if there is a hint unlocked
        if (currentHint != -1){
            currentHint--;
            if (currentHint == -1){
                currentHint = nextHint - 1;
            }
            hint.text = hints[currentHint];
        }
    }

    //Called when the right arrow is pressed
    public void Right(){
        //Only move right if there is a hint unlocked
        if (currentHint != -1){
            currentHint++;
            if (currentHint == nextHint){
                currentHint = 0;
            }
            hint.text = hints[currentHint];
        }
    }

    //Called when the new hint button is pressed
    public void NewHint(){
        //If there are still hints to unlock and the player has enough money then show the panel
        int money = quizManager.GetMoney();
        if (nextHint != hints.Count && money >= 5){
            confirmationPanel.SetActive(true);
        }
        else if(nextHint == hints.Count){
            hint.text = "<color=#00ff00ff>You have bought all the hints for this question!\n(Press <- or -> to remove this message.)</color>";
        }
        else{
            hint.text = "<color=#00ff00ff>You cannot afford a hint!</color>";
        }
    }

    //Called when the yes button is pressed
    public void Yes(){
        string line = new string('-', 5);
        FindObjectOfType<Evaluation>().AddAnswer("\n" + line + "HINT" + line);
        quizManager.UpdateMoney(-5);
        currentHint = nextHint;
        nextHint++;
        hint.text = hints[currentHint];
        confirmationPanel.SetActive(false);
    }

    //Called when the no button is pressed
    public void No(){
        confirmationPanel.SetActive(false);
    }

    //Called when the corss is pressed
    public void Exit(){
        gameObject.SetActive(false);
    }
}
