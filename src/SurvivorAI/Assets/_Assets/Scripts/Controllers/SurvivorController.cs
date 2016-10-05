using UnityEngine;
using System.Collections;

public class SurvivorController : MonoBehaviour
{
	void Start()
	{
		StartCoroutine (KillByTimeout ());
	}

	IEnumerator KillByTimeout ()
	{
		yield return new WaitForSeconds (ChallengeControllerBase.Current.ReleaseTimeout);
		ChallengeControllerBase.Current.RegisterSurvivorStop (name);
	}
}
