using UnityEngine;
using System.Collections.Generic;

namespace SpaceInvaders{
    public class RowOfAliens : MonoBehaviour{

        //The list of the Aliens belonging to this row
        [SerializeField] List<Alien> row;
        Alien rightMostAlien;
        Alien leftMostAlien;

        //At the start the left most alien of the row is the 1st Alien and the right most Alien of the row is the last Alien.
        private void Start(){
            leftMostAlien = row[0];
            rightMostAlien = row[row.Count - 1];
        }

        //To move the row right we move each alien in the row right
        public void MoveRowRight(){
            foreach (Alien alien in row){
                alien.MoveRight();
            }
        }

        //To move the row left we move each alien in the row left
        public void MoveRowLeft(){
            foreach (Alien alien in row){
                alien.MoveLeft();
            }
        }

        //To move the row down we move each alien in the row down
        public void MoveRowDown(){
            foreach (Alien alien in row){
                alien.MoveDown();
            }
        }

        //Removes the given alien from the row and updates left and right most alien
        public void RemoveAlien(Alien shotAlien){
            row.Remove(shotAlien);
            //If the row is empty then remove the row from the block
            if (row.Count == 0){
                transform.parent.parent.GetComponent<BlockOfAliens>().RemoveRow(this);
            }
            //If the left most alien is shot then the new left most alien of the row is the new first alien of the list
            //because the old left most alien has just been removed from the list
            else if (shotAlien == leftMostAlien){
                leftMostAlien = row[0];
                //Update the left most alien of the block
                transform.parent.parent.GetComponent<BlockOfAliens>().LeftRightShot(shotAlien);
            }
            //Analogous to the left most alien case above
            else if (shotAlien == rightMostAlien){
                rightMostAlien = row[row.Count - 1];
                transform.parent.parent.GetComponent<BlockOfAliens>().LeftRightShot(shotAlien);
            }
        }

        //Return the left/right most alien depending on what the value of the given boolean is
        public Alien GetAlien(bool right){
            if (right){
                return rightMostAlien;
            }
            else{
                return leftMostAlien;
            }
        }

        //Return the row of aliens
        public List<Alien> GetRow(){
            return row;
        }
    }
}