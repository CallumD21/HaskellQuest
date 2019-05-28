using UnityEngine;
using UnityEngine.UI;

namespace SpaceInvaders{
    public class AddScore : MonoBehaviour{

        //The text for the enter and cancel button
        [SerializeField] Text enter;
        [SerializeField] Text cancel;
        [SerializeField] InputField input;
        [SerializeField] GameObject errorMessage;
        //The game object of the game over panel
        [SerializeField] GameObject gameOver;
        //The players score in the game
        private int score;

        public void Enter(){
            //If the input is not the required length then show and error
            string text = input.text;
            if (text.Length < 1 || text.Length > 13){
                errorMessage.SetActive(true);
            }
            else{
                FindObjectOfType<HighscoresContainer>().AddHighScore(text, score);
                enter.color = Color.white;
                errorMessage.SetActive(false);
                input.text = string.Empty;
                gameOver.SetActive(true);
                gameObject.SetActive(false);
            }
        }

        public void Cancel(){
            cancel.color = Color.white;
            errorMessage.SetActive(false);
            input.text = string.Empty;
            gameOver.SetActive(true);
            gameObject.SetActive(false);
        }

        public void SetScore(int i){
            score = i;
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