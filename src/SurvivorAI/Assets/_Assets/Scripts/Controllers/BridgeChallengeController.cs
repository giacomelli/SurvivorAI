using SurvivorAI.Domain.Challenges;
using UnityEngine;
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
               
                lastPieceRb = pieceRb;
            }
        }
    }
}