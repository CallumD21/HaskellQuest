using UnityEngine;

namespace SpaceInvaders{
    public class FlyingSaucer : MonoBehaviour{

        //The first sprite is the sprite for the flying saucer 
        //The next 12 are the sprites of the score for shooting the flying saucer
        //The scores are multiples of 25 so 25,50,75,...,300
        [SerializeField] Sprite[] sprites;
        //The sprite renderer of the flying saucer
        [SerializeField] SpriteRenderer sr;
        //The audio source of the flying saucer
        [SerializeField] AudioSource audioSource;
        //The x values of the left and right of the camera
        private float leftOfScreen;
        private float rightOfScreen;
        //True if the flying saucer is moving right
        private bool right = false;
        //Speed the flying saucer is moving at
        private readonly float speed = 4f;
        //True if the flying saucer should be moving
        private bool move = false;
        //True if the flying saucers score is being displayed so the flying saucer shouldnt be able to move
        private bool displayingScore = false;
        //A timer for how long to display the score
        private float timer = 0;
        //The positions to teleport the flying saucer to when it is shot
        private Vector3 tpLeft = new Vector3(-7.716498f, 4.11f, 0);
        private Vector3 tpRight = new Vector3(7.716498f, 4.11f, 0);

        private void Start(){
            float cameraHeight = 2f * Camera.main.orthographicSize;
            rightOfScreen = (cameraHeight * Camera.main.aspect) / 2;
            //+1 so the flying saucer moves fully off the screen
            rightOfScreen = rightOfScreen + 1;
            leftOfScreen = -rightOfScreen;
        }

        private void Update(){
            //If the score is being displayed then.. 
            if (displayingScore){
                //Update the timer
                timer += Time.deltaTime;
                //If it has been displayed for more than 1 second then..
                if (timer > 1){
                    //Reset the timer
                    timer = 0;
                    //flying saucer should no longer be moving and no longer displaying the score
                    move = false;
                    displayingScore = false;
                    //Reset its sprite to the sprite of the flying saucer
                    sr.sprite = sprites[0];
                    //If it was moving right before it was shot then move it to the left of the screen
                    if (right){
                        transform.position = tpLeft;
                    }
                    //else move it to the right of the screen
                    else{
                        transform.position = tpRight;
                    }
                }
            }
            //If the score is not being displayed then...
            else{
                //If the flying saucer is not currently moving then...
                if (!move){
                    //Select a random number between 0 and 1
                    float rand = Random.Range(0f, 1f);
                    //If the number is less than 0.001 then start moving the flying saucer and playing its audio
                    if (rand < 0.001){
                        move = true;
                        audioSource.Play();
                    }
                }
                //If the flying saucer is currently moving then...
                else{
                    //If the flying saucer goes off the screen then stop it from moving, stop its audio and flip its direction
                    if (transform.position.x < leftOfScreen){
                        right = true;
                        move = false;
                        audioSource.Stop();
                    }
                    else if (transform.position.x > rightOfScreen){
                        right = false;
                        move = false;
                        audioSource.Stop();
                    }
                }
            }
        }

        private void FixedUpdate(){
            //If the score is not being displayed and the ship is moving then move the ship in the correct direction
            if (!displayingScore){
                if (move){
                    if (right){
                        transform.Translate(Vector2.right * Time.deltaTime * speed);
                    }
                    else{
                        transform.Translate(Vector2.left * Time.deltaTime * speed);
                    }
                }
            }
        }

        private void OnTriggerEnter2D(Collider2D other){
            //If the flying saucer collides with the player bullet and is not displaying the score then...
            if (other.tag == "PlayerBullet" && !displayingScore){
                //Stop playing the audio and make the bullet inactive
                audioSource.Stop();
                other.gameObject.SetActive(false);
                //Random select an index of the sprites array to represent the score that the flying saucer was worth
                int index = Random.Range(1, sprites.Length - 1);
                //Update the score
                FindObjectOfType<GameManager>().AddScore(25 * index);
                //Display the sprite of the score
                sr.sprite = sprites[index];
                displayingScore = true;
            }
        }

        //Called when the player is shot and moves the flying saucer of the screen from where it was coming form
        public void SetInactive(){
            audioSource.Stop();
            if (right){
                transform.position = tpLeft;
            }
            else{
                transform.position = tpRight;
            }
        }
    }
}