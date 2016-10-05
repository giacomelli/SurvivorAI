using System;
using GeneticSharp.Domain.Chromosomes;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace SurvivorAI.Domain.Challenges
{
    public abstract class ChallengeChromosomeBase : ChromosomeBase
    {
        #region Consts

        public const int TotalPieces = 8;

        #endregion


        protected ChallengeChromosomeBase(int length)
            : base(length)
        {
        }

        public string Id { get; protected set; }


        public double TimeToReachFloor { get; set; }

        public Vector3 DiePosition { get; set; }

    }
}

