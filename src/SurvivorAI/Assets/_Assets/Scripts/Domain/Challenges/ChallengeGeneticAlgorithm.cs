using System;
using GeneticSharp.Domain;
using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Domain.Crossovers;
using GeneticSharp.Domain.Mutations;
using GeneticSharp.Domain.Populations;
using GeneticSharp.Domain.Reinsertions;
using GeneticSharp.Domain.Selections;
using GeneticSharp.Domain.Terminations;
using GeneticSharp.Infrastructure.Threading;
using SurvivorAI.Domain.Challenges;

namespace SurvivorAI.Domain.Challenges
{
    public class ChallengeGeneticAlgorithm
    {
        #region Fields

        private IChallenge m_challenge;
        private IChromosome m_ancestorChromosome;
        public GeneticAlgorithm GA;

        #endregion

        #region Constructors

        public ChallengeGeneticAlgorithm(IChallenge challenge, IChromosome ancestorChromosome)
        {	
            m_challenge = challenge;
            m_ancestorChromosome = ancestorChromosome;
            PopulationSize = 20;
            Selection = new EliteSelection();
				
        }

        #endregion

        #region Properties

        public Population Population { get; private set; }

        public bool Initialized { get; private set; }

        public int PopulationSize { get; set; }

        public ISelection Selection { get; private set; }

        #endregion

        #region Methods

        public void Initialize()
        {		
            Population = new Population(
                PopulationSize,
                PopulationSize * 2,
                m_ancestorChromosome);
			
            Population.GenerationStrategy = new PerformanceGenerationStrategy(10);
			
            GA = new GeneticAlgorithm(Population, 
                m_challenge,
                new EliteSelection(),
                new UniformCrossover(0.5f),
                new UniformMutation(true));	
		
            GA.Termination = new FitnessStagnationTermination(1000); 
            GA.MutationProbability = 0.4f;
            GA.Reinsertion = new ElitistReinsertion();

            var taskExecutor = new SmartThreadPoolTaskExecutor();
            taskExecutor.MinThreads = 40;
            taskExecutor.MaxThreads = 100;
            GA.TaskExecutor = taskExecutor;
            m_challenge.ConfigGA(GA);
                     
            Initialized = true;
        }

        public void RunGeneration()
        {
            if (Initialized)
            {
                m_challenge.Enabled = true;

                GA.Start();
				
                m_challenge.Enabled = false;
            }
            else
            {
                throw new InvalidOperationException("Call Initialize before RunGeneration.");
            }
        }

        public void AbortGeneration()
        {
            GA.Stop();
        }

        #endregion
    }
}

