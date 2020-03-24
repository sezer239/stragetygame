using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Npc nin göresel efetleri
/// </summary>
public class NPCVisual : MonoBehaviour
{
    private NPCData npcData;
    private LineRenderer lineRenderer;

    System.Random random = new System.Random();

    public TextBubble speechBubble;

    void Start()
    {

        // speechBubble = GetComponentInChildren<TextBubble>();
        npcData = GetComponent<NPCData>();

        npcData.selectedChanged += NpcData_selectedChanged;

        npcData.npcLogic.onJobFailed += NpcLogic_onJobFailed;
        npcData.npcLogic.onJobCancelled += NpcLogic_onJobCancelled;
        npcData.npcLogic.onJobStarted += NpcLogic_onJobStarted;

        npcData.npcLogic.onJobCompleted += NpcLogic_onJobCompleted;
    }



    private void NpcLogic_onJobStarted(Job job)
    {
        if (random.NextDouble() < 0.8)
            return;
        if (job.jobType == Job.JobType.GIVEORDER)
        {
            var otherJob = job.extra1 as Job;
            speechBubble.ShowText("Hey Idiot Do This");
        }
        else if (job.jobType == Job.JobType.WOOD_CUTTING)
        {
            speechBubble.ShowText("Do Wood Cutting Yourself Idiot");
        }
        else if (job.jobType == Job.JobType.MOVE)
        {
            speechBubble.ShowText("MOVING");
        }
        else if (job.jobType == Job.JobType.HALT)
        {
            speechBubble.ShowText("YOU MOTHER FUCKER");
        }


    }

    private void NpcLogic_onJobFailed(Job job)
    {


    }

    private void NpcLogic_onJobCompleted(Job job)
    {


    }

    private void NpcLogic_onJobCancelled(Job job)
    {


    }

    private void NpcData_waypointAdded(Vector3 newWaypoint)
    {

    }

    private void NpcData_waypointChanged(Vector3 newWaypoint)
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (npcData.npcLogic == null) return;

        if (npcData.isSelected)
            UpdateLineRenderPath();
    }

    private void NpcData_selectedChanged(Selectable self, bool isSelected)
    {
        if (isSelected)
        {
            if (lineRenderer == null)
            {
                var go = new GameObject();
                go.name = this.GetHashCode() + "_LineRenderer";
                lineRenderer = go.AddComponent<LineRenderer>();
                lineRenderer.material = new Material(Shader.Find("Unlit/Texture"));
                lineRenderer.material.color = Color.yellow;
                lineRenderer.startWidth = 0.2f;
                lineRenderer.endWidth = 0.2f;
                lineRenderer.transform.rotation = Quaternion.LookRotation(Vector3.up);
            }
            lineRenderer.enabled = true;
        }
        else
        {
            if (lineRenderer != null)
                lineRenderer.enabled = false;
        }
    }

    private void UpdateLineRenderPath()
    {
        Vector3 origin = transform.position;
        var waypoints = npcData.jobQueue.GetAllMoveJobsAsVector3Array();
        int index = 0;
        lineRenderer.positionCount = waypoints.Length + 1;
        lineRenderer.SetPosition(index++, origin);
        for (int i = 0; i < waypoints.Length; i++)
        {
            var copy = waypoints[i];
            copy.y = 1;
            lineRenderer.SetPosition(i + 1, copy);
        }
    }
}
