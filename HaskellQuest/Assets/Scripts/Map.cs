using UnityEngine;
using UnityEngine.SceneManagement;

public class Map : MonoBehaviour {

    private void Start(){
        //Unlock the mouse and make it visible
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void School(){
        SceneManager.LoadScene(Scenes.school);
    }

    public void Home(){
        SceneManager.LoadScene(Scenes.room);
    }
}
