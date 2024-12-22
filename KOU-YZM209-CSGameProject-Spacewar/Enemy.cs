using System.Diagnostics;
using Raylib_cs;
using System.Numerics;

namespace KOU_YZM209_CSGameProject_Spacewar;

// Abstract base class for all types of enemy ships
public abstract class Enemy 
{
    protected const double RAD2DEG = 180.0 / Math.PI;
    protected const double DEG2RAD = Math.PI / 180; 
    
    protected static Random random = new Random();

    protected SoundManager soundManager;
        
    public bool IsDamaged = false; // Flag indicating if the enemy has taken damage
    public bool isEnemyDestroyed = false; // Flag indicating if the enemy is destroyed
    public bool isEnemyDestroyedBecauseOutOfRange = false; // Flag indicating if the enemy was destroyed because it went out of range
    
    public int point; // Points awarded for destroying the enemy
    protected int health; // Enemy's health
    protected int speed; // Enemy's movement speed
    protected int damage; // Damage dealt by the enemy
    
    protected int spawnX; // X position for enemy spawn
    protected int spawnY; // Y position for enemy spawn

    protected Texture2D 
        spaceShips, // Texture for the enemy ship
        bulletTexture; // Texture for the enemy's bullets
        
    public Rectangle
        enemyShipRec, // Rectangle defining the source area of the enemy ship in the texture
        enemyShipsDes; // Rectangle defining the destination area for drawing the enemy ship
        
    public float rotationAngle; // Rotation angle for the enemy ship

    protected float lastShootTime = 0.0f; // Time of the last shot
    protected bool isShooting = false; // Flag indicating if the enemy is shooting
    protected float shootStartTime = 0.0f; // Time when shooting started
    
    protected Enemy(Texture2D spaceShips)
    { 
        this.spaceShips = spaceShips;
    }
    
    // Abstract method for moving the enemy, to be implemented by subclasses
    public abstract void Move(int playersPositionX, int playersPositionY); 
    
    // Abstract method for attacking, to be implemented by subclasses
    public abstract void Attack(Spaceship player, List<Bullet> bullets);
    
    // Method to apply damage to the enemy
    public void TakeDamage(int amount)
    { 
        health -= amount; 
        if (health <= 0) Destroy();
    }

    // Destroy the enemy (set the destruction flag)
    private void Destroy()
    {
        isEnemyDestroyed = true;
    }
        
    // Check if the enemy is out of range (off-screen)
    public void OutOfRange()
    {
        if (enemyShipsDes.X < -150 || enemyShipsDes.X > Raylib.GetScreenWidth() + 150 ||
            enemyShipsDes.Y < -150 || enemyShipsDes.Y > Raylib.GetScreenHeight() + 150)
        {
            isEnemyDestroyed = true;
            isEnemyDestroyedBecauseOutOfRange = true;
        }
    }
        
    // Calculate the angle towards the player's position
    protected float FindShortestPathAngle(Vector2 playerPosition, Vector2 enemyPosition)
    {
        Vector2 directionToPlayer = Vector2.Normalize(playerPosition - enemyPosition);
        return (float)(Math.Atan2(directionToPlayer.Y, directionToPlayer.X) * RAD2DEG) + 90;
    }
}

// Basic enemy class with simple movement and no special attack
public class BasicEnemy : Enemy
{
    public BasicEnemy(Texture2D spaceShips) : base(spaceShips)
    {
        point = 20;
        health = 50;
        speed = 2;
        damage = 10;
        
        enemyShipRec = new Rectangle(118, 0, 88, 89);
        enemyShipsDes = Utils.ScaleImage(0, 0, 57, 58, Raylib.GetScreenWidth(), Raylib.GetScreenHeight());
        SpawnPosition();
    }
    
    // Set the spawn position randomly on the left or right side of the screen
    private void SpawnPosition()
    {
        int screenWidth = Raylib.GetScreenWidth();
        int screenHeight = Raylib.GetScreenHeight();
        
        int side = random.Next(2);

        switch (side)
        {
            case 0: // left
                spawnX = -random.Next(50, 100);
                spawnY = random.Next(100, screenHeight - 100);
                rotationAngle = 90;
                break;
            case 1: // right
                spawnX = screenWidth + random.Next(50, 100);
                spawnY = random.Next(100, screenHeight - 100);
                rotationAngle = 270;
                break;
        }

        enemyShipsDes.X = spawnX;
        enemyShipsDes.Y = spawnY;
    }
    
