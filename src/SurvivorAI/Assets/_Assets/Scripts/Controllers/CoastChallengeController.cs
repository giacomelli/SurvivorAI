using System;
using SurvivorAI.Domain.Challenges;
using UnityEngine;
using SurvivorAI.Domain.Challenges.Coast;

public class CoastChallengeController : ChallengeControllerBase
{
    private UnityEngine.Object m_survivorCubePrefab;

    public CoastChallengeController()
        : base(new CoastChallenge(), new CoastChromosome())
    {


    }

    protected override void LoadPrefabs()
    {
        m_survivorCubePrefab = Resources.Load("CoastSurvivorCubePrefab");
    }

    protected override void BuildSurvivor(Survivor survivor, GameObject survivorGO, GameObject laneGO)
    {
        var c = survivor.Chromosome as CoastChromosome;
        var positions = c.Positions;

        foreach (var pos in positions)
        {
            var cube = (GameObject)Instantiate(m_survivorCubePrefab, pos, Quaternion.identity);
            cube.transform.SetParent(survivorGO.transform, false);
        }

        var cubesCount = survivorGO.transform.childCount;
        var connectionIndexes = c.ConnectionIndexes;
        var rb = survivorGO.GetComponent<Rigidbody>();

        for (var i = 0; i < cubesCount; i++)
        {
            var cube = survivorGO.transform.GetChild(i);
            Transform nextCube = null;
            int nextCubeIndex = connectionIndexes[i];

            if (nextCubeIndex != i)
            {
                nextCube = survivorGO.transform.GetChild(nextCubeIndex);
            }
            else
            {
                nextCube = survivorGO.transform;	
            }

            var joints = cube.GetComponents<Joint>();
            joints[0].connectedBody = rb;
            joints[1].connectedBody = nextCube.GetComponent<Rigidbody>();
        }

        survivorGO.GetComponent<Joint>().connectedBody = survivorGO.transform.GetChild(0).GetComponent<Rigidbody>();

    }
}


