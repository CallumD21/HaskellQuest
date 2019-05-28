using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.IO;
using TMPro;

public class SchoolManager : MonoBehaviour {

    //The text box to put the sentences in
    [SerializeField] TextMeshProUGUI sentence;
    //The text box to put the questions in
    [SerializeField] Text question;
    //The array of the buttons for the answers
    [SerializeField] Button[] answers;
    //The text box on the button
    [SerializeField] Text buttonText;
    //The panel for displaying the text
    [SerializeField] GameObject dialoguePanel;
    //The panel for asking a question
    [SerializeField] GameObject questionPanel;
    //The button on the question panel to move onto the next panel
    [SerializeField] GameObject nextButton;
    //The queue of text for this day
    private Queue<string> text;
    //True if dialoguePanel is showing
    private bool showingDialogue = true;
    //The index of the button in answers that is the right answer
    private int rightAnswer;
    //True if the user got the question right
    private bool gotRight;
    //The education reward for completing the day
    private int educationReward;
    private int schoolDay;

    private void Start(){
        text = new Queue<string>();
        schoolDay = FindObjectOfType<GameManager>().GetSchoolDay();
        //Read in the days text
        StreamReader reader = File.OpenText(Application.dataPath + "/StreamingAssets/School/" + schoolDay.ToString() + ".txt");
        string line = reader.ReadLine();
        bool firstLine = true;
        while (line != null){
            //The first line (if it exists) is the amount of education to get for completing the day
            if (firstLine){
                firstLine = false;
                educationReward = int.Parse(line);
            }
            else{
                //Remove the \n and put them back in as they are read from a file as characters and not as a new line 
                string[] splitLine = line.Split(new string[] { "\\n" }, System.StringSplitOptions.RemoveEmptyEntries);
                line = "";
                for (int i = 0; i < splitLine.Length - 1; i++){
                    line += splitLine[i] + "\n";
                }
                //Add the final line
                line += splitLine[splitLine.Length - 1];
                text.Enqueue(line);
            }
            line = reader.ReadLine();
        }
        reader.Close();

        //Display the first sentence
        sentence.text = text.Dequeue();
    }

    //Called when next button is pressed on dialogue
    public void ChangeSentence(){
        //If no sentences left then go back home and change time
        if (text.Count == 0){
            GameManager gm = FindObjectOfType<GameManager>();
            SceneManager.LoadScene(Scenes.room);
            //If there exists a worksheet with the same name as this school day then set the time to the afternoon
            if(File.Exists(Application.dataPath + "/StreamingAssets/Worksheet/" + schoolDay.ToString() + ".txt")){
                gm.SetTime(1);
            }
            else{
                gm.SetTime(2);
            }
            gm.ChangeSchoolDay();
            gm.UpdateEducation(educationReward);
        }
        //If 1 sentence left then change the text of the button and display the final text
        else if (text.Count == 1){
            buttonText.text = "Go home";
            DisplayText();
        }
        //else display the next text
        else{
            DisplayText();
        }
    }

    private void DisplayText(){
        string line = text.Dequeue();
        //If the first two characters of the line are Q2/Q4 then then we have a question with 2/4 answers
        if(line.Substring(0,2) == "Q2"){
            AskQuestion(2, line.Substring(2));
        }
        else if(line.Substring(0,2) == "Q4"){
            AskQuestion(4, line.Substring(2));
        }
        //Else it is just a sentence so, if it isnt already, show the dialogue panel and display the text
        else{
            if (!showingDialogue){
                questionPanel.SetActive(false);
                dialoguePanel.SetActive(true);
                showingDialogue = true;
            }
            //If the line is <l> it is a start of a bullet point list
            if (line == "<l>"){
                BulletList();
            }
            else{
                //If the 1st character of the line is a * then the line is the response to a question
                if (line[0] == '*'){
                    //If the user got it right we want to show this line and remove the next else remove this line show the next
                    if (gotRight){
                        line = line.Substring(1);
                        text.Dequeue();
                    }
                    else{
                        line = text.Dequeue();
                    }
                }
                sentence.text = line;
            }
        }
    }

    //If it isnt already, show the question panel and display the question and its answers
    private void AskQuestion(int numAnswers, string q){
        if (showingDialogue){
            dialoguePanel.SetActive(false);
            questionPanel.SetActive(true);
            showingDialogue = false;
        }
        question.text = q;
        for(int i = 0; i < numAnswers; i++){
            string answer = text.Dequeue();
            //If the answer starts with * then it is the right answer
            if(answer[0] == '*'){
                rightAnswer = i;
                answer = answer.Substring(1);
            }
            answers[i].GetComponentInChildren<Text>().text = answer;
        }
        //If 2 answers then set the last two button inactive else active
        bool active = true;
        if (numAnswers == 2){
            active = false;
        }
        answers[2].gameObject.SetActive(active);
        answers[3].gameObject.SetActive(active);
    }

    //Called when an answer button is pressed
    public void CheckAnswer(Button btn){
        //Change the colour of the buttons and set them as unclickable
        for(int i = 0; i < 4; i++){
            Color colour = Color.red;
            if(rightAnswer == i){
                colour = new Color32(0, 142, 14, 255);
            }
            answers[i].GetComponent<Image>().color = colour;
            answers[i].interactable = false;
        }
        //The player got the right answer if the name of the clicked button is the same as rightAnswer
        gotRight = System.Convert.ToInt16(btn.name) == rightAnswer;
        nextButton.SetActive(true);
    }

    //Called when nextButton is pressed
    public void Next(){
        nextButton.SetActive(false);
        foreach(Button button in answers){
            button.GetComponent<Image>().color = Color.black;
            button.interactable = true;
        }
        ChangeSentence();
    }

    //Create and output a bullet point list
    private void BulletList(){
        string list = text.Dequeue();
        string line = text.Dequeue();
        while (line != "</l>"){
            list += "\n•   " + line;
            line = text.Dequeue();
        }
        //If there is no text left then change the text of the button to go home
        if (text.Count == 0){
            buttonText.text = "Go home";
        }
        sentence.text = list;
    }
}
