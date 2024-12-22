using Raylib_cs;
using System.Numerics;

namespace KOU_YZM209_CSGameProject_Spacewar;

public class Spaceship
{
    const double DEG2RAD = Math.PI / 180;
    
    private SoundManager soundManager;
    private const int MaxHealth = 100;
    private const int MaxShield = 100;

    private int health = MaxHealth; // Player's health
    private int shield = 0; // Player's shield
    private int speed = 3; // Movement speed
    private int damage = 10; // Damage dealt by the spaceship
    private int rotationSpeed = 3; // Rotation speed of the spaceship
    public int rotation = 0; // Current rotation angle

    public bool IsDamaged = false; // Flag to check if the spaceship is damaged
    private bool isShootInCooldown = false; // Flag to check if the spaceship is in shoot cooldown
    private float lastShootTime; // The last time the spaceship shot

    private float speedBoostTimer = 0.0f; // Timer for speed boost power-up
    private bool isSpeedBoostActive = false; // Flag to check if speed boost is active
    private float damageBoostTimer = 0.0f; // Timer for damage boost power-up
    private bool isDamageBoostActive = false; // Flag to check if damage boost is active
    private float doubleFireTimer = 0.0f; // Timer for double fire power-up
    private bool isDoubleFireActive = false; // Flag to check if double fire is active
    private float invisibilityTimer = 0.0f; // Timer for invisibility power-up
    public bool isInvisibilityActive = false; // Flag to check if invisibility is active

    public List<Bullet> bullets; // List to store bullets fired by the spaceship

    private Texture2D 
        spaceShips, 
        bulletTexture, 
        collectibleTexture; // Textures for the spaceship, bullets, and collectibles
    
    public Rectangle 
        playersShipRec = new Rectangle(0, 0, 118, 108), // The original spaceship texture rectangle
        playersShipsDes = Utils.ScaleImage(960, 540, 77, 70, Raylib.GetScreenWidth(), Raylib.GetScreenHeight()), // The scaled spaceship texture rectangle
        shieldbBarrierRec = new Rectangle(192, 0, 128, 128), // Shield texture rectangle
        shieldbBarrierDes = Utils.ScaleImage(937, 516, 123, 128, Raylib.GetScreenWidth(), Raylib.GetScreenHeight()); // Scaled shield texture rectangle

    private Vector2 
        origin = new Vector2(0, 0); // Origin point for rotation

    // Constructor to initialize the spaceship with necessary components
    public Spaceship(SoundManager soundManager, Texture2D spaceShips, Texture2D bulletTexture, Texture2D collectibleTexture)
    {
        this.bullets = new List<Bullet>();
        this.soundManager = soundManager;
        this.spaceShips = spaceShips;
        this.bulletTexture = bulletTexture;
        this.collectibleTexture = collectibleTexture;
    }

    // Method to handle spaceship movement and rotation
    public void Move()
    {
        HandleRotation();
        HandleMovement();
        DrawShield();
        DrawSpaceShip();
    }

    // Method to handle spaceship rotation based on key inputs
    private void HandleRotation()
    {
        if (Raylib.IsKeyDown(KeyboardKey.A))
            rotation -= rotationSpeed; // Rotate counterclockwise

        if (Raylib.IsKeyDown(KeyboardKey.D))
            rotation += rotationSpeed; // Rotate clockwise
    }

    // Method to handle spaceship movement based on key inputs
    private void HandleMovement()
    {
        if (Raylib.IsKeyDown(KeyboardKey.W))
            MoveSpaceship(speed); // Move up (forward)

        if (Raylib.IsKeyDown(KeyboardKey.S))
            MoveSpaceship(-speed); // Move down (backward)
    }

    // Method to move the spaceship in the direction of its rotation
    private void MoveSpaceship(int moveSpeed)
    {
        playersShipsDes.X += (float)Math.Sin(rotation * DEG2RAD) * moveSpeed; // Calculate X movement
        playersShipsDes.Y -= (float)Math.Cos(rotation * DEG2RAD) * moveSpeed; // Calculate Y movement
        ConstrainPositionWithinBounds(); // Constrain position within screen bounds
    }

    // Method to constrain spaceship's position within screen bounds
    private void ConstrainPositionWithinBounds()
    {
        playersShipsDes.X = Math.Clamp(playersShipsDes.X, 0, Raylib.GetScreenWidth()); // X position must be within screen width
        playersShipsDes.Y = Math.Clamp(playersShipsDes.Y, 0, Raylib.GetScreenHeight()); // Y position must be within screen height
    }

