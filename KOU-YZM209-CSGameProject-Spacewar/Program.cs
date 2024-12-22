using Raylib_cs;

namespace KOU_YZM209_CSGameProject_Spacewar;

class Program
{
    static void Main()
    {
        InitWindow(); // Initialize the game window
        
        InitAudioDevice(); // Initialize the audio device
        SoundManager soundManager = new SoundManager(); // Sound manager to handle all sound-related operations
        
        UI ui = new UI(soundManager); // User interface manager
        Utils utils = new Utils(); // Utility class for animations and other general functions
        Game game = new Game(ui, soundManager); // Main game logic
        
        while (ui.isApplicationRunning) // Main application loop
        {
            Raylib.BeginDrawing(); // Begin drawing frame

            if (!utils.isRaylibAnimationDone) // If the Raylib animation is not completed
            {
                utils.RaylibAnimationUpdate(); // Update the animation state
                utils.RaylibAnimationDraw(); // Draw the animation
            }
            else if (!utils.isCreditAnimationDone) // If the credit animation is not completed
            {
                utils.CreditAnimationUpdate(); // Update the credit animation
                utils.CreditAnimationDraw(); // Draw the credit animation
            }
            else
            {
                soundManager.SetMusicVolume(); // Adjust the background music volume
                soundManager.SetEffectVolume(); // Adjust the sound effects volume
                
                if (!ui.isPressedStartButton) // If the start button has not been pressed
                {
                    soundManager.PlayMenuMusic(); // Play the menu music
                    
                    ui.CheckMouse(); // Check for mouse interactions in the menu
                    ui.Draw(); // Draw the UI elements
                }
                else
                {
                    soundManager.StopMenuMusic(); // Stop the menu music
                    
                    if (!game.isGameFinished) // If the game is still ongoing
                    {
                        soundManager.PlayGameMusic(); // Play the game music
                        
                        game.PlayGame(); // Main game logic
                    }
                    else
                    {
                        soundManager.StopGameMusic(); // Stop the game music
                        
                        game = new Game(ui, soundManager); // Reset the game state
                        ui.isPressedStartButton = false; // Reset the start button state
                    }
                }
            }
            
            Raylib.EndDrawing(); // End drawing frame
            
            ui.ShouldApplicationRunning(); // Check if the application should continue running
        }
        
        soundManager.UnloadAll(); // Unload all sound resources
        Raylib.CloseAudioDevice(); // Close the audio device
        Raylib.CloseWindow(); // Close the game window
    }

    private static void InitWindow()
    {
        Raylib.InitWindow(24, 25, "Spacewar"); // Create a window with the title "Spacewar"
        
        int screenWidth = Raylib.GetMonitorWidth(Raylib.GetCurrentMonitor()); // Get the current monitor's width
        int screenHeight = Raylib.GetMonitorHeight(Raylib.GetCurrentMonitor()); // Get the current monitor's height
        
        (int scaledScreenWidth, int scaledScreenHeight) = Utils.ScaleScreen(screenWidth, screenHeight); // Scale the screen resolution
        
        Raylib.SetWindowSize(scaledScreenWidth, scaledScreenHeight); // Set the window size to the scaled resolution
        Raylib.SetWindowPosition((screenWidth - scaledScreenWidth) / 2, (screenHeight - scaledScreenHeight) / 2); // Center the window on the screen
        Raylib.SetTargetFPS(60); // Set the target frames per second to 60
    }
    
    private static void InitAudioDevice()
    {
        while (!Raylib.IsAudioDeviceReady()) // Wait until the audio device is ready
        {
            Raylib.InitAudioDevice(); // Initialize the audio device
        }
    }
}