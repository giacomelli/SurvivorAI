using GeneticSharp.Domain.Chromosomes;

namespace SurvivorAI.Domain.Challenges.Coast
{
    public class CoastChallenge : ChallengeBase<CoastChromosome>
    {
        #region Constructors
        public CoastChallenge()
            : base("Coast")
        {
        }
        #endregion

        #region Methods
        protected override double CalculateFitness(CoastChromosome chromosome)
        {
            return (chromosome.TimeToReachFloor * -0.1f) + (chromosome.DiePosition.x);
        }
        #endregion
    }
}