using UnityEngine;

namespace SpaceInvaders{
    public class AlienBullet : MonoBehaviour{

        //Only the bottom alien of a column can shoot so there is only one bullet per column.
        //The column the bullet belongs to
        [SerializeField] ColumnOfAliens column;
        //The y value of the bottom of the screen
        private float bottomOfScreen;
        //The y offset of the bullet from the alien
        private readonly float offset = -0.252f;

        private void Start(){
            bottomOfScreen = -Camera.main.orthographicSize;
        }

        private void OnEnable(){
            //Get the alien that is shoting the bullet
            Alien alien = column.GetBottomAlien();
            //Move the bullet to the position of the alien plus the offset
            Vector3 pos = new Vector3(alien.transform.position.x, alien.transform.position.y + offset, 0);
            transform.localPosition = pos;
        }

        private void FixedUpdate(){
            //If the bullet is bellow the bottom of the screen then set it to inactive
            if (transform.position.y < bottomOfScreen){
                gameObject.SetActive(false);
            }
            //Otherwise move the bullet down at a speed of 10f
            transform.Translate(Vector2.down * Time.deltaTime * 10f);
        }

        //If the bullet hits the player or a block then set it to inactive
        private void OnTriggerEnter2D(Collider2D other){
            if (other.tag == "Player" || other.tag == "Block"){
                gameObject.SetActive(false);
            }
        }
    }
}