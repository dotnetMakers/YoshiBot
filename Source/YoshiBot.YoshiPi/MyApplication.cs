using Meadow;
using YoshiBot.Core;

namespace YoshiBot.YoshiPi;

internal sealed class MyApplication : YoshiBotApp
{
    private MainController _controller;

    public override Task Run()
    {
        _controller = new MainController(Hardware);

        return _controller.Run();
    }

    //public async Task RunServo()
    //{
    //    while (true)
    //    {
    //        Resolver.Log.Info("R");
    //        _driveAssembly.SteeringServo.RotateTo(new Angle(30, Angle.UnitType.Degrees));
    //        await Task.Delay(3000);
    //        Resolver.Log.Info("L");
    //        _driveAssembly.SteeringServo.RotateTo(new Angle(-30, Angle.UnitType.Degrees));
    //        await Task.Delay(3000);
    //    }
    //}

    //public async Task RunMotor()
    //{
    //    Resolver.Log.Info("Run");

    //    var display = Resolver.Services.Get<DisplayService>();

    //    var speed = 0.0f;
    //    var direction = RotationDirection.Clockwise;
    //    var increment = 10f;
    //    var delay = 200;

    //    while (true)
    //    {
    //        display?.SetLabelText($"{speed:N0} {direction}");

    //        try
    //        {
    //            _ = _driveAssembly.DriveMotor.Run(direction, speed);
    //            await Task.Delay(delay);

    //            speed += increment;

    //            if (speed > 100)
    //            {
    //                _driveAssembly.SteeringServo.RotateTo(new Angle(30, Angle.UnitType.Degrees));

    //                speed = 100f;
    //                increment *= -1;
    //                delay = 2000;
    //            }
    //            else if (speed < 0)
    //            {
    //                speed = 0;
    //                increment *= -1;

    //                _driveAssembly.SteeringServo.RotateTo(new Angle(-30, Angle.UnitType.Degrees));

    //                direction = direction == RotationDirection.Clockwise
    //                    ? RotationDirection.CounterClockwise
    //                    : RotationDirection.Clockwise;
    //                delay = 3000;
    //            }
    //            else
    //            {
    //                delay = 200;
    //            }
    //        }
    //        catch (Exception ex)
    //        {
    //            Resolver.Log.Info($"EXCEPTION: {ex.Message}");
    //        }
    //    }
    //}

    public static async Task Main(string[] args)
    {
        await MeadowOS.Start(args);
    }
}
