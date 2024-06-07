public class ThresholdPoints
{
    private int thresholds = 3;
    private int currentThreshold = 0;

    public bool ClaimThreshhold()
    {
        currentThreshold++;
        if (currentThreshold < thresholds)
        {
            return false;
        }
        
        currentThreshold -= thresholds;
        return true;
    }
}
