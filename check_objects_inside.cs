// You can use this class to check objects inside the player's screen in percentage coefficients.
// For example, you can say, "We can check if some objects are inside" if they are not, we will show the user a marker to move the camera to the center.

public static class UIUtilityGetRect
{
	public static Rect GetRect(float percentWidth, float percentHeight)
	{
		float setPercentWidth = percentWidth / 100;
		float setPercentHeight = percentHeight / 100;

		float screenWidth = Screen.width;
		float screenHeight = Screen.height;
		float width = (screenWidth * setPercentWidth) / screenWidth;
		float height = (screenHeight * setPercentHeight) / screenHeight;
		float x = 0.5f - (width / 2f);
		float y = 0.5f - (height / 2f);

		return new Rect(x, y, width, height);
	}

	public static bool CheckObjectInRect(Vector3 point, float percentWidth, float percentHeight)
	{
		var screenPoint = Camera.WorldToViewportPoint(point) ?? Vector3.positiveInfinity;
		var vectorFrames = new Vector3(screenPoint.x, screenPoint.y);
		Rect rect = GetRect(percentWidth, percentHeight);

		return rect.Contains(vectorFrames);
	}
}