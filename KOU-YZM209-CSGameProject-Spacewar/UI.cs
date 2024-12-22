using Raylib_cs;
using System.Numerics;

namespace KOU_YZM209_CSGameProject_Spacewar;

public class UI
{
    private SoundManager soundManager;
        
    // Flags to check the status of buttons and application.
    public bool isPressedStartButton = false;
    public bool isApplicationRunning = true;

    // Health and shield variables to track player's status.
    private int lastHealth = 100;
    private int lastShield = 0;
        
    // Variables for handling animations and text size changes.
    private float animationTime = 0.0f;
    private float pressAnyKeyTexFontSize = 50;

    // Loading the font from the specified file.
    private Font font = Raylib.LoadFont("./resources/fonts/font.ttf");

    // Loading textures for menu, game background, and UI elements.
    private Texture2D
        menuBackground = Raylib.LoadTexture("./resources/image/menuBackground.png"),
        gameBackground = Raylib.LoadTexture("./resources/image/gameBackground.png"),
        uiElements = Raylib.LoadTexture("./resources/image/buttons.png");

    // Defining rectangles for various UI elements like buttons, backgrounds, and sliders.
    private Rectangle
        menuBackgroundRec = new Rectangle(0, 0, 1920, 1080),
        menuBackgroundDes = new Rectangle(0, 0, Raylib.GetScreenWidth(), Raylib.GetScreenHeight()),
        
        endScreenBackRec = new Rectangle(6, 12, 154, 48),
        endScreenBackDes = Utils.ScaleImage(480, 270,960, 540, Raylib.GetScreenWidth(), Raylib.GetScreenHeight()),

        emptyButtonRec = new Rectangle(0, 0, 166, 78),
        emptyButtonHoverRec = new Rectangle(166, 0, 168, 78),
        emptyButtonPressedRec = new Rectangle(334, 0, 166, 78),
        startButtonDes = Utils.ScaleImage(828, 742,265, 125, Raylib.GetScreenWidth(), Raylib.GetScreenHeight()),
        settingButtonDes = Utils.ScaleImage(388, 742,265, 125, Raylib.GetScreenWidth(), Raylib.GetScreenHeight()),
        exitButtonDes = Utils.ScaleImage(1268, 742,265, 125, Raylib.GetScreenWidth(), Raylib.GetScreenHeight()),
        
        hearthRec = new Rectangle(358, 259, 36, 36),
        hearthDes = Utils.ScaleImage(30, 30, 67, 67, Raylib.GetScreenWidth(), Raylib.GetScreenHeight()),
        
        shieldRec = new Rectangle(394, 259, 36, 36),
        shieldDes = Utils.ScaleImage(30, 117, 67, 67, Raylib.GetScreenWidth(), Raylib.GetScreenHeight()),
        
        lifeBarRec = new Rectangle(358, 232, 72, 27),
        lifeBarDes = Utils.ScaleImage(110, 30, 270, 67, Raylib.GetScreenWidth(), Raylib.GetScreenHeight()),
        shieldBarDes = Utils.ScaleImage(110, 117, 270, 67, Raylib.GetScreenWidth(), Raylib.GetScreenHeight()),
        
        resumeButtonRec = new Rectangle(214, 78, 72, 76),
        resumeButtonHoverRec = new Rectangle(286, 78, 72, 76),
        resumeButtonDes = Utils.ScaleImage(1000, 466, 140, 148, Raylib.GetScreenWidth(), Raylib.GetScreenHeight()),  
        
        menuButtonRec = new Rectangle(214, 154, 72, 78),
        menuButtonHoverRec = new Rectangle(286, 154, 72, 78),
        menuButtonDes = Utils.ScaleImage(710, 466, 140, 148, Raylib.GetScreenWidth(), Raylib.GetScreenHeight()),
        
        sliderBarRec = new Rectangle(0, 310, 143, 30),
        sliderBarMusicDes = Utils.ScaleImage(663, 321, 594, 60, Raylib.GetScreenWidth(), Raylib.GetScreenHeight()),
        sliderBarEffectDes = Utils.ScaleImage(663, 555, 594, 60, Raylib.GetScreenWidth(), Raylib.GetScreenHeight()),
        