    // Method to handle shooting functionality
    public void Shoot()
    {
        if (!isShootInCooldown) // Check if shooting is not on cooldown
        {
            if (Raylib.IsKeyDown(KeyboardKey.Space)) // Check if space key is pressed
            {
                soundManager.PlayPlayerShootEffect(); // Play shooting sound effect
                isShootInCooldown = true; // Start shoot cooldown
                lastShootTime = (float)Raylib.GetTime(); // Record the time of the shot

                Vector2 center = new Vector2(playersShipsDes.X, playersShipsDes.Y); // Get the center of the spaceship
                
                if (!isDoubleFireActive) // Single fire mode
                {
                    Vector2
                        startPoint = new Vector2(playersShipsDes.X, playersShipsDes.Y - playersShipsDes.Height / 2), // Calculate the starting point of the bullet
                        rotatedStartPoint = Utils.RotatePoint(startPoint, center, rotation); // Rotate the start point based on the spaceship's rotation
                
                    CreateBullet(rotatedStartPoint); // Create a single bullet
                }
                else // Double fire mode
                {
                    Vector2
                        startPointOne = new Vector2(playersShipsDes.X - playersShipsDes.Width / 3, playersShipsDes.Y - playersShipsDes.Height / 2), // Calculate the first bullet's start point
                        startPointTwo = new Vector2(playersShipsDes.X + playersShipsDes.Width / 3, playersShipsDes.Y - playersShipsDes.Height / 2), // Calculate the second bullet's start point
                        rotatedStartPointOne = Utils.RotatePoint(startPointOne, center, rotation), // Rotate the first bullet's start point
                        rotatedStartPointTwo = Utils.RotatePoint(startPointTwo, center, rotation); // Rotate the second bullet's start point
                    
                    CreateBullet(rotatedStartPointOne); // Create the first bullet
                    CreateBullet(rotatedStartPointTwo); // Create the second bullet
                }
            }
        }
        else // If on cooldown, check if the cooldown period has passed
        {
            float cureentTime = (float)Raylib.GetTime(); // Get current time

            if (cureentTime - lastShootTime >= 0.2) // If 0.2 seconds have passed
            {
                isShootInCooldown = false; // End cooldown
            }
        }
    }

    // Method to create a new bullet based on the starting point
    private void CreateBullet(Vector2 startPoint)
    {
        Bullet bullet = new PlayerBullet((int)startPoint.X, (int)startPoint.Y, rotation, damage, bulletTexture); // Create a new bullet
        bullets.Add(bullet); // Add bullet to the list
    }

    // Method to handle taking damage
    public void TakeDamage(int amount)
    {
        if (shield > 0) // If shield is active
        {
            shield -= amount; // Reduce shield by damage amount
            if (shield < 0) // If shield goes below 0
            {
                health = Math.Max(health + shield, 0); // Reduce health by the remaining damage
                shield = 0; // Shield becomes 0
            }
        }
        else // If no shield, reduce health directly
        {
            health = Math.Max(health - amount, 0); // Reduce health by the damage amount
        }
    }

    // Getter for health
    public int GetPlayerHealth() => health;
    
    // Getter for shield
    public int GetPlayerShield() => shield;

    // Method to increase health
    public void IncreaseHealth(int amount) => health = Math.Min(health + amount, MaxHealth);

    // Method to increase shield
    public void IncreaseShield(int amount) => shield = Math.Min(shield + amount, MaxShield);
    
    // Method to increase speed
    public void IncreaseSpeed(int amount) => speed += amount;

    // Method to decrease speed
    private void DecreaseSpeed(int amount) => speed = Math.Max(speed - amount, 1);

    // Method to increase damage
    public void IncreaseDamage(int amount) => damage += amount;

    // Method to decrease damage
    private void DecreaseDamage(int amount) => damage = Math.Max(damage - amount, 10);
    
    // Method to increase speed boost time
    public void IncreaseSpeedBoostTime(float amount) => speedBoostTimer += amount;
    
    // Method to increase damage boost time
    public void IncreaseDamageBoostTime(float amount) => damageBoostTimer += amount;
    
