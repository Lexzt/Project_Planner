using UnityEngine;
using System;
using System.Collections;

public enum StickState
{
	ENABLED = 1,
	DISABLED,
	LAST
}

public static class StaticVars{
	public static int MaxNoPerMountWeekDay = 8;
	public static int MaxNoPerMountWeekEnd = 10;
	public static int MaxNoOfDutyInARow = 2;
	public static int StickInHours = 3;
	public static int RestAfterSticks = 2;
	public static DateTime StartDate = new DateTime(2016,8,15,14,00,00);
	public static DateTime EndDate = new DateTime(2016,8,17,14,00,00);
	public static int StartHourOffset = 1;

	// GUI Stuff
	public static float StickPadding = 3f;
	public static float xPixelPadding = 20f;
	public static float yPixelPadding = 50f;
}
