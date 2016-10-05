using UnityEngine;
using System.Collections;
using SurvivorAI.Domain.Challenges.Coast;
using System.Linq;
using System.Threading;
using SurvivorAI.Domain.Challenges;
using HelperSharp;
using SurvivorAI.Domain.Challenges.Bridge;

public abstract class ChallengeControllerBase : MonoBehaviour
{
    #region Fields

    private object m_lock = new object();
    private ChallengeGeneticAlgorithm m_ga;
    private IChallenge m_challenge;
    private ChallengeChromosomeBase m_chromosome;
		
    private int m_releasesCount;
    private int m_killsCount;
    private Object m_survivorLanePrefab;
    private Vector3 m_firstLanePosition = new Vector3(0, 20, -20);
    protected Vector3 LastLanePositiion = Vector3.zero;
    public Vector3 m_lanesDistance = new Vector3(0, 0, 50);
    private System.DateTime m_currentGenerationStartTime;

    #endregion

    #region Constructors

    public ChallengeControllerBase(IChallenge challenge, ChallengeChromosomeBase chromosome)
    {
        m_challenge = challenge;
        m_chromosome = chromosome;

    }

    #endregion

    #region Properties

    public static ChallengeControllerBase Current { get; private set; }

    public float ReleaseTimeout = 10;
    public float ScreenshotInterval = 15;
    public Vector3 DeployPositionModifier = new Vector3(-35, 55, 0);
    public int PopulationSize = 40;

    protected Object SurvivorPrefab;

    #endregion

    private void Start()
    {
        Current = this;
        m_ga = new ChallengeGeneticAlgorithm(m_challenge, m_chromosome);

        var prefabName = m_challenge.Name + "SurvivorLanePrefab";
        m_survivorLanePrefab = Resources.Load(prefabName);

        if (m_survivorLanePrefab == null)
        {
            Debug.LogErrorFormat("Prefab '{0}' not found", prefabName);
        }   

        SurvivorPrefab = Resources.Load(m_challenge.Name + "SurvivorPrefab");
        LoadPrefabs();
	
        m_ga.PopulationSize = PopulationSize;	
        StartCoroutine(CheckReleaseSurvivor());
        StartCoroutine(TakeScreenshot());
    }

    private IEnumerator TakeScreenshot()
    {
        while (true)
        {
            yield return new WaitForSeconds(ScreenshotInterval);
            Application.CaptureScreenshot("{0:yyyyMMddHHmmss}_generation_{1}.png".With(System.DateTime.Now, m_ga.GA.GenerationsNumber));
        }
    }

    private void OnGUI()
    {
        GUILayout.BeginVertical(GUILayout.MinWidth(300));
		
        if (m_ga.Initialized)
        {
            GUILayout.Label("Challenge: " + m_challenge.Name);
            GUILayout.Label("Population size: " + (m_ga.PopulationSize));
            GUILayout.Label("Current generation: " + (m_ga.Population.GenerationsNumber));
            GUILayout.Label("Generation time: {0} secs".With((System.DateTime.Now - m_currentGenerationStartTime).Seconds));
            GUILayout.Label("Evaluations (end/deploy): {0}/{1}".With(m_killsCount, m_releasesCount));
            GUILayout.Label("Best chromosomes: ");
		
            var generations = m_ga.Population.Generations
				.Where(g => g.BestChromosome != null)
				.OrderByDescending(g => g.BestChromosome.Fitness)
				.ThenByDescending(e => e.Number)
				.Take(10);
			
            foreach (var g in generations)
            {
                var c = (ChallengeChromosomeBase)g.BestChromosome;
				
                if (!c.Fitness.HasValue)
                {
                    Debug.LogError("no fitness: generation " + g.Number);
                }
				
                var msg = string.Format("Generation {0}: {1:n5} secs / {2:n5} x", g.Number, c.TimeToReachFloor, c.DiePosition.x);
                GUILayout.Label(msg);
            }
        }
        else
        {
			
            GUILayout.Label("Population size: " + m_ga.PopulationSize);
            m_ga.PopulationSize = System.Convert.ToInt32(GUILayout.HorizontalSlider(m_ga.PopulationSize, 2, 1000));
			
            if (GUILayout.Button("Run"))
            {
                var t = new Thread(StartChallenge);
                t.Start();
            }
        }
		
        GUILayout.EndVertical();
    }

    private void StartChallenge()
    {
        Debug.Log("Starting challenge...");
        m_ga.Initialize();
        LastLanePositiion = m_firstLanePosition;
        m_ga.GA.GenerationRan += delegate
        {
            Debug.Log("Generation ran.");
            m_currentGenerationStartTime = System.DateTime.Now;
            LastLanePositiion = m_firstLanePosition;
        };
	
        m_currentGenerationStartTime = System.DateTime.Now;

        Debug.Log("Running first generation..\t.");
        m_ga.RunGeneration();
        Debug.Log("Challenge started.");
    }

    private IEnumerator CheckReleaseSurvivor()
    {
        while (true)
        {
            yield return new WaitForEndOfFrame();
			
            var survivor = m_challenge.GetSurvivorWaitingForRelease();
	       
            if (survivor != null)
            {
                ReleaseSurvivor(survivor);
            }	
        }
    }

    protected abstract void LoadPrefabs();

    protected abstract void BuildSurvivor(Survivor survivor, GameObject survivorGO, GameObject laneGO);

    private void ReleaseSurvivor(Survivor survivor)
    {
        Debug.LogFormat("Deploying survivor {0}...", survivor.Chromosome.Id);
        var laneGO = (GameObject)Instantiate(m_survivorLanePrefab);
        laneGO.name = survivor.Chromosome.Id + "_lane";
        LastLanePositiion += m_lanesDistance;
        laneGO.transform.position = LastLanePositiion;
      	
        var survivorGO = (GameObject)Instantiate(SurvivorPrefab);
        survivorGO.name = survivor.Chromosome.Id;
        survivorGO.SetActive(false);
        survivorGO.transform.position = LastLanePositiion + DeployPositionModifier;

        BuildSurvivor(survivor, survivorGO, laneGO);
        survivorGO.SetActive(true);
			
        lock (m_lock)
        {
            m_releasesCount++;	
        }
    }

    public void RegisterSurvivorReachFloor(string survivorId)
    {
        var diePosition = KillSurvivor(survivorId);	
        m_challenge.RegisterSurvivorReachFloorTime(survivorId, diePosition);
    }

    public void RegisterSurvivorStop(string survivorId)
    {
        var diePosition = KillSurvivor(survivorId);
        m_challenge.RegisterSurvivorFreeze(survivorId, diePosition);
    }

    public void RegisterSurvivorFallInDeadZone(string survivorId)
    {
        var diePosition = KillSurvivor(survivorId);
        m_challenge.RegisterSurvivorFallInDeadZone(survivorId, diePosition);
    }

    private Vector3 KillSurvivor(string survivorId)
    {
        var survivor = GameObject.Find(survivorId);
        survivor.name += "_DEAD";
        var diePosition = survivor.transform.position;

        foreach (Transform c in survivor.transform)
        {
            Destroy(c.gameObject);
        }

        Destroy(survivor);
        Destroy(GameObject.Find(survivorId + "_lane"));

        lock (m_lock)
        {
            m_killsCount++;	
        }

        return diePosition;
    }

    private void OnApplicationQuit()
    {
        if (m_ga != null && m_ga.Population != null)
        {
            m_ga.AbortGeneration();
        }
    }
}
