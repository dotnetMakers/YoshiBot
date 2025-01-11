using Meadow;
using Meadow.Foundation.ICs.IOExpanders;
using Meadow.Units;
using YoshiPi;

namespace YoshiBot.YoshiPi;

internal sealed class MyApplication : YoshiPiApp
{
    private Pca9685 _pwmGenerator;
    private MotorAssembly _motor;

    public override Task Initialize()
    {
        Resolver.Log.Info("Initialize");

        Resolver.Log.Info("Creating PCA9685");

        var frequency = 500.Hertz();

        _pwmGenerator = new Pca9685(Hardware.GroveI2c, frequency, 64);

        Resolver.Log.Info("Creating MotorAssembly");
        _motor = new MotorAssembly(
            _pwmGenerator.Pins.LED0.CreatePwmPort(frequency),
            _pwmGenerator.Pins.LED1.CreatePwmPort(frequency)
            );

        Resolver.Services.Add(
            new DisplayService(Hardware.Display, Hardware.Touchscreen)
            );

        return base.Initialize();
    }

    public override async Task Run()
    {
        Resolver.Log.Info("Run");

        var display = Resolver.Services.Get<DisplayService>();

        display?.SetLabelText("Hello YoshiPi!");

        var speed = 0.0f;
        var direction = MotorDirection.Forward;
        var increment = 0.1f;
        var delay = 200;

        while (true)
        {
            display?.SetLabelText($"{speed * 100:N0} {direction}");

            _motor.Run(speed, direction);

            await Task.Delay(delay);

            speed += increment;

            if (speed > 1.0)
            {
                speed = 1.0f;
                increment *= -1;
                delay = 2000;
            }
            else if (speed < 0)
            {
                speed = 0;
                increment *= -1;

                direction = direction == MotorDirection.Forward
                    ? MotorDirection.Backward
                    : MotorDirection.Forward;
                delay = 3000;
            }
            else
            {
                delay = 200;
            }
        }
    }

    public static async Task Main(string[] args)
    {
        await MeadowOS.Start(args);
    }
}
