using System;

namespace AzureSignalRSample.Domain
{
    public class LightBulb
    {
        public bool IsOn { get; set; }

        public event EventHandler<LightBulbPowerStatusChangedEventArgs> PowerStatusChanged;

        public void Toggle(string notes)
        {
            IsOn = !IsOn;

            PowerStatusChanged?.Invoke(this, new LightBulbPowerStatusChangedEventArgs(IsOn, notes));
        }
    }
}
