using Meadow;
using Meadow.Foundation.ICs.IOExpanders;
using Meadow.Foundation.Servos;
using Meadow.Peripherals;
using Meadow.Units;
using YoshiPi;

namespace YoshiBot.YoshiPi;

internal sealed class MyApplication : YoshiPiApp
{
    private Pca9685 _pwmGenerator;
    private Drv8833 _motorDrive;
    private Ds3218 _servo;
    private SteerableDriveAssembly _driveAssembly;

    public override Task Initialize()
    {
        Resolver.Log.Info("Initialize");

        Resolver.Log.Info("Creating PCA9685");

        var frequency = 50.Hertz();
        //var frequency = 500.Hertz();

        _pwmGenerator = new Pca9685(Hardware.GroveI2c, frequency, 64);

        Resolver.Log.Info("Creating MotorAssembly");
        _motorDrive = new Drv8833(
            new Drv8833.Channel(
                _pwmGenerator.Pins.LED0.CreatePwmPort(frequency),
                _pwmGenerator.Pins.LED1.CreatePwmPort(frequency)
                ),
            null
            );

        _servo = new Ds3218(_pwmGenerator.Pins.LED15.CreatePwmPort(frequency));

        _driveAssembly = new SteerableDriveAssembly(
            _motorDrive.Channel1,
            _servo);

        Resolver.Services.Add(
            new DisplayService(Hardware.Display, Hardware.Touchscreen)
            );

        return base.Initialize();
    }

    public override Task Run()
    {
        return RunMotor();
    }

    public async Task RunServo()
    {
        while (true)
        {
            Resolver.Log.Info("R");
            _driveAssembly.SteeringServo.RotateTo(new Angle(30, Angle.UnitType.Degrees));
            await Task.Delay(3000);
            Resolver.Log.Info("L");
            _driveAssembly.SteeringServo.RotateTo(new Angle(-30, Angle.UnitType.Degrees));
            await Task.Delay(3000);
        }
    }

    public async Task RunMotor()
    {
        Resolver.Log.Info("Run");

        var display = Resolver.Services.Get<DisplayService>();

        var speed = 0.0f;
        var direction = RotationDirection.Clockwise;
        var increment = 10f;
        var delay = 200;

        while (true)
        {
            display?.SetLabelText($"{speed:N0} {direction}");

            try
            {
                _ = _driveAssembly.DriveMotor.Run(direction, speed);
                await Task.Delay(delay);

                speed += increment;

                if (speed > 100)
                {
                    _driveAssembly.SteeringServo.RotateTo(new Angle(30, Angle.UnitType.Degrees));

                    speed = 100f;
                    increment *= -1;
                    delay = 2000;
                }
                else if (speed < 0)
                {
                    speed = 0;
                    increment *= -1;

                    _driveAssembly.SteeringServo.RotateTo(new Angle(-30, Angle.UnitType.Degrees));

                    direction = direction == RotationDirection.Clockwise
                        ? RotationDirection.CounterClockwise
                        : RotationDirection.Clockwise;
                    delay = 3000;
                }
                else
                {
                    delay = 200;
                }
            }
            catch (Exception ex)
            {
                Resolver.Log.Info($"EXCEPTION: {ex.Message}");
            }
        }
    }

    public static async Task Main(string[] args)
    {
        await MeadowOS.Start(args);
    }
}
