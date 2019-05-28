using UnityEngine;

namespace SpaceInvaders{
    public class PlayerBullet : MonoBehaviour{

        [SerializeField] Transform player;
        //The y value of the top of the screen used to know when the bullet should be inactive
        private float topOfScreen;
        //The y offset of the bullet from the player
        private readonly float offset = 0.26f;

        private void Awake(){
            //The y value of the top of the screen is given by Camera.main.orthographicSize
            //I -1f as the bullet needs to be inactive before the UI bar at the top of the screen
            topOfScreen = Camera.main.orthographicSize - 1f;
        }

        //When the bullet is shot it needs to be in the position of the player plus the offset
        private void OnEnable(){
            Vector3 pos = new Vector3(player.position.x, player.position.y + offset, 0);
            transform.localPosition = pos;
        }

        private void FixedUpdate(){
            //If the bullet is above the top of the screen then be inactive
            if (transform.position.y > topOfScreen){
                gameObject.SetActive(false);
            }
            //Otherwise move the bullet up the screen at a speed of 10 kept constant across frame rates 
            transform.Translate(Vector2.up * Time.deltaTime * 10f);
        }

        //If the bullet hits the green blocks then be inactive
        private void OnTriggerEnter2D(Collider2D other){
            if (other.tag == "Block"){
                gameObject.SetActive(false);
            }
        }
    }

}