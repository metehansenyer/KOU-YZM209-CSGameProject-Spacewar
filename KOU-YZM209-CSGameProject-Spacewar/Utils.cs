using Raylib_cs;
using System.Text;
using System.Numerics;

namespace KOU_YZM209_CSGameProject_Spacewar;

public class Utils
{
    private const double DEG2RAD = Math.PI / 180;
    private const float referenceWidth = 1920f;
    private const float referenceHeight = 1080f;
    
    public bool isRaylibAnimationDone = false;
    public bool isCreditAnimationDone = false;
    
    private int logoPositionX = Raylib.GetScreenWidth()/2 - 128; 
    private int logoPositionY = Raylib.GetScreenHeight()/2 - 128;
    
    private int framesCounter = 0;
    private int letterCount = 0;
    
    private int topSideRecWidth = 16;
    private int leftSideRecHeight = 16;
    private int bottomSideRecWidth = 16;
    private int rightSideRecHeight = 16;
    
    private int state = 0;
    private float alpha = 1.0f;
    
    // Scales the screen based on the original width and height
    public static (int, int) ScaleScreen(int originalWidth, int originalHeight)
    {
        double scaleFactor = 1.5;
        (double frameWidthRatio, double frameHeightRatio) = (16.0, 9.0);
        
        // Calculate the scaled width and height using a scale factor
        double scaledWidth = Math.Min(originalWidth, (frameWidthRatio / frameHeightRatio) * originalHeight) / scaleFactor;
        double scaledHeight = Math.Min(originalHeight, (frameHeightRatio / frameWidthRatio) * originalWidth) / scaleFactor;

        return ((int)scaledWidth, (int)scaledHeight);
    }

    // Scales an image's rectangle according to the scaled screen dimensions
    public static Rectangle ScaleImage(int originalX, int originalY, int originalWidth, int originalHeight, int scaledScreenWidth, int scaledScreenHeight)
    {
        float widthRatio = scaledScreenWidth / referenceWidth;
        float heightRatio = scaledScreenHeight / referenceHeight;

        // Scale the position and size of the rectangle based on the screen ratio
        int scaledX = (int)Math.Round(originalX * widthRatio);
        int scaledY = (int)Math.Round(originalY * heightRatio);
        int scaledRectWidth = (int)Math.Round(originalWidth * widthRatio);
        int scaledRectHeight = (int)Math.Round(originalHeight * heightRatio);

        return new Rectangle(scaledX, scaledY, scaledRectWidth, scaledRectHeight);
    }
    
    // Scales a point based on the screen's scaled dimensions
    public static Vector2 ScalePoint(int originalX, int originalY, int scaledScreenWidth, int scaledScreenHeight)
    {
        float widthRatio = scaledScreenWidth / referenceWidth;
        float heightRatio = scaledScreenHeight / referenceHeight;

        int scaledX = (int)Math.Round(originalX * widthRatio);
        int scaledY = (int)Math.Round(originalY * heightRatio);

        return new Vector2(scaledX, scaledY);
    }

    // Rotates a point around a center by a given angle in degrees
    public static Vector2 RotatePoint(Vector2 point, Vector2 center, float angle)
    {
        float radians = angle * (float)DEG2RAD;
        float cos = (float)Math.Cos(radians);
        float sin = (float)Math.Sin(radians);
        
        // Apply rotation matrix transformation
        float x = cos * (point.X - center.X) - sin * (point.Y - center.Y) + center.X;
        float y = sin * (point.X - center.X) + cos * (point.Y - center.Y) + center.Y;

        return new Vector2(x, y);
    }

    // Scales the font size based on the screen height
    public static float ScaleFontSize(float fontSize, int scaledScreenHeight)
    {
        float newFontSize = fontSize * (scaledScreenHeight / referenceHeight);
        return newFontSize;
    }
    
    // Linearly interpolates between two values based on the t factor
    public static float Lerp(float a, float b, float t)
    {
        return a + (b - a) * t;
    }
    
    // Updates the animation state for the Raylib logo animation
    public void RaylibAnimationUpdate()
    {
        if (state == 0)
        {
            framesCounter++;
            
            if (framesCounter == 120)
            {
                state = 1;
                framesCounter = 0;
            }
        }
        else if (state == 1)
        {
            // Expand the top and left side rectangles
            topSideRecWidth += 4;
            leftSideRecHeight += 4;

            if (topSideRecWidth == 256) 
            {
                state = 2;
            }
        }
        else if (state == 2)
        {
            // Expand the bottom and right side rectangles
            bottomSideRecWidth += 4;
            rightSideRecHeight += 4;

            if (bottomSideRecWidth == 256) 
            {
                state = 3;
            }
        }
        else if (state == 3)
        {
            framesCounter++;

            if (framesCounter/12 == 1) 
            {
                letterCount++;
                framesCounter = 0;
            }

            if (letterCount >= 10) 
            {
                alpha -= 0.02f;

                if (alpha <= 0.0f) 
                {
                    alpha = 0.0f;
                    state = 4;
                }
            }
        }
        else if (state == 4)
        {
            // Mark the animation as done
            isRaylibAnimationDone = true;
            
            framesCounter = 0;
            letterCount = 0;

            // Reset animation variables
            topSideRecWidth = 16;
            leftSideRecHeight = 16;
            bottomSideRecWidth = 16;
            rightSideRecHeight = 16;

            state = 0;
            alpha = 1.0f;
        }
    }

