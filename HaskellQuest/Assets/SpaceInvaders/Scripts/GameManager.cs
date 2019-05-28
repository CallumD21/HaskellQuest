using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.IO;

namespace SpaceInvaders{
    public class GameManager : MonoBehaviour{

        //An array for the ten life images
        [SerializeField] Image[] lifeImages;
        //The game objects of the playable ships
        [SerializeField] GameObject[] ships;
        //The game object for the explosion played when the player is shot
        [SerializeField] GameObject explosion;
        //The text field where the players score gets written
        [SerializeField] Text scoreText;
        //When the player gets more than 10 lives there is not enough space to display 
        //the images so write their lives in this text field
        [SerializeField] Text lifeText;
        //The prefab of the block of aliens
        [SerializeField] GameObject aliens;
        //The game over panel
        [SerializeField] GameObject gameOver;
        //The script to add a new highscore
        [SerializeField] AddScore addScore;
        //The list of the green bases
        [SerializeField] List<Base> bases;
        //The panel to tell the player they have unlocked a new ship 
        [SerializeField] GameObject newShip;

        //Timer since the player lost their final life
        private float restartTimer = 0;
        //True if game needs to be restarted
        private bool restart = false;
        private int lives = 3;
        private int score = 0;
        //True if the life text is showing instead of the images
        private bool showingLifeText = false;
        //The index of the current ship in ships
        private int currentShip;
        private BonusInfo bonus;
        //Timer to show the new ship panel
        private float shipTimer;
        //True if new ship panel is showing
        private bool showingNewShip = false;

        private void Start(){
            bonus = FindObjectOfType<BonusInfo>();
            //Change the playable ship to the correct ship
            ChangeShip();
            Time.timeScale = 1f;
            //If there is something to load then load it
            if (PlayerPrefs.GetInt("SI_Load") == 1){
                Load();
            }
        }

        private void Update(){
            //Delay between the player dying and ending the game to allow the explosion animation to play
            if (restart){
                restartTimer += Time.deltaTime;
                if (restartTimer > 0.5){
                    GameOver();
                    restart = false;
                }
            }
            //If the new ship panel is showing then...
            if (showingNewShip){
                shipTimer += Time.deltaTime;
                //If it has been showing for more than 5 seconds then stop showing it
                if (shipTimer > 5){
                    newShip.SetActive(false);
                    showingNewShip = false;
                    shipTimer = 0;
                }
            }
            //If a new ship has been unlocked the show the new ship panel
            if (bonus.UnlockShip2(score) || bonus.UnlockShip3(score) || bonus.UnlockShip4(score)){
                newShip.SetActive(true);
                showingNewShip = true;
            }
        }

        //Add the given points onto the score and update the score text
        public void AddScore(int points){
            score += points;
            scoreText.text = score.ToString("n0");
        }

        //Called when a player gets shot
        public void PlayerShot(){
            //If the player has no more lives then the game needs restarting
            if (lives == 0){
                restart = true;
                explosion.SetActive(true);
            }
            else{
                //Decrease the number of lives
                lives--;
                //If the life images are being shown then deactivate the furthest to the right image
                if (!showingLifeText){
                    lifeImages[lives].gameObject.SetActive(false);
                }
                //Else the text is showing but can now show all of the images so stop showing the text and show the images instead
                else if (lives == lifeImages.Length){
                    lifeText.gameObject.SetActive(false);
                    foreach (Image image in lifeImages){
                        image.gameObject.SetActive(true);
                    }
                    showingLifeText = false;
                }
                //Else just update the lives text
                else{
                    lifeText.text = lives.ToString();
                }
                explosion.SetActive(true);
            }
        }

        //All of the aliens have been shot so reload the block of aliens
        public void ReloadAliens(GameObject oldAliens){
            Destroy(oldAliens);
            Instantiate(aliens);
            //When the player kills all the aliens they get a new life
            lives++;
            //If the life images are being shown but the player has more lives than images then stop showing the images show the life text
            if (!showingLifeText && lives > lifeImages.Length){
                foreach (Image image in lifeImages){
                    image.gameObject.SetActive(false);
                }
                lifeText.gameObject.SetActive(true);
                lifeText.text = lives.ToString();
                showingLifeText = true;
            }
            //Else if lives text is showing then just update the text
            else if (showingLifeText){
                lifeText.text = lives.ToString();
            }
            //Else just show the next life image
            else{
                lifeImages[lives - 1].gameObject.SetActive(true);
            }
        }

