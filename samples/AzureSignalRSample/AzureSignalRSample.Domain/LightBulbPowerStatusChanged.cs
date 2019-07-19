namespace AzureSignalRSample.Domain
{
    public class LightBulbPowerStatusChanged
    {
        public bool IsOn { get; }
        public string Notes { get; }

        public LightBulbPowerStatusChanged(bool isOn, string notes)
        {
            IsOn = isOn;
            Notes = notes;
        }
    }
}