    // Move the enemy by a fixed speed in the current direction
    public override void Move(int playersPositionX, int playersPositionY)
    {
        enemyShipsDes.X += (float)Math.Sin(rotationAngle * DEG2RAD) * speed;
        
        Raylib.DrawTexturePro(spaceShips, enemyShipRec, enemyShipsDes, new Vector2(enemyShipsDes.Width/2, enemyShipsDes.Height/2), rotationAngle, Color.RayWhite);
    }
    
    // Basic enemies do not attack
    public override void Attack(Spaceship player, List<Bullet> bullets)
    {
        // No special attack
    }
}

// Fast enemy with more complex movement and no attack
public class FastEnemy : Enemy
{
    private double lastActionTime = Raylib.GetTime() - 6;
    private double actionStartTime = 0.0;
    private Vector2 directionVector;
    
    public FastEnemy(Texture2D spaceShips) : base(spaceShips)
    {
        point = 20;
        health = 30;
        speed = 4;
        damage = 20;
        
        enemyShipRec = new Rectangle(206, 0, 127, 137);
        enemyShipsDes = Utils.ScaleImage(0, 0, 77, 84, Raylib.GetScreenWidth(), Raylib.GetScreenHeight());
        SpawnPosition();
    }
    
    // Set the spawn position randomly on any of the four sides of the screen
    private void SpawnPosition()
    {
        int screenWidth = Raylib.GetScreenWidth();
        int screenHeight = Raylib.GetScreenHeight();
        int side = random.Next(4);

        switch (side)
        {
            case 0: // left
                spawnX = -random.Next(50, 100);
                spawnY = random.Next(0, screenHeight);
                break;
            case 1: // right
                spawnX = screenWidth + random.Next(50, 100);
                spawnY = random.Next(0, screenHeight);
                break;
            case 2: // top
                spawnX = random.Next(0, screenWidth);
                spawnY = -random.Next(50, 100);
                break;
            case 3: // bottom
                spawnX = random.Next(0, screenWidth);
                spawnY = screenHeight + random.Next(50, 100);
                break;
        }

        enemyShipsDes.X = spawnX;
        enemyShipsDes.Y = spawnY;
    }
    
    // Move the enemy with complex movement logic
    public override void Move(int playersPositionX, int playersPositionY)
    {
        double currentTime = Raylib.GetTime();
        
        Vector2 playerPosition = new Vector2(playersPositionX, playersPositionY);
        Vector2 enemyPosition = new Vector2(enemyShipsDes.X, enemyShipsDes.Y);
        
        if (currentTime - lastActionTime >= 5.0)
        {
            lastActionTime = currentTime;
            actionStartTime = currentTime;
        }
        
        if (currentTime - actionStartTime <= 2.0)
        {
            rotationAngle = Utils.Lerp(rotationAngle, FindShortestPathAngle(playerPosition, enemyPosition), 0.05f);
        }
        
        enemyShipsDes.X += (float)Math.Sin(rotationAngle * DEG2RAD) * speed;
        enemyShipsDes.Y -= (float)Math.Cos(rotationAngle * DEG2RAD) * speed;
        
        Raylib.DrawTexturePro(spaceShips, enemyShipRec, enemyShipsDes, new Vector2(enemyShipsDes.Width / 2, enemyShipsDes.Height / 2), rotationAngle, Color.RayWhite);
    }
    
    // Fast enemies do not attack
    public override void Attack(Spaceship player, List<Bullet> bullets)
    {
        // No attack, just hit
    }
}

// Strong enemy with shooting capabilities
public class StrongEnemy : Enemy
{
    public StrongEnemy(Texture2D spaceShips, Texture2D bulletTexture, SoundManager soundManager) : base(spaceShips)
    {
        this.soundManager = soundManager;
        this.bulletTexture = bulletTexture;

        point = 50;
        health = 150;
        speed = 1;
        damage = 30;
        
        enemyShipRec = new Rectangle(333, 0, 177, 118);
        enemyShipsDes = Utils.ScaleImage(0, 0, 129, 86, Raylib.GetScreenWidth(), Raylib.GetScreenHeight());
        SpawnPosition();
    }

