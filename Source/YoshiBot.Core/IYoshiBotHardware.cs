using Meadow;
using Meadow.Peripherals.Displays;

namespace YoshiBot.Core;

public interface IYoshiBotHardware : IMeadowAppEmbeddedHardware
{
    IPixelDisplay? Display { get; }
    ISteerableDriveAssembly LeftDriveAssembly { get; }
    ISteerableDriveAssembly RightDriveAssembly { get; }
}
