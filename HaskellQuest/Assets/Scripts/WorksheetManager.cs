using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class WorksheetManager : MonoBehaviour {

    //The text of the total number of exercises
    [SerializeField] Text totalExercises;
    //The text of the completed number of exercises
    [SerializeField] Text completedExercises;
    //The text box where the content of the worksheet goes
    [SerializeField] TextBox content;
    //The text box where the output of the worksheet goes
    [SerializeField] TextBox output;
    //The next question, exit worksheet and run buttons
    [SerializeField] Button next;
    [SerializeField] Button exit;
    [SerializeField] Button run;
    //The prefab of the dropdown
    [SerializeField] GameObject dropdown;
    //The prefab of the inputfield
    [SerializeField] GameObject inputField;
    //The total number of exercises
    private int numExercies = 0;
    //The number of completed exercises
    private int completed = 0;
    //A queue of all of the exercises in the sheet
    private Queue<Exercise> exercises = new Queue<Exercise>();
    //The current exercise
    private Exercise currentExercise;
    private int educationReward;
    
    private void Start(){
        //Contains each line of the exercise being read
        Queue<string> exercise = new Queue<string>();
        //The worksheet is named after the school day it is associated too
        //As this school day has just been finished we -1.
        string fileName = (FindObjectOfType<GameManager>().GetSchoolDay() - 1).ToString();
        StreamReader reader = File.OpenText(Application.dataPath + "/StreamingAssets/Worksheet/" + fileName + ".txt");
        string line = reader.ReadLine();
        int numInputFields = 0;
        bool firstLine = true;
        while (line != null){
            if (firstLine){
                firstLine = false;
                educationReward = int.Parse(line);
            }
            else{
                if (line.Contains("<if>")){
                    numInputFields++;
                }
                if (line == "<mc>" || line == "<h>"){
                    numExercies++;
                }
                else if (line == "</mc>"){
                    //Instantiate the dropdown prefab
                    GameObject obj = Instantiate(dropdown, content.text.transform);
                    exercises.Enqueue(new MultipleChoice(exercise, content, output, obj, this));
                }
                else if (line == "</h>"){
                    //Instantiate the inputfield prefab
                    List<GameObject> objects = new List<GameObject>();
                    for (int i = 0; i < numInputFields; i++){
                        objects.Add(Instantiate(inputField, content.text.transform));
                    }
                    numInputFields = 0;
                    exercises.Enqueue(new HaskellQuestion(exercise, content, output, objects, this, Application.dataPath));
                }
                else{
                    exercise.Enqueue(line);
                }
            }
            line = reader.ReadLine();
        }
        reader.Close();
        totalExercises.text = numExercies.ToString();
        //Display the first exercise
        currentExercise = exercises.Dequeue();
        currentExercise.Display();
        RunButton();
    }

    private void RunButton(){
        //If the current exercise is a haskell question then display the run button
        if (currentExercise.Type() == 1){
            run.gameObject.SetActive(true);
        }
        else{
            run.gameObject.SetActive(false);
        }
    }

    //Called when the current exercise has been completed
    public void ChangeExercise(){
        //Change Exercise
        completed++;
        completedExercises.text = completed.ToString();
        if (exercises.Count != 0){
            next.interactable = true;
        }
        else{
            exit.interactable = true;
        }
    }

    public void Next(){
        currentExercise.Destroy();
        currentExercise = exercises.Dequeue();
        currentExercise.Display();
        RunButton();
        next.interactable = false;
        if (exercises.Count == 0){
            next.gameObject.SetActive(false);
            exit.gameObject.SetActive(true);
        }
    }

    public void Exit(){
        GameManager gm = FindObjectOfType<GameManager>();
        gm.SetTime(2);
        gm.UpdateEducation(educationReward);
        FindObjectOfType<Evaluation>().AddContent("W");
        SceneManager.LoadScene(Scenes.PC);
    }

    public void Run(){
        currentExercise.Completed();
    }
}
