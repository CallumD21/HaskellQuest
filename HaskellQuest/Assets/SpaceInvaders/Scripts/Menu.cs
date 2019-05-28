using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace SpaceInvaders{
    public class Menu : MonoBehaviour{

        //When the player presses to play the game open the game screen
        public void PlayGame(){
            SceneManager.LoadScene(Scenes.spaceInvaders_game);
        }

        //When the player presses to view the highscores open the highscores screen
        public void Highscores(){
            SceneManager.LoadScene(Scenes.spaceInvaders_highscores);
        }

        public void ExitGame(){
            SceneManager.LoadScene(Scenes.PC);
        }

        //When the player presses to view the bonuses open the bonuses screen
        public void Bonuses(){
            SceneManager.LoadScene(Scenes.spaceInvaders_bonuses);
        }

        //When the mouse is over the button change the colour of the buttons text to cyan
        public void OnMouseEnterButton(Text buttonText){
            buttonText.color = Color.cyan;
        }

        //When the mouse is no longer over the button change the colour of the buttons text to white
        public void OnMouseExitButton(Text buttonText){
            buttonText.color = Color.white;
        }
    }
}