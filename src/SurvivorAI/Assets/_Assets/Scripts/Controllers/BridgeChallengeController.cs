using System;
using SurvivorAI.Domain.Challenges;
using UnityEngine;
using SurvivorAI.Domain.Challenges.Coast;
using SurvivorAI.Domain.Challenges.Bridge;

public class BridgeChallengeController : ChallengeControllerBase
{
    private UnityEngine.Object m_survivorPiecePrefab;

    public BridgeChallengeController()
        : base(new BridgeChallenge(), new BridgeChromosome())
    {


    }

    protected override void LoadPrefabs()
    {
        m_survivorPiecePrefab = Resources.Load("BridgeSurvivorBridgePiecePrefab");
    }

    protected override void BuildSurvivor(Survivor survivor, GameObject survivorGO, GameObject laneGO)
    {
        var c = survivor.Chromosome as BridgeChromosome;
        var partsEnabled = c.PartsEnabled;
        var laneGOPosition = laneGO.transform.position;
        Rigidbody lastPieceRb = null;

        for (int i = 0; i < partsEnabled.Length; i++)
        {
            if (partsEnabled[i])
            {
                var piece = (GameObject)Instantiate(
                            m_survivorPiecePrefab, 
                            Vector3.zero,
                            Quaternion.identity);
            
                piece.transform.position = new Vector3(i * piece.transform.localScale.x + 25, laneGOPosition.y - 0.5f, 0);
                piece.transform.SetParent(laneGO.transform, false);

                var pieceRb = piece.GetComponent<Rigidbody>();

//                if (i > 0 && i < partsEnabled.Length - 1)
//                {
//                    pieceRb.useGravity = true;
//                    pieceRb.isKinematic = false;
//                    pieceRb.constraints = RigidbodyConstraints.FreezeRotationZ;    
//                }
//
//                if (lastPieceRb != null && i > 0 && i < partsEnabled.Length)
//                {
//                    var joint = piece.AddComponent<HingeJoint>();
//                    joint.breakForce = 10000;
//                    //joint.spring = 50;
//                    joint.connectedBody = lastPieceRb;
//                }
               
                lastPieceRb = pieceRb;
            }
        }

//        var survivor2GO = (GameObject)Instantiate(SurvivorPrefab);
//        survivor2GO.name = survivor.Chromosome.Id;
//        survivor2GO.SetActive(false);
//        survivor2GO.transform.position = LastLanePositiion + new Vector3(200, 0, 0);
//        survivor2GO.GetComponent<BridgeCarController>().InvertSpeed();
//        survivor2GO.SetActive(true);

        

//        var cubesCount = laneGO.transform.childCount;
//        var connectionIndexes = survivor.Chromosome.ConnectionIndexes;
//        var rb = laneGO.GetComponent<Rigidbody>();
//
//        for (var i = 0; i < cubesCount; i++)
//        {
//            var cube = laneGO.transform.GetChild(i);
//            Transform nextCube = null;
//            int nextCubeIndex = connectionIndexes[i];
//
//            if (nextCubeIndex != i)
//            {
//                nextCube = laneGO.transform.GetChild(nextCubeIndex);
//            }
//            else
//            {
//                nextCube = laneGO.transform;	
//            }
//
//            var joints = cube.GetComponents<Joint>();
//            joints[0].connectedBody = rb;
//            joints[1].connectedBody = nextCube.GetComponent<Rigidbody>();
//        }
//
//        laneGO.GetComponent<Joint>().connectedBody = laneGO.transform.GetChild(0).GetComponent<Rigidbody>();

    }
}


