﻿using Meadow;
using Meadow.Units;
using System.Threading;
using System.Threading.Tasks;

namespace YoshiBot.Core;

public class MainController
{
    private readonly IYoshiBotHardware _hardware;
    private readonly DisplayService? _displayService;

    public MainController(IYoshiBotHardware hardware)
    {
        _hardware = hardware;
        if (hardware.Display != null)
        {
            _displayService = new DisplayService(hardware.Display);
        }
    }

    private SteeringTest _exercise;

    public async Task Run()
    {
        ulong tick = 0;

        _exercise = new SteeringTest(_hardware);

        _exercise.Run();

        while (true)
        {
            tick++;
            _displayService?.UpdateTick(tick);
            _exercise.Step();

            Resolver.Log.Info("MainController state machine tick");

            await Task.Delay(1000);
        }
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
        var speed = new AngularVelocity(50, AngularVelocity.UnitType.RevolutionsPerMinute);

        _hardware.FrontDrivePair.SetSpeed(speed);

        int i;
        for (i = 0; i < 30; i++)
        {
            _hardware.FrontDrivePair.Steer(new Angle(i, Angle.UnitType.Degrees));
            Thread.Sleep(50);
        }
        while (true)
        {
            for (; i > -30; i--)
            {
                _hardware.FrontDrivePair.Steer(new Angle(i, Angle.UnitType.Degrees));
                Thread.Sleep(50);
            }
            for (; i < 40; i++)
            {
                _hardware.FrontDrivePair.Steer(new Angle(i, Angle.UnitType.Degrees));
                Thread.Sleep(50);
            }
        }
    }

    public void Step()
    {
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