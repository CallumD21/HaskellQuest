using UnityEngine;
using System.IO;

namespace SpaceInvaders{
    public class BonusInfo : MonoBehaviour{

        //The index of the current ship in unlocked
        private int currentShip;
        //One entry per ship, true if the ship is unlocked
        [SerializeField] bool[] unlocked;

        private void Start(){
            //Load the current ship
            currentShip = PlayerPrefs.GetInt("SI_currentShip");
            //Load unlocked info from a CSV
            StreamReader reader = File.OpenText(Application.dataPath + "/StreamingAssets/SpaceInvaders/ShipInfo.csv");
            string line = reader.ReadLine();
            int index = 0;
            while (line != null){
                unlocked[index] = bool.Parse(line);
                index++;
                line = reader.ReadLine();
            }
            reader.Close();
        }

        private void OnDisable(){
            //Save the current ship
            PlayerPrefs.SetInt("SI_currentShip", currentShip);
            //Save the unlocked info to a CSV
            StreamWriter writer = File.CreateText(Application.dataPath + "/StreamingAssets/SpaceInvaders/ShipInfo.csv");
            for (int i = 0; i < unlocked.Length; i++){
                writer.WriteLine(unlocked[i]);
            }
            writer.Close();
        }

        public void SetCurrentShip(int ship){
            currentShip = ship;
        }

        public int GetCurrentShip(){
            return currentShip;
        }

        //Return whether the given ship is unlocked or not
        public bool GetUnlocked(int index){
            return unlocked[index];
        }

        public void SetUnlocked(int index, bool val){
            unlocked[index] = val;
        }

        public bool UnlockShip2(int score){
            //If the 2nd ship is not currently unlocked and its criteria is met (score > 100) then unlock it and return true
            //Else return false
            if (!unlocked[1]){
                if (score > 100){
                    unlocked[1] = true;
                    return true;
                }
            }
            return false;
        }

        public bool UnlockShip3(int score){
            //If the 3rd ship is not currently unlocked and its criteria is met (score > 1000) then unlock it and return true
            //Else return false
            if (!unlocked[2]){
                if (score > 1000){
                    unlocked[2] = true;
                    return true;
                }
            }
            return false;
        }

        public bool UnlockShip4(int score){
            //If the 4th ship is not currently unlocked and its criteria is met (score > 4000) then unlock it and return true
            //Else return false
            if (!unlocked[3]){
                if (score > 4000){
                    unlocked[3] = true;
                    return true;
                }
            }
            return false;
        }
    }
}