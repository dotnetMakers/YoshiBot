﻿using Meadow.Peripherals.Motors;
using Meadow.Peripherals.Servos;

namespace YoshiBot.Core;

public class SteerableDriveAssembly : ISteerableDriveAssembly
{
    public IVariableSpeedMotor DriveMotor { get; }
    public IAngularServo SteeringServo { get; }

    public SteerableDriveAssembly(
         IVariableSpeedMotor driveMotor,
         IAngularServo steeringServo)
    {
        DriveMotor = driveMotor;
        SteeringServo = steeringServo;
    }
}
