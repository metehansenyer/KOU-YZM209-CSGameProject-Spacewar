namespace KOU_YZM209_CSGameProject_Spacewar;

public class Level
{
    public int id; // The unique identifier for the level
    public int scoreThreshold; // The score required to move to the next level
    public int maxEnemyCount; // The maximum number of enemies allowed in the level
    public float levelUpAnimationTime; // The time duration for the level-up animation
    public float elapsedTime; // The time that has passed since the level started
    public bool isLevelPowerUpActive; // Flag indicating if the power-up feature is active in this level
    public int collectibleChance; // The probability of collectibles appearing in the level
}