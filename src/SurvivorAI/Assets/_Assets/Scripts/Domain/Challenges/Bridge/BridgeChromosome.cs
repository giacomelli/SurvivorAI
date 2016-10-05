using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Domain.Randomizations;

namespace SurvivorAI.Domain.Challenges.Bridge
{
    public sealed class BridgeChromosome : ChallengeChromosomeBase
    {
        #region Consts
        public const int TotalPieces = 40;
        public const int BridgeLength = 200;
      
        public const int BridgeMinX = 0;
        public const int BridgeMaxX = BridgeLength;
        #endregion

        #region Fields
        private IRandomization m_random = RandomizationProvider.Current;
        #endregion

        public BridgeChromosome()
            : base(TotalPieces)
        {
            Id = Guid.NewGuid().ToString();
            PartsEnabled = new bool[Length];
                
            for (int i = 0; i < Length; i++)
            {
                ReplaceGene(i, GenerateGene(i));
            }
        }

        public override IChromosome CreateNew()
        {
            return new BridgeChromosome();
        }

        public bool[] PartsEnabled { get; set; }
        public float PartsEnabledCount { get; set; }

        #region implemented abstract members of GeneticSharp.Domain.Chromosomes.ChromosomeBase

        public override Gene GenerateGene(int geneIndex)
        {
            var value = m_random.GetFloat() >= 0.5f;
            PartsEnabled[geneIndex] = value;

            if (value)
            {
                PartsEnabledCount++;
            }

            return new Gene(value);
        }

        public override IChromosome Clone()
        {
            var clone = base.Clone() as BridgeChromosome;
            clone.TimeToReachFloor = TimeToReachFloor;
            Array.Copy(PartsEnabled, clone.PartsEnabled, PartsEnabled.Length);
            clone.PartsEnabledCount = PartsEnabledCount;
            return clone;
        }
        #endregion
    }
}

