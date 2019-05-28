using UnityEngine;
using UnityEngine.UI;

namespace SpaceInvaders{
    public class Bonuses : MonoBehaviour{

        //List of sprites of the playable ships
        [SerializeField] Sprite[] ships;
        //List of the buttons to change the ships
        [SerializeField] Button[] buttons;
        //The game object of the confirmation panel
        [SerializeField] GameObject confirmation;
        //The image where the sprite of the current sprite is shown in the confirmation panel
        [SerializeField] Image activeShip;
        //The sprite of an unlocked button
        [SerializeField] Sprite unlockedButton;

        //True if the confirmation panel is being shown
        private bool showingPanel = false;
        //Time how long the confirmation panel is displayed
        private float timer = 0;
        //The information of which ships are unlocked
        private BonusInfo bonusInfo;

        private void Start(){
            bonusInfo = FindObjectOfType<BonusInfo>();
            //Loop throught the buttons
            for (int i = 0; i < buttons.Length; i++){
                //If the ship is unlocked then the button should be interactable
                //If the ship is locked it shouldnt be interactable
                buttons[i].interactable = bonusInfo.GetUnlocked(i);
                //If the ship is unlocked then unlock its button
                if (bonusInfo.GetUnlocked(i)){
                    UnlockButton(i);
                }
            }
        }

        private void Update(){
            //If the panel is showing and 2 seconds has passed then stop showing the panel, reset the timer 
            //and make the (unlocked) buttons interactable again.
            if (showingPanel){
                timer += Time.deltaTime;
                if (timer > 2){
                    confirmation.SetActive(false);
                    showingPanel = false;
                    timer = 0;
                    SetButtonsClickable();
                }
            }
        }

        //Called when the button for the 1st ship is pressed
        public void Ship1(){
            //Set the current ship to the 1st ship
            bonusInfo.SetCurrentShip(0);
            //Display the confirmation panel
            confirmation.SetActive(true);
            showingPanel = true;
            //Set the sprite of the active ship to be the sprite of this (1st) ship
            activeShip.sprite = ships[0];
            //Change the shape of the image to be the shape of the sprite of this ship
            activeShip.rectTransform.sizeDelta = new Vector2(57, 68);
            //Set all the buttons to be not interactable
            SetButtonsUnclickable();
        }

        //Ship2, Ship3 and Ship4 are analogous to Ship1
        public void Ship2(){
            bonusInfo.SetCurrentShip(1);
            confirmation.SetActive(true);
            showingPanel = true;
            activeShip.sprite = ships[1];
            activeShip.rectTransform.sizeDelta = new Vector2(65, 40);
            bonusInfo.SetUnlocked(1, true);
            SetButtonsUnclickable();
        }

        public void Ship3(){
            bonusInfo.SetCurrentShip(2);
            confirmation.SetActive(true);
            showingPanel = true;
            activeShip.sprite = ships[2];
            activeShip.rectTransform.sizeDelta = new Vector2(65, 65);
            SetButtonsUnclickable();
        }

        public void Ship4(){
            bonusInfo.SetCurrentShip(3);
            confirmation.SetActive(true);
            showingPanel = true;
            activeShip.sprite = ships[3];
            activeShip.rectTransform.sizeDelta = new Vector2(70, 38);
            SetButtonsUnclickable();
        }

        //When the confirmation panel is being displayed the buttons should not be interactable
        private void SetButtonsUnclickable(){
            //Loop through the buttons setting the interactable ones to not interactable
            for (int i = 0; i < buttons.Length; i++){
                if (bonusInfo.GetUnlocked(i)){
                    buttons[i].interactable = false;
                }
            }
        }

        //When the confrimation panel closes the buttons corresponding to unlocked ships are set interactable
        private void SetButtonsClickable(){
            for (int i = 0; i < buttons.Length; i++){
                if (bonusInfo.GetUnlocked(i)){
                    buttons[i].interactable = true;
                }
            }
        }

        //Change the image of the button to unlocked
        private void UnlockButton(int index){
            buttons[index].GetComponent<Image>().sprite = unlockedButton;
        }
    }
}