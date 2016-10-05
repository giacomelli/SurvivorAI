using System;
using UnityEngine;

namespace SurvivorAI.Domain.Challenges
{
    public enum SurvivorState
    {
        NotEvaluated,
        WaitingForReleaseBegin,
        WaitingForReleaseEnd,
        ReleaseEnded
    }

    public class Survivor
    {
        private DateTime m_releaseBeginTime;

        public Survivor(ChallengeChromosomeBase chromosome)
        {
            Chromosome = chromosome;
            State = SurvivorState.NotEvaluated;	
        }

        public ChallengeChromosomeBase Chromosome  { get; private set; }

        public SurvivorState State { get; set; }

        public void WaitForRelease()
        {
            State = SurvivorState.WaitingForReleaseBegin;	
        }

        public void BeginRelease()
        {
            m_releaseBeginTime = DateTime.Now;
            State = SurvivorState.WaitingForReleaseEnd;
        }

        public double CalculateSinceReleaseBegin()
        {
            return (DateTime.Now - m_releaseBeginTime).TotalSeconds;
        }

        public void EndRelease(Vector3 diePosition)
        {
            if (State != SurvivorState.ReleaseEnded)
            {
                Chromosome.TimeToReachFloor = CalculateSinceReleaseBegin();
                Chromosome.DiePosition = diePosition;
                State = SurvivorState.ReleaseEnded;
            }
        }

        public void Freeze(Vector3 diePosition, float timeToReachFloor)
        {
            Chromosome.TimeToReachFloor = timeToReachFloor;
            Chromosome.DiePosition = diePosition;
            State = SurvivorState.ReleaseEnded;
        }

        public void FallInDeadZone(Vector3 diePosition, float timeToReachFloor)
        {
            Chromosome.TimeToReachFloor = timeToReachFloor;
            Chromosome.DiePosition = diePosition;
            State = SurvivorState.ReleaseEnded;
        }
    }
}

