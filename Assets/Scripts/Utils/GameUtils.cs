using UnityEngine;

public static class GameUtils
{
    public static Vector3 GetMouseWorldPosition()
    {
        // It's good practice to cache Camera.main if called very frequently,
        // but for a utility like this, direct access is often fine.
        // If performance issues arise, consider passing Camera as a parameter.
        Camera mainCamera = Camera.main;
        if (mainCamera == null)
        {
            Debug.LogError("GameUtils.GetMouseWorldPosition: Camera.main is null!");
            return Vector3.zero;
        }

        var mousePosition = Input.mousePosition;
        // ScreenToWorldPoint expects Z to be the distance from the camera.
        // If using a perspective camera and you want the position on a specific Z plane,
        // mousePosition.z should be set to that distance.
        // For a 2D setup with an orthographic camera, Z is often irrelevant for ScreenToWorldPoint
        // but we set the result's Z to 0f afterwards.
        Vector3 screenToWorldPosition = mainCamera.ScreenToWorldPoint(new Vector3(mousePosition.x, mousePosition.y, mainCamera.nearClipPlane));
        
        screenToWorldPosition.z = 0f; // Ensure Z is 0 for 2D
        return screenToWorldPosition;
    }
}
