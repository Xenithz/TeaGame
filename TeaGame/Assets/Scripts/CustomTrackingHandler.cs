using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vuforia;

public class CustomTrackingHandler : DefaultTrackableEventHandler 
{
	protected override void	 OnTrackingFound()
	{
		base.OnTrackingFound();
		GameController.instance.isTracking = true;
	}

	protected override void OnTrackingLost()
	{
		base.OnTrackingLost();
		GameController.instance.isTracking = false;
	}
}
