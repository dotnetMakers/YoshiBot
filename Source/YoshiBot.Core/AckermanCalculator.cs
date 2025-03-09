using Meadow.Units;
using System;

namespace YoshiBot.Core;

/// <summary>
/// Provides functions for calculating Ackerman steering angles and wheel speed ratios
/// </summary>
public class AckermanCalculator
{
    /// <summary>
    /// Calculates the Ackerman angles and speed ratios for wheels during a turn,
    /// assuming wheels are at vehicle edges (y = w/2)
    /// </summary>
    /// <param name="w">Distance between wheels (track width)</param>
    /// <param name="x">Distance to the turn center (from vehicle center)</param>
    /// <returns>A tuple containing inner wheel angle, outer wheel angle, and speed ratio</returns>
    public static (double InnerWheelAngle, double OuterWheelAngle, double SpeedRatio) CalculateAckerman(double w, double x)
    {
        // Validate input parameters
        if (w <= 0)
            throw new ArgumentException("Track width must be positive", nameof(w));

        if (x == 0)
            return (0, 0, 1); // No turning, wheels straight, equal speed

        // Calculate wheel angles using the Ackerman principle
        // For a right turn (x > 0), the left wheel is the inner wheel
        // For a left turn (x < 0), the right wheel is the inner wheel
        bool isRightTurn = x > 0;

        // Calculate distances from each wheel to the turn center
        double innerWheelToCenter = Math.Abs(x) - w / 2;
        double outerWheelToCenter = Math.Abs(x) + w / 2;

        // Calculate angles (in radians)
        double innerWheelAngle = Math.Atan(w / (2 * innerWheelToCenter));
        double outerWheelAngle = Math.Atan(w / (2 * outerWheelToCenter));

        // Convert to degrees
        innerWheelAngle = innerWheelAngle * 180 / Math.PI;
        outerWheelAngle = outerWheelAngle * 180 / Math.PI;

        // Calculate speed ratio (outer wheel speed / inner wheel speed)
        double speedRatio = outerWheelToCenter / innerWheelToCenter;

        return (innerWheelAngle, outerWheelAngle, speedRatio);
    }

    /// <summary>
    /// Calculates the outer wheel angle and speed based on the inner wheel angle and speed,
    /// assuming wheels are at vehicle edges (y = w/2)
    /// </summary>
    /// <param name="trackWidth">Distance between wheels (track width)</param>
    /// <param name="innerWheelAngle">Angle of the inner wheel (in degrees)</param>
    /// <param name="innerWheelSpeed">Speed of the inner wheel</param>
    /// <returns>A tuple containing the outer wheel angle (in degrees) and outer wheel speed</returns>
    public static (Angle OuterWheelAngle, double SpeedRatio) CalculateOuterWheelFromInner(
        Length trackWidth, Angle innerWheelAngle)
    {
        // Validate input parameters
        if (trackWidth <= Length.Zero)
        {
            throw new ArgumentException("Track width must be positive", nameof(trackWidth));
        }
        if (innerWheelAngle.Degrees > 90 || innerWheelAngle.Degrees < -90)
        {
            throw new ArgumentException("Wheen angle cannot be > 90 or < -90)", nameof(innerWheelAngle));
        }

        if (innerWheelAngle == Angle.Zero)
        {
            return (Angle.Zero, 1d); // No turning, straight line
        }

        // Store the sign of the angle to preserve direction
        var sign = Math.Sign(innerWheelAngle.Radians);
        var absInnerAngle = new Angle(Math.Abs(innerWheelAngle.Radians), Angle.UnitType.Radians);

        // Calculate turn radius for the inner wheel (absolute value)
        var innerRadius = trackWidth.Millimeters / (2 * Math.Tan(absInnerAngle.Radians));

        // Calculate distance to turn center
        var distanceToTurnCenter = innerRadius + trackWidth.Millimeters / 2d;

        // Calculate outer wheel angle
        var outerRadius = distanceToTurnCenter + trackWidth.Millimeters / 2;
        var outerAngle = new Angle(
            sign * Math.Atan(trackWidth.Millimeters / (2d * outerRadius)),
            Angle.UnitType.Radians);

        // Calculate speed ratio and outer wheel speed
        var speedRatio = outerRadius / innerRadius;

        return (outerAngle, speedRatio);
    }
}
