using Meadow;
using YoshiBot.Core;

namespace YoshiBot.YoshiPi;

public class YoshiBotHardwareProvider : IMeadowAppEmbeddedHardwareProvider<IYoshiBotHardware>
{
    private YoshiBotHardwareProvider()
    {
    }

    public IYoshiBotHardware Create(IMeadowDevice device)
    {
        if (device is RaspberryPi pi)
        {
            return new YoshiBotProto(pi);
        }

        // this method is called by MeadowOS, so we should never get here
        throw new Exception("Invalid IMeadowDevice provided");
    }
}
