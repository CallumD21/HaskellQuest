using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using UnityEngine.UI;

public class Question{
    private string questionText = "";
    private List<string> answer = new List<string>();
    //Hints for this question
    private List<string> hints = new List<string>();
    private Text output;
    private InputField input;
    //True if this question is the last of its stage
    private bool lastQuestion;
    private string inputtedText;
    private Evaluation evaluation;
    private string dashedLine = "\n" + new string('-', 14);
    //The location of the data for our game
    private string applicationPath;

    public Question(Queue<string> question, Text o, InputField i, bool lp, Evaluation eval, string app){
        applicationPath = app;
        evaluation = eval;
        output = o;
        input = i;
        lastQuestion = lp;
        string line = question.Dequeue();
        while (line != "<a>"){
            questionText += line + " ";
            line = question.Dequeue();
        }
        line = question.Dequeue();
        while(line != "</a>"){
            answer.Add(line);
            line = question.Dequeue();
        }
        //Remove <h> from the queue
        question.Dequeue();
        while (question.Count != 0){
            line = question.Dequeue();
            hints.Add(line);
        }
    }

    public void DisplayQuestion(){
        output.text = questionText;
    }

    //Return true if the inputted code is correct and false otherwise
    public bool Correct(){
        inputtedText = input.text;
        string noSpaces = "";
        //Remove all spaces
        noSpaces = Regex.Replace(inputtedText, @"\s+", string.Empty);
        //Only run if there is something to run
        if (noSpaces.Equals("")){
            evaluation.AddAnswer("\n" + "EMPTY" + dashedLine);
            return false;
        }
        return Run();
    }

    private bool Run(){
        Save();
        //Open the command prompt compile and execute the test file
        Process process = new Process();
        ProcessStartInfo startInfo = new ProcessStartInfo("cmd.exe"){
            WindowStyle = ProcessWindowStyle.Normal,
            Arguments = "/C cd " + applicationPath + "/StreamingAssets/Haskell" + " && ghc main.hs && main.exe",
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            CreateNoWindow = true
        };
        process.StartInfo = startInfo;
        process.Start();
        if (process.WaitForExit(10000)){
            return ParseOutput(process);
        }
        else{
            Process[] processes = Process.GetProcessesByName("main");
            foreach (Process p in processes){
                p.CloseMainWindow();
            }
            process.CloseMainWindow();
            evaluation.AddAnswer("\n" + "TIMEOUT" + "\n" + inputtedText + dashedLine);
            return false;
        }
    }

    private void Save(){
        List<string> data = new List<string>{
            "import Test.QuickCheck",
            "import ConvertMessage",
        };
        for (int i = 0; i < answer.Count; i++){
            if (answer[i] == "<i>"){
                data.Add(inputtedText);
            }
            else{
                data.Add(answer[i]);
            }
        }
        data.Add("main ::IO()");
        data.Add("main = do");
        data.Add("result <- quickCheckResult prop_test");
        data.Add("putStrLn (convertMessage result)");
        File.WriteAllLines(applicationPath + "/StreamingAssets/Haskell/main.hs", data.ToArray());
    }

    //Parse the given output from the process
    private bool ParseOutput(Process process){
        //If there has been an error return false
        if (!process.StandardError.EndOfStream){
            evaluation.AddAnswer("\n" + "ERROR" + "\n" + inputtedText + dashedLine);
            return false;
        }
        else{
            //Read in the output only keeping the final line
            string line = "";
            while (!process.StandardOutput.EndOfStream){
                line = process.StandardOutput.ReadLine();
            }
            if (line == "+++ OK, passed 100 tests"){
                evaluation.AddAnswer("\n" + "CORRECT" + "\n" + inputtedText + dashedLine);
                return true;
            }
            else{
                evaluation.AddAnswer("\n" + "INCORRECT" + "\n" + inputtedText + dashedLine);
                return false;
            }
        }
    }

    //Returns true if this question is the last question of its stage
    public bool LastQuestion(){
        return lastQuestion;
    }

    //Returns the hints for this question
    public List<string> GetHints(){
        return hints;
    }
}
