using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {

    private int day;
    private readonly string[] days = { "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday", "Sunday" };
    private int time;
    private readonly string[] times = { "07:30", "17:00", "23:00" };
    private int schoolDay;
    //The amount of money the user has
    private int money;
    //The education level
    private int educationLevel;
    //The progress in the education level
    private int educationProgress;
    //True if the dialogue has been seen for this time
    private bool dialoguePlayed = false;
    //The unique identifier of the current quiz
    private int currentQuiz;
    //The number of stages completed in the current Quiz
    private int currentStage;
    //True if space invaders has been bought
    private bool unlockedSpaceInvaders;
    //True if the new bed has been bought
    private bool unlockedBed;
    private Evaluation evaluation;
    //The integer identifying the final school day
    private readonly int finalSchoolDay = 13;

    private void Start(){
        Load();
        DontDestroyOnLoad(this);
        SceneManager.LoadScene(Scenes.room);
        evaluation = FindObjectOfType<Evaluation>();
    }

    private void OnApplicationQuit(){
        Save();
    }

    //Return the current date
    public string GetDate(){
        string date = "Day " + day + ": " + days[(day-1)%7] + " " + times[time];
        return date;
    }

    public void ChangeDay(){
        day++;
        evaluation.AddDay(day);
        SetTime(0);
        SaveDay();
    }

    public void SetTime(int t){
        dialoguePlayed = false;
        time = t;
    }

    public int GetTime(){
        return time;
    }

    public void ChangeSchoolDay(){
        evaluation.AddContent("S" + schoolDay.ToString());
        schoolDay++;
    }

    public int GetSchoolDay(){
        return schoolDay;
    }

    public int GetMoney(){
        return money;
    }

    public void SetMoney(int m){
        money = m;
    }

    public int GetEducationLevel(){
        return educationLevel;
    }

    public int GetEducationPorgress(){
        return educationProgress;
    }

    public void SetCurrentStage(int s){
        currentStage = s;
    }

    public int GetCurrentStage(){
        return currentStage;
    }

    public void UpdateEducation(int progress){
        educationProgress += progress;
        //While the progress on the level is higher than the total progress for that level keep leveling up
        while (educationProgress >= (educationLevel + 1) * 10){
            educationProgress = educationProgress - (educationLevel + 1) * 10;
            educationLevel++;
        }
    }

    public int GetDay(){
        return day;
    }

    public bool CompletedSchool(){
        return schoolDay>finalSchoolDay;
    }

    public int GetQuiz(){
        return currentQuiz;
    }

    public void UpdateQuiz(){
        currentQuiz++;
        currentStage = 0;
    }

    public void DialogueCompleted(){
        dialoguePlayed = true;
    }
    
    public bool IsDialogueFinished(){
        return dialoguePlayed;
    }

    public void UnlockSpaceInvaders(){
        unlockedSpaceInvaders = true;
    }

    public bool UnlockedSpaceInvaders(){
        return unlockedSpaceInvaders;
    }

    public bool UnlockedBed(){
        return unlockedBed;
    }

    public void UnlockBed(){
        unlockedBed = true;
    }

    //Restart from the given day
    public void RestartFromDay(int i){
        LoadDay(i);
        evaluation.AddDay(i);
        dialoguePlayed = false;
        Time.timeScale = 1;
        SceneManager.LoadScene(Scenes.room);
    }

    private void Load(){
        //If the int doesn't exist then playerprefs returns 0.
        //So if 0 is returned set the variables to their default values
        bool defaultValues = PlayerPrefs.GetInt("Load") == 0;
        day = defaultValues ? 1 : PlayerPrefs.GetInt("day");
        time = PlayerPrefs.GetInt("time");
        schoolDay = defaultValues ? 1 : PlayerPrefs.GetInt("schoolDay");
        money = defaultValues ? 110 : PlayerPrefs.GetInt("money");
        educationLevel = PlayerPrefs.GetInt("educationLevel");
        educationProgress = PlayerPrefs.GetInt("educationProgress");
        currentQuiz = PlayerPrefs.GetInt("currentQuiz");
        currentStage = PlayerPrefs.GetInt("currentStage");
        //PlayerPrefs cannot save booleans so instead save an int where 0 is false and 1 is true
        unlockedSpaceInvaders = PlayerPrefs.GetInt("unlockedSpaceInvaders") == 1;
        unlockedBed = PlayerPrefs.GetInt("unlockedBed") == 1;
    }

    private void Save(){
        PlayerPrefs.SetInt("Load", 1);
        PlayerPrefs.SetInt("day", day);
        PlayerPrefs.SetInt("time", time);
        PlayerPrefs.SetInt("schoolDay", schoolDay);
        PlayerPrefs.SetInt("money", money);
        PlayerPrefs.SetInt("educationLevel", educationLevel);
        PlayerPrefs.SetInt("educationProgress", educationProgress);
        PlayerPrefs.SetInt("currentQuiz", currentQuiz);
        PlayerPrefs.SetInt("currentStage", currentStage);
        PlayerPrefs.SetInt("unlockedSpaceInvaders", unlockedSpaceInvaders ? 1 : 0);
        PlayerPrefs.SetInt("unlockedBed", unlockedBed ? 1 : 0);
        PlayerPrefs.Save();
    }

    //Load the ith day
    private void LoadDay(int i){
        if(i == 1){
            PlayerPrefs.DeleteAll();
            Load();
        }
        else{
            string dayPrefix = "day" + i + "_";
            day = PlayerPrefs.GetInt(dayPrefix + "day");
            time = PlayerPrefs.GetInt(dayPrefix + "time");
            schoolDay = PlayerPrefs.GetInt(dayPrefix + "schoolDay");
            money = PlayerPrefs.GetInt(dayPrefix + "money");
            educationLevel = PlayerPrefs.GetInt(dayPrefix + "educationLevel");
            educationProgress = PlayerPrefs.GetInt(dayPrefix + "educationProgress");
            currentQuiz = PlayerPrefs.GetInt(dayPrefix + "currentQuiz");
            currentStage = PlayerPrefs.GetInt(dayPrefix + "currentStage");
            //PlayerPrefs cannot save booleans so instead save an int where 0 is false and 1 is true
            unlockedSpaceInvaders = PlayerPrefs.GetInt(dayPrefix + "unlockedSpaceInvaders") == 1;
            unlockedBed = PlayerPrefs.GetInt(dayPrefix + "unlockedBed") == 1;
        }
    }

    //Called at the beginning of every day
    private void SaveDay(){
        string dayPrefix = "day" + day.ToString() + "_";
        PlayerPrefs.SetInt(dayPrefix + "day", day);
        PlayerPrefs.SetInt(dayPrefix + "time", time);
        PlayerPrefs.SetInt(dayPrefix + "schoolDay", schoolDay);
        PlayerPrefs.SetInt(dayPrefix + "money", money);
        PlayerPrefs.SetInt(dayPrefix + "educationLevel", educationLevel);
        PlayerPrefs.SetInt(dayPrefix + "educationProgress", educationProgress);
        PlayerPrefs.SetInt(dayPrefix + "currentQuiz", currentQuiz);
        PlayerPrefs.SetInt(dayPrefix + "currentStage", currentStage);
        PlayerPrefs.SetInt(dayPrefix + "unlockedSpaceInvaders", unlockedSpaceInvaders ? 1 : 0);
        PlayerPrefs.SetInt(dayPrefix + "unlockedBed", unlockedBed ? 1 : 0);
        PlayerPrefs.Save();
    }
}