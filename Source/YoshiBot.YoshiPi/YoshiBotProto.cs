using Meadow;
using Meadow.Foundation.Displays;
using Meadow.Foundation.ICs.IOExpanders;
using Meadow.Foundation.Servos;
using Meadow.Hardware;
using Meadow.Peripherals.Displays;
using Meadow.Units;
using YoshiBot.Core;

namespace YoshiBot.YoshiPi;

public class YoshiBotProto : IYoshiBotHardware
{
    private II2cBus _i2cBus;
    private Pca9685 _pwmGenerator;
    private Drv8833 _motorDrive;
    private Ds3218 _leftServo;
    private Ds3218 _rightServo;

    private readonly RaspberryPi _computeModule;

    /// <inheritdoc/>
    public IMeadowDevice ComputeModule => _computeModule;

    public YoshiBotProto(RaspberryPi computeModule)
    {
        var frequency = 50.Hertz();

        _computeModule = computeModule;
        _i2cBus = _computeModule.CreateI2cBus();

        Resolver.Log.Info("Creating Display");
        Display = new Ssd1306(_i2cBus);

        _pwmGenerator = new Pca9685(_i2cBus, frequency, 64);

        Resolver.Log.Info("Creating MotorAssembly");
        _motorDrive = new Drv8833(
            new Drv8833.Channel(
                _pwmGenerator.Pins.LED0.CreatePwmPort(frequency),
                _pwmGenerator.Pins.LED1.CreatePwmPort(frequency)
                ),
            new Drv8833.Channel(
                _pwmGenerator.Pins.LED2.CreatePwmPort(frequency),
                _pwmGenerator.Pins.LED3.CreatePwmPort(frequency)
                )
        );

        Resolver.Log.Info("Creating Servos");
        _leftServo = new Ds3218(_pwmGenerator.Pins.LED14.CreatePwmPort(frequency));
        _rightServo = new Ds3218(_pwmGenerator.Pins.LED15.CreatePwmPort(frequency));

        LeftDriveAssembly = new SteerableDriveAssembly(
            _motorDrive.Channel1!,
            _leftServo);

        RightDriveAssembly = new SteerableDriveAssembly(
            _motorDrive.Channel2!,
            _rightServo);

        Resolver.Log.Info($"Left DA: {LeftDriveAssembly} {LeftDriveAssembly.DriveMotor} {LeftDriveAssembly.SteeringServo}");
        Resolver.Log.Info($"Right DA: {RightDriveAssembly} {RightDriveAssembly.DriveMotor} {RightDriveAssembly.SteeringServo}");
    }

    public IPixelDisplay? Display { get; }
    public ISteerableDriveAssembly LeftDriveAssembly { get; }
    public ISteerableDriveAssembly RightDriveAssembly { get; }
}
