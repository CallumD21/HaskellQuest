using UnityEngine;
using System.Collections.Generic;
using System.IO;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class QuizManager : MonoBehaviour {

    //The prefab of the light
    [SerializeField] GameObject m_light;
    //The text box to put the question in
    [SerializeField] Text questionText;
    //The Inutfield that the answer is typed into
    [SerializeField] InputField input;
    //The sprite of the green light
    [SerializeField] Sprite greenLight;
    //The text box containing the number of lives left
    [SerializeField] Text livesText;
    //The rewards panel
    [SerializeField] GameObject rewards;
    //The quiz panel
    [SerializeField] GameObject quizPanel;
    //The stages completed text box
    [SerializeField] Text stagessCompleted;
    //The current money text box
    [SerializeField] Text money;
    //The money earned text box
    [SerializeField] Text moneyEarned;
    //Animation controller
    [SerializeField] Animator animator;
    //The hints script
    [SerializeField] Hints hints;
    //Queue of the questions in this quiz
    Queue<Question> questions = new Queue<Question>();
    //The number of stages in the quiz
    private int numStages = 0;
    //The list of lights on the screen
    private List<Image> images = new List<Image>();
    //The current stage the user is on
    private int currentStage = 0;
    //The number of remaining lives
    private int lives;
    //The money per completed stage
    private int stageBonus;
    //The money for getting all questions right
    private int completedBonus;
    private Question currentQuestion;
    private int currentMoney;
    //The initial stage the quiz starts on
    private int startStage;
    private GameManager gameManager;

    private void Start(){
        gameManager = FindObjectOfType<GameManager>();
        currentMoney = gameManager.GetMoney();
        UpdateMoney(0);
        Queue<string> question = new Queue<string>();
        //Read in the quizzes text
        string fileName = gameManager.GetQuiz().ToString();
        startStage = gameManager.GetCurrentStage();
        StreamReader reader = File.OpenText(Application.dataPath + "/StreamingAssets/Quizzes/" + fileName + ".txt");
        string line = reader.ReadLine();
        bool firstLine = true;
        while (line != null){
            //The first line (if it exists) is the number of lives for this quiz,money per question,money per stage,total bonus for completing quiz
            if (firstLine){
                string[] list = line.Split(',');
                lives = int.Parse(list[0]);
                livesText.text = list[0];
                stageBonus = int.Parse(list[1]);
                completedBonus = int.Parse(list[2]);
                firstLine = false;
                line = reader.ReadLine();
            }
            else{
                ReplaceNewLine(ref line);
                if (line == "<s>"){
                    numStages++;
                    line = reader.ReadLine();
                }
                else if (line == "</h>"){
                    //If next line is </s> then this questions is the last question of this stage
                    line = reader.ReadLine();
                    bool lastQuestion = false;
                    if(line == "</s>"){
                        lastQuestion = true;
                        line = reader.ReadLine();
                    }
                    questions.Enqueue(new Question(question,questionText,input, lastQuestion, FindObjectOfType<Evaluation>(),Application.dataPath));
                }
                else{
                    question.Enqueue(line);
                    line = reader.ReadLine();
                }
            }
        }
        reader.Close();
        //Create the lights
        CreateLights();
        //Skip to the correct stage
        while(currentStage < startStage){
            currentQuestion = questions.Dequeue();
            if (currentQuestion.LastQuestion()){
                images[currentStage].sprite = greenLight;
                currentStage++;
            }
        }
        //Ask the first question
        AskQuestion();
    }

    //When a line is read from a file the \n is read as a charcter and not as a new line 
    //To fix this we have to remove them and put them nack in again
    private void ReplaceNewLine(ref string line){
        string[] splitLine = line.Split(new string[] { "\\n" }, System.StringSplitOptions.RemoveEmptyEntries);
        line = "";
        for (int i = 0; i < splitLine.Length - 1; i++){
            line += splitLine[i] + "\n";
        }
        //Add the final line
        line += splitLine[splitLine.Length - 1];
    }

    private void CreateLights(){
        //Width of the canvas
        float width = GetComponent<RectTransform>().rect.width;
        //Evenly split the canvas into numStages + 1 pieces 
        float evenlySpaced = width / (numStages + 1);
        //The x coordinated of the left hand side of the canvas
        float xpos = -width / 2; 
        for (int i = 0; i < numStages; i++){
            //Instantiate the light prefab
            GameObject obj = Instantiate(m_light, quizPanel.transform);
            obj.transform.SetAsFirstSibling();
            xpos += evenlySpaced;
            Vector3 pos = new Vector3(xpos, -300, 0);
            obj.transform.localPosition = pos;

            //Extract the image object from the object we have created
            Image image = obj.GetComponent<Image>();
            images.Add(image);
        }
    }

    //Ask the next question from the quiz
    private void AskQuestion(){
        currentQuestion = questions.Dequeue();
        hints.ChangeQuestion(currentQuestion.GetHints());
        currentQuestion.DisplayQuestion();
    }

    //Called when the submit button is pressed
    public void Submit(){
        bool comletedQuestion = currentQuestion.Correct();
        if (comletedQuestion)
        {
            input.text = "";
            animator.SetTrigger("Correct");
            UpdateQuestion();
        }
        else{
            animator.SetTrigger("Incorrect");
            lives--;
            if (lives < 0){
                QuizCompleted(0);
            }
            else{
                livesText.text = lives.ToString();
            }
        }
    }

    public void UpdateMoney(int m){
        currentMoney += m;
        money.text = "£" + currentMoney.ToString();
    }

    public int GetMoney(){
        return currentMoney;
    }

    private void UpdateQuestion(){
        //If the current question is the last of its stage then set the light to green and move to the next stage
        if (currentQuestion.LastQuestion()){
            images[currentStage].sprite = greenLight;
            UpdateMoney(stageBonus);
            currentStage++;
            if (currentStage == numStages){
                UpdateMoney(completedBonus);
                QuizCompleted(1);
            }
            else{
                //Ask the next question
                AskQuestion();
            }
        }
        else{
            //Ask the next question
            AskQuestion();
        }
    }

    //Pass is 1 if the player got all qustions right and 0 otherwise
    private void QuizCompleted(int pass){
        rewards.SetActive(true);
        quizPanel.SetActive(false);
        string line = new string('-', 5);
        FindObjectOfType<Evaluation>().AddAnswer("\n" + line + "EXIT" + line);
        int numStages = currentStage - startStage;
        stagessCompleted.text = numStages.ToString();
        int totalMoney =  numStages * stageBonus + pass * completedBonus;
        moneyEarned.text = "£" + totalMoney.ToString();
        gameManager.SetTime(2);
        gameManager.SetMoney(currentMoney);
        FindObjectOfType<Evaluation>().AddContent("Q(" + gameManager.GetQuiz() + "," + currentStage + ")");
        if (pass == 1){
            gameManager.UpdateQuiz();
        }
        else{
            gameManager.SetCurrentStage(currentStage);
        }
    }

    public void Exit(){
        SceneManager.LoadScene(Scenes.PC);
    }
}
