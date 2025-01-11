using Meadow.Hardware;

namespace YoshiBot.YoshiPi;

public enum MotorDirection
{
    Forward = 0,
    Backward = 1
}

public class MotorAssembly
{
    // since the motor doesn't move at < 0.3 duty cycle, scale the request from this floor to a maximum
    private const float MinimumDutyCycle = 0.1f;

    private IPwmPort _pwm1;
    private IPwmPort _pwm2;
    private bool _invertDirection;

    public MotorAssembly(IPwmPort pwm1, IPwmPort pwm2, bool invertDirection = false)
    {
        _pwm1 = pwm1;
        _pwm2 = pwm2;

        _pwm1.DutyCycle = _pwm2.DutyCycle = 0;
    }

    public void Run(float speed, MotorDirection direction)
    {
        if (speed < 0) speed = 0.0f;
        if (speed > 1.0) speed = 1.0f;

        // scale to actual moveable speed
        if (speed > 0)
        {
            speed = MinimumDutyCycle + (speed * (1.0f - MinimumDutyCycle));
        }

        switch (direction)
        {
            case MotorDirection.Forward:
                _pwm1.DutyCycle = speed;
                _pwm2.DutyCycle = 0.0;
                break;
            default:
                _pwm1.DutyCycle = 0.0;
                _pwm2.DutyCycle = speed;
                break;
        }

        _pwm1.Start();
        _pwm2.Start();
    }
}
