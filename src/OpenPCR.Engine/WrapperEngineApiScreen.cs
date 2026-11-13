using global::OpenPCR.Runtime.Api;
using Elements;
public static class WrapperEngineApiScreen
{
	private const float ANDROID_SAFE_AREA_ASPECT_RATIO_THRESHOLD = 2.1666667f;
	[global::OpenPCR.Migration("b03cb755-b309-406e-a62b-8cbb3b2210c2")]
	public static Rect GetGameSafeArea()
	{
		Rect safe = Screen.safeArea;
		float width = safe.width;
		float x = safe.xMin;
		if (safe.width / safe.height > ANDROID_SAFE_AREA_ASPECT_RATIO_THRESHOLD)
		{
			width = safe.height * ANDROID_SAFE_AREA_ASPECT_RATIO_THRESHOLD;
			x = ((float)Screen.width - width) * 0.5f;
		}
		else if (safe.xMin + safe.width == (float)Screen.width)
		{
			width = safe.width - safe.xMin;
		}
		return new Rect(x, safe.yMin, width, safe.height);
	}
	public static Vector2 GetBaseNGUIResolution()
	{
		ViewManager manager = ManagerSingleton<ViewManager>.Instance;
		return manager != null ? new Vector2(manager.ActiveScreenWidth, manager.ActiveScreenHeight) : new Vector2((float)Screen.width, (float)Screen.height);
	}
	public static Rect GetNGUISafeArea()
	{
		Rect safe = GetGameSafeArea();
		Vector2 resolution = GetBaseNGUIResolution();
		return new Rect((safe.xMin / safe.width) * resolution.x, (safe.yMin / safe.height) * resolution.y, resolution.x, resolution.y);
	}
	public static Vector2 GetNGUIResolution()
	{
		Rect safe = GetNGUISafeArea();
		return new Vector2(safe.width + (safe.xMin + safe.xMin), safe.height + safe.yMin);
	}
	public static float GetNGUIOffsetY() => GetNGUISafeArea().yMin * 0.5f;
	public static float GetRestoreNGUIOffsetY() => -GetNGUIOffsetY();
	[global::OpenPCR.Migration("dc898346-a14f-46fa-b258-ab1b69ee68ea")]
	public static Vector3 GetRestoreNGUIOffsetYVector3() => new Vector3(0f, GetRestoreNGUIOffsetY(), 0f);
	[global::OpenPCR.Migration("8ea30af1-6081-40cd-97fd-e33847137b69")]
	public static bool IsLongerThanBaseWidth()
	{
		Vector2 resolution = GetNGUIResolution();
		return resolution.x / resolution.y >= 1.7777778f;
	}
}
