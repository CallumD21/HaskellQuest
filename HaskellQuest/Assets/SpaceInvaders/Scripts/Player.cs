using UnityEngine;

namespace SpaceInvaders{
    public class Player : MonoBehaviour{

        //The game object of the players bullet
        [SerializeField] GameObject myBullet;
        //The vertical movement of the player {1=right,-1=left,0=no movement}
        private float move;
        //The speed to move the player
        private readonly float speed = 10f;
        //The x value of the right hand side of the screen
        private float rightOfScreen;
        //True if the player needs to shoot
        private bool shoot = false;
        //The starting position of the player
        private Vector3 startPos = new Vector3(0, -4.4f, 0);

        private void Start(){
            float cameraHeight = 2f * Camera.main.orthographicSize;
            rightOfScreen = (cameraHeight * Camera.main.aspect) / 2;
            //Minus one as the x of the ship is its center coord
            rightOfScreen = rightOfScreen - 1;
        }

        private void OnEnable(){
            //When the player respawns restart the aliens, reset the players position and stop the players movement
            FindObjectOfType<BlockOfAliens>().SetPaused(false);
            transform.localPosition = startPos;
            move = 0;
        }

        //Get the input from the user
        private void Update(){
            move = Input.GetAxisRaw("Horizontal");
            if (Input.GetKey(KeyCode.Space)){
                shoot = true;
            }
        }

        private void FixedUpdate(){
            //If the player has pressed to move right and there is space to move right then move them right
            if (move == 1 && transform.position.x < rightOfScreen){
                transform.Translate(Vector2.right * Time.deltaTime * speed);
            }
            //Analogous to the move right case above
            else if (move == -1 && transform.position.x > -rightOfScreen){
                transform.Translate(Vector2.left * Time.deltaTime * speed);
            }
            //If the player has pressed to shoot then activate the players bullet and reset shoot
            if (shoot){
                myBullet.SetActive(true);
                shoot = false;
            }
        }

        //If the player is shot by the alien then call the appropriate function in the game manager
        //Pause the aliens and flying saucers movement and deactivate the the players bullet
        private void OnTriggerEnter2D(Collider2D other){
            if (other.tag == "AlienBullet"){
                FindObjectOfType<GameManager>().PlayerShot();
                FindObjectOfType<BlockOfAliens>().SetPaused(true);
                FindObjectOfType<FlyingSaucer>().SetInactive();
                myBullet.SetActive(false);
            }
        }
    }
}
