namespace AzureSignalRSample.Domain
{
    public class LightBulbPowerStatusChangedEventArgs
    {
        public bool IsOn { get; }
        public string Notes { get; }

        public LightBulbPowerStatusChangedEventArgs(bool isOn, string notes)
        {
            IsOn = isOn;
            Notes = notes;
        }
    }
}