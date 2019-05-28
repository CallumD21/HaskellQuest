using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

namespace SpaceInvaders{
    public class Highscores : MonoBehaviour{

        //The text for the buttons of the same name
        [SerializeField] Text delete;
        [SerializeField] Text clearAll;
        [SerializeField] Text yes;
        [SerializeField] Text no;
        [SerializeField] Text enter;
        [SerializeField] Text cancel;
        //The input field where the player gives the index of the highscore they wish to delete
        [SerializeField] InputField input;
        //The text fields where the names and score are written
        [SerializeField] Text[] namesTextBoxes;
        [SerializeField] Text[] scoresTextBoxes;
        //The game object of the panel that checks the player wants to clear all scores
        [SerializeField] GameObject checkPanel;
        //The game object of the panel that gets the index the player wants to delete
        [SerializeField] GameObject deletePanel;
        //The game object for the text error message
        [SerializeField] GameObject errorMessage;
        //True if a popup is showing
        private bool showingPopUp = false;
        //The highscores container that contains all of the current highscores
        private HighscoresContainer highscores;

        //When the scene opens initialize highscores and fill the text boxes with the highscores
        private void OnEnable(){
            highscores = FindObjectOfType<HighscoresContainer>();
            FillTextBoxes();
        }

        //Writes the current highscores to the screen, if there are highscores to write
        private void FillTextBoxes(){
            if (highscores.GetNumberOfHighScores() != 0){
                //Write the highscores into the textboxes
                List<string> names = highscores.GetNames();
                List<int> scores = highscores.GetScores();
                for (int i = 0; i < names.Count; i++){
                    namesTextBoxes[i].text = names[i];
                    //Format the scores
                    scoresTextBoxes[i].text = scores[i].ToString("n0");
                }
            }
        }

        //Empty the contents of each text box
        private void ClearTextBoxes(){
            for (int i = 0; i < 10; i++){
                namesTextBoxes[i].text = string.Empty;
                scoresTextBoxes[i].text = string.Empty;
            }
        }

        //The player can only click on the home button if a popup isnt showing
        public void Home(){
            if (!showingPopUp){
                SceneManager.LoadScene(Scenes.spaceInvaders_mainMenu);
            }
        }

        //The player can only click on the delete button if a popup isnt showing
        public void Delete(){
            if (!showingPopUp){
                //Show the delete panel
                deletePanel.SetActive(true);
                showingPopUp = true;
            }
        }

        //The player can only click on the clear all button if a popup isnt showing
        public void ClearAll(){
            if (!showingPopUp){
                //Show the check panel
                checkPanel.SetActive(true);
                showingPopUp = true;
            }
        }

        //Called when the player presses to clear all of the scores
        public void Yes(){
            //Stop displaying the panel
            checkPanel.SetActive(false);
            showingPopUp = false;
            //Reset the colour of the buttons
            clearAll.color = Color.white;
            yes.color = Color.white;
            //Clear the highscores and the textboxes
            highscores.ClearAll();
            ClearTextBoxes();
        }

        //Called when the player presses to not clear all of the scores
        public void No(){
            //Stop displaying the panel and reset the colour of the buttons
            checkPanel.SetActive(false);
            showingPopUp = false;
            clearAll.color = Color.white;
            no.color = Color.white;
        }

        //Called when the user presses enter to delete a given score in the input field
        public void Enter(){
            //Before converting to an integer we have to check the input isnt empty and isnt too big
            string text = input.text;
            if (text.Length == 0 || text.Length > 2){
                errorMessage.SetActive(true);
            }
            else{
                //Convert to an integer and check the given rank is between 1 and 10
                int rank = int.Parse(text);
                if (rank < 1 || rank > 10){
                    errorMessage.SetActive(true);
                }
                else{
                    //Delete the highscore and refill the text boxes
                    if (highscores.Delete(rank - 1)){
                        ClearTextBoxes();
                        FillTextBoxes();
                    }
                    //Stop displaying the panel, reset the colour of the buttons and reset the data on the panel
                    deletePanel.SetActive(false);
                    showingPopUp = false;
                    delete.color = Color.white;
                    enter.color = Color.white;
                    errorMessage.SetActive(false);
                    input.text = string.Empty;
                }
            }
        }

        //Called when the player presses to not delete a score
        public void Cancel(){
            //Stop displaying the panel, reset the colour of the buttons and reset the data on the panel
            deletePanel.SetActive(false);
            showingPopUp = false;
            delete.color = Color.white;
            cancel.color = Color.white;
            errorMessage.SetActive(false);
            input.text = string.Empty;
        }

        //When the mouse is over the button change the colour of the buttons text to black
        public void OnMouseEnterButton(Text buttonText){
            buttonText.color = Color.black;
        }

        //When the mouse is no longer over the button change the colour of the buttons text to white
        public void OnMouseExitButton(Text buttonText){
            buttonText.color = Color.white;
        }

        //When the mouse is over the button change the colour of the buttons text to cyan provided a popup isnt showing
        public void OnMouseEnterText(Text buttonText){
            if (!showingPopUp){
                buttonText.color = Color.cyan;
            }
        }

        //When the mouse is no longer over the button change the colour of the buttons text to white provided a popup isnt showing 
        public void OnMouseExitText(Text buttonText){
            if (!showingPopUp){
                buttonText.color = Color.white;
            }
        }
    }
}