    // Draws the Raylib logo animation on the screen
    public void RaylibAnimationDraw()
    {
        Raylib.ClearBackground(Color.RayWhite);

        if (state == 0)
        {
            // Flash the logo's initial position
            if ((framesCounter/15)%2 == 1) 
            {
                Raylib.DrawRectangle(logoPositionX, logoPositionY, 16, 16, Color.Black);
            }
        }
        else if (state == 1)
        {
            // Draw expanding top and left side rectangles
            Raylib.DrawRectangle(logoPositionX, logoPositionY, topSideRecWidth, 16, Color.Black);
            Raylib.DrawRectangle(logoPositionX, logoPositionY, 16, leftSideRecHeight, Color.Black);
        }
        else if (state == 2)
        {
            // Draw expanding bottom and right side rectangles
            Raylib.DrawRectangle(logoPositionX, logoPositionY, topSideRecWidth, 16, Color.Black);
            Raylib.DrawRectangle(logoPositionX, logoPositionY, 16, leftSideRecHeight, Color.Black);

            Raylib.DrawRectangle(logoPositionX + 240, logoPositionY, 16, rightSideRecHeight, Color.Black);
            Raylib.DrawRectangle(logoPositionX, logoPositionY + 240, bottomSideRecWidth, 16, Color.Black);
        }
        else if (state == 3)
        { 
            // Draw the final animation frame with fading effect
            Raylib.DrawRectangle(logoPositionX, logoPositionY, topSideRecWidth, 16, Raylib.Fade(Color.Black, alpha));
            Raylib.DrawRectangle(logoPositionX, logoPositionY + 16, 16, leftSideRecHeight - 32, Raylib.Fade(Color.Black, alpha));

            Raylib.DrawRectangle(logoPositionX + 240, logoPositionY + 16, 16, rightSideRecHeight - 32, Raylib.Fade(Color.Black, alpha));
            Raylib.DrawRectangle(logoPositionX, logoPositionY + 240, bottomSideRecWidth, 16, Raylib.Fade(Color.Black, alpha));

            Raylib.DrawRectangle(Raylib.GetScreenWidth()/2 - 112, Raylib.GetScreenHeight()/2 - 112, 224, 224, Raylib.Fade(Color.RayWhite, alpha));
            
            // Draw the text with fading effect
            unsafe
            {
                string text = "raylib";
                byte[] utf8Bytes = Encoding.UTF8.GetBytes(text);
                
                fixed (byte* ptr = utf8Bytes)
                {
                    sbyte* sbytePtr = (sbyte*)ptr;
                    Raylib.DrawText(Raylib.TextSubtext(sbytePtr, 0, letterCount), Raylib.GetScreenWidth()/2 - 44, Raylib.GetScreenHeight()/2 + 48, 50, Raylib.Fade(Color.Black, alpha));
                }
            }
        }
    }

    // Updates the credit animation state
    public void CreditAnimationUpdate()
    {
        if (state == 0) 
        { 
            framesCounter++;

            if (framesCounter/12 == 1) 
            {
                letterCount++;
                framesCounter = 0;
            }

            if (letterCount >= 10) 
            {
                alpha -= 0.02f;

                if (alpha <= 0.0f) 
                {
                    alpha = 0.0f;
                    state = 1;
                }
            }
        } 
        else if(state == 1) 
        {
            // Mark credit animation as done
            isCreditAnimationDone = true;

            framesCounter = 0;
            letterCount = 0;

            state = 0;
            alpha = 1.0f;
        }
    }

    // Draws the credit animation with fading text
    public void CreditAnimationDraw()
    {
        Raylib.ClearBackground(Color.RayWhite);
        
        if(state == 0) {
            string text = "Mete";
            int fontSize = 100;
            int textWidth = Raylib.MeasureText(text, fontSize);

            unsafe
            {
                byte[] utf8Bytes = Encoding.UTF8.GetBytes(text);

                fixed (byte* ptr = utf8Bytes)
                {
                    sbyte* sbytePtr = (sbyte*)ptr;
                    Raylib.DrawText(Raylib.TextSubtext(sbytePtr, 0, letterCount), (Raylib.GetScreenWidth() - textWidth)/2, (Raylib.GetScreenHeight() - fontSize)/2, fontSize, Raylib.Fade(Color.Black, alpha));
                }
            }
        }
    }
}