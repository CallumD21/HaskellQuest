using UnityEngine;
using System.Collections.Generic;

namespace SpaceInvaders{
    public class ColumnOfAliens : MonoBehaviour{

        //The list of the Aliens belonging to this column
        [SerializeField] List<Alien> column;

        //Removes the given alien from the column
        //If it is the bottom alien then the bottom alien in block must be updated
        public void RemoveAlien(Alien shotAlien){
            //The bottom alien in the column is the last alien in the list
            if (shotAlien == column[column.Count - 1]){
                //Get the new bottom alien i.e the 2nd to last alien in the list (if it exists)
                Alien newAlien = null;
                if (column.Count > 1) { newAlien = column[column.Count - 2]; }
                transform.parent.parent.GetComponent<BlockOfAliens>().UpdateBottomRow(shotAlien, newAlien);
            }
            column.Remove(shotAlien);
        }

        //Return the alien at the bottom of the column
        public Alien GetBottomAlien(){
            return column[column.Count - 1];
        }
    }
}