    // Method to increase double fire time
    public void IncreaseDoubleFireTime(float amount) => doubleFireTimer += amount;
    
    // Method to increase invisibility time
    public void IncreaseInvisibilityTime(float amount) => invisibilityTimer += amount;
    
    // Method to update power-ups
    public void UpdatePowerUps()
    {
        UpdateSpeedBoost(); // Update speed boost
        UpdateDamageBoost(); // Update damage boost
        UpdateDoubleFire(); // Update double fire
        UpdateInvisibility(); // Update invisibility
    }

    // Method to handle speed boost power-up
    private void UpdateSpeedBoost()
    {
        if (speedBoostTimer > 0) // If speed boost time is greater than 0
        {
            if (!isSpeedBoostActive) // If speed boost is not already active
            {
                IncreaseSpeed(2); // Increase speed
                isSpeedBoostActive = true; // Set speed boost to active
            }
            speedBoostTimer = Math.Max(speedBoostTimer - Raylib.GetFrameTime(), 0.0f); // Decrease speed boost time
        }
        else if (isSpeedBoostActive) // If speed boost timer is finished
        {
            DecreaseSpeed(2); // Decrease speed
            isSpeedBoostActive = false; // Set speed boost to inactive
        }
    }

    // Method to handle damage boost power-up
    private void UpdateDamageBoost()
    {
        if (damageBoostTimer > 0) // If damage boost time is greater than 0
        {
            if (!isDamageBoostActive) // If damage boost is not already active
            {
                IncreaseDamage(10); // Increase damage
                isDamageBoostActive = true; // Set damage boost to active
            }
            damageBoostTimer = Math.Max(damageBoostTimer - Raylib.GetFrameTime(), 0.0f); // Decrease damage boost time
        }
        else if (isDamageBoostActive) // If damage boost timer is finished
        {
            DecreaseDamage(10); // Decrease damage
            isDamageBoostActive = false; // Set damage boost to inactive
        }
    }

    // Method to handle double fire power-up
    private void UpdateDoubleFire()
    {
        if (doubleFireTimer > 0) // If double fire time is greater than 0
        {
            isDoubleFireActive = true; // Set double fire to active
            doubleFireTimer = Math.Max(doubleFireTimer - Raylib.GetFrameTime(), 0.0f); // Decrease double fire time
        }
        else
        {
            isDoubleFireActive = false; // Set double fire to inactive
        }
    }

    // Method to handle invisibility power-up
    private void UpdateInvisibility()
    {
        if (invisibilityTimer > 0) // If invisibility time is greater than 0
        {
            isInvisibilityActive = true; // Set invisibility to active
            invisibilityTimer = Math.Max(invisibilityTimer - Raylib.GetFrameTime(), 0.0f); // Decrease invisibility time
        }
        else
        {
            isInvisibilityActive = false; // Set invisibility to inactive
        }
    }

    // Draws the shield around the spaceship if the shield is greater than 0
    private void DrawShield()
    {
        if (shield > 0)
        {
            // Position the shield texture at the spaceship's position
            shieldbBarrierDes.X = playersShipsDes.X;
            shieldbBarrierDes.Y = playersShipsDes.Y;

            // Normalize the shield value to determine the transparency of the shield texture
            float normalizedShield = shield / (float)MaxShield * 255f;

            // Draw the shield texture with the calculated transparency based on remaining shield
            Raylib.DrawTexturePro(collectibleTexture, shieldbBarrierRec, shieldbBarrierDes, new Vector2(shieldbBarrierDes.Width / 2, shieldbBarrierDes.Height / 2), rotation, new Color(255, 255, 255, (int)normalizedShield));
        }
    }

    // Draws the spaceship, with transparency applied if invisibility is active
    private void DrawSpaceShip()
    {
        if (isInvisibilityActive)
        {
            // Draw the spaceship with reduced opacity if invisibility is active
            Raylib.DrawTexturePro(spaceShips, playersShipRec, playersShipsDes, new Vector2(playersShipsDes.Width / 2, playersShipsDes.Height / 2), rotation, new Color(255, 255, 255, 128));
        }
        else
        {
            // Draw the spaceship normally if invisibility is not active
            Raylib.DrawTexturePro(spaceShips, playersShipRec, playersShipsDes, new Vector2(playersShipsDes.Width / 2, playersShipsDes.Height / 2), rotation, Color.RayWhite);
        }
    }
}