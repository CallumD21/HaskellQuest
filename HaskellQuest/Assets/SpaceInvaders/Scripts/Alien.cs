using UnityEngine;

namespace SpaceInvaders{
    public class Alien : MonoBehaviour{

        //The bullet belonging to the column the alien is in
        [SerializeField] AlienBullet myBullet;
        //The sprite renderer for this alien
        [SerializeField] SpriteRenderer sr;
        //An array of the 3 sprites for this alien, 2 for the alien moving and 1 for a shot alien
        [SerializeField] Sprite[] sprites;
        //The column this alien belongs to
        [SerializeField] ColumnOfAliens myColumn;
        //True if the sprite of the alien is sprites[0]
        private bool sprite1Active = true;
        //The x value of the right hand side of the screen
        private float rightOfScreen;
        //Moves the alien down a row
        private Vector3 down = new Vector3(0, -0.5f, 0);
        //Moves the alien across a column
        private Vector3 across = new Vector3(0.2f, 0, 0);
        //Multplied by 10 to give the score it is worth
        [SerializeField] private int type;
        //A timer from being shot to being deactivated
        private float timer = 0;
        //True if the sprite being displayed is sprites[2] i.e a shot alien
        private bool dead = false;

        void Start(){
            float cameraHeight = 2f * Camera.main.orthographicSize;
            rightOfScreen = (cameraHeight * Camera.main.aspect) / 2;
            //Minus one as the x of the ship is its center coord
            rightOfScreen = rightOfScreen - 1;
        }

        private void Update(){
            //If the alien is displaying the shot sprite then...
            if (dead){
                //Update the timer
                timer += Time.deltaTime;
                //If 0.5 seconds have passed then deactivate the alien
                if (timer > 0.5){
                    gameObject.SetActive(false);
                }
            }
        }

        //Called when the alien has to fire its bullet so set the bullet to active
        public void Shoot(){
            myBullet.gameObject.SetActive(true);
        }

        //Retruns true if the alien can move right and stay on the screen
        public bool CanMoveRight(){
            return transform.position.x + 0.2 < rightOfScreen;
        }

        //Move the alien right
        public void MoveRight(){
            transform.position += across;
            ChangeSprite();
        }

        //Retruns true if the alien can move left and stay on the screen
        public bool CanMoveLeft(){
            return transform.position.x - 0.2 > -rightOfScreen;
        }

        //Move the alien left
        public void MoveLeft(){
            transform.position -= across;
            ChangeSprite();
        }

        //Move the alien down a row
        public void MoveDown(){
            ChangeSprite();
            transform.localPosition += down;
        }

        //Alternate sprites each time the alien is moved
        private void ChangeSprite(){
            if (sprite1Active){
                sr.sprite = sprites[1];
                sprite1Active = false;
            }
            else{
                sr.sprite = sprites[0];
                sprite1Active = true;
            }
        }

        private void OnTriggerEnter2D(Collider2D other){
            //If the alien is hit by the player bullet and is not displaying the shot sprite then...
            if (other.tag == "PlayerBullet" && !dead){
                //Update the score and play the shot audio
                FindObjectOfType<GameManager>().AddScore(10 * type);
                GetComponent<AudioSource>().Play();
                //Change the sprite to the shot sprite
                sr.sprite = sprites[2];
                //Deactivate the bullet
                other.gameObject.SetActive(false);
                //Remove this alien from its row and column
                transform.parent.GetComponent<RowOfAliens>().RemoveAlien(this);
                myColumn.RemoveAlien(this);
                //Set dead to true so it can no longer collide with player bullet
                dead = true;
            }
        }

        public ColumnOfAliens GetColumn(){
            return myColumn;
        }
    }
}