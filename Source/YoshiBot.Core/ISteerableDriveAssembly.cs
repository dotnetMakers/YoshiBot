using Meadow.Peripherals.Motors;
using Meadow.Peripherals.Servos;

namespace YoshiBot.Core;

public interface ISteerableDriveAssembly
{
    IVariableSpeedMotor DriveMotor { get; }
    IAngularServo SteeringServo { get; }
}