        decreaseButtonRec = new Rectangle(144, 310, 20, 30),
        decreaseHoverButtonRec = new Rectangle(358, 295, 20, 30),
        decreaseButtonMusicDes = Utils.ScaleImage(613, 321, 40, 60, Raylib.GetScreenWidth(), Raylib.GetScreenHeight()),
        decreaseButtonEffectDes = Utils.ScaleImage(613, 555, 40, 60, Raylib.GetScreenWidth(), Raylib.GetScreenHeight()),
        
        increaseButtonRec = new Rectangle(152, 310, 20, 30),
        increaseHoverButtonRec = new Rectangle(366, 295, 20, 30),
        increaseButtonMusicDes = Utils.ScaleImage(1267, 321, 40, 60, Raylib.GetScreenWidth(), Raylib.GetScreenHeight()),
        increaseButtonEffectDes = Utils.ScaleImage(1267, 555, 40, 60, Raylib.GetScreenWidth(), Raylib.GetScreenHeight()),
        
        slideIndicatorRec = new Rectangle(173, 310, 31, 36),
        slideIndicatorMusicDes = Utils.ScaleImage(837, 315, 62, 72, Raylib.GetScreenWidth(), Raylib.GetScreenHeight()),
        slideIndicatorEffectDes = Utils.ScaleImage(837, 549, 62, 72, Raylib.GetScreenWidth(), Raylib.GetScreenHeight()),
        
        selectButtonRec = new Rectangle(214, 232, 48, 54),
        selectHoverButtonRec = new Rectangle(262, 232, 48, 54),
        selectedButtonRec = new Rectangle(214, 286, 48, 54),
        selectionButtonTRDes = Utils.ScaleImage(967, 801, 48, 54, Raylib.GetScreenWidth(), Raylib.GetScreenHeight()),
        selectionButtonENDes = Utils.ScaleImage(703, 801, 48, 54, Raylib.GetScreenWidth(), Raylib.GetScreenHeight());

    // Variables for origin point and boundaries for sliding indicators.
    private Vector2
        origin = new Vector2(0, 0),
        minSlideIndicatorPoint = Utils.ScalePoint(670, 315, Raylib.GetScreenWidth(), Raylib.GetScreenHeight()),
        maxSlideIndicatorPoint = Utils.ScalePoint(1187, 315, Raylib.GetScreenWidth(), Raylib.GetScreenHeight());

    // Colors used in the UI for various elements.
    private Color
        originalYellow = new Color(255, 162, 0, 255),
        lightYellow = new Color(255, 203, 186, 255);

    // Flags to track the UI element interaction status (e.g. buttons, language selection).
    private bool
        isGameLangDefault = true,
        isPlayerInMenu = true,
        isMouseOverStartButton = false,
        isMouseOverSettingsButton = false,
        isMouseOverExitButton = false,
        isMouseOverResumeButton = false,
        isMouseOverMenuButton = false,
        isMouseOverDecreaseMusicButton = false,
        isMouseOverIncreaseMusicButton = false,
        isMouseOverDecreaseEffectButton = false,
        isMouseOverIncreaseEffectButton = false,
        isMouseOverSelectionTRButton = false,
        isMouseOverSelectionENButton = false;

    // Constructor to initialize UI with SoundManager instance
    public UI(SoundManager soundManager)
    {
        this.soundManager = soundManager;
    }

    // Checks if the application window should close and sets the application's running state
    public void ShouldApplicationRunning()
    {
        if (Raylib.WindowShouldClose() && !Raylib.IsKeyPressed(KeyboardKey.Escape))
        {
            isApplicationRunning = false;
        }
    }

    // Main Draw method, depending on the state it draws the menu or the options screen
    public void Draw()
    {
        if (isPlayerInMenu)
        {
            DrawMenu(); // Draw the menu if the player is in the menu
        }
        else
        {
            DrawOptions(); // Draw the options screen if the player is in options
        }
    }

    // Draws the menu with background and buttons
    private void DrawMenu()
    {
        Raylib.DrawTexturePro(menuBackground, menuBackgroundRec, menuBackgroundDes, origin, 0, Color.RayWhite);

        // Draw each button with appropriate text and positioning
        DrawMenuButton(isGameLangDefault ? "START" : "BASLA", 70, 965, 793, startButtonDes, isMouseOverStartButton);
        DrawMenuButton(isGameLangDefault ? "OPTIONS" : "AYARLAR", 65, 522, 793, settingButtonDes, isMouseOverSettingsButton);
        DrawMenuButton(isGameLangDefault ? "EXIT" : "CIKIS", 70, 1400, 793, exitButtonDes, isMouseOverExitButton);
    }

