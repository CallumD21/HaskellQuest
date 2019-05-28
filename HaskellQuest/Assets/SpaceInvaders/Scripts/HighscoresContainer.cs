using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.IO;

namespace SpaceInvaders{
    public class HighscoresContainer : MonoBehaviour{

        //The scores list is ordered and the score in position i belongs to the name in position i
        private List<string> names = new List<string>();
        private List<int> scores = new List<int>();

        //Is called in the preload scene
        private void Start(){
            //The object that this script belongs to needs to last the entire life of the game
            DontDestroyOnLoad(this);
            //Load Menu
            SceneManager.LoadScene(Scenes.spaceInvaders_mainMenu);
        }

        private void OnEnable(){
            //Read the highscores from a CSV file
            StreamReader reader = File.OpenText(Application.dataPath + "/StreamingAssets/SpaceInvaders/HighscoresData.csv");
            //Read a line of data
            string line = reader.ReadLine();
            while (line != null){
                //Split the only line on ","
                string[] data = line.Split(","[0]);
                //Load data into the two arrays
                names.Add(data[0]);
                scores.Add(int.Parse(data[1]));
                line = reader.ReadLine();
            }
            reader.Close();
        }

        private void OnDisable(){
            //Save the highscores to a CSV file
            StreamWriter writer = File.CreateText(Application.dataPath + "/StreamingAssets/SpaceInvaders/HighscoresData.csv");
            //A line to be saved to the file
            string save = string.Empty;
            for (int i = 0; i < names.Count; i++){
                save = names[i] + "," + scores[i];
                writer.WriteLine(save);
            }
            writer.Close();
        }

        public int GetNumberOfHighScores(){
            return scores.Count;
        }

        //Return the lowest highscore, if there are no highscores return -1
        public int GetLowestScore(){
            if (scores.Count == 0){
                return -1;
            }
            else{
                return scores[scores.Count - 1];
            }
        }

        public List<string> GetNames(){
            return names;
        }

        public List<int> GetScores(){
            return scores;
        }

        //Delete all of the highscores
        public void ClearAll(){
            names.Clear();
            scores.Clear();
        }

        //Delete the highscore at the given index if a score exitsts at the index
        public bool Delete(int index){
            if (index < names.Count){
                names.RemoveAt(index);
                scores.RemoveAt(index);
            }
            return true;
        }

        //Only called when the score being added is in the top ten scores
        public void AddHighScore(string name, int score){
            //If there are no highscores then just add this one
            if (names.Count == 0){
                names.Add(name);
                scores.Add(score);
            }
            else{
                int i;
                //Find the correct position for the new score to be added
                for (i = 0; i < scores.Count; i++){
                    if (scores[i] < score){
                        names.Insert(i, name);
                        scores.Insert(i, score);
                        break;
                    }
                }
                //If we search the entire list and dont find a place for the score add it at the end
                if (i == scores.Count){
                    names.Add(name);
                    scores.Add(score);
                }
                //If there are now 11 scores delete the lowest
                if (scores.Count == 11){
                    names.RemoveAt(10);
                    scores.RemoveAt(10);
                }
            }
        }
    }
}