        //Called when the game ends i.e the player loses all their lives
        public void GameOver(){
            HighscoresContainer highscoresContainer = FindObjectOfType<HighscoresContainer>();
            BlockOfAliens blockOfAliens = FindObjectOfType<BlockOfAliens>();
            FlyingSaucer flyingSaucer = FindObjectOfType<FlyingSaucer>();
            int numHighscores = highscoresContainer.GetNumberOfHighScores();
            int lowestScore = highscoresContainer.GetLowestScore();
            //If there are less than 10 highscores or the player got a score greater than the lowest high score
            //Then the player has just got a highscore so...
            if (numHighscores < 10 || score > lowestScore){
                //Stop the aliens and flying saucer from moving
                blockOfAliens.SetPaused(true);
                flyingSaucer.gameObject.SetActive(false);
                //Make the add score panel active and give the add score script the new score
                addScore.gameObject.SetActive(true);
                addScore.SetScore(score);
            }
            //Else the player didnt get a high score so...
            else{
                //Stop the aliens and flying saucer from moving
                blockOfAliens.SetPaused(true);
                flyingSaucer.gameObject.SetActive(false);
                //Display the game over panel
                gameOver.SetActive(true);
            }
            //Pause the game
            Time.timeScale = 0f;
        }

        //Save the game progress
        //Playerprefs cannot save bools so some ints are used to represent bools with 1=true and 0=false
        public void Save(){
            BlockOfAliens blockOfAliens = FindObjectOfType<BlockOfAliens>();
            //Save the score and lives to playerprefs
            PlayerPrefs.SetInt("SI_Score", score);
            PlayerPrefs.SetInt("SI_Lives", lives);
            //The game has been saved so the next time the game is ran something needs loading
            PlayerPrefs.SetInt("SI_Load", 1);
            PlayerPrefs.SetFloat("SI_Speed", blockOfAliens.GetSpeed());
            //Convert the direction to an int
            int dir = 0;
            if (blockOfAliens.GetDirection()){
                dir = 1;
            }
            PlayerPrefs.SetInt("SI_right", dir);
            PlayerPrefs.SetInt("SI_nextToLoad", blockOfAliens.GetNextToLoad());
            Alien leftMostAlien = blockOfAliens.GetLeft();
            Alien rightMostAlien = blockOfAliens.GetRight();
            //An alien is uniquely identified by its parents name along with its own
            //So save this information for the left/right most aliens
            PlayerPrefs.SetString("SI_leftMostAlien", leftMostAlien.transform.parent.name + leftMostAlien.name);
            PlayerPrefs.SetString("SI_rightMostAlien", rightMostAlien.transform.parent.name + rightMostAlien.name);
            PlayerPrefs.Save();

            //Save the aliens info to a CSV
            StreamWriter writer = File.CreateText(Application.dataPath + "/StreamingAssets/SpaceInvaders/AlienInfo.csv");
            string line = string.Empty;
            List<Alien> aliens = blockOfAliens.GetAliens();
            for (int i = 0; i < aliens.Count; i++){
                Alien alien = aliens[i];
                //Alien id = its parents name along with its own 
                //The data that needs saving is alien id and is position
                line = alien.transform.parent.name + alien.name + "," + alien.transform.position.x + "," + alien.transform.position.y;
                writer.WriteLine(line);
            }
            writer.Close();

            //Save the blocks info to a CSV
            writer = File.CreateText(Application.dataPath + "/StreamingAssets/SpaceInvaders/BlockInfo.csv");
            line = string.Empty;
            //A list of all of the blocks
            List<Block> blocks = new List<Block>();
            //Add all the blocks from each base to blocks
            blocks.AddRange(bases[0].GetBlocks());
            blocks.AddRange(bases[1].GetBlocks());
            blocks.AddRange(bases[2].GetBlocks());
            for (int i = 0; i < blocks.Count; i++){
                Block block = blocks[i];
                //Block id = parents name along with its own 
                //The data that needs saving is block id and its current state (the sprite it is showing)
                line = block.transform.parent.name + block.name + "," + block.GetCurrentSprite().ToString();
                writer.WriteLine(line);
            }
            writer.Close();
        }