    // Helper method to draw a menu button with specific text and hover effect
    private void DrawMenuButton(string text, int menuButtonFontSize, int x, int y, Rectangle buttonDes, bool isHovered)
    {
        float fontSize = Utils.ScaleFontSize(menuButtonFontSize, Raylib.GetScreenHeight());
        Vector2 textSize = Raylib.MeasureTextEx(font, text, fontSize, 1.0f);
        Vector2 textPosition = new Vector2(textSize.X / 2, textSize.Y / 2);

        Raylib.DrawTexturePro(uiElements, isHovered ? emptyButtonHoverRec : emptyButtonRec, buttonDes, origin, 0, Color.RayWhite);
        Raylib.DrawTextPro(font, text, Utils.ScalePoint(x, y, Raylib.GetScreenWidth(), Raylib.GetScreenHeight()), textPosition, 0, fontSize, 1.0f, isHovered ? lightYellow : originalYellow);
    }

    // Draws the options screen with controls for volume and language
    private void DrawOptions()
    {
        WatchKeyEscapeOptionScreen(); // Check if Escape is pressed to return to menu
        
        Raylib.DrawTexturePro(gameBackground, menuBackgroundRec, menuBackgroundDes, origin, 0, Color.RayWhite);

        // Draw volume control for music and effect
        DrawVolumeControl(isGameLangDefault ? "MUSIC VOLUME" : "MUZIK SESI", 90, 960, 245, soundManager.GetMusicVolume(), sliderBarMusicDes, decreaseButtonMusicDes, increaseButtonMusicDes, slideIndicatorMusicDes, isMouseOverDecreaseMusicButton, isMouseOverIncreaseMusicButton);
        DrawVolumeControl(isGameLangDefault ? "EFFECT VOLUME" : "EFEKT SESI", 90, 960, 479, soundManager.GetEffectVolume(), sliderBarEffectDes, decreaseButtonEffectDes, increaseButtonEffectDes, slideIndicatorEffectDes, isMouseOverDecreaseEffectButton, isMouseOverIncreaseEffectButton);

        // Draw language selection and instructions
        DrawText(isGameLangDefault ? "LANGUAGE" : "DIL", 90, 960, 710, originalYellow);
        DrawText("Turkce", 60, 1125, 822, originalYellow);
        DrawText("English", 60, 860, 822, originalYellow);
        DrawText(isGameLangDefault ? "Press Escape to Go Back" : "Cikmak icin Basin, Escape", 60, 960, 960, Color.RayWhite);

        // Draw language selection buttons
        Raylib.DrawTexturePro(uiElements, isGameLangDefault ? isMouseOverSelectionTRButton ? selectHoverButtonRec : selectButtonRec : selectedButtonRec, selectionButtonTRDes, origin, 0, Color.RayWhite);
        Raylib.DrawTexturePro(uiElements, isGameLangDefault ? selectedButtonRec : isMouseOverSelectionENButton ? selectHoverButtonRec : selectButtonRec, selectionButtonENDes, origin, 0, Color.RayWhite);
    }

    // Draws the volume control sliders with buttons for increasing/decreasing volume
    private void DrawVolumeControl(string labelText, int optionsLabelFontSize, int x, int y, float volume, Rectangle sliderBarDes, Rectangle decreaseButtonDes, Rectangle increaseButtonDes, Rectangle slideIndicatorDes, bool isMouseOverDecreaseButton, bool isMouseOverIncreaseButton)
    {
        float labelTextFontSize = Utils.ScaleFontSize(optionsLabelFontSize, Raylib.GetScreenHeight());
        Vector2 labelTextSize = Raylib.MeasureTextEx(font, labelText, labelTextFontSize, 1.0f);
        Vector2 labelTextPosition = new Vector2(labelTextSize.X / 2, labelTextSize.Y / 2);
        
        Raylib.DrawTextPro(font, labelText, Utils.ScalePoint(x, y, Raylib.GetScreenWidth(), Raylib.GetScreenHeight()), labelTextPosition, 0.0f, labelTextFontSize, 1.0f, originalYellow);
        
        // Draw slider bar and buttons
        Raylib.DrawTexturePro(uiElements, sliderBarRec, sliderBarDes, origin, 0, Color.RayWhite);
        Raylib.DrawTexturePro(uiElements, isMouseOverDecreaseButton ? decreaseHoverButtonRec : decreaseButtonRec, decreaseButtonDes, origin, 0, Color.RayWhite);
        Raylib.DrawTexturePro(uiElements, isMouseOverIncreaseButton ? increaseHoverButtonRec : increaseButtonRec, increaseButtonDes, origin, 0, Color.RayWhite);
        
        // Position the slide indicator based on volume
        slideIndicatorDes.X = (1.0f - volume) * minSlideIndicatorPoint.X + volume * maxSlideIndicatorPoint.X;
        Raylib.DrawTexturePro(uiElements, slideIndicatorRec, slideIndicatorDes, origin, 0, Color.RayWhite);
    }

