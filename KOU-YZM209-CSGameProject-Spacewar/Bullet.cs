using Raylib_cs;
using System.Numerics;

namespace KOU_YZM209_CSGameProject_Spacewar;

public abstract class Bullet
{
    // Constants to convert between radians and degrees
    protected const double RAD2DEG = 180.0 / Math.PI;
    protected const double DEG2RAD = Math.PI / 180;

    public bool isBulletDestroyed = false; // Indicates whether the bullet is destroyed
    
    public int damage; // Damage dealt by the bullet
    protected int speed; // Speed of the bullet
    protected int rotationAngle; // Rotation angle of the bullet
    protected float positionX; // X-coordinate of the bullet's position
    protected float positionY; // Y-coordinate of the bullet's position
    protected Vector2 directionVector; // Direction vector of the bullet
    
    protected Texture2D bullets; // Texture for the bullet
    
    protected Rectangle bulletRec; // Rectangle defining the bullet's texture region
    public Rectangle bulletDes; // Destination rectangle for rendering the bullet
    
    public Bullet(int positionX, int positionY, int rotationAngle, int damage, Texture2D bullets)
    {
        this.positionX = positionX;
        this.positionY = positionY;
        this.rotationAngle = rotationAngle;
        this.damage = damage;
        this.bullets = bullets;
        this.directionVector = new Vector2((float)Math.Sin(rotationAngle * DEG2RAD), -(float)Math.Cos(rotationAngle * DEG2RAD));
    }

    public abstract void Move(); // Abstract method for bullet movement
    
    public void OnHit()
    {
        isBulletDestroyed = true; // Mark the bullet as destroyed upon hitting a target
    }
    
    protected void SetTexture(int sourceX, int sourceY, int width, int height)
    {
        bulletRec = new Rectangle(sourceX, sourceY, width, height); // Define the texture source rectangle
        bulletDes = Utils.ScaleImage(0, 0, width, height, Raylib.GetScreenWidth(), Raylib.GetScreenHeight()); // Scale the texture
    }
    
    protected void UpdateAndDraw()
    {
        UpdatePosition(); // Update the bullet's position
        CheckBoundaries(); // Check if the bullet goes out of bounds
        DrawBullet(); // Draw the bullet
    }
    
    protected void UpdatePosition()
    {
        positionX += directionVector.X * speed; // Update the X position
        positionY += directionVector.Y * speed; // Update the Y position
    }

    protected void DrawBullet()
    {
        bulletDes.X = positionX; // Update the destination rectangle's X position
        bulletDes.Y = positionY; // Update the destination rectangle's Y position
        
        // Draw the bullet with rotation and scaling
        Raylib.DrawTexturePro(bullets, bulletRec, bulletDes, new Vector2(bulletDes.Width / 2, bulletDes.Height / 2), rotationAngle, Color.RayWhite);
    }

    protected void CheckBoundaries()
    {
        // Destroy the bullet if it goes out of bounds
        if (positionX < -50 || positionX > Raylib.GetScreenWidth() + 50 || positionY < -50 || positionY > Raylib.GetScreenHeight() + 50)
        {
            isBulletDestroyed = true;
        }
    }
}

public class PlayerBullet : Bullet
{
    public PlayerBullet(int positionX, int positionY, int rotationAngle, int damage, Texture2D bullets) : base(positionX, positionY, rotationAngle, damage, bullets)
    { 
        speed = 5; // Set the speed of the player's bullet
        SetTexture(0, 0, 121, 126); // Set the bullet's texture region
    }

    public override void Move()
    {
        UpdateAndDraw(); // Update the bullet and render it
    }
}

public class StrongBullet : Bullet
{
    public StrongBullet(int positionX, int positionY, int rotationAngle, int damage, Texture2D bullets) : base(positionX, positionY, rotationAngle, damage, bullets)
    {
        speed = 5; // Set the speed of the strong bullet
        SetTexture(121, 0, 121, 126); // Set the bullet's texture region
    }

    public override void Move()
    {
        UpdateAndDraw(); // Update the bullet and render it
    }
}

public class HomingBullet : Bullet
{
    private Spaceship player; // Reference to the player's spaceship
    private bool isTracking = true; // Indicates if the bullet is still tracking the player
    private float traveledDistance = 0; // Distance traveled by the bullet
    private readonly float homingDistance = 100f; // Distance before starting to home in on the player
    private readonly float stopTrackingDistance = 100f; // Distance to stop homing
    private readonly float lerpFactor = 0.05f; // Factor for smoothing the direction change

