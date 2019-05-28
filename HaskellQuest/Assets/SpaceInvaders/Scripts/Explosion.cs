using UnityEngine;

namespace SpaceInvaders{
    public class Explosion : MonoBehaviour{

        private GameObject player;
        //The timer of the explosion animation
        private float timer = 0;

        //Awake is called before OnEnable so initalize the player in here 
        private void Awake(){
            player = FindObjectOfType<Player>().gameObject;
        }

        //When the explosion is enabled make the player inactive and move the explosion to the position of the player
        private void OnEnable(){
            player.SetActive(false);
            transform.localPosition = player.transform.position;
        }

        //The explosion lasts for 1.2 seconds so after this time has passed reset the timer,
        //reactivate the player and make itself (the explosion) inactive
        private void Update(){
            timer += Time.deltaTime;
            if (timer > 1.2){
                timer = 0;
                player.SetActive(true);
                gameObject.SetActive(false);
            }
        }
    }
}