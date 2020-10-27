using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class GoalpostScript : NetworkBehaviour
{
    AudioSource AS;

    public Material on;
    public Material off;

    public MeshRenderer[] lights;
    public MeshRenderer[] points;

    public GoalpostScript otherGoal;
    public AudioSource scoreGoal;

    int Score;

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < points.Length; i++)
        {
            points[i].enabled = i < Score;
        }
    }

    [Command(ignoreAuthority = true)]
    public void CmdScoredGoal() => RpcScoredGoal();

    [ClientRpc]
    void RpcScoredGoal()
    {
        // scoreGoal.pitch = UnityEngine.Random.Range(.75f, 1.25f);
        scoreGoal.Play();
        StartCoroutine(GoalLights());
        Score++;
        if (Score>3)
        {
            Score = 0;
            Reset();
            otherGoal.Reset();
        }
        for (int i = 0; i < points.Length; i++)
        {
            points[i].enabled = i < Score;
        }

    }

    void Reset()
    {
        for (int i = 0; i < points.Length; i++)
        {
            points[i].enabled = false;
        }
    }


    IEnumerator GoalLights()
    {
        for (int i =0; i < lights.Length; i++)
        {
            lights[i].material = on;
        }
    
        yield return new WaitForSeconds(1.2f);

        for (int i =0; i < lights.Length; i++)
        {
            lights[i].material = off;
        }
    }
}
