using Meadow;
using Meadow.Logging;
using Meadow.Peripherals;
using Meadow.Peripherals.Displays;
using Meadow.Peripherals.Motors;
using Meadow.Peripherals.Servos;
using Meadow.Units;
using YoshiBot.Core;

namespace YoshiBot.Desktop;

internal class Program
{
    private static void Main(string[] args)
    {
        Resolver.Services.Add(new Logger());
        Resolver.Log.AddProvider(new ConsoleLogProvider());
        Resolver.Log.LogLevel = LogLevel.Information;

        var test = new SteeringTest(new SimulatedYoshiBotProto());
        test.Run();
    }
}

public class SimulatedAngularServo : IAngularServo
{
    public Angle Angle { get; private set; }
    public Angle MinimumAngle { get; }
    public Angle MaximumAngle { get; }
    public TimePeriod TrimOffset { get; set; }

    public SimulatedAngularServo(Angle boundingAngle)
    {
        MaximumAngle = boundingAngle;
        MinimumAngle = boundingAngle * -1;
    }

    public SimulatedAngularServo(Angle minimumAngle, Angle maximumAngle)
    {
        MinimumAngle = minimumAngle;
        MaximumAngle = maximumAngle;
    }

    public void Disable()
    {
    }

    public void Neutral()
    {
    }

    public void RotateTo(Angle angle)
    {
        if (angle < MinimumAngle)
        {
            Angle = MinimumAngle;
        }
        else if (angle > MaximumAngle)
        {
            Angle = MaximumAngle;
        }
        else
        {
            Angle = angle;
        }

        Resolver.Log.Info($"Angle set to: {Angle.Degrees:N1}");
    }
}

public class SimulatedMotor : IMotor
{
    private bool _isMoving;

    public event EventHandler<bool>? StateChanged;

    public RotationDirection Direction { get; private set; }
    public bool IsMoving
    {
        get => _isMoving;
        protected set
        {
            if (value == IsMoving) { return; }
            _isMoving = value;
            StateChanged?.Invoke(this, IsMoving);
        }
    }

    public SimulatedMotor()
    {
    }

    public Task Run(RotationDirection direction, CancellationToken cancellationToken = default)
    {
        Direction = direction;
        IsMoving = true;
        return Task.CompletedTask;
    }

    public async Task RunFor(TimeSpan runTime, RotationDirection direction, CancellationToken cancellationToken = default)
    {
        await Run(direction);
        await Task.Delay(runTime);
        await Stop();
    }

    public Task Stop(CancellationToken cancellationToken = default)
    {
        IsMoving = false;
        return Task.CompletedTask;
    }
}

public class SimulatedVariableSpeedMotor : SimulatedMotor, IVariableSpeedMotor
{
    public AngularVelocity CommandedVelocity { get; private set; }
    public AngularVelocity MaxVelocity { get; }

    public SimulatedVariableSpeedMotor()
        : this(new AngularVelocity(1750, AngularVelocity.UnitType.RevolutionsPerMinute))
    {
    }

    public SimulatedVariableSpeedMotor(AngularVelocity maxVelocity)
    {
        MaxVelocity = maxVelocity;
    }

    public Task Run(RotationDirection direction, AngularVelocity angularVelocity, CancellationToken cancellationToken = default)
    {
        CommandedVelocity = angularVelocity;
        return base.Run(direction);
    }

    public Task RunFor(TimeSpan runTime, RotationDirection direction, AngularVelocity angularVelocity, CancellationToken cancellationToken = default)
    {
        CommandedVelocity = angularVelocity;
        return base.RunFor(runTime, direction);
    }
}

public class SimulatedSteerableDriveAssembly : ISteerableDriveAssembly
{
    public IVariableSpeedMotor DriveMotor { get; }
    public IAngularServo SteeringServo { get; }

    public SimulatedSteerableDriveAssembly()
    {
        DriveMotor = new SimulatedVariableSpeedMotor();
        SteeringServo = new SimulatedAngularServo(new Angle(120, Angle.UnitType.Degrees));
    }

}

public class SimulatedYoshiBotProto : IYoshiBotHardware
{
    public IPixelDisplay? Display => throw new NotImplementedException();

    public SteerableDrivePair FrontDrivePair { get; }

    public IMeadowDevice ComputeModule => throw new NotImplementedException();

    public SimulatedYoshiBotProto()
    {
        FrontDrivePair = new SteerableDrivePair(
            new SteerableDriveAssembly(new SimulatedVariableSpeedMotor(), new SimulatedAngularServo(new Angle(120, Angle.UnitType.Degrees))),
            new SteerableDriveAssembly(new SimulatedVariableSpeedMotor(), new SimulatedAngularServo(new Angle(120, Angle.UnitType.Degrees))),
            new Length(12, Length.UnitType.Inches),
            new Angle(35, Angle.UnitType.Degrees)
        );
    }
}

public class SteeringTest
{
    private readonly IYoshiBotHardware _hardware;

    public SteeringTest(IYoshiBotHardware hardware)
    {
        _hardware = hardware;

        _hardware.FrontDrivePair.Stop();
        _hardware.FrontDrivePair.Center();
    }

    public void Run()
    {
        int i;
        for (i = 0; i < 30; i++)
        {
            _hardware.FrontDrivePair.Steer(new Angle(i, Angle.UnitType.Degrees));
            Thread.Sleep(100);
        }
        while (true)
        {
            for (; i > -30; i--)
            {
                _hardware.FrontDrivePair.Steer(new Angle(i, Angle.UnitType.Degrees));
                Thread.Sleep(100);
            }
            for (; i < 30; i++)
            {
                _hardware.FrontDrivePair.Steer(new Angle(i, Angle.UnitType.Degrees));
                Thread.Sleep(100);
            }
        }
    }

    public void Step()
    {
    }
}

