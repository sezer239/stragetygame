using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Data structure for holding the data
/// </summary>
public class JobQueue
{

    public delegate void OnJobAdded(Job job, int index);

    public event OnJobAdded onJobAdded;

    public List<Job> jobs {
        get {
            if (_jobs == null)
                _jobs = new List<Job>();
            return _jobs;
        }
    }
    private List<Job> _jobs;

    public bool HasPickUpJob()
    {
        foreach (var job in jobs)
        {
            if (job.jobType == Job.JobType.ITEM_PICKUP)
                return true;
        }

        return false;
    }

    public Job GetTransferJob()
    {
        Job result = null;
        foreach (var job in jobs)
        {
            if (job.jobType == Job.JobType.ITEM_TRANSFER)
            {
                result = job;
                break;
            }
        }

        return result;
    }

    public Vector3[] GetAllMoveJobsAsVector3Array()
    {
        List<Vector3> moveJobs = new List<Vector3>();
        foreach (var job in jobs)
        {
            if (job.jobType == Job.JobType.MOVE || job.jobType == Job.JobType.GOTO ||job.jobType == Job.JobType.ITEM_TRANSFER)
            {
                Vector3 pos = Vector3.zero;

                if (job.jobType == Job.JobType.MOVE)
                {
                    pos = job.position;
                }
                else
                {
                    pos = ((NPCLogic)job.extra0).transform.position;
                }

                moveJobs.Add(pos);
            }
        }
        return moveJobs.ToArray();
    }

    public Job[] GetAllMoveJobs()
    {
        List<Job> moveJobs = new List<Job>();
        foreach (var job in jobs)
        {
            if (job.jobType == Job.JobType.MOVE)
            {
                moveJobs.Add(job);
            }
        }
        return moveJobs.ToArray();
    }

    public void AddMoveJob(Vector3 location)
    {
        Job move = new Job(Job.JobType.MOVE);
        move.position = location;
        AddJob(move);
    }

    public void AddJob(Job job)
    {
        jobs.Add(job);
        if (onJobAdded != null)
            onJobAdded.Invoke(job, jobs.Count - 1);
    }

    public void SetSingleMoveJob(Vector3 location)
    {
        jobs.Clear();
        AddMoveJob(location);
    }


    public void SetSingleJob(Job job)
    {
        jobs.Clear();
        AddJob(job);
    }
    public void AddPriorityMoveJob(Vector3 location)
    {
        Job move = new Job(Job.JobType.MOVE);
        move.position = location;
        AddPriorityJob(move);
    }

    public void AddPriorityJob(Job job)
    {
        jobs.Insert(0, job);
        if (onJobAdded != null)
            onJobAdded.Invoke(job, 0);
    }
}
