using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{

	public AmoebaManager protectedAmoeba;
	public DamageBar damageBar;

	public float maxLashoutLength = 5f;

	public float lashoutDuration { get; private set; }

	// Update is called once per frame
	void Update ()
	{
		if (protectedAmoeba.isLashingOut) {
			lashoutDuration += Time.deltaTime;

			if (lashoutDuration > maxLashoutLength) {
				TriggerGameOut ();
			} else {
				damageBar.percent = 1f - lashoutDuration / maxLashoutLength;
			}
		}
	}

	void TriggerGameOut ()
	{
		// TODO implement effects outside of this class
		damageBar.percent = 0;
	}
}
