using System;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using GeneticSharp.Domain.Chromosomes;
using System.Linq;
using GeneticSharp.Domain.Crossovers;
using GeneticSharp.Domain.Selections;
using GeneticSharp.Domain.Mutations;
using GeneticSharp.Domain.Reinsertions;
using GeneticSharp.Domain.Terminations;

namespace SurvivorAI.Domain.Challenges.Bridge
{
    public class BridgeChallenge : ChallengeBase<BridgeChromosome>
    {
        #region Constructors
        public BridgeChallenge()
            : base("Bridge")
        {
            TimeToReachFloorFallInDeadZone = -60;
        }
        #endregion

        #region Methods
        public override void ConfigGA(GeneticSharp.Domain.GeneticAlgorithm ga)
        {
            ga.Crossover = new UniformCrossover();
            ga.Mutation = new UniformMutation();
            ga.Selection = new RouletteWheelSelection();
            ga.Termination = new FitnessThresholdTermination(250);
        }

        public override void RegisterSurvivorFreeze(string id, Vector3 diePosition)
        {
            base.RegisterSurvivorReachFloorTime(id, diePosition);
        }

        protected override double CalculateFitness(BridgeChromosome chromosome)
        {
            return chromosome.DiePosition.x;
        }
        #endregion
    }
}