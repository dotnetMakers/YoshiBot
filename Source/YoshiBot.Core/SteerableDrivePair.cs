using Meadow.Peripherals;
using Meadow.Units;

namespace YoshiBot.Core;

public class SteerableDrivePair
{
    private readonly ISteerableDriveAssembly _leftDrive;
    private readonly ISteerableDriveAssembly _rightDrive;
    private readonly Length _trackWidth;
    private readonly Angle _maxAngle;
    private Angle _currentSteerAngle;

    public SteerableDrivePair(
        ISteerableDriveAssembly leftDrive,
        ISteerableDriveAssembly rightDrive,
        Length trackWidth,
        Angle maxAngle)
    {
        _leftDrive = leftDrive;
        _rightDrive = rightDrive;
        _trackWidth = trackWidth;
        _maxAngle = maxAngle;

        Stop();
        Center();
    }

    public void Stop()
    {
        _leftDrive.DriveMotor.Run(RotationDirection.Clockwise, AngularVelocity.Zero);
        _leftDrive.DriveMotor.Run(RotationDirection.CounterClockwise, AngularVelocity.Zero);
    }

    public void Center()
    {
        _leftDrive.SteeringServo.RotateTo(Angle.Zero);
        _rightDrive.SteeringServo.RotateTo(Angle.Zero);
        _currentSteerAngle = Angle.Zero;
    }

    private AngularVelocity _speed;

    public void SetSpeed(AngularVelocity speed)
    {
        _speed = speed;

        if (_currentSteerAngle == Angle.Zero)
        {
            _leftDrive.DriveMotor.Run(RotationDirection.Clockwise, _speed);
            _rightDrive.DriveMotor.Run(RotationDirection.CounterClockwise, _speed);
        }
        else
        {
            var results = AckermanCalculator.CalculateOuterWheelFromInner(_trackWidth, _currentSteerAngle);

            var outer = _speed * results.SpeedRatio;
            var inner = _speed;

            if (_currentSteerAngle.Degrees < 0)
            { // left turn
                _leftDrive.DriveMotor.Run(RotationDirection.Clockwise, inner);
                _rightDrive.DriveMotor.Run(RotationDirection.CounterClockwise, outer);
            }
            else
            { // right turn
                _leftDrive.DriveMotor.Run(RotationDirection.Clockwise, outer);
                _rightDrive.DriveMotor.Run(RotationDirection.CounterClockwise, inner);
            }
        }
    }

    public void Steer(Angle innerWheelAngle)
    {
        if (innerWheelAngle > _maxAngle)
        {
            innerWheelAngle = _maxAngle;
        }

        _currentSteerAngle = innerWheelAngle;

        // TODO: cache these results
        var results = AckermanCalculator.CalculateOuterWheelFromInner(_trackWidth, _currentSteerAngle);

        if (innerWheelAngle >= Angle.Zero)
        {
            // right turn
            _leftDrive.SteeringServo.RotateTo(results.OuterWheelAngle);
            _rightDrive.SteeringServo.RotateTo(innerWheelAngle);
        }
        else
        {
            // left turn
            _leftDrive.SteeringServo.RotateTo(innerWheelAngle);
            _rightDrive.SteeringServo.RotateTo(results.OuterWheelAngle);
        }

        SetSpeed(_speed);
    }
}
/*
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

    private readonly IYoshiBotHardware _hardware;
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
*/