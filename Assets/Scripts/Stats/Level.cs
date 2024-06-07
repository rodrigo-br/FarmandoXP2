using UnityEngine;

public class Level
{
    private int level;
    private int experience;
    private int baseExperience;
    private int experienceFactor;

    Level(int level, int baseExperience, int experienceFactor)
    {
        this.level = level;
        this.baseExperience = baseExperience;
        this.experienceFactor = experienceFactor;
        AddExperience(CalculateExperience(level, baseExperience, experienceFactor));
    }

    private int CalculateExperience(int level, int baseExperience, int experienceFactor)
    {
        return baseExperience * (int)Mathf.Pow(experienceFactor, level - 1);
    }

    public int GetCurrentExperience()
    {
        return experience;
    }

    public int GetExperienceForNextLevel()
    {
        return CalculateExperience(level + 1, baseExperience, experienceFactor);
    }

    public int GetLevel()
    {
        return level;
    }

    public void AddExperience(int amount)
    {
        experience += amount;

        while (experience >= GetExperienceForNextLevel())
        {
            experience -= GetExperienceForNextLevel();
            level++;
        }
    }
}
