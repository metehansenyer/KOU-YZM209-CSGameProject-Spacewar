using Raylib_cs;

namespace KOU_YZM209_CSGameProject_Spacewar;

public class SoundManager
{
    private bool isGameEndSounPlayedOnce = false; // Flag to track if the game end sound has already been played once
    private bool isGameMusicPaused = false; // Flag to check if the game music is paused
    private float musicVolumeBeforePause; // Store music volume before it is paused
    
    private float
        musicVolume = 0.3f, // Default music volume
        effectVolume = 1.0f; // Default effect sound volume
    
    private Music
        menuMusic = Raylib.LoadMusicStream("./resources/audio/music/Dynatron_DustoftheSaturn.mp3"), // Menu music stream
        gameMusic = Raylib.LoadMusicStream("./resources/audio/music/gameMusic.mp3"); // Game music stream
    
    private Sound
        buttonSound = Raylib.LoadSound("./resources/audio/sound/buttonSound.wav"), // Button sound effect
        playerShootEffect = Raylib.LoadSound("./resources/audio/sound/playerShootEffect.mp3"), // Player shooting sound effect
        enemyShootEffect = Raylib.LoadSound("./resources/audio/sound/enemyShootEffect.mp3"), // Enemy shooting sound effect
        bossShootEffect = Raylib.LoadSound("./resources/audio/sound/bossShootEffect.mp3"), // Boss shooting sound effect
        winSound = Raylib.LoadSound("./resources/audio/sound/winSound.wav"), // Winning sound effect
        gameOverSound = Raylib.LoadSound("./resources/audio/sound/gameOverSound.wav"), // Game over sound effect
        collisionEffect = Raylib.LoadSound("./resources/audio/sound/collisionEffect.mp3"), // Collision sound effect
        bombEffect = Raylib.LoadSound("./resources/audio/sound/bombEffect.mp3"), // Bomb explosion sound effect
        bulletCollisionEffect = Raylib.LoadSound("./resources/audio/sound/bulletCollisionEffect.wav"), // Bullet collision sound effect
        coinSound = Raylib.LoadSound("./resources/audio/sound/coinSound.wav"); // Coin collection sound effect
    
    // Play the menu background music
    public void PlayMenuMusic()
    {
        if (!Raylib.IsMusicStreamPlaying(menuMusic)) // Check if menu music is not already playing
        {
            Raylib.PlayMusicStream(menuMusic); // Play menu music
        }
        
        Raylib.UpdateMusicStream(menuMusic); // Update music stream
    }
    
    // Stop the menu background music
    public void StopMenuMusic()
    {
        if (Raylib.IsMusicStreamPlaying(menuMusic)) // Check if menu music is currently playing
        {
            Raylib.StopMusicStream(menuMusic); // Stop menu music
        }
    }
    
    // Play the game background music
    public void PlayGameMusic()
    {
        if (!Raylib.IsMusicStreamPlaying(gameMusic)) // Check if game music is not already playing
        {
            Raylib.PlayMusicStream(gameMusic); // Play game music
        }
        
        Raylib.UpdateMusicStream(gameMusic); // Update music stream
    }
    
    // Pause the game music
    public void PauseGameMusic()
    {
        if (!isGameMusicPaused) // Check if the game music is not already paused
        {
            musicVolumeBeforePause = musicVolume; // Store the current volume
            musicVolume = 0.0f; // Set music volume to 0 (paused)
            isGameMusicPaused = true; // Set the flag to indicate the music is paused
        }
    }
    
    // Resume the game music
    public void ResumeGameMusic()
    {
        if (isGameMusicPaused) // Check if the game music is paused
        {
            musicVolume = musicVolumeBeforePause; // Restore the original volume
            isGameMusicPaused = false; // Set the flag to indicate the music is resumed
        }
    }
    
    // Stop the game music
    public void StopGameMusic()
    {
        if (Raylib.IsMusicStreamPlaying(gameMusic)) // Check if game music is currently playing
        {
            Raylib.StopMusicStream(gameMusic); // Stop game music
        }
    }

    // Play the button click sound effect
    public void PlayButtonSound()
    {
        if (!Raylib.IsSoundPlaying(buttonSound)) // Check if the button sound is not already playing
        {
            Raylib.PlaySound(buttonSound); // Play button sound
        }
    }
    
    // Play the player's shooting sound effect
    public void PlayPlayerShootEffect()
    {
        Raylib.PlaySound(playerShootEffect); // Play player shoot sound
    }
    
    // Play the enemy's shooting sound effect
    public void PlayEnemyShootEffect()
    {
        Raylib.PlaySound(enemyShootEffect); // Play enemy shoot sound
    }
    
    // Play the boss's shooting sound effect
    public void PlayBossShootEffect()
    {
        Raylib.PlaySound(bossShootEffect); // Play boss shoot sound
    }
    
