/// <summary>
/// This class handles quality (QA) events, such as crashes, fps, etc.
/// </summary>

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

#if UNITY_METRO && !UNITY_EDITOR
using GA_Compatibility.Collections;
#endif

public class GA_Error 
{
	public enum SeverityType { critical, error, warning, info, debug }
	
	#region public methods
	
	public void NewEvent(SeverityType severity, string message, Vector3 trackPosition)
	{
		CreateNewEvent(severity, message, trackPosition.x, trackPosition.y, trackPosition.z, false);
	}
	
	public void NewEvent(SeverityType severity, string message, float x, float y, float z)
	{
		CreateNewEvent(severity, message, x, y, z, false);
	}
	
	public void NewEvent(SeverityType severity, string message)
	{
		CreateNewEvent(severity, message, null, null, null, false);
	}
	
	public void NewErrorEvent(SeverityType severity, string message, float x, float y, float z)
	{
		CreateNewEvent(severity, message, x, y, z, true);
	}
	
	#endregion
	
	#region private methods
	
	/// <summary>
	/// Used for player QA events
	/// </summary>
	/// <param name="businessID">
	/// The event identifier. F.x. "FailedToLoadLevel" <see cref="System.String"/>
	/// </param>
	/// <param name="message">
	/// A string detailing the event, F.x. the stack trace from an exception <see cref="System.String"/>
	/// </param>
	/// <param name="stack">
	/// If true any identical messages in the queue will be merged/stacked as a single message, to save server load
	/// </param>
	private  void CreateNewEvent(SeverityType severity, string message, float? x, float? y, float? z, bool stack)
	{
		Hashtable parameters = new Hashtable()
		{
			{ GA_ServerFieldTypes.Fields[GA_ServerFieldTypes.FieldType.Severity], severity.ToString() },
			{ GA_ServerFieldTypes.Fields[GA_ServerFieldTypes.FieldType.Message], message },
			{ GA_ServerFieldTypes.Fields[GA_ServerFieldTypes.FieldType.Level], GA.SettingsGA.CustomArea.Equals(string.Empty)?Application.loadedLevelName:GA.SettingsGA.CustomArea }
		};
		
		if (x.HasValue)
		{
			parameters.Add(GA_ServerFieldTypes.Fields[GA_ServerFieldTypes.FieldType.X], (x*GA.SettingsGA.HeatmapGridSize.x).ToString());
		}
		
		if (y.HasValue)
		{
			parameters.Add(GA_ServerFieldTypes.Fields[GA_ServerFieldTypes.FieldType.Y], (y*GA.SettingsGA.HeatmapGridSize.y).ToString());
		}
		
		if (z.HasValue)
		{
			parameters.Add(GA_ServerFieldTypes.Fields[GA_ServerFieldTypes.FieldType.Z], (z*GA.SettingsGA.HeatmapGridSize.z).ToString());
		}
		
		GA_Queue.AddItem(parameters, GA_Submit.CategoryType.GA_Error, stack);
	}
	
	#endregion
}