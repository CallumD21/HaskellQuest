using UnityEngine;
using System.Collections.Generic;

namespace SpaceInvaders{
    public class BlockOfAliens : MonoBehaviour{

        //The alien that is furthest to the right/left of the block
        [SerializeField] Alien rightMostAlien;
        [SerializeField] Alien leftMostAlien;
        //The block of aliens is a list of the rows of aliens
        [SerializeField] List<RowOfAliens> block;
        //The bottom aliens in each column (these are the aliens that can shoot)
        [SerializeField] List<Alien> bottomAliens;
        //The alien lowest down in the block
        [SerializeField] Alien bottomAlien;
        //True if the block is moving right
        private bool right = true;
        //True if the aliens are being loaded
        private bool loading = true;
        //Timer used to put a delay on alien movement
        private float alienTimer = 0;
        //Timer used to put a delay on loading the aliens
        private float loadingTimer = 0;
        //The index in block of the next row to be loaded the bottom row is loaded first
        private int nextToLoad = 4;
        //A player has been shot and the aliens need to not shoot or move
        private bool paused = false;
        //The multiplier of how often to move the block (as its a multiplier a lower value means update quicker so move faster)
        private float speed = 1;

        private void Update(){
            //If the aliens are being loaded then...
            if (loading){
                //Update the timer
                loadingTimer += Time.deltaTime;
                //If .2 seconds has passed then...
                if (loadingTimer > 0.2){
                    //Set the next row to be loaded of the block to active
                    block[nextToLoad].gameObject.SetActive(true);
                    //Reset the timer
                    loadingTimer = 0;
                    //The next row to load is higher in the block so earlier in the list so decrement the value
                    nextToLoad--;
                    //If the next row to load is -1 then we have finished loading
                    if (nextToLoad == -1){
                        loading = false;
                    }
                }
            }
            //Else if the aliens are not paused then... 
            else if (!paused){
                //Update the timer
                alienTimer += Time.deltaTime;
                //If a certain time adjusted by the speed has passed then move the block and reset the timer
                if (alienTimer > 0.8 * speed){
                    Move();
                    alienTimer = 0;
                }
                //If a random number between 0 and 1 is less than 0.009 then an alien in the block should shoot
                float rand = Random.Range(0f, 1f);
                if (rand <= 0.009){
                    //Randomly pick an alien at the bottom of each column to shoot
                    int index = Random.Range(0, bottomAliens.Count);
                    bottomAliens[index].Shoot();
                }
            }
        }

        //Move the block of aliens
        public void Move(){
            if (right){
                //If rightMostAlien can move right then move all rows right
                if (rightMostAlien.CanMoveRight()){
                    foreach (RowOfAliens row in block){
                        row.MoveRowRight();
                    }
                }
                //Else move the block down and change direction
                else{
                    MoveBlockDown();
                    right = false;
                }
            }
            else{
                //If leftMostAlien can move left then move all rows left
                if (leftMostAlien.CanMoveLeft()){
                    foreach (RowOfAliens row in block){
                        row.MoveRowLeft();
                    }
                }
                //Else move the block down and change direction
                else{
                    MoveBlockDown();
                    right = true;
                }
            }
        }

        //Move all rows in the block down
        private void MoveBlockDown(){
            //Increase how often the block moves
            speed -= 0.08f;
            //If the bottomAlien can move down and not crash into the bases then move the block down
            //Else game over!
            if (bottomAlien.transform.position.y - 0.796 > -2.2){
                foreach (RowOfAliens row in block){
                    row.MoveRowDown();
                }
            }
            else{
                FindObjectOfType<GameManager>().GameOver();
            }
        }

        //Update the leftMostAlien
        private void UpdateLeft(){
            List<Alien> leftMostAliens = new List<Alien>();
            //Get the leftMostAlien of each row
            foreach (RowOfAliens row in block){
                leftMostAliens.Add(row.GetAlien(false));
            }
            //Find the furthest left alien out of the leftMostAliens
            float minX = 100000;
            foreach (Alien alien in leftMostAliens){
                if (alien.transform.position.x < minX){
                    leftMostAlien = alien;
                    minX = alien.transform.position.x;
                }
            }
        }

        //Update the rightMostAlien
        private void UpdateRight(){
            List<Alien> rightMostAliens = new List<Alien>();
            //Get the rightMostAlien of each row
            foreach (RowOfAliens row in block){
                rightMostAliens.Add(row.GetAlien(true));
            }
            //Find the furthest right alien out of the rightMostAliens
            float maxX = -100000;
            foreach (Alien alien in rightMostAliens){
                if (maxX < alien.transform.position.x){
                    rightMostAlien = alien;
                    maxX = alien.transform.position.x;
                }
            }
        }

        //Called when the left or right most alien of a row is shot
        public void LeftRightShot(Alien shotAlien){
            if (shotAlien == leftMostAlien){
                UpdateLeft();
            }
            else if (shotAlien == rightMostAlien){
                UpdateRight();
            }
        }

        //Remove the given row (which is empty)
        public void RemoveRow(RowOfAliens rowOfAliens){
            block.Remove(rowOfAliens);
            //If the block is empty then reload the block
            if (block.Count == 0){
                FindObjectOfType<GameManager>().ReloadAliens(gameObject);
            }
        }

        //Replaces the oldAlien in the bottomRow with the newAlien
        //If the newAlien is null then just remove the oldAlien
        public void UpdateBottomRow(Alien oldAlien, Alien newAlien){
            bottomAliens.Remove(oldAlien);
            if (newAlien != null){
                bottomAliens.Add(newAlien);
            }
            //Update bottomAlien
            //If the old alien was the bottom alien and there are still aliens alive then...
            if (oldAlien == bottomAlien && bottomAliens.Count != 0){
                //First set the bottomAlien to the first alien in bottomAliens
                bottomAlien = bottomAliens[0];
                float lowestY = bottomAliens[0].transform.position.y;
                //Search through the other aliens and if they are lower than the current bottom alien then
                //they are the new bottom alien
                foreach (Alien alien in bottomAliens){
                    if (alien.transform.position.y < lowestY){
                        lowestY = alien.transform.position.y;
                        bottomAlien = alien;
                    }
                }
            }
        }

        public void SetPaused(bool val){
            paused = val;
        }

        //Return a list of all of the alive aliens
        public List<Alien> GetAliens(){
            List<Alien> aliens = new List<Alien>();
            //Loop through the rows adding a row at a time
            for (int i = 0; i < block.Count; i++){
                List<Alien> row = block[i].GetRow();
                aliens.AddRange(row);
            }
            return aliens;
        }

        public bool GetDirection(){
            return right;
        }

        //The next row to load is the bottom row of the block
        public int GetNextToLoad(){
            return block.Count - 1;
        }

        public Alien GetLeft(){
            return leftMostAlien;
        }

        public Alien GetRight(){
            return rightMostAlien;
        }

        public void SetLeft(Alien alien){
            leftMostAlien = alien;
        }

        public void SetRight(Alien alien){
            rightMostAlien = alien;
        }

        public void SetDirection(bool dir){
            right = dir;
        }

        public void SetNextToLoad(int load){
            nextToLoad = load;
        }

        public float GetSpeed(){
            return speed;
        }

        public void SetSpeed(float val){
            speed = val;
        }
    }
}