namespace YoshiBot.Core;

public class YoshiBotFwd
{
    private readonly IYoshiBotHardware _hardware;

    public YoshiBotFwd(IYoshiBotHardware hardware)
    {
        _hardware = hardware;
    }
}