        //Called when there is data to load
        private void Load(){
            BlockOfAliens blockOfAliens = FindObjectOfType<BlockOfAliens>();
            score = PlayerPrefs.GetInt("SI_Score");
            lives = PlayerPrefs.GetInt("SI_Lives");
            string leftAlienID = PlayerPrefs.GetString("SI_leftMostAlien");
            string rightAlienID = PlayerPrefs.GetString("SI_rightMostAlien");
            blockOfAliens.SetNextToLoad(PlayerPrefs.GetInt("SI_nextToLoad"));
            //Right is true if the saved value is a 1
            blockOfAliens.SetDirection(PlayerPrefs.GetInt("SI_right") == 1);
            blockOfAliens.SetSpeed(PlayerPrefs.GetFloat("SI_Speed"));
            scoreText.text = score.ToString("n0");
            //By default the first 3 life images are shown so...
            //If the lives is less than or equal to 3 then deactivate the images starting from the right most images so 3-i not i
            if (lives <= 3){
                for (int i = 3 - lives; i > 0; i--){
                    lifeImages[3 - i].gameObject.SetActive(false);
                }
            }
            //Else if the number of lives is less than or equal to the number of images then
            //activate the correct number of images starting at the 4th image
            else if (lives <= lifeImages.Length){
                for (int i = 3; i < lives; i++){
                    lifeImages[i].gameObject.SetActive(true);
                }
            }
            //Else life text needs showing so deactivate all the images
            else{
                for (int i = 0; i < 3; i++){
                    lifeImages[i].gameObject.SetActive(false);
                }
                lifeText.gameObject.SetActive(true);
                lifeText.text = lives.ToString();
                showingLifeText = true;
            }

            //Read the alien info from a CSV
            StreamReader reader = File.OpenText(Application.dataPath + "/StreamingAssets/SpaceInvaders/AlienInfo.csv");
            string line = reader.ReadLine();
            List<Alien> aliens = blockOfAliens.GetAliens();
            //Start at -1 as it gets incremented at the start of the loop
            int currentAlien = -1;
            Alien alien;
            while (line != null){
                //Get the next alien
                currentAlien++;
                alien = aliens[currentAlien];
                //Split the line on ","
                string[] data = line.Split(","[0]);
                //Keep searching for the alien that has the info stored in data
                while (data[0] != alien.transform.parent.name + alien.name){
                    //As the CSV data is stored in the same order as the aliens arra then
                    //if this data doesnt correspond to the current alien then we havent saved any info for this alien
                    //i.e it was shot so remove it
                    alien.transform.parent.GetComponent<RowOfAliens>().RemoveAlien(alien);
                    alien.GetColumn().RemoveAlien(alien);
                    alien.gameObject.SetActive(false);
                    currentAlien++;
                    alien = aliens[currentAlien];
                }
                //We have found the alien so update its position to the data we read
                alien.transform.position = new Vector3(float.Parse(data[1]), float.Parse(data[2]), 0);
                //If this alien is the left or right most alien then set them appropriately
                if (data[0] == leftAlienID){
                    blockOfAliens.SetLeft(alien);
                }
                if (data[0] == rightAlienID){
                    blockOfAliens.SetRight(alien);
                }
                line = reader.ReadLine();
            }
            reader.Close();
            //We have now read all of the data from the file so any remaining aliens were shot
            //Delete any remaining aliens
            for (int i = currentAlien + 1; i < aliens.Count; i++){
                alien = aliens[i];
                alien.transform.parent.GetComponent<RowOfAliens>().RemoveAlien(alien);
                alien.GetColumn().RemoveAlien(alien);
                alien.gameObject.SetActive(false);
            }

            //Read the block info from a CSV (this is very similar to loading the aliens)
            reader = File.OpenText(Application.dataPath + "/StreamingAssets/SpaceInvaders/BlockInfo.csv");
            line = reader.ReadLine();
            List<Block> blocks = new List<Block>();
            blocks.AddRange(bases[0].GetBlocks());
            blocks.AddRange(bases[1].GetBlocks());
            blocks.AddRange(bases[2].GetBlocks());
            int currentBlock = -1;
            Block block;
            while (line != null){
                currentBlock++;
                block = blocks[currentBlock];
                string[] data = line.Split(","[0]);
                while (data[0] != block.transform.parent.name + block.name){
                    //The block wasnt save so is inactive
                    block.SetInactive(); ;
                    currentBlock++;
                    block = blocks[currentBlock];
                }
                //We have found the block so set its sprite to the saved value
                block.ChangeSprite(int.Parse(data[1]));
                line = reader.ReadLine();
            }
            reader.Close();
            //Delete any remaining blocks
            for (int i = currentBlock + 1; i < blocks.Count; i++){
                block = blocks[i];
                block.SetInactive();
            }
        }

        //Change ship to the selected ship from bonuses
        private void ChangeShip(){
            //Make the current ship active
            currentShip = FindObjectOfType<BonusInfo>().GetCurrentShip();
            ships[currentShip].SetActive(true);
            //Adjust the height of the ships accordingly
            int height = 26;
            if (currentShip == 1 || currentShip == 3){
                height = 15;
            }
            Vector2 size = new Vector2(25, height);
            //Change the life images to be the image of the current ship
            foreach (Image image in lifeImages){
                image.sprite = ships[currentShip].GetComponent<SpriteRenderer>().sprite;
                image.rectTransform.sizeDelta = size;
            }
        }
    }
}