    public HomingBullet(Spaceship player, int positionX, int positionY, int rotationAngle, int damage, Texture2D bullets) : base(positionX, positionY, rotationAngle, damage, bullets)
    {
        speed = 6; // Set the speed of the homing bullet
        this.player = player; // Initialize the player reference
        SetTexture(0, 126, 121, 126); // Set the bullet's texture region
    }

    public override void Move()
    {
        Vector2 playerPosition = new Vector2(player.playersShipsDes.X, player.playersShipsDes.Y); // Get the player's position
        Vector2 bulletPosition = new Vector2(positionX, positionY); // Get the bullet's position
        
        float distanceToPlayer = Vector2.Distance(bulletPosition, playerPosition); // Calculate distance to the player
        
        if (traveledDistance < homingDistance) // Move in a straight line until the homing distance is reached
        {
            traveledDistance += speed;
        }
        else if (isTracking)
        {
            if (distanceToPlayer > stopTrackingDistance) // Continue homing if the player is far enough
            {
                Vector2 directionToPlayer = Vector2.Normalize(playerPosition - bulletPosition); // Calculate direction to the player
                float adjustedLerpFactor = Math.Clamp(lerpFactor * (distanceToPlayer / stopTrackingDistance), 0.01f, lerpFactor); // Adjust the lerp factor
                
                directionVector = Vector2.Lerp(directionVector, directionToPlayer, adjustedLerpFactor); // Smoothly update direction
                rotationAngle = (int)(Math.Atan2(directionVector.Y, directionVector.X) * RAD2DEG) + 90; // Update rotation angle
            }
            else
            {
                isTracking = false; // Stop tracking if the player is close enough
            }
        }

        UpdateAndDraw(); // Update the bullet's position and render it
    }
}

public class EnergyBullet : Bullet
{
    private SoundManager soundManager; // Reference to the sound manager
    private bool isBombEffectPlayedOnce = false; // Ensure the bomb effect sound is played only once
    
    public Vector2 playerPosition; // Target player's position
    
    private float traveledDistance = 0; // Distance traveled by the bullet
    private float homingDistance = 100f; // Distance before homing starts
    private float lerpFactor = 0.05f; // Smoothing factor for homing
    
    private Explosion explosion; // Explosion effect
    public bool hasExploded = false; // Indicates if the bullet has exploded

    public EnergyBullet(Vector2 playerPosition, int positionX, int positionY, int rotationAngle, int damage, Texture2D bullets, Texture2D explosionTexture, SoundManager soundManager) : base(positionX, positionY, rotationAngle, damage, bullets)
    {
        speed = 4; // Set the speed of the energy bullet
        this.playerPosition = playerPosition; // Set the player's position
        this.soundManager = soundManager; // Initialize the sound manager
        SetTexture(121, 126, 121, 126); // Set the bullet's texture region
        
        explosion = new Explosion(explosionTexture, new Vector2(playerPosition.X, playerPosition.Y)); // Initialize the explosion effect
    }

    public override void Move()
    {
        if (hasExploded) // If the bullet has exploded, display the explosion effect
        {
            explosion.Update(Raylib.GetFrameTime()); // Update the explosion animation
            explosion.Draw(); // Draw the explosion
            return;
        }

        Vector2 bulletPosition = new Vector2(positionX, positionY); // Get the bullet's position

        if (traveledDistance < homingDistance) // Move in a straight line until the homing distance is reached
        {
            traveledDistance += speed;
            positionX += (int)(directionVector.X * speed);
            positionY += (int)(directionVector.Y * speed);
        }
        else
        {
            Vector2 directionToPlayer = Vector2.Normalize(playerPosition - bulletPosition); // Calculate direction to the player
            
            directionVector = Vector2.Lerp(directionVector, directionToPlayer, lerpFactor); // Smoothly update direction
            rotationAngle = (int)(Math.Atan2(directionVector.Y, directionVector.X) * RAD2DEG) + 90; // Update rotation angle
            positionX += (int)(directionVector.X * speed);
            positionY += (int)(directionVector.Y * speed);
        }
        
        if (Vector2.Distance(bulletPosition, playerPosition) < 10f) // Check if the bullet is near the player
        {
            hasExploded = true; // Trigger the explosion
            
            if (!isBombEffectPlayedOnce) // Play the bomb effect sound only once
            {
                soundManager.PlayBombEffect();
            }
        }
        else
        {
            UpdateAndDraw(); // Update the bullet's position and render it
        }
    }

    public bool IsAnimationFinished()
    {
        return hasExploded && explosion.IsFinished; // Check if the explosion animation has finished
    }
}