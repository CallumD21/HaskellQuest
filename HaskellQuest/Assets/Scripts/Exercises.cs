using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;

public abstract class Exercise{
    protected TextBox content;
    protected TextBox output;
    //The height of a single line of text
    protected int textHeight = 22;
    protected WorksheetManager worksheetManager;
    protected Queue<string> text = new Queue<string>();

    public abstract void Display();
    public abstract void Completed();
    public abstract void Destroy();
    public abstract int Type();

    //Increases the size of the given text box to fit numLines
    protected void IncreaseTextBoxSize(TextBox textBox, int numLines){
        //Get all of the variables
        Text text = textBox.text;
        int xPos = textBox.xPos;
        int yPos = textBox.yPos;
        float height = textBox.height;
        int maxLines = textBox.maxLines;
        if (numLines > maxLines){
            //The number of lines not being displayed
            int missingLines = numLines - maxLines;
            //The new height is the old height+textHeight*missingLines
            text.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height + textHeight * missingLines);
            //The textBox increase in size from the center so we need to move it down by half of the increase in size
            //i.e (textHeight*missingLines)/2
            //+1 to missingLines to make sure we move it down enough
            text.GetComponent<RectTransform>().localPosition = new Vector2(xPos, yPos - textHeight / 2 * (missingLines + 1));
        }
    }

    protected void ResetTextBoxSize(TextBox textBox){
        //Get all of the variables
        Text text = textBox.text;
        int xPos = textBox.xPos;
        int yPos = textBox.yPos;
        float height = textBox.height;
        text.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);
        //The textBox increase in size from the center so we need to move it down by half of the increase in size
        //i.e (textHeight*missingLines)/2
        //+1 to missingLines to make sure we move it down enough
        text.GetComponent<RectTransform>().localPosition = new Vector2(xPos, yPos);
    }

    protected void MakeCommentsGreen(ref string line){
        //If it is a comment then  make it green
        //First check the line has at least two char
        if (line.Length >= 2){
            //A line is a comment if it starts with --
            if (line.Substring(0, 2) == "--"){
                line = "<color=green>" + line + "</color>";
            }
        }
    }
}

public class MultipleChoice : Exercise{
    private List<string> responses = new List<string>();
    private List<bool> selected = new List<bool>();
    private GameObject _object;
    private Dropdown dropdown;

    public MultipleChoice(Queue<string> exercise, TextBox c, TextBox o, GameObject dd, WorksheetManager wm){
        content = c;
        output = o;
        _object = dd;
        worksheetManager = wm;
        string line = exercise.Dequeue();
        //The number of characters in the previous line
        int previousLength = -1;
        while (line != "<c>"){
            MakeCommentsGreen(ref line);
            text.Enqueue(line+"\n");
            previousLength = line.Length;
            line = exercise.Dequeue();
        }
        //Create dropdown
        //Read in the dropdown choices
        List<string> choices = new List<string>();
        line = exercise.Dequeue();
        while (line != "</c>"){
            choices.Add(line);
            line = exercise.Dequeue();
        }
        CreateDropdown(text.Count,previousLength,choices);
        //Remove <r>
        exercise.Dequeue();
        //Get the responses
        line = exercise.Dequeue();
        while (line != "</r>"){
            responses.Add(line);
            selected.Add(false);
            line = exercise.Dequeue();
        }
        //Read in the remaining text
        while (exercise.Count!=0){
            line = exercise.Dequeue();
            MakeCommentsGreen(ref line);
            text.Enqueue(line + "\n");
        }
    }

    private void CreateDropdown(int line, int col, List<string> choices){
        //The x position is the number of characters in the line * the length of a character 
        //added on to the x coord of the left hand side of the text box 
        //+boxWidth/2 as we want the left of the box to be aligned not the center
        float left = -(content.width / 2);
        float xpos = left + 80 + (col * 12f);
        //The number of lines not being displayed
        int missingLines = 0;
        if (line > content.maxLines){
            missingLines = line - content.maxLines;
        }
        //The y coordinate of the top of the text box is a half of the new height of the text box
        //where new height of text box is height + textHeight * missingLines
        float yCoordTop = (content.height + textHeight * missingLines) / 2;
        //The y position is the y coordinate of the top of the textbox - number of lines * height of a line
        float ypos = yCoordTop - (line * textHeight);
        Vector3 pos = new Vector3(xpos, ypos, 0);
        _object.transform.localPosition = pos;

        //Extract the dropdown object from the object we have created
        dropdown = _object.GetComponent<Dropdown>();

        //Clear the default options and add the new ones
        dropdown.ClearOptions();
        dropdown.AddOptions(choices);
        dropdown.gameObject.SetActive(false);
        dropdown.onValueChanged.AddListener(delegate { ShowResponse(); });
    }

    //Called when the user selects an option from the dropdown menu
    private void ShowResponse(){
        if (dropdown.value != 0){
            //The index of the feedback
            int index = dropdown.value - 1;
            if (!selected[index]){
                selected[index] = true;
                Completed();
            }
            output.text.text = responses[index];
        }
        else{
            output.text.text = "";
        }
    }

    public override void Display(){
        content.text.text = "";
        output.text.text = "";
        int numLines = 0;
        while (text.Count != 0){
            numLines++;
            content.text.text += text.Dequeue();
        }
        dropdown.gameObject.SetActive(true);
        IncreaseTextBoxSize(content,numLines);
    }

