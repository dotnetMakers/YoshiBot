using Meadow;
using Meadow.Peripherals.Displays;

namespace YoshiBot.Core;

public interface IYoshiBotHardware : IMeadowAppEmbeddedHardware
{
    IPixelDisplay? Display { get; }
    SteerableDrivePair FrontDrivePair { get; }
}
