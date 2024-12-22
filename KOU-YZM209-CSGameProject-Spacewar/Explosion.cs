using Raylib_cs;
using System.Numerics;

namespace KOU_YZM209_CSGameProject_Spacewar;

public class Explosion
{
    private Texture2D explosionTexture;
    
    private Vector2 position;
    
    private int frameWidth = 64; // Width of each explosion frame
    private int frameHeight = 64; // Height of each explosion frame
    private int frameCount = 10; // Total number of frames for the explosion animation
    
    private Rectangle[] frames; // Array of rectangles to hold the frame information
    
    private int currentFrame = 0; // The current frame to be drawn
    private float timer = 0f; // Timer to track the frame update rate
    private float frameTime = 0.1f; // Time duration per frame
    
    private bool isFinished = false; // Flag to indicate if the explosion animation is finished

    // Constructor that initializes the explosion with the texture and position
    public Explosion(Texture2D explosionTexture, Vector2 position)
    {
        this.explosionTexture = explosionTexture; // Set the explosion texture
        this.position = position; // Set the position of the explosion
        
        frames = new Rectangle[frameCount]; // Initialize the frames array
        // Set up the frames based on the explosion texture's width and height
        for (int i = 0; i < frameCount; i++)
        {
            frames[i] = new Rectangle(i * frameWidth, 0, frameWidth, frameHeight); // Set each frame's rectangle
        }
    }

    // Property to check if the explosion animation is finished
    public bool IsFinished => isFinished;

    // Method to update the explosion animation based on delta time
    public void Update(float deltaTime)
    {
        if (!isFinished) // Only update if the explosion is not finished
        {
            timer += deltaTime; // Increment the timer by the time passed since the last frame
            if (timer >= frameTime) // Check if it's time to change the frame
            {
                timer = 0f; // Reset the timer
                currentFrame++; // Move to the next frame
                if (currentFrame >= frames.Length) // If all frames have been played
                {
                    isFinished = true; // Mark the explosion as finished
                }
            }
        }
    }

    // Method to draw the explosion on the screen
    public void Draw()
    {
        if (!isFinished) // Only draw if the explosion is not finished
        {
            // Create a destination rectangle for drawing the explosion
            Rectangle dest = new Rectangle(position.X, position.Y, frameWidth*2, frameHeight*2); // Scale the explosion size
            // Draw the current frame of the explosion at the specified position
            Raylib.DrawTexturePro(explosionTexture, frames[currentFrame], dest, new Vector2(dest.Width/2, dest.Height/2), 0, Color.RayWhite);
        }
    }
}