    // Play the game over or win sound effect depending on the game result
    public void PlayGameEndSound(string causeOfGameOver)
    {
        if (causeOfGameOver == "MAYBE NEXT TIME!") // Check if the game is over (lose)
        {
            if (!Raylib.IsSoundPlaying(gameOverSound) && !isGameEndSounPlayedOnce) {
                Raylib.PlaySound(gameOverSound); // Play game over sound
                isGameEndSounPlayedOnce = true; // Mark the sound as played
            }
        }
        else // If the game is won
        {
            if (!Raylib.IsSoundPlaying(winSound) && !isGameEndSounPlayedOnce) {
                Raylib.PlaySound(winSound); // Play win sound
                isGameEndSounPlayedOnce = true; // Mark the sound as played
            }
        }
    }
    
    // Play the collision sound effect
    public void PlayCollisionSound()
    {
        Raylib.PlaySound(collisionEffect); // Play collision sound
    }
    
    // Play the bullet collision sound effect
    public void PlayBulletCollisionSound()
    {
        Raylib.PlaySound(bulletCollisionEffect); // Play bullet collision sound
    }
    
    // Play the bomb explosion sound effect
    public void PlayBombEffect()
    {
        if (!Raylib.IsSoundPlaying(bombEffect)) { // Check if the bomb sound is not already playing
            Raylib.PlaySound(bombEffect); // Play bomb sound
        }
    }
    
    // Play the coin collection sound effect
    public void PlayCoinSound()
    {
        Raylib.PlaySound(coinSound); // Play coin collection sound
    }

    // Get the current music volume level
    public float GetMusicVolume()
    {
        return musicVolume; // Return current music volume
    }

    // Increase the music volume by a certain amount
    public void IncreaseMusicVolume(float amount)
    {
        musicVolume = Math.Min(musicVolume + amount, 1.0f); // Ensure music volume doesn't exceed 1.0
    }
    
    // Decrease the music volume by a certain amount
    public void DecreaseMusicVolume(float amount)
    {
        musicVolume = Math.Max(musicVolume - amount, 0.0f); // Ensure music volume doesn't go below 0
    }
    
    // Get the current effect sound volume level
    public float GetEffectVolume()
    {
        return effectVolume; // Return current effect sound volume
    }

    // Increase the effect sound volume by a certain amount
    public void IncreaseeffectVolume(float amount)
    {
        effectVolume = Math.Min(effectVolume + amount, 1.0f); // Ensure effect volume doesn't exceed 1.0
    }
    
    // Decrease the effect sound volume by a certain amount
    public void DecreaseeffectVolume(float amount)
    {
        effectVolume = Math.Max(effectVolume - amount, 0.0f); // Ensure effect volume doesn't go below 0
    }
    
    // Set the volume for the music streams
    public void SetMusicVolume()
    {
        Raylib.SetMusicVolume(menuMusic, musicVolume); // Set menu music volume
        Raylib.SetMusicVolume(gameMusic, musicVolume); // Set game music volume
    }
    
    // Set the volume for all sound effects
    public void SetEffectVolume()
    {
        Raylib.SetSoundVolume(winSound, effectVolume); // Set win sound volume
        Raylib.SetSoundVolume(gameOverSound, effectVolume); // Set game over sound volume
        Raylib.SetSoundVolume(buttonSound, effectVolume); // Set button sound volume
        Raylib.SetSoundVolume(playerShootEffect, effectVolume); // Set player shoot sound volume
        Raylib.SetSoundVolume(enemyShootEffect, effectVolume); // Set enemy shoot sound volume
        Raylib.SetSoundVolume(bossShootEffect, effectVolume); // Set boss shoot sound volume
        Raylib.SetSoundVolume(collisionEffect, effectVolume); // Set collision sound volume
        Raylib.SetSoundVolume(bombEffect, effectVolume); // Set bomb sound volume
        Raylib.SetSoundVolume(bulletCollisionEffect, effectVolume); // Set bullet collision sound volume
        Raylib.SetSoundVolume(coinSound, effectVolume); // Set coin collection sound volume
    }

    // Unload all the audio resources
    public void UnloadAll()
    {
        Raylib.UnloadMusicStream(menuMusic); // Unload menu music stream
        Raylib.UnloadMusicStream(gameMusic); // Unload game music stream

        Raylib.UnloadSound(buttonSound); // Unload button sound
        Raylib.UnloadSound(playerShootEffect); // Unload player shoot sound
        Raylib.UnloadSound(enemyShootEffect); // Unload enemy shoot sound
        Raylib.UnloadSound(bossShootEffect); // Unload boss shoot sound
        Raylib.UnloadSound(winSound); // Unload win sound
        Raylib.UnloadSound(gameOverSound); // Unload game over sound
        Raylib.UnloadSound(collisionEffect); // Unload collision sound
        Raylib.UnloadSound(bombEffect); // Unload bomb sound
        Raylib.UnloadSound(bulletCollisionEffect); // Unload bullet collision sound
        Raylib.UnloadSound(coinSound); // Unload coin collection sound
    }
}