using System;
using GeneticSharp.Domain.Fitnesses;
using UnityEngine;
using GeneticSharp.Domain;

namespace SurvivorAI.Domain.Challenges
{
	public interface IChallenge : IFitness
	{
		#region Properties
		string Name { get; }
		bool Enabled { get; set; }
		#endregion

		#region Methods
        void ConfigGA(GeneticAlgorithm ga);

		void RegisterSurvivorReachFloorTime (string id, Vector3 diePosition);

		void RegisterSurvivorFreeze (string id, Vector3 diePosition);

		void RegisterSurvivorFallInDeadZone (string id, Vector3 diePosition);

		Survivor GetSurvivorWaitingForRelease ();
		#endregion
	}
}