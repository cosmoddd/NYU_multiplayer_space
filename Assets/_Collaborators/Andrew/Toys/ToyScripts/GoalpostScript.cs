using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoalpostScript : MonoBehaviour
{
    AudioSource AS;

    public Material on;
    public Material off;

    public MeshRenderer[] lights;
    public MeshRenderer[] points;

    int Score;

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < points.Length; i++)
        {
            points[i].enabled = i < Score;
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void ScoredGoal()
    {
        Score++;
        if (Score>3)
        {
            Score = 0;
        }
        for (int i = 0; i < points.Length; i++)
        {
            points[i].enabled = i < Score;
        }
    }
}
