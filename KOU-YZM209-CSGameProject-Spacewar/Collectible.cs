using Raylib_cs;
using System.Numerics;

namespace KOU_YZM209_CSGameProject_Spacewar;

// Abstract base class for all collectible items
public abstract class Collectible
{
    private Texture2D collectibleTexture; // Texture for the collectible
    protected Rectangle collectibleRec; // Source rectangle for the texture
    public Rectangle collectibleDes = Utils.ScaleImage(0, 0, 32, 32, Raylib.GetScreenWidth(), Raylib.GetScreenHeight()); // Scaled destination rectangle
    
    public bool isCollected = false; // Determines if the collectible has been collected
    
    private float scaleAnimation = 1.0f; // Scale animation factor
    private float animationTime = 0.0f; // Time for scale animation
    protected Action<Spaceship>? onCollectAction; // Delegate for custom behavior when collected

    // Constructor for base collectible
    protected Collectible(float positionX, float positionY, Texture2D collectibleTexture, Action<Spaceship> onCollectAction)
    {
        collectibleDes.X = positionX;
        collectibleDes.Y = positionY;
        this.collectibleTexture = collectibleTexture;
        this.onCollectAction = onCollectAction;
    }

    // Draws the collectible with an animated scale effect
    public void DrawCollectible()
    {
        animationTime += Raylib.GetFrameTime();
        scaleAnimation = 1.0f + 0.1f * (float)Math.Sin(animationTime * 5 * (Math.PI / 2)); // Smooth scale animation
        
        Rectangle scaledDes = new Rectangle(collectibleDes.X, collectibleDes.Y, collectibleDes.Width * scaleAnimation, collectibleDes.Height * scaleAnimation);
        
        Raylib.DrawTexturePro(collectibleTexture, collectibleRec, scaledDes, new Vector2(scaledDes.Width / 2, scaledDes.Height / 2), 0, Color.RayWhite);
    }

    // Called when the collectible is collected
    public void OnCollect(Spaceship player)
    {
        onCollectAction?.Invoke(player); // Execute the custom behavior
        isCollected = true;
    }
}

// Health collectible: Increases player's health
public class HealthCollectible : Collectible
{
    public HealthCollectible(float positionX, float positionY, Texture2D collectibleTexture) : base(positionX, positionY, collectibleTexture, player => player.IncreaseHealth(20))
    {
        collectibleRec = new Rectangle(0, 0, 64, 64); // Set texture source rectangle
    }
}

// Speed boost collectible: Increases player's speed temporarily
public class SpeedBoostCollectible : Collectible
{
    public SpeedBoostCollectible(float positionX, float positionY, Texture2D collectibleTexture) : base(positionX, positionY, collectibleTexture, player => player.IncreaseSpeedBoostTime(5.0f))
    {
        collectibleRec = new Rectangle(0, 64, 64, 64);
    }
}

// Damage boost collectible: Increases player's damage temporarily
public class DamageBoostCollectible : Collectible
{
    public DamageBoostCollectible(float positionX, float positionY, Texture2D collectibleTexture) : base(positionX, positionY, collectibleTexture, player => player.IncreaseDamageBoostTime(5.0f))
    {
        collectibleRec = new Rectangle(64, 64, 64, 64);
    }
}

// Shield collectible: Increases player's shield capacity
public class ShieldCollectible : Collectible
{
    public ShieldCollectible(float positionX, float positionY, Texture2D collectibleTexture) : base(positionX, positionY, collectibleTexture, player => player.IncreaseShield(100))
    {
        collectibleRec = new Rectangle(64, 0, 64, 64);
    }
}

// Double fire collectible: Enables double fire mode for the player
public class DoubleFireCollectible : Collectible
{
    public DoubleFireCollectible(float positionX, float positionY, Texture2D collectibleTexture) : base(positionX, positionY, collectibleTexture, player => player.IncreaseDoubleFireTime(10.0f))
    {
        collectibleRec = new Rectangle(128, 64, 64, 64);
    }
}

// Score boost collectible: Increases the game's score
public class ScoreBoostCollectible : Collectible
{
    private Game game; // Reference to the Game class instance

    public ScoreBoostCollectible(float positionX, float positionY, Texture2D collectibleTexture, Game game) : base(positionX, positionY, collectibleTexture, player => { })
    {
        this.game = game; // Store the Game instance to access its methods
        collectibleRec = new Rectangle(128, 0, 64, 64); // Define the texture source rectangle
    }

    public new void OnCollect(Spaceship player)
    {
        game.IncreaseScore(50); // Increase the game score by 50
        isCollected = true; // Mark the collectible as collected
    }
}

// Invisibility collectible: Makes the player invisible temporarily
public class InvisibilityCollectible : Collectible
{
    public InvisibilityCollectible(float positionX, float positionY, Texture2D collectibleTexture) : base(positionX, positionY, collectibleTexture, player => player.IncreaseInvisibilityTime(5.0f))
    {
        collectibleRec = new Rectangle(320, 0, 88, 88);
    }
}