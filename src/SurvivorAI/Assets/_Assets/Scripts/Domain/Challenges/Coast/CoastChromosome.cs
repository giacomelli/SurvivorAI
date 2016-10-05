using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using GeneticSharp.Domain.Chromosomes;

namespace SurvivorAI.Domain.Challenges.Coast
{
    public class CoastChromosome : ChallengeChromosomeBase
    {
        #region Fields

        private System.Random m_random = new System.Random(DateTime.Now.Millisecond);

        #endregion

        public CoastChromosome()
            : base(TotalPieces * 2)
        {
            Id = Guid.NewGuid().ToString();
            for (int i = 0; i < Length; i++)
            {
                ReplaceGene(i, GenerateGene(i));
            }
        }

        public override IChromosome CreateNew()
        {
            return new CoastChromosome();
        }

        public IList<Vector3> Positions
        {
            get
            {
                return GetGenes().Take(TotalPieces).Select(g => (Vector3)g.Value).ToList();
            }
        }

        public IList<int> ConnectionIndexes
        {
            get
            {
                return GetGenes().Skip(TotalPieces).Select(g => (int)g.Value).ToList();
            }
        }

        #region implemented abstract members of GeneticSharp.Domain.Chromosomes.ChromosomeBase

        public override Gene GenerateGene(int geneIndex)
        {
            if (geneIndex < TotalPieces)
            {
                return new Gene(
                    new Vector3(
                        m_random.Next(-20, 20),
                        m_random.Next(-20, 20),
                        m_random.Next(-20, 20)));
            }
            else
            {
                return new Gene(m_random.Next(0, TotalPieces));
            }
        }

        public override IChromosome Clone()
        {
            var clone = base.Clone() as CoastChromosome;
            clone.TimeToReachFloor = TimeToReachFloor;
			
            return clone;
        }

        #endregion
    }
}