    public override void Completed(){
        foreach(bool done in selected){
            if (done == false){
                return;
            }
        }
        worksheetManager.ChangeExercise();
    }

    public override void Destroy(){
        ResetTextBoxSize(content);
        ResetTextBoxSize(output);
        dropdown.gameObject.SetActive(false);
    }

    public override int Type(){
        return 0;
    }
}

public class HaskellQuestion : Exercise{
    private List<string> answer = new List<string>();
    private List<GameObject> objects;
    private List<InputField> inputFields = new List<InputField>();
    private int currentObject = 0;
    private string applicationPath;

    public HaskellQuestion(Queue<string> exercise, TextBox c, TextBox o, List<GameObject> os, WorksheetManager wm, string app){
        applicationPath = app;
        content = c;
        output = o;
        objects = os;
        worksheetManager = wm;
        string line = exercise.Dequeue();
        while (line != "<a>"){
            MakeCommentsGreen(ref line);
            if (line.Contains("<if>")){
                //Remove the <if>
                line = line.Replace("<if>", "");
                //Create inputfield
                CreateInputfield(text.Count+1, line.Length);
            }
            text.Enqueue(line + "\n");
            line = exercise.Dequeue();
        }
        line = exercise.Dequeue();
        while (line != "</a>"){
            answer.Add(line);
            line = exercise.Dequeue();
        }
    }

    private void CreateInputfield(int line, int col){
        //The x position is the number of characters in the line * the length of a character 
        //added on to the x coord of the left hand side of the text box 
        //+boxWidth/2 as we want the left of the box to be aligned not the center
        float left = -(content.width / 2);
        float xpos = left + 80 + (col * 9f);
        //The number of lines not being displayed
        int missingLines = 0;
        if (line > content.maxLines){
            missingLines = line - content.maxLines;
        }
        //The y coordinate of the top of the text box is a half of the new height of the text box
        //where new height of text box is height + textHeight * missingLines
        float yCoordTop = (content.height + textHeight * missingLines) / 2;
        //The y position is the y coordinate of the top of the textbox - number of lines * height of a line
        float ypos = yCoordTop - (line * textHeight);
        Vector3 pos = new Vector3(xpos, ypos, 0);
        objects[currentObject].transform.localPosition = pos;

        //Extract the inputfield object from the object we have created
        InputField inputField = objects[currentObject].GetComponent<InputField>();

        inputField.gameObject.SetActive(false);
        inputFields.Add(inputField);
        currentObject++;
    }

    public override void Display(){
        content.text.text = "";
        output.text.text = "";
        int numLines = 0;
        while (text.Count != 0){
            numLines++;
            content.text.text += text.Dequeue();
        }
        foreach (InputField inputField in inputFields){
            inputField.gameObject.SetActive(true);
        }
        IncreaseTextBoxSize(content, numLines);
    }

    public override void Completed(){
        List<string> inputs = new List<string>();
        string input = "";
        string noSpaces = "";
        output.text.text = "";
        ResetTextBoxSize(output);
        for (int i = 0; i < inputFields.Count; i++){
            input = inputFields[i].text;
            //Remove all spaces
            noSpaces = Regex.Replace(input, @"\s+", string.Empty);
            //Only run if there is something to run
            if (noSpaces.Equals("")){
                output.text.text = "error: One or more of the inputfields is empty!";
                return;
            }
            inputs.Add(input);
        }
        Run(inputs);
    }

    private void Run(List<string> inputs){
        Save(inputs);
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
            ParseOutput(process);
        }
        else{
            Process[] processes = Process.GetProcessesByName("main");
            foreach (Process p in processes){
                p.CloseMainWindow();
            }
            process.CloseMainWindow();
            output.text.text = "Time limit exceeded. Check your code for errors.";
        }
    }

    private void Save(List<string> inputs)    {
        List<string> data = new List<string>{
            "import Test.QuickCheck",
            "import ConvertMessage",
        };
        string line = "";
        int index = 0;
        for(int i = 0; i < answer.Count; i++){
            line = answer[i];
            if (line.Contains("<i>")){
                line = line.Replace("<i>", inputs[index]);
                index++;
            }
            data.Add(line);

        }
        data.Add("main ::IO()");
        data.Add("main = do");
        data.Add("result <- quickCheckResult prop");
        data.Add("putStrLn (convertMessage result)");
        File.WriteAllLines(applicationPath + "/StreamingAssets/Haskell/main.hs", data.ToArray());
    }

    //Parse the given output from the process
    private void ParseOutput(Process process){
        string line = "";
        int numLines = 0;
        //If there has been an error report it
        if (!process.StandardError.EndOfStream){
            while (!process.StandardError.EndOfStream){
                line = process.StandardError.ReadLine();
                if (line.Length!=0){
                    output.text.text += line + "\n";
                    numLines++;
                }
            }
        }
        else{
            //Read in the output only keeping the final line
            while (!process.StandardOutput.EndOfStream){
                output.text.text = process.StandardOutput.ReadLine();
            }
            numLines = 1;
            if(output.text.text == "+++ OK, passed 100 tests"){
                worksheetManager.ChangeExercise();
            }
        }
        IncreaseTextBoxSize(output, numLines);
    }
    public override void Destroy(){
        ResetTextBoxSize(content);
        ResetTextBoxSize(output);
        foreach (InputField inputField in inputFields){
            inputField.gameObject.SetActive(false);
        }
    }

    public override int Type(){
        return 1;
    }
}