    // Checks for mouse interactions with the UI (menu or options)
    public void CheckMouse()
    {
        Vector2 mousePos = Raylib.GetMousePosition();

        if (isPlayerInMenu)
        {
            CheckMenuMouse(mousePos); // Check mouse over menu buttons
        }
        else
        {
            CheckOptionMouse(mousePos); // Check mouse over options controls
        }
    }

    // Checks for mouse interactions in the menu (start, options, exit buttons)
    private void CheckMenuMouse(Vector2 mousePos)
    {
        if (Raylib.CheckCollisionPointRec(mousePos, startButtonDes))
        {
            isMouseOverStartButton = true;
            
            if (Raylib.IsMouseButtonPressed(MouseButton.Left))
            {
                soundManager.PlayButtonSound(); // Play sound on button press
                        
                isMouseOverStartButton = false;
                isPressedStartButton = true; // Start the game
            }
        }
        else
        {
            isMouseOverStartButton = false;
        }

        if (Raylib.CheckCollisionPointRec(mousePos, settingButtonDes))
        {
            isMouseOverSettingsButton = true;
            
            if (Raylib.IsMouseButtonPressed(MouseButton.Left))
            {
                soundManager.PlayButtonSound(); // Play sound on button press
                        
                isPlayerInMenu = false; // Go to settings
                isMouseOverSettingsButton = false;
            }
        }
        else
        {
            isMouseOverSettingsButton = false;
        }

        if (Raylib.CheckCollisionPointRec(mousePos, exitButtonDes))
        {
            isMouseOverExitButton = true;
            
            if (Raylib.IsMouseButtonPressed(MouseButton.Left))
            {
                soundManager.PlayButtonSound(); // Play sound on button press
                        
                isApplicationRunning = false; // Exit the application
            }
        }
        else
        {
            isMouseOverExitButton = false;
        }
    }