    // Set the spawn position randomly on any of the four sides of the screen
    private void SpawnPosition()
    {
        int screenWidth = Raylib.GetScreenWidth();
        int screenHeight = Raylib.GetScreenHeight();

        int side = random.Next(4);
        
        switch (side)
        {
            case 0: // left
                spawnX = -random.Next(50, 100);
                spawnY = random.Next(0, screenHeight);
                break;
            case 1: // right
                spawnX = screenWidth + random.Next(50, 100);
                spawnY = random.Next(0, screenHeight);
                break;
            case 2: // top
                spawnX = random.Next(0, screenWidth);
                spawnY = -random.Next(50, 100);
                break;
            case 3: // bottom
                spawnX = random.Next(0, screenWidth);
                spawnY = screenHeight + random.Next(50, 100);
                break;
        }

        enemyShipsDes.X = spawnX;
        enemyShipsDes.Y = spawnY;
    }

    // Move the enemy towards the player
    public override void Move(int playersPositionX, int playersPositionY)
    {
        Vector2 playerPosition = new Vector2(playersPositionX, playersPositionY);
        Vector2 enemyPosition = new Vector2(enemyShipsDes.X, enemyShipsDes.Y);
        
        rotationAngle = FindShortestPathAngle(playerPosition, enemyPosition);

        enemyShipsDes.X += (float)Math.Sin(rotationAngle * DEG2RAD) * speed;
        enemyShipsDes.Y -= (float)Math.Cos(rotationAngle * DEG2RAD) * speed;
        
        Raylib.DrawTexturePro(spaceShips, enemyShipRec, enemyShipsDes, new Vector2(enemyShipsDes.Width/2, enemyShipsDes.Height/2), rotationAngle, Color.RayWhite);
    }
    
    // Strong enemies shoot at the player
    public override void Attack(Spaceship player, List<Bullet> bullets)
    {
        float currentTime = (float)Raylib.GetTime();
        
        if (currentTime - lastShootTime >= 5.0f && !isShooting)
        {
            isShooting = true;
            shootStartTime = currentTime;
            lastShootTime = currentTime;
        }
        
        if (isShooting && currentTime - shootStartTime <= 2.0f)
        {
            if (currentTime - lastShootTime >= 0.5f)
            {
                soundManager.PlayEnemyShootEffect();
                
                Vector2
                    center = new Vector2(enemyShipsDes.X, enemyShipsDes.Y),
                    startPointOne = new Vector2(enemyShipsDes.X - enemyShipsDes.Width / 3, enemyShipsDes.Y - enemyShipsDes.Height / 2),
                    startPointTwo = new Vector2(enemyShipsDes.X + enemyShipsDes.Width / 3, enemyShipsDes.Y - enemyShipsDes.Height / 2),
                    rotatedStartPointOne = Utils.RotatePoint(startPointOne, center, rotationAngle),
                    rotatedStartPointTwo = Utils.RotatePoint(startPointTwo, center, rotationAngle);
                
                CreateBullet(rotatedStartPointOne, bullets);
                CreateBullet(rotatedStartPointTwo, bullets);
                
                lastShootTime = currentTime;
            }
        }
        else
        {
            if (currentTime - shootStartTime > 2.0f)
            {
                isShooting = false;
            }
        }
    }
    
    // Create a bullet at the specified start point
    private void CreateBullet(Vector2 startPoint, List<Bullet> bullets)
    {
        Bullet bullet = new StrongBullet((int)startPoint.X, (int)startPoint.Y, (int)rotationAngle, damage, bulletTexture);
        bullets.Add(bullet);
    }
}

public class BossEnemy : Enemy
{
    private Vector2 playerPosition;
    private Texture2D explosionTexture;

    // Constructor that initializes the BossEnemy with the player's position, textures, and sound manager
    public BossEnemy(Vector2 playerPosition, Texture2D spaceShips, Texture2D bulletTexture, Texture2D explosionTexture, SoundManager soundManager) : base(spaceShips)
    {
        this.playerPosition = playerPosition;
        this.bulletTexture = bulletTexture;
        this.explosionTexture = explosionTexture;
        this.soundManager = soundManager;

        point = 1000; // Points awarded for defeating the boss
        health = 1000; // Health of the boss
        speed = 1;    // Movement speed of the boss
        damage = 40;  // Damage dealt by the boss

        enemyShipRec = new Rectangle(510, 0, 224, 156); // Texture coordinates for the boss ship
        enemyShipsDes = Utils.ScaleImage(0, 0, 224, 156, Raylib.GetScreenWidth(), Raylib.GetScreenHeight()); // Scaled size for the boss ship
        SpawnPosition(); // Sets the initial position of the boss
    }

