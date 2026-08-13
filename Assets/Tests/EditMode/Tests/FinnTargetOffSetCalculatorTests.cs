using NUnit.Framework;
using UnityEngine;

public class FinnTargetOffsetCalculatorTests
{
    private const float MaxRadius = 1.5f;
    private const float SmoothTime = 0.15f;
    private const float MaxSpeed = 8f;
    private const float DeltaTime = 1f / 60f;
    private const int FramesToConverge = 300; // 5 segundos simulados, tempo de sobra para convergir

    private static Vector2 Simulate(Vector2 stickInput, Vector2 directionToPointer, bool pointerHeld, int frames)
    {
        Vector2 offset = Vector2.zero;
        Vector2 velocity = Vector2.zero;

        for (int i = 0; i < frames; i++)
        {
            offset = FinnTargetOffsetCalculator.CalculateNewOffset(
                currentOffset: offset,
                currentVelocity: ref velocity,
                stickInput: stickInput,
                directionToPointer: directionToPointer,
                pointerHeld: pointerHeld,
                maxRadius: MaxRadius,
                smoothTime: SmoothTime,
                maxSpeed: MaxSpeed,
                deltaTime: DeltaTime
            );
        }

        return offset;
    }

    [Test]
    public void ShouldConvergeTowardsZero_WhenNoInputOverTime()
    {
        Vector2 result = Simulate(Vector2.zero, Vector2.zero, false, FramesToConverge);

        Assert.Less(result.magnitude, 0.01f);
    }

    [Test]
    public void ShouldConvergeTowardsStickDirection_AtMaxRadius()
    {
        Vector2 result = Simulate(Vector2.right, Vector2.zero, false, FramesToConverge);

        Assert.AreEqual(MaxRadius, result.magnitude, 0.02f);
        Assert.Greater(result.x, 0f);
    }

    [Test]
    public void ShouldConvergeProportionally_WhenStickMagnitudeIsHalf()
    {
        Vector2 result = Simulate(Vector2.right * 0.5f, Vector2.zero, false, FramesToConverge);

        Assert.AreEqual(MaxRadius * 0.5f, result.magnitude, 0.02f);
    }

    [Test]
    public void ShouldIgnoreStickInput_BelowDeadZone()
    {
        Vector2 result = Simulate(Vector2.right * 0.05f, Vector2.zero, false, FramesToConverge);

        Assert.Less(result.magnitude, 0.01f);
    }

    [Test]
    public void ShouldConvergeTowardsPointerDirection_WhenHeldAtMaxRadius()
    {
        Vector2 result = Simulate(Vector2.zero, Vector2.up, true, FramesToConverge);

        Assert.AreEqual(MaxRadius, result.magnitude, 0.02f);
        Assert.Greater(result.y, 0f);
    }

    [Test]
    public void ShouldNotMoveTowardsPointer_WhenNotHeld()
    {
        Vector2 result = Simulate(Vector2.zero, Vector2.up, false, FramesToConverge);

        Assert.Less(result.magnitude, 0.01f);
    }

    [Test]
    public void ShouldPrioritizeStick_WhenBothStickAndPointerAreActive()
    {
        Vector2 result = Simulate(Vector2.right, Vector2.up, true, FramesToConverge);

        Assert.Greater(result.x, 1f);
        Assert.Less(Mathf.Abs(result.y), 0.02f);
    }

    [Test]
    public void ShouldAccelerateGradually_InsteadOfJumpingToFullSpeedImmediately()
    {
        Vector2 offset = Vector2.zero;
        Vector2 velocity = Vector2.zero;

        Vector2 previousOffset = offset;
        offset = FinnTargetOffsetCalculator.CalculateNewOffset(
            offset, ref velocity, Vector2.right, Vector2.zero, false, MaxRadius, SmoothTime, MaxSpeed, DeltaTime);
        float firstStepDistance = Vector2.Distance(previousOffset, offset);

        // avança alguns frames para chegar perto do pico de velocidade
        for (int i = 0; i < 5; i++)
        {
            previousOffset = offset;
            offset = FinnTargetOffsetCalculator.CalculateNewOffset(
                offset, ref velocity, Vector2.right, Vector2.zero, false, MaxRadius, SmoothTime, MaxSpeed, DeltaTime);
        }
        float laterStepDistance = Vector2.Distance(previousOffset, offset);

        Assert.Greater(laterStepDistance, firstStepDistance);
    }
}