    // Checks for mouse interactions in the options
    private void CheckOptionMouse(Vector2 mousePos)
    {
        // Check if the mouse is over the decrease music volume button
        if (Raylib.CheckCollisionPointRec(mousePos, decreaseButtonMusicDes))
        {
            isMouseOverDecreaseMusicButton = true;
            
            // If the left mouse button is pressed, decrease music volume
            if (Raylib.IsMouseButtonDown(MouseButton.Left))
            {
                soundManager.PlayButtonSound();
                soundManager.DecreaseMusicVolume(0.02f);
            }
        }
        else
        {
            isMouseOverDecreaseMusicButton = false;
        }
        
        // Check if the mouse is over the increase music volume button
        if (Raylib.CheckCollisionPointRec(mousePos, increaseButtonMusicDes))
        {
            isMouseOverIncreaseMusicButton = true;
            
            // If the left mouse button is pressed, increase music volume
            if (Raylib.IsMouseButtonDown(MouseButton.Left))
            {
                soundManager.PlayButtonSound();
                soundManager.IncreaseMusicVolume(0.02f);
            }
        }
        else
        {
            isMouseOverIncreaseMusicButton = false;
        }
        
        // Check if the mouse is over the decrease effect volume button
        if (Raylib.CheckCollisionPointRec(mousePos, decreaseButtonEffectDes))
        {
            isMouseOverDecreaseEffectButton = true;
            
            // If the left mouse button is pressed, decrease effect volume
            if (Raylib.IsMouseButtonDown(MouseButton.Left))
            {
                soundManager.PlayButtonSound();
                soundManager.DecreaseeffectVolume(0.02f);
            }
        }
        else
        {
            isMouseOverDecreaseEffectButton = false;
        }
        
        // Check if the mouse is over the increase effect volume button
        if (Raylib.CheckCollisionPointRec(mousePos, increaseButtonEffectDes))
        {
            isMouseOverIncreaseEffectButton = true;
            
            // If the left mouse button is pressed, increase effect volume
            if (Raylib.IsMouseButtonDown(MouseButton.Left))
            {
                soundManager.PlayButtonSound();
                soundManager.IncreaseeffectVolume(0.02f);
            }
        }
        else
        {
            isMouseOverIncreaseEffectButton = false;
        }
        
        // Check if the mouse is over the TR language selection button
        if (Raylib.CheckCollisionPointRec(mousePos, selectionButtonTRDes))
        {
            isMouseOverSelectionTRButton = true;

            // If the game language is set to default (EN), change it to TR on button click
            if (isGameLangDefault)
            {
                if (Raylib.IsMouseButtonDown(MouseButton.Left))
                {
                    soundManager.PlayButtonSound();
                    isGameLangDefault = false;
                }
            }
        }
        else
        {
            isMouseOverSelectionTRButton = false;
        }
        
        // Check if the mouse is over the EN language selection button
        if (Raylib.CheckCollisionPointRec(mousePos, selectionButtonENDes))
        {
            isMouseOverSelectionENButton = true;

            // If the game language is set to TR, change it to EN on button click
            if (!isGameLangDefault)
            {
                if (Raylib.IsMouseButtonDown(MouseButton.Left))
                {
                    soundManager.PlayButtonSound();
                    isGameLangDefault = true;
                }
            }
        }
        else
        {
            isMouseOverSelectionENButton = false;
        }
    }

    // Draws the pause screen
    public void DrawPauseScreen()
    {
        // Draw the resume button with hover effect
        Raylib.DrawTexturePro(uiElements, isMouseOverResumeButton ? resumeButtonHoverRec : resumeButtonRec, resumeButtonDes, origin, 0, Color.RayWhite);
        
        // Draw the menu button with hover effect
        Raylib.DrawTexturePro(uiElements, isMouseOverMenuButton ? menuButtonHoverRec : menuButtonRec, menuButtonDes, origin, 0, Color.RayWhite);
    }

    // Checks for mouse interactions in the pause screen
    public (bool, bool) CheckPauseScreenMouse(bool isGameFinished, bool isGamePaused)
    {
        // Get the current mouse position
        Vector2 mousePos = Raylib.GetMousePosition();
        
        // Check if the mouse is over the resume button
        if (Raylib.CheckCollisionPointRec(mousePos, resumeButtonDes))
        {
            isMouseOverResumeButton = true;
            
            // If the left mouse button is pressed, resume the game
            if (Raylib.IsMouseButtonPressed(MouseButton.Left))
            {
                soundManager.PlayButtonSound();
                
                isGamePaused = false;
            }
        }
        else
        {
            isMouseOverResumeButton = false;
        }
        
        // Check if the mouse is over the menu button
        if (Raylib.CheckCollisionPointRec(mousePos, menuButtonDes))
        {
            isMouseOverMenuButton = true;
            
            // If the left mouse button is pressed, finish the game and go to the menu
            if (Raylib.IsMouseButtonPressed(MouseButton.Left))
            {
                soundManager.ResumeGameMusic();
                soundManager.PlayButtonSound();
                
                isGameFinished = true;
            }
        }
        else
        {
            isMouseOverMenuButton = false;
        }

        // Return the game state (finished and paused)
        return (isGameFinished, isGamePaused);
    }

    // Draws the in game User Interface
    public void DrawInGameUI(int playerHealth, int playerShield, int point)
    {
        // Adjust health bar based on player's health
        if (lastHealth > playerHealth)
        {
            lastHealth -= 1;
        }
        else if (lastHealth < playerHealth)
        {
            lastHealth += 1;
        }
        
        // Adjust shield bar based on player's shield
        if (lastShield > playerShield)
        {
            lastShield -= 1;
        }
        else if (lastShield < playerShield)
        {
            lastShield += 1;
        }

        // Draw health bar
        DrawStaticsBar(52, lastHealth, hearthRec, hearthDes, lifeBarDes, new Color(243, 97, 255, 255));
        
        // Draw shield bar if the player has shield
        if (playerShield > 0)
        {
            DrawStaticsBar(139, lastShield, shieldRec, shieldDes, shieldBarDes, new Color(97, 211, 227, 255));
        }
    }

