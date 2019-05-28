using UnityEngine;

namespace SpaceInvaders{
    public class Block : MonoBehaviour{

        //A block can be shot 4 times
        //For the first 3 times it gets shot it changes its sprite to the next sprite in sprites
        //The final time it gets shot it gets made inactive
        [SerializeField] Sprite[] sprites;
        //The sprite renderer of this block
        [SerializeField] SpriteRenderer sr;
        //The index of the current sprite in sprites
        //-1 stands for displaying its default sprite
        private int currentSprite = -1;

        private void OnTriggerEnter2D(Collider2D other){
            //If the block is hit by the player or alien bullet then...
            if (other.tag == "PlayerBullet" || other.tag == "AlienBullet"){
                //If it is displaying the final sprite then call SeInactive
                if (currentSprite == 2){
                    SetInactive();
                }
                //Else display the next sprite
                else{
                    currentSprite++;
                    sr.sprite = sprites[currentSprite];
                }
            }
        }

        public int GetCurrentSprite(){
            return currentSprite;
        }

        //Set the current sprite to the given value and if the current sprite isnt the default sprite then change the sprite.
        public void ChangeSprite(int input){
            currentSprite = input;
            if (currentSprite != -1){
                sr.sprite = sprites[currentSprite];
            }
        }

        //Remove this block from its base and set itself as inactive
        public void SetInactive(){
            transform.parent.GetComponent<Base>().RemoveBlock(this);
            gameObject.SetActive(false);
        }
    }
}