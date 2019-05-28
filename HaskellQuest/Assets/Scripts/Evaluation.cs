using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Evaluation : MonoBehaviour{ 

    private float playTime = 0;
    private string days = "1";
    private string content = "";
    private string answers = new string('-', 14);

    private void Start(){
        DontDestroyOnLoad(this);
        Load();
    }

    private void OnApplicationQuit(){
        if(SceneManager.GetActiveScene().buildIndex == Scenes.quiz){
            string line = new string('-', 5);
            FindObjectOfType<Evaluation>().AddAnswer("\n" + line + "QUIT" + line);
        }
        Save();
    }

    private void Save(){
        StreamWriter writer = File.CreateText(Application.dataPath + "/StreamingAssets/Evaluation/Information.txt");
        float time = Time.time / 60;
        time += playTime;
        writer.WriteLine("Time played (mins):" + time.ToString());
        writer.WriteLine("Days:" + days);
        writer.WriteLine("Content:" + content);
        //Break the answers into its lines
        string[] listOfAnswers = answers.Split('\n');
        //Write a line at a time
        for(int i=0; i < listOfAnswers.Length; i++){
            writer.WriteLine(listOfAnswers[i]);
        }
        writer.Close();
    }

    private void Load(){
        StreamReader reader = File.OpenText(Application.dataPath + "/StreamingAssets/Evaluation/Information.txt");
        string line = reader.ReadLine();
        int numLines = 1;
        while (line != null){
            switch (numLines){
                case 1:
                    //The first line is the time played
                    playTime = float.Parse(line.Substring(19));
                    break;
                case 2:
                    //The second line is the days
                    days = line.Substring(5);
                    break;
                case 3:
                    //The third line is the content
                    content = line.Substring(8);
                    break;
                default:
                    answers += "\n" + reader.ReadToEnd();
                    break;

            }
            line = reader.ReadLine();
            numLines++;
        }
        if (playTime == 0){
            PlayerPrefs.DeleteAll();
        }
    }

    public void AddDay(int day){
        days += "," + day.ToString();
    }

    public void AddContent(string info){
        if (content != ""){
            content += "," + info;
        }
        else{
            content = info;
        }
    }

    public void AddAnswer(string answer){
        answers += answer;
    }
}
