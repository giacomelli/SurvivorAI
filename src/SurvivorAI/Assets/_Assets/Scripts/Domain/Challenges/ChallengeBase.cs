using System;
using GeneticSharp.Domain.Chromosomes;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Threading;
using GeneticSharp.Domain;

namespace SurvivorAI.Domain.Challenges
{
    public abstract class ChallengeBase<TChromosome> : IChallenge where TChromosome : ChallengeChromosomeBase
    {
        #region Fields

        private Dictionary<string, Survivor> m_survivorTimeToReachFlor;
        private static object s_lock = new object();

        #endregion

        #region Constructors

        protected ChallengeBase(string name)
        {
            Name = name;
            m_survivorTimeToReachFlor = new Dictionary<string, Survivor>();
            TimeToReachFloorFallInDeadZone = 30;
        }

        #endregion

        #region Properties

        public string Name { get; private set; }

        public bool Enabled { get; set; }

        public bool SupportsParallel { get { return true; } }

        public float TimeToReachFloorFallInDeadZone { get; protected set; }

        #endregion

        #region Methods
        public virtual void ConfigGA(GeneticAlgorithm ga)
        {
        }

        public double Evaluate(IChromosome chromosome)
        {
            if (Enabled)
            {
                return EvaluteChromosome(chromosome);
            }
			
            return 0;
        }

        public virtual void RegisterSurvivorReachFloorTime(string id, Vector3 diePosition)
        {
            lock (s_lock)
            {
                if (m_survivorTimeToReachFlor.ContainsKey(id))
                {
                    m_survivorTimeToReachFlor[id].EndRelease(diePosition);	
                }
            }
        }

        public virtual void RegisterSurvivorFreeze(string id, Vector3 diePosition)
        {
            lock (s_lock)
            {
                if (m_survivorTimeToReachFlor.ContainsKey(id))
                {
                    m_survivorTimeToReachFlor[id].Freeze(diePosition, TimeToReachFloorFallInDeadZone);
                }
            }
        }

        public virtual void RegisterSurvivorFallInDeadZone(string id, Vector3 diePosition)
        {
            lock (s_lock)
            {
                if (m_survivorTimeToReachFlor.ContainsKey(id))
                {
                    m_survivorTimeToReachFlor[id].FallInDeadZone(diePosition, TimeToReachFloorFallInDeadZone);
                }
            }
        }

        public Survivor GetSurvivorWaitingForRelease()
        {
            lock (s_lock)
            {
                var survivor = m_survivorTimeToReachFlor.FirstOrDefault(s => s.Value.State == SurvivorState.WaitingForReleaseBegin);

                if (survivor.Key == null)
                {
                    return null;
                }

                m_survivorTimeToReachFlor[survivor.Key].BeginRelease();

                return survivor.Value;
            }
        }

        protected double EvaluteChromosome(IChromosome chromosome)
        {	
            try
            {
                var c = chromosome as TChromosome;

                var survivor = new Survivor(c);

                lock (s_lock)
                {
                    m_survivorTimeToReachFlor[c.Id] = survivor;
                }

                survivor.WaitForRelease();

                do
                {
                    Thread.Sleep(500);

                } while(survivor.State == SurvivorState.WaitingForReleaseBegin || survivor.State == SurvivorState.WaitingForReleaseEnd);


                return CalculateFitness(c);
            }
            catch (Exception ex)
            {
                Debug.LogError(ex.Message);
                throw;
            }
        }

        protected abstract double CalculateFitness(TChromosome chromosome);
        #endregion
    }
}