using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace SpaceInvaders{
    public class Save : MonoBehaviour{

        //Called when the home buton is pressed in the game scene
        public void Home(){
            //Pause the game
            Time.timeScale = 0f;
            FindObjectOfType<FlyingSaucer>().gameObject.SetActive(false);
            FindObjectOfType<BlockOfAliens>().SetPaused(true);
            //Display the save panel which this script is attached to
            gameObject.SetActive(true);
        }

        //Called when the user presses the yes button on the save screen
        public void Yes(){
            //Save the progress
            FindObjectOfType<GameManager>().Save();
            //Set time back to normal
            Time.timeScale = 1f;
            //Load the menu
            SceneManager.LoadScene(Scenes.spaceInvaders_mainMenu);
        }

        //Called when the user presses the no button on the save screen
        public void No(){
            //Update the load int as there is now no need to load when the game starts
            PlayerPrefs.SetInt("SI_Load", 0);
            PlayerPrefs.Save();
            //Set time back to normal
            Time.timeScale = 1f;
            //Load the menu
            SceneManager.LoadScene(Scenes.spaceInvaders_mainMenu);
        }

        //When the mouse is over the button change the colour of the buttons text to black
        public void OnMouseEnterButton(Text buttonText){
            buttonText.color = Color.black;
        }

        //When the mouse is no longer over the button change the colour of the buttons text to white
        public void OnMouseExitButton(Text buttonText){
            buttonText.color = Color.white;
        }
    }
}