    // Draws the in level up message
    public void DrawLevelUpMessage(float elapsedTime, float animationTime)
    {
        // Calculate scale and opacity based on elapsed time and animation time
        float scale = 1 + (elapsedTime / animationTime) * (400 / 150 - 1);
        float opacity = 255 - (elapsedTime / animationTime) * 255;
        
        // Draw the level up message with scaling and fading effects
        DrawText(isGameLangDefault ? "LEVEL UP" : "SEVIYE ATLADIN", (int)(150 * scale), 960, 540, new Color(255, 255, 255, (int)opacity));
    }

    // Draws the game over information
    public void DrawEndScreen(string status, int point)
    {
        // Draw the background for the end screen with transparency
        Raylib.DrawTexturePro(uiElements, endScreenBackRec, endScreenBackDes, origin, 0, new Color(255, 255, 255, 128));
        
        // Draw the game status and points
        DrawText(status, 100, 960, 389, originalYellow);
        DrawText(isGameLangDefault ? $"Total Point: {point}" : $"Toplam Puan: {point}", 70, 960, 545, originalYellow);
        
        // Update animation time and apply sine wave for press key text size
        animationTime += Raylib.GetFrameTime();
        pressAnyKeyTexFontSize += + (float)Math.Sin(animationTime * 2 * Math.PI) * 0.3f;
        
        // Draw the "Press Escape to Return to Menu" message with dynamic font size
        DrawText(isGameLangDefault ? "Press Escape to Return to Menu" : "Menuye Donmek icin ESC Basin", pressAnyKeyTexFontSize, 960, 695, Color.RayWhite);
    }

    // Checks for escape interaction in the game over screen
    public bool WatchKeyPressEndScreen(bool isGameFinished)
    {
        // Check if the escape key is pressed to finish the game and return to the menu
        if (Raylib.IsKeyPressed(KeyboardKey.Escape))
        {
            soundManager.ResumeGameMusic();
            isGameFinished = true;
        }
        
        // Return the updated game finished state
        return isGameFinished;
    }

    // Checks for escape interaction in the options screen
    private void WatchKeyEscapeOptionScreen()
    {
        // Toggle menu state when escape key is pressed
        if (Raylib.IsKeyPressed(KeyboardKey.Escape))
        {
            isPlayerInMenu = !isPlayerInMenu;
        }
    }
    
    // Helper method to draw text on the screen
    private void DrawText(string text, float originalFontSize, int x, int y, Color color)
    {
        float textFontSize = Utils.ScaleFontSize(originalFontSize, Raylib.GetScreenHeight());
        Vector2 textSize = Raylib.MeasureTextEx(font, text, textFontSize, 1.0f);
        Vector2 textPosition = new Vector2(textSize.X / 2, textSize.Y / 2);
        
        Raylib.DrawTextPro(font, text, Utils.ScalePoint(x, y, Raylib.GetScreenWidth(), Raylib.GetScreenHeight()), textPosition, 0.0f, textFontSize, 1.0f, color);
    }
    
    // Helper method to draw life and shield bar on the screen
    private void DrawStaticsBar(int y, int lastInfo, Rectangle iconRec, Rectangle iconDes, Rectangle barDes, Color color)
    {
        // Scale the bar's dimensions according to the screen size
        Rectangle bar = Utils.ScaleImage(143, y, 204, 22, Raylib.GetScreenWidth(), Raylib.GetScreenHeight());
        
        // Adjust the bar width based on the player's health or shield
        bar.Width *= (float)lastInfo / 100;
        
        // Draw the icon for the health or shield
        Raylib.DrawTexturePro(uiElements, iconRec, iconDes, origin, 0, Color.RayWhite);
        
        // Draw the health or shield bar
        Raylib.DrawRectangle((int)bar.X, (int)bar.Y, (int)bar.Width, (int)bar.Height, color);
        
        // Draw the background of the bar
        Raylib.DrawTexturePro(uiElements, lifeBarRec, barDes, origin, 0, Color.RayWhite);
    }
}