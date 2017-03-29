using System;
using UnityEngine.EventSystems;

	public interface IMaxStressTarget : IEventSystemHandler
	{
		void MaxStressReached(AmoebaManager manager);
	}