    // Method to randomly determine the spawn position of the boss
    private void SpawnPosition()
    {
        int screenWidth = Raylib.GetScreenWidth();
        int screenHeight = Raylib.GetScreenHeight();

        int side = random.Next(2); // Randomly choose which side of the screen to spawn from
        
        switch (side)
        {
            case 0: // Spawn from the left
                spawnX = -random.Next(50, 100);
                spawnY = random.Next(100, screenHeight);
                rotationAngle = 90; // Set rotation angle for left spawn
                break;
            case 1: // Spawn from the right
                spawnX = screenWidth + random.Next(50, 100);
                spawnY = random.Next(100, screenHeight);
                rotationAngle = 270; // Set rotation angle for right spawn
                break;
        }

        enemyShipsDes.X = spawnX;
        enemyShipsDes.Y = spawnY;
    }

    // Method to handle the boss ship when it goes out of range
    public void OutOfRangeReverse()
    {
        // Check if the boss ship is out of the screen bounds
        if (enemyShipsDes.X < -150 || enemyShipsDes.X > Raylib.GetScreenWidth() + 150 ||
            enemyShipsDes.Y < -150 || enemyShipsDes.Y > Raylib.GetScreenHeight() + 150)
        {
            rotationAngle = rotationAngle == 90 ? 270 : 90; // Reverse the rotation direction
            enemyShipsDes.Y = random.Next(100, Raylib.GetScreenHeight() - 100); // Reset the vertical position
        }
    }

    // Method to control the movement of the boss ship towards the player
    public override void Move(int playersPositionX, int playersPositionY)
    {
        enemyShipsDes.X += (float)Math.Sin(rotationAngle * DEG2RAD) * speed; // Move the boss horizontally based on its rotation
        
        // Draw the boss ship with the current rotation and position
        Raylib.DrawTexturePro(spaceShips, enemyShipRec, enemyShipsDes, new Vector2(enemyShipsDes.Width/2, enemyShipsDes.Height/2), rotationAngle, Color.RayWhite);
    }

    // Method for the boss to attack the player
    public override void Attack(Spaceship player, List<Bullet> bullets)
    {
        float currentTime = (float)Raylib.GetTime(); // Get the current time

        // Check if enough time has passed to fire another bullet
        if (currentTime - lastShootTime >= 1.5f && !isShooting)
        {
            isShooting = true; // Set the shooting flag to true
        }
        
        if (isShooting)
        {
            soundManager.PlayBossShootEffect(); // Play the boss shooting sound effect

            // Randomly decide which type of bullet to shoot
            Bullet newBullet = random.Next(2) == 0 ? CreateHomingBullet(player) : CreateEnergyBullet(player);

            bullets.Add(newBullet); // Add the bullet to the list

            lastShootTime = currentTime; // Update the last shoot time
            isShooting = false; // Reset the shooting flag
        }
    }

    // Method to create a homing bullet that tracks the player's position
    private Bullet CreateHomingBullet(Spaceship player)
    {
        Vector2
            center = new Vector2(enemyShipsDes.X, enemyShipsDes.Y),
            startPoint = new Vector2(enemyShipsDes.X, enemyShipsDes.Y - enemyShipsDes.Height / 2),
            rotatedStartPoint = Utils.RotatePoint(startPoint, center, rotationAngle); // Calculate the rotated start point for the bullet

        return new HomingBullet(player, (int)rotatedStartPoint.X, (int)rotatedStartPoint.Y, (int)rotationAngle, damage, bulletTexture); // Return a new homing bullet
    }

    // Method to create an energy bullet that targets the player
    private Bullet CreateEnergyBullet(Spaceship player)
    {
        Vector2
            center = new Vector2(enemyShipsDes.X, enemyShipsDes.Y),
            startPoint = new Vector2(enemyShipsDes.X, enemyShipsDes.Y - enemyShipsDes.Height / 2),
            rotatedStartPoint = Utils.RotatePoint(startPoint, center, rotationAngle); // Calculate the rotated start point for the bullet

        return new EnergyBullet(new Vector2(player.playersShipsDes.X, player.playersShipsDes.Y), (int)rotatedStartPoint.X, (int)rotatedStartPoint.Y, (int)rotationAngle, damage, bulletTexture, explosionTexture, soundManager); // Return a new energy bullet
    }
}