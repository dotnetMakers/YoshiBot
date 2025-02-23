using Meadow;
using Meadow.Peripherals;
using Meadow.Units;
using System.Threading.Tasks;

namespace YoshiBot.Core;

public class MainController
{
    private IYoshiBotHardware _hardware;
    private DisplayService? _displayService;

    public MainController(IYoshiBotHardware hardware)
    {
        _hardware = hardware;
        if (hardware.Display != null)
        {
            _displayService = new DisplayService(hardware.Display);
        }
    }

    private DriveExercise _exercise;

    public async Task Run()
    {
        ulong tick = 0;

        _exercise = new DriveExercise(_hardware);

        while (true)
        {
            tick++;
            _displayService?.UpdateTick(tick);
            _exercise.Step();

            Resolver.Log.Info("MainController state machine tick");

            await Task.Delay(3000);
        }
    }
}

public class DriveExercise
{
    private enum ExerciseState
    {
        Stopped,
        ForwardStraight,
        ForwardRight,
        ForwardLeft,
        BackwardStraight,
        BackwardRight,
        BackwardLeft,
    }

    private IYoshiBotHardware _hardware;
    private ExerciseState _currentState;

    public DriveExercise(IYoshiBotHardware hardware)
    {
        _currentState = ExerciseState.Stopped;
        _hardware = hardware;
    }

    public void Step()
    {
        Resolver.Log.Info(_currentState.ToString());

        var posAngle = new Angle(25, Angle.UnitType.Degrees);
        var negAngle = new Angle(-25, Angle.UnitType.Degrees);
        var speed = 25;

        switch (_currentState)
        {
            case ExerciseState.Stopped:
                _hardware.LeftDriveAssembly.DriveMotor.Stop();
                _hardware.RightDriveAssembly.DriveMotor.Stop();
                _hardware.RightDriveAssembly.SteeringServo.RotateTo(Angle.Zero);
                _hardware.LeftDriveAssembly.SteeringServo.RotateTo(Angle.Zero);
                break;
            case ExerciseState.ForwardStraight:
                _hardware.LeftDriveAssembly.DriveMotor.Run(RotationDirection.Clockwise, speed);
                _hardware.RightDriveAssembly.DriveMotor.Run(RotationDirection.CounterClockwise, speed);
                _hardware.RightDriveAssembly.SteeringServo.RotateTo(Angle.Zero);
                _hardware.LeftDriveAssembly.SteeringServo.RotateTo(Angle.Zero);
                break;
            case ExerciseState.ForwardRight:
                _hardware.LeftDriveAssembly.DriveMotor.Run(RotationDirection.Clockwise, speed);
                _hardware.RightDriveAssembly.DriveMotor.Run(RotationDirection.CounterClockwise, speed);
                _hardware.RightDriveAssembly.SteeringServo.RotateTo(posAngle);
                _hardware.LeftDriveAssembly.SteeringServo.RotateTo(posAngle);
                break;
            case ExerciseState.ForwardLeft:
                _hardware.LeftDriveAssembly.DriveMotor.Run(RotationDirection.Clockwise, speed);
                _hardware.RightDriveAssembly.DriveMotor.Run(RotationDirection.CounterClockwise, speed);
                _hardware.RightDriveAssembly.SteeringServo.RotateTo(negAngle);
                _hardware.LeftDriveAssembly.SteeringServo.RotateTo(negAngle);
                break;
            case ExerciseState.BackwardStraight:
                _hardware.LeftDriveAssembly.DriveMotor.Run(RotationDirection.CounterClockwise, speed);
                _hardware.RightDriveAssembly.DriveMotor.Run(RotationDirection.Clockwise, speed);
                _hardware.RightDriveAssembly.SteeringServo.RotateTo(Angle.Zero);
                _hardware.LeftDriveAssembly.SteeringServo.RotateTo(Angle.Zero);
                break;
            case ExerciseState.BackwardRight:
                _hardware.LeftDriveAssembly.DriveMotor.Run(RotationDirection.CounterClockwise, speed);
                _hardware.RightDriveAssembly.DriveMotor.Run(RotationDirection.Clockwise, speed);
                _hardware.RightDriveAssembly.SteeringServo.RotateTo(posAngle);
                _hardware.LeftDriveAssembly.SteeringServo.RotateTo(posAngle);
                break;
            case ExerciseState.BackwardLeft:
                _hardware.LeftDriveAssembly.DriveMotor.Run(RotationDirection.CounterClockwise, speed);
                _hardware.RightDriveAssembly.DriveMotor.Run(RotationDirection.Clockwise, speed);
                _hardware.RightDriveAssembly.SteeringServo.RotateTo(negAngle);
                _hardware.LeftDriveAssembly.SteeringServo.RotateTo(negAngle);
                break;
        }

        _currentState = _currentState switch
        {
            ExerciseState.BackwardLeft => ExerciseState.Stopped,
            _ => (ExerciseState)(int)_currentState + 1
        };
    }
}
