using Raylib_cs;
using System.IO;
using System.Numerics;

namespace KOU_YZM209_CSGameProject_Spacewar;

public class Game
{
    // Flag to check if the game is finished.
    public bool isGameFinished = false;
    
    // UI, sound manager, and game objects.
    private UI ui;
    private SoundManager soundManager;
    
    // Player spaceship, enemies, bullets, collectibles, and level-related variables.
    private Spaceship player;
    private List<Enemy> enemies;
    private List<Bullet> enemyBullets;
    private List<Collectible> collectibles;
    private Level level;
    private List<Level> levels;
    private List<Explosion> explosions;
    
    // Score and reason for game over.
    private int score = 0;
    private string causeOfGameOver;
    
    // Flags to track game status (over, started, paused, score saved).
    private bool isGameOver = false;
    private bool isGameStarted = false;
    private bool isGamePaused = false;
    private bool isGameScoreSaved = false;

    // Loading textures for game assets (spaceships, background, bullets, explosions, and collectibles).
    private Texture2D
        spaceShips = Raylib.LoadTexture("./resources/image/spaceShips.png"),
        gameBackground = Raylib.LoadTexture("./resources/image/gameBackground.png"),
        bulletTexture = Raylib.LoadTexture("./resources/image/bullets.png"),
        explosionTexture = Raylib.LoadTexture("./resources/image/explosion.png"),
        collectibleTexture = Raylib.LoadTexture("./resources/image/collectibles.png");

    // Defining rectangles for background and game element positioning.
    private Rectangle
        gameBackgroundRec = new Rectangle(0, 0, 1920, 1080),
        gameBackgroundDes = new Rectangle(0, 0, Raylib.GetScreenWidth(), Raylib.GetScreenHeight());
    
    // Origin point for scaling and positioning of elements.
    private Vector2
        origin = new Vector2(0, 0);

    // Random instance to generate random values (e.g., for enemy spawn positions).
    private Random random = new Random();
    
    // Constructor to initialize UI and SoundManager for the game.
    public Game(UI ui, SoundManager soundManager)
    {
        this.ui = ui;  // Assigning the UI object to the class.
        this.soundManager = soundManager;  // Assigning the SoundManager object to the class.
    }

    // Main game loop method.
    public void PlayGame()
    {
        // Check if the game has started. If not, start the game.
        if (!isGameStarted)
        {
            StartGame();
        }
        else
        {
            // Drawing the background texture on the screen.
            Raylib.DrawTexturePro(gameBackground, gameBackgroundRec, gameBackgroundDes, origin, 0, Color.RayWhite);
            
            // If the game is paused, manage the pause behavior.
            if (isGamePaused)
            {
                soundManager.PauseGameMusic();  // Pause the background music.
                WatchKeyEscape();  // Check for the escape key to unpause or quit.
                
                // Check if the user clicked on the pause screen and update the game state.
                (isGameFinished, isGamePaused) = ui.CheckPauseScreenMouse(isGameFinished, isGamePaused);
                ui.DrawPauseScreen();  // Draw the pause screen UI.
            }
            else
            {
                // If the game is not over, resume game music and continue normal gameplay.
                if (!isGameOver)
                {
                    soundManager.ResumeGameMusic();  // Resume the background music.
                    
                    WatchKeyEscape();  // Check for the escape key during normal gameplay.
                    
                    // Handle player health, level progression, and other gameplay elements.
                    PlayerHealthControl();  
                    LevelControl();
                    LevelUpMessageControl();
                    LevelUpControl();
                    
                    UpdateGame();  // Update game elements (e.g., enemies, collectibles, etc.).
                    CheckCollisions();  // Check for collisions between game objects.
                    
                    // Draw the in-game UI (health, shield, score).
                    ui.DrawInGameUI(player.GetPlayerHealth(), player.GetPlayerShield(), score);
                }
                else
                {
                    // If the game is over, pause the game music and play end game sound.
                    soundManager.PauseGameMusic();
                    
                    soundManager.PlayGameEndSound(causeOfGameOver);  // Play the sound for game over.
                    EndGame();  // Handle the end game behavior (e.g., show final score).
                }
            }
        }
    }

