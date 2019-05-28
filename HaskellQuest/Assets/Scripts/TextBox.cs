using System;
using UnityEngine.UI;

[Serializable]
public class TextBox{
    //A textbox needs its x and y position, its height, its maximum number of lines and the text
    public Text text;
    public int xPos;
    public int yPos;
    public float height;
    public float width;
    public int maxLines;
}
