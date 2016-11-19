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
	public static int MaxNoPerMountWeekDay = 6;
	public static int MaxNoPerMountWeekEnd = 10;
	public static int MaxNoOfDutyInARow = 2;
	public static int StickInHours = 3;
	public static int RestAfterSticks = 2;
	public static DateTime StartDate = new DateTime(2016,8,15,14,00,00);
	public static DateTime EndDate = new DateTime(2016,8,17,14,00,00);

	public static bool StartEndStickOffset = true;
	public static int StartHourOffset = 1;
	public static bool DoWithSameBatch = false;

	// ICT Variables
	public static int NumberOfICT = 4;
	public static int NumberOfStickforICTWeekDay = 4;
	public static int NumberOfStickforICTWeekEnd = 6;

	// GUI Stuff
	public static float StickPadding = 3f;
	public static float xPixelPadding = 20f;
	public static float yPixelPadding = 50f;

	public static Roles RolesParseJson(string Json)
	{
		switch (Json)
		{
		case "Checker":
			return Roles.eCHECKER;
			break;
		case "Sentry":
			return Roles.eSENTRY;
			break;
		case "Giro":
			return Roles.ePASS_OFFICE;
			break;
		case "Console":
			return Roles.eCONSOLE;
			break;
		case "Driver":
			return Roles.eDRIVER;
			break;
		case "Armorer":
			return Roles.eARMOURER;
		default:
			return Roles.eNONE;
			break;
		}
	}

	public static string RolesParseJson(Roles role)
	{
		switch (role)
		{
		case Roles.eCHECKER:
			return "Checker";
			break;
		case Roles.eSENTRY:
			return "Sentry";
			break;
		case Roles.ePASS_OFFICE:
			return "Giro";
			break;
		case Roles.eCONSOLE:
			return "Console";
			break;
		case Roles.eDRIVER:
			return "Driver";
			break;
		case Roles.eARMOURER:
			return "Armorer";
		default:
			return "None";
			break;
		}
	}
}
