using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour {

    //The camera
    [SerializeField] private Camera mainCamera;
    //Text to display when the player is looking at the door
    [SerializeField] private Text doorText;
    //Text to display when the player is looking at the desk
    [SerializeField] private Text deskText;
    //Text to display when the player is looking at the bed
    [SerializeField] private Text bedText;
    //The payments panel
    [SerializeField] private GameObject payments;
    //True if the door text is active (i.e the player is looking at the door)
    private bool displayingDoorText = false;
    //True if the desk text is active (i.e the player is looking at the desk)
    private bool displayingDeskText = false;
    //True if the bed text is active (i.e the player is looking at the bed)
    private bool displayingBedText = false;
    private GameManager gm;

    private void Start(){
        gm  = FindObjectOfType<GameManager>();
    }

    private void FixedUpdate(){
        //If the player clicks on the door then change scene
        if (Input.GetKey(KeyCode.Mouse0) && displayingDoorText){
            SceneManager.LoadScene(Scenes.map);
        }

        //If the player clicks on the desk then change scene
        if (Input.GetKey(KeyCode.Mouse0) && displayingDeskText){
            SceneManager.LoadScene(Scenes.PC);
        }

        //If the player clicks on the bed then stop showing the text and display the payments panel
        if (Input.GetKey(KeyCode.Mouse0) && displayingBedText){
            bedText.gameObject.SetActive(false);
            displayingBedText = false;
            payments.SetActive(true);
        }

        //Fire out a ray from where the camera is pointing and save the information of what is hit in hit
        RaycastHit hit;
        if(Physics.Raycast(mainCamera.transform.position, mainCamera.transform.forward, out hit)){
            //If the player is looking at the door, its the beginning of the day and the text isnt showing then display the text
            //They can not go to school on day 3
            if (hit.transform.tag == "Door" && gm.GetTime() == 0 && !displayingDoorText && gm.GetDay() != 3 && !gm.CompletedSchool()){
                doorText.gameObject.SetActive(true);
                displayingDoorText = true;
            }

            //If the player isnt looking at the door and the text is showing then stop displaying the text
            if (hit.transform.tag != "Door" && displayingDoorText){
                doorText.gameObject.SetActive(false);
                displayingDoorText = false;
            }

            //If the player is looking at the desk and the text isnt showing then display the text
            //The player cannot click on the desk until day2, time 1
            if (gm.GetDay() > 2 || (gm.GetDay() == 2 && gm.GetTime() != 0)){
                if (hit.transform.tag == "Desk" && !displayingDeskText){
                    deskText.gameObject.SetActive(true);
                    displayingDeskText = true;
                }
            }

            //If the player isnt looking at the desk and the text is showing then stop displaying the text
            if (hit.transform.tag != "Desk" && displayingDeskText){
                deskText.gameObject.SetActive(false);
                displayingDeskText = false;
            }

            //If the player is looking at the bed and it is bed time and the text isnt showing then display the text
            if (hit.transform.tag == "Bed" && gm.GetTime() == 2 && !displayingBedText){
                bedText.gameObject.SetActive(true);
                displayingBedText = true;
            }

            //If the player isnt looking at the bed and the text is showing then stop displaying the text
            if (hit.transform.tag != "Bed" && displayingBedText){
                bedText.gameObject.SetActive(false);
                displayingBedText = false;
            }
        }
    }
}