    // Method to start the game by initializing game variables and creating necessary objects.
    private void StartGame()
    {
        isGameStarted = true;  // Set the game as started.
        
        // Initialize the player object, passing soundManager, textures for spaceship, bullets, and collectibles.
        player = new Spaceship(soundManager, spaceShips, bulletTexture, collectibleTexture);
        
        // Initialize lists for enemies, enemy bullets, collectibles, and explosions.
        enemies = new List<Enemy>();
        enemyBullets = new List<Bullet>();
        collectibles = new List<Collectible>();
        explosions = new List<Explosion>();
        
        // Define levels with their respective properties such as score threshold, max enemy count, and collectible chances.
        levels = new List<Level>
        {
            new Level { id = 1, scoreThreshold = 100, maxEnemyCount = 6, levelUpAnimationTime = 0.0f, isLevelPowerUpActive = false, collectibleChance = 20 },
            new Level { id = 2, scoreThreshold = 200, maxEnemyCount = 5, levelUpAnimationTime = 2.0f, isLevelPowerUpActive = false, collectibleChance = 30 },
            new Level { id = 3, scoreThreshold = 300, maxEnemyCount = 3, levelUpAnimationTime = 2.0f, isLevelPowerUpActive = false, collectibleChance = 40 },
            new Level { id = 4, scoreThreshold = 400, maxEnemyCount = 2, levelUpAnimationTime = 2.0f, isLevelPowerUpActive = false, collectibleChance = 50 },
            new Level { id = 5, scoreThreshold = 1400, maxEnemyCount = 1, levelUpAnimationTime = 2.0f, isLevelPowerUpActive = false, collectibleChance = 60 }
        };
    }

    // Method to check if the Escape key is pressed and toggle the paused state of the game.
    private void WatchKeyEscape()
    {
        // If the Escape key is pressed, toggle the paused state.
        if (Raylib.IsKeyPressed(KeyboardKey.Escape))
        {
            isGamePaused = !isGamePaused;
        }
    }

    // Method to check and control the player's health.
    private void PlayerHealthControl()
    {
        // If the player's health reaches 0 or below, set the game over state.
        if (player.GetPlayerHealth() <= 0)
        {
            causeOfGameOver = "MAYBE NEXT TIME!";  // Set the cause of game over message.
            isGameOver = true;  // Mark the game as over.
        }
    }
    
    // Method to control the level progression based on the player's score.
    private void LevelControl()
    {
        // Iterate through the list of levels to find the appropriate level based on the score.
        foreach (var lvl in levels)
        {
            // If the player's score is less than the current level's threshold, set the current level.
            if (score < lvl.scoreThreshold)
            {
                level = lvl;  // Set the level to the current level in the loop.
                break;  // Exit the loop as we've found the correct level.
            }
            // If the player has reached the final level (score threshold 1400), end the game with a congratulatory message.
            else if (lvl.scoreThreshold == 1400)
            {
                causeOfGameOver = "CONGRATULATIONS!";  // Set the cause of the game over message to a congratulatory one.
                isGameOver = true;  // Mark the game as over.
            }
        }
    }
    
    // Method to apply level-up powers or upgrades based on the current level.
    private void LevelUpControl()
    {
        // Check if level-up powers have not yet been applied for the current level.
        if (!level.isLevelPowerUpActive)
        {
            // Switch case for each level, applying the appropriate upgrades for the player's spaceship.
            switch (level.id)
            {
                case 1:
                    // Nothing happens at level 1.
                    break;
                case 2:
                    player.IncreaseDamage(10);  // Increase the player's damage by 10 at level 2.
                    break;
                case 3:
                    player.IncreaseDamage(10);  // Increase damage by 10 and speed by 2 at level 3.
                    player.IncreaseSpeed(2);
                    break;
                case 4:
                    player.IncreaseDamage(10);  // Increase damage by 10 at level 4.
                    break;
                case 5:
                    player.IncreaseShield(100);  // Increase shield by 100, damage by 30, and speed by 1 at level 5.
                    player.IncreaseDamage(30);
                    player.IncreaseSpeed(1);
                    break;
            }
            
            level.isLevelPowerUpActive = true;  // Mark that the level-up powers have been applied for this level.
        }
    }

    // Method to control the level-up message animation when the player levels up.
    private void LevelUpMessageControl()
    {
        // If the elapsed time is less than the level-up animation time, display the level-up message.
        if (level.elapsedTime < level.levelUpAnimationTime)
        {
            ui.DrawLevelUpMessage(level.elapsedTime, level.levelUpAnimationTime);  // Draw the level-up message on the UI.
            level.elapsedTime += Raylib.GetFrameTime();  // Increase the elapsed time by the frame time to progress the animation.
        }
    }
    
    // Method to update the game state by processing various game mechanics.
    private void UpdateGame()
    {
        // Process collectible items in the game (e.g., power-ups or items the player can collect).
        CollectibleProcess();
    
        // Update the player's power-ups (e.g., effects from power-ups that modify gameplay).
        player.UpdatePowerUps();
    
        // Process the player's bullets (e.g., moving or handling the player's shots).
        PlayerBulletProcess();
    
        // Process the enemy's bullets (e.g., moving or handling enemy projectiles).
        EnemyBulletProcess();
    
        // Process the enemies in the game (e.g., spawning, moving, attacking).
        EnemyProcess();
    
        // Process the player's actions (e.g., movement, collision detection, etc.).
        PlayerProcess();
    }

    // Method to handle the enemies in the game.
    private void EnemyProcess()
    {
        // Add enemies to the game based on the current level's enemy count.
        AddEnemies();
    
        // Update the behavior and actions of the enemies (e.g., movement, AI).
        UpdateEnemies();
    }

    // Method to add enemies to the game if the number of enemies is less than the max allowed for the current level.
    private void AddEnemies()
    {
        // Check if the current number of enemies is less than the maximum allowed for the level.
        if (enemies.Count < level.maxEnemyCount)
        {
            // Add a new enemy based on the player's current score.
            enemies.Add(CreateEnemyBasedOnScore());
        }
    }
    
    // Method to create an enemy based on the current score.
    private Enemy CreateEnemyBasedOnScore()
    {
        // If the score is below the threshold for level 1, return a BasicEnemy.
        if (score < levels[0].scoreThreshold)
            return new BasicEnemy(spaceShips);
        
        // If the score is below the threshold for level 2, return either a BasicEnemy or a FastEnemy randomly.
        if (score < levels[1].scoreThreshold)
            return random.Next(2) == 0 ? new BasicEnemy(spaceShips) : new FastEnemy(spaceShips);
        
        // If the score is below the threshold for level 3, return either a FastEnemy or a StrongEnemy.
        if (score < levels[2].scoreThreshold)
            return random.Next(2) == 0 ? new FastEnemy(spaceShips) : new StrongEnemy(spaceShips, bulletTexture, soundManager);
        
        // If the score is below the threshold for level 4, return a StrongEnemy.
        if (score < levels[3].scoreThreshold)
            return new StrongEnemy(spaceShips, bulletTexture, soundManager);
        
        // If the score is above the threshold for level 4, return a BossEnemy.
        return new BossEnemy(new Vector2(player.playersShipsDes.X, player.playersShipsDes.Y), spaceShips, bulletTexture, explosionTexture, soundManager);
    }

    // Method to update the enemies in the game.
    private void UpdateEnemies()
    {
        // Loop through each enemy in the game.
        foreach (var enemy in enemies)
        {
            // Calculate the center position of the player.
            int playerCenterX = (int)(player.playersShipsDes.X + player.playersShipsDes.Width);
            int playerCenterY = (int)(player.playersShipsDes.Y + player.playersShipsDes.Height);
            
            // Move the enemy towards the player's position.
            enemy.Move(playerCenterX, playerCenterY);
            
            // Make the enemy attack the player and add their bullets to the enemyBullets list.
            enemy.Attack(player, enemyBullets);
            
            // Check if the enemy is a basic enemy and handle it accordingly.
            if (enemy is BasicEnemy basicEnemy)
            {
                enemy.OutOfRange();  // Basic enemy goes out of range.
            }
            // Check if the enemy is a boss enemy and handle it accordingly.
            else if (enemy is BossEnemy bossEnemy)
            {
                bossEnemy.OutOfRangeReverse();  // Boss enemy has a different out-of-range behavior.
            }
            // Check if the number of enemies exceeds the max limit for the level.
            else if (enemies.Count > level.maxEnemyCount)
            {
                enemy.OutOfRange();  // Remove enemies if there are too many.
            }
            
            // If the enemy is destroyed and not because it's out of range, increase the score.
            if (enemy.isEnemyDestroyed && !enemy.isEnemyDestroyedBecauseOutOfRange)
            {
                IncreaseScore(enemy.point);
                
                // Random chance to drop a collectible when an enemy is destroyed.
                if (Random.Shared.Next(0, 100) < level.collectibleChance)
                {
                    // Add a collectible at the enemy's location.
                    collectibles.Add(CreateRandomCollectible(enemy.enemyShipsDes.X, enemy.enemyShipsDes.Y));
                }
            }
        }
        
        // Remove all destroyed enemies from the list.
        enemies.RemoveAll(enemy => enemy.isEnemyDestroyed);
    }
    
    // Method to increase the score by a specified amount.
    public void IncreaseScore(int amount)
    {
        score += amount;
    }
    
    // Method to create a random collectible at the given position.
    private Collectible CreateRandomCollectible(float positionX, float positionY)
    {
        // Generate a random number between 0 and 6 to determine the type of collectible.
        int randomType = random.Next(7);

        Collectible collectible = null;

        // Based on the random number, create a specific type of collectible.
        switch (randomType)
        {
            case 0:
                collectible = new HealthCollectible(positionX, positionY, collectibleTexture);
                break;
            case 1:
                collectible = new SpeedBoostCollectible(positionX, positionY, collectibleTexture);
                break;
            case 2:
                collectible = new DamageBoostCollectible(positionX, positionY, collectibleTexture);
                break;
            case 3:
                collectible = new ShieldCollectible(positionX, positionY, collectibleTexture);
                break;
            case 4:
                collectible = new DoubleFireCollectible(positionX, positionY, collectibleTexture);
                break;
            case 5:
                collectible = new ScoreBoostCollectible(positionX, positionY, collectibleTexture, this);
                break;
            case 6:
                collectible = new InvisibilityCollectible(positionX, positionY, collectibleTexture);
                break;
        }

        // Return the created collectible.
        return collectible;
    }

    // Method to process the player's actions.
    private void PlayerProcess()
    {
        // Move the player based on input or AI logic.
        player.Move();
        
        // Make the player shoot if they are allowed to.
        player.Shoot();
    }

    // Method to process the player's bullets.
    private void PlayerBulletProcess()
    {
        // Loop through each bullet the player has fired.
        foreach (var bullet in player.bullets)
        {
            // Move the bullet according to its trajectory.
            bullet.Move();
        }
        
        // Remove all bullets that are destroyed from the list.
        player.bullets.RemoveAll(bullet => bullet.isBulletDestroyed);
    }
    
