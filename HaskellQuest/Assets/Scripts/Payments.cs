using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityStandardAssets.Characters.FirstPerson;

public class Payments : MonoBehaviour {

    //The panel to be shown if the player does not have enough money
    [SerializeField] GameObject notEnoughMoney;
    //The text for the space invaders information
    [SerializeField] Text spaceInvaders;
    [SerializeField] Text spaceInvadersCost;
    //The space invaders toggle
    [SerializeField] Toggle spaceInvadersToggle;
    //The text for the bed information
    [SerializeField] Text bed;
    [SerializeField] Text bedCost;
    //The bed toggle
    [SerializeField] Toggle bedToggle;
    [SerializeField] Text warning;
    [SerializeField] Text bonus;
    [SerializeField] Dropdown days;
    private FirstPersonController personController;
    private const int dailyCost = 50;
    private GameManager gameManager;

    private void Start(){
        gameManager = FindObjectOfType<GameManager>();
        personController = FindObjectOfType<FirstPersonController>();
        //Stop time
        Time.timeScale = 0;
        //Unlock the cursor and stop player rotation
        personController.SetSensitivityAndMouse(0, 0, false);
        //Bonus payments should not be visible on day 1
        if (gameManager.GetDay() == 1){
            bonus.gameObject.SetActive(false);
            spaceInvaders.gameObject.SetActive(false);
            spaceInvadersCost.gameObject.SetActive(false);
            spaceInvadersToggle.gameObject.SetActive(false);
            bed.gameObject.SetActive(false);
            bedCost.gameObject.SetActive(false);
            bedToggle.gameObject.SetActive(false);
        }
        //If the player has bought space invaders then lock the toggle
        if (gameManager.UnlockedSpaceInvaders()){
            spaceInvadersToggle.interactable = false;
        }
        //If the player has bought a new bed then lock the toggle
        if (gameManager.UnlockedBed()){
            bedToggle.interactable = false;
        }
    }

    public void Submit(){
        CheckMoney(spaceInvadersToggle.isOn, bedToggle.isOn);        
    }
    
    //si and b represent the isOn of the space invaders and bed toggles respectively
    private void CheckMoney(bool si, bool b){
        int totalCost = dailyCost;
        int money = gameManager.GetMoney();
        if (si){
            //Remove £ from the cost
            totalCost += int.Parse(spaceInvadersCost.text.Substring(1));
        }
        if (b){
            totalCost += int.Parse(bedCost.text.Substring(1));
        }
        if (money >= totalCost){
            if (si){
                gameManager.UnlockSpaceInvaders();
            }
            if (b){
                gameManager.UnlockBed();
            }
            gameManager.SetMoney(money - totalCost);
            gameManager.ChangeDay();
            //Restart time
            Time.timeScale = 1;
            SceneManager.LoadScene(Scenes.room);
        }
        else if (money >= dailyCost){
            warning.gameObject.SetActive(true);
        }
        else{
            OpenNotEnoughMoney();
        }
    }

    //Activate the notEnoughMoney panel and set the dropdown choices
    private void OpenNotEnoughMoney(){
        //Create a list of the days the player can restart from
        //From day 1 to todays day
        List<string> options = new List<string>();
        int today = gameManager.GetDay();
        for (int i = 1; i < today; i++){
            options.Add("Day "+i.ToString());
        }
        options.Add("Day " + today.ToString() + " (today)");
        days.ClearOptions();
        days.AddOptions(options);
        notEnoughMoney.SetActive(true);
    }

    public void Restart(){
        gameManager.RestartFromDay(days.value + 1);
    }

    //Called when the toggle changes state. Depending on the state make the text less or more visible
    public void SpaceInvadersToggle(bool isOn){
        if (isOn){
            spaceInvaders.color = new Color32(255, 255, 255, 255);
            spaceInvadersCost.color = new Color32(255, 215, 0, 255);
        }
        else{
            spaceInvaders.color = new Color32(255, 255, 255, 100);
            spaceInvadersCost.color = new Color32(255, 215, 0, 100);
        }
    }

    //Called when the toggle changes state. Depending on the state make the text less or more visible
    public void BedToggle(bool isOn){
        if (isOn){
            bed.color = new Color32(255, 255, 255, 255);
            bedCost.color = new Color32(255, 215, 0, 255);
        }
        else{
            bed.color = new Color32(255, 255, 255, 100);
            bedCost.color = new Color32(255, 215, 0, 100);
        }
    }
}
