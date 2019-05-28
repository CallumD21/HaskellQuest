using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityStandardAssets.Characters.FirstPerson;

public class DialogueManager : MonoBehaviour {

    [SerializeField] GameObject dialoguePanel;
    [SerializeField] Text dialogueText;
    [SerializeField] Text buttonText;
    private Queue<string> dialogue;
    private FirstPersonController personController;
    private GameManager gameManager;

    private void Start(){
        personController = FindObjectOfType<FirstPersonController>();
        gameManager = FindObjectOfType<GameManager>();
        string day = "Day" + gameManager.GetDay().ToString();
        string time = "Time" + gameManager.GetTime().ToString();
        string filePath = Application.dataPath + "/StreamingAssets/Dialogue/" + day + time + ".txt";
        if (File.Exists(filePath) && !gameManager.IsDialogueFinished()){
            StartDialogue(filePath);
        }
    }

    public void StartDialogue(string filePath){
        dialogue = new Queue<string>();
        StreamReader reader = File.OpenText(filePath);
        string line = reader.ReadLine();
        while (line != null){
            dialogue.Enqueue(line);
            line = reader.ReadLine();
        }
        dialoguePanel.SetActive(true);
        //Stop time
        Time.timeScale = 0;
        //Unlock the cursor and stop player rotation
        personController.SetSensitivityAndMouse(0, 0, false);
        DisplayNextLine();
    }

    //Called when next button pressed
    public void DisplayNextLine(){
        if (dialogue.Count == 0){
            dialoguePanel.SetActive(false);
            gameManager.DialogueCompleted();
            //Restart time
            Time.timeScale = 1;
            //Day 1, time 0 should automatically load school
            if (gameManager.GetDay() == 1 && gameManager.GetTime() == 0){
                SceneManager.LoadScene(Scenes.school);
            }
            else{
                //Lock the cursor and restart player rotation
                personController.SetSensitivityAndMouse(2, 2, true);
            }
        }
        else if (dialogue.Count == 1){
            buttonText.text = "OK!";
            dialogueText.text = dialogue.Dequeue();
        }
        else{
            dialogueText.text = dialogue.Dequeue();
        }
    }
}