    // Method to process the bullets fired by enemies.
    private void EnemyBulletProcess()
    {
        // Loop through each bullet fired by enemies.
        foreach (var bullet in enemyBullets) 
        { 
            // Move the enemy bullet.
            bullet.Move();
        }
    
        // Remove bullets that are destroyed or finished their animation (in case of EnergyBullet).
        enemyBullets.RemoveAll(bullet => bullet.isBulletDestroyed || (bullet is EnergyBullet energyBullet && energyBullet.IsAnimationFinished()));
    }

    // Method to process the collectibles in the game.
    private void CollectibleProcess()
    {
        // Loop through each collectible item in the game.
        foreach (var collectible in collectibles) 
        { 
            // Draw the collectible on the screen.
            collectible.DrawCollectible();
        }
    
        // Remove the collectibles that have been collected by the player.
        collectibles.RemoveAll(collectible => collectible.isCollected);
    }

    // Method to check for collisions in the game.
    private void CheckCollisions()
    {
        // Check collision between player and enemy.
        CheckPlayerEnemyCollision();
    
        // Check for collisions between player bullets and enemies.
        CollisionDetector.CheckBulletCollision(player.bullets, enemies, soundManager);
    
        // Check for collisions between enemy bullets and the player.
        CollisionDetector.CheckEnemyBulletCollision(player, enemyBullets, soundManager);
    
        // Check if the player collects any collectibles.
        CollisionDetector.CheckPlayerCollectibleCollision(player, collectibles, soundManager);
    }

    // Method to check if the player collides with any enemies.
    private void CheckPlayerEnemyCollision()
    {
        // Loop through each enemy to check for collisions with the player.
        foreach (var enemy in enemies)
        {
            // If there is a collision between the player and the enemy.
            if (CollisionDetector.CheckCollision(player, enemy))
            {
                // Only process collision if neither the player nor the enemy is already damaged.
                if (!player.IsDamaged && !enemy.IsDamaged)
                {
                    // If player isn't invisible, play the collision sound and apply damage to both.
                    if (!player.isInvisibilityActive)
                    {
                        soundManager.PlayCollisionSound();
                        
                        // Player and enemy both take damage.
                        player.TakeDamage(10);
                        enemy.TakeDamage(10);
                    }
                    
                    // Set the player and enemy as damaged to prevent multiple damage events in the same frame.
                    player.IsDamaged = true;
                    enemy.IsDamaged = true;
                }
            }
            else
            {
                // Reset the damaged state if no collision occurs.
                player.IsDamaged = false;
                enemy.IsDamaged = false;
            }
        }
    }

    // Method to handle the end of the game.
    private void EndGame()
    {
        // Save the score if it hasn't been saved yet.
        if (!isGameScoreSaved)
        {
            SaveScore();
        }
        
        // Check if the player pressed a key to end the game and show the end screen.
        isGameFinished = ui.WatchKeyPressEndScreen(isGameFinished);
        ui.DrawEndScreen(causeOfGameOver, score);
    }

    // Method to save the game score to a file.
    private void SaveScore()
    {
        // Define the path where the score will be saved.
        string filePath = "./saved/Scores.txt";
        
        // Set the cause of the game over as either "LOSE" or "WIN".
        string causeOfEnd = causeOfGameOver == "MAYBE NEXT TIME!" ? "LOSE" : "WIN ";

        try
        {
            // Create the score entry string with the current date and score.
            string scoreEntry = $"{DateTime.Now} ||      {causeOfEnd}    || Score: {score}{Environment.NewLine}";
            
            // Append the score entry to the file.
            File.AppendAllText(filePath, scoreEntry);

            // Inform the console that the score was saved successfully.
            Console.WriteLine("Game score saved successfully.");
            
            // Mark the score as saved.
            isGameScoreSaved = true;
        }
        catch (Exception ex)
        {
            // In case of an error, log the error message.
            Console.WriteLine($"FILE SAVE ERROR: {ex.Message}");
        }
    }
}