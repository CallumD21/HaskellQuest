using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace SpaceInvaders{
    public class GameOver : MonoBehaviour{

        //The player has pressed to replay the game
        public void Yes(){
            //Update load as the game has now ended so nothing is needed to be loaded next time the game is ran
            PlayerPrefs.SetInt("SI_Load", 0);
            PlayerPrefs.Save();
            //Reload the game
            SceneManager.LoadScene(Scenes.spaceInvaders_game);
        }

        //The player has pressed to not replay the game
        public void No(){
            //Update load as the game has now ended so nothing is needed to be loaded next time the game is ran
            PlayerPrefs.SetInt("SI_Load", 0);
            PlayerPrefs.Save();
            //Open the menu
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