using FluentEvents;

namespace AzureSignalRSample.Domain
{
    public class LightBulb
    {
        public bool IsOn { get; set; }

        public event EventPublisher<LightBulbPowerStatusChanged> PowerStatusChanged;

        public void Toggle(string notes)
        {
            IsOn = !IsOn;

            PowerStatusChanged?.Invoke(new LightBulbPowerStatusChanged(IsOn, notes));
        }
    }
}
