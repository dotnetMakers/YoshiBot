using Meadow.Hardware;
using Meadow.Peripherals;
using Meadow.Peripherals.Motors;
using Meadow.Units;

namespace YoshiBot.YoshiPi;

public partial class Drv8833
{
    public class Channel : IVariableSpeedMotor
    {
        // since the motor doesn't move at < 0.3 duty cycle, scale the request from this floor to a maximum
        private const float MinimumDutyCycle = 0.1f;

        private readonly IPwmPort pwm1;
        private readonly IPwmPort pwm2;
        private readonly bool invertDirection;

        /// <summary>
        /// Initializes a new instance of the DRV8833 motor driver.
        /// </summary>
        /// <param name="pwm1">First PWM port for motor control</param>
        /// <param name="pwm2">Second PWM port for motor control</param>
        /// <param name="invertDirection">Optional parameter to invert the motor direction. Default is false.</param>
        public Channel(IPwmPort pwm1, IPwmPort pwm2, AngularVelocity maxVelocity, bool invertDirection = false)
        {
            this.invertDirection = invertDirection;
            this.pwm1 = pwm1;
            this.pwm2 = pwm2;

            this.pwm1.DutyCycle = this.pwm2.DutyCycle = 0;
            MaxVelocity = maxVelocity;
        }

        public RotationDirection Direction { get; private set; }

        public bool IsMoving => throw new NotImplementedException();

        public AngularVelocity MaxVelocity { get; private set; }

        public event EventHandler<bool>? StateChanged;

        /// <inheritdoc/>
        public async Task Run(RotationDirection direction, float speed = 100, CancellationToken cancellationToken = default)
        {
            if (speed < 0) speed = 0.0f;
            if (speed > 100) speed = 100f;

            // scale to actual moveable speed
            if (speed > 0)
            {
                speed = MinimumDutyCycle + (speed * (1.0f - MinimumDutyCycle));
            }

            var selectedDirection = direction;
            if (invertDirection)
            {
                selectedDirection = direction switch
                {
                    RotationDirection.Clockwise => RotationDirection.CounterClockwise,
                    _ => RotationDirection.Clockwise
                };
            }

            switch (selectedDirection)
            {
                case RotationDirection.Clockwise:
                    pwm1.DutyCycle = speed / 100f;
                    pwm2.DutyCycle = 0.0;
                    break;
                default:
                    pwm1.DutyCycle = 0.0;
                    pwm2.DutyCycle = speed / 100f;
                    break;
            }

            pwm1.Start();
            pwm2.Start();

            await Task.Run(async () =>
            {
                while (true)
                {
                    if (cancellationToken.IsCancellationRequested)
                    {
                        await Stop();
                    }
                    await Task.Delay(100);
                }
            });
        }

        public Task Run(RotationDirection direction, CancellationToken cancellationToken = default)
        {
            return Run(direction, 100, cancellationToken);
        }

        public Task Run(RotationDirection direction, AngularVelocity angularVelocity, CancellationToken cancellationToken = default)
        {
            var speed = angularVelocity.RevolutionsPerMinute / MaxVelocity.RevolutionsPerMinute;
            if (speed > 1) speed = 1;
            speed *= 100;
            return Run(direction, (float)speed, cancellationToken);
        }

        public async Task RunFor(TimeSpan runTime, RotationDirection direction, float speed = 100, CancellationToken cancellationToken = default)
        {
            _ = Run(direction, speed, cancellationToken);
            await Task.Delay(runTime);
            _ = Stop();
        }

        public async Task RunFor(TimeSpan runTime, RotationDirection direction, CancellationToken cancellationToken = default)
        {
            _ = Run(direction, 100f, cancellationToken);
            await Task.Delay(runTime);
            _ = Stop();
        }

        public Task RunFor(TimeSpan runTime, RotationDirection direction, AngularVelocity angularVelocity, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task Stop(CancellationToken cancellationToken = default)
        {
            pwm1.DutyCycle = pwm2.DutyCycle = 0;
            return Task.CompletedTask;
        }
    }
}
