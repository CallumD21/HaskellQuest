using UnityEngine;
using UnityEngine.SceneManagement;

namespace SpaceInvaders{
    public class HomeButton : MonoBehaviour{

        //When the home button is pressed open the menu
        public void Home(){
            SceneManager.LoadScene(Scenes.spaceInvaders_mainMenu);
        }
    }
}