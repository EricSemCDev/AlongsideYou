using UnityEngine;

public static class FinnTargetOffsetCalculator
{
    public static Vector2 CalculateNewOffset(
        Vector2 currentOffset,
        ref Vector2 currentVelocity,
        Vector2 stickInput,
        Vector2 directionToPointer,
        bool pointerHeld,
        float maxRadius,
        float smoothTime,
        float maxSpeed,
        float deltaTime,
        float stickDeadZone = 0.15f)
    {
        Vector2 desiredOffset = Vector2.zero;

        if (stickInput.magnitude > stickDeadZone)
        {
            float clampedMagnitude = Mathf.Clamp01(stickInput.magnitude);
            desiredOffset = stickInput.normalized * clampedMagnitude * maxRadius;
        }
        else if (pointerHeld && directionToPointer != Vector2.zero)
        {
            desiredOffset = directionToPointer.normalized * maxRadius;
        }

        float velocityX = currentVelocity.x;
        float velocityY = currentVelocity.y;

        float newX = Mathf.SmoothDamp(currentOffset.x, desiredOffset.x, ref velocityX, smoothTime, maxSpeed, deltaTime);
        float newY = Mathf.SmoothDamp(currentOffset.y, desiredOffset.y, ref velocityY, smoothTime, maxSpeed, deltaTime);

        currentVelocity = new Vector2(velocityX, velocityY);

        return new Vector2(newX, newY);
    }
}