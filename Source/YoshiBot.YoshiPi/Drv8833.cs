using Meadow.Hardware;

namespace YoshiBot.YoshiPi;

/// <summary>
/// Represents a DRV8833 dual H-bridge motor driver controller.
/// </summary>
public partial class Drv8833
{
    public Channel? Channel1 { get; private set; }
    public Channel? Channel2 { get; private set; }

    public Drv8833(IPwmPort? in1, IPwmPort? in2, IPwmPort? in3, IPwmPort? in4)
    {
        Channel? channel1 = null;
        Channel? channel2 = null;

        if (in1 != null && in2 != null)
        {
            channel1 = new Channel(in1, in2);
        }
        if (in3 != null && in4 != null)
        {
            channel2 = new Channel(in3, in4);
        }

        Initialize(channel1, channel2);
    }

    public Drv8833(Channel? channel1, Channel? channel2)
    {
        Initialize(channel1, channel2);
    }

    private void Initialize(Channel? channel1, Channel? channel2)
    {
        if (channel1 == null && channel2 == null)
        {
            throw new ArgumentException("At least one channel must be non-null");
        }

        Channel1 = channel1;
        Channel2 = channel2;
    }
}
