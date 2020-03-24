using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Job
{
    public delegate void JobTypeChanged(JobType oldJob, JobType newJob);
    public delegate void JobPositionChanged(Vector3 oldPosition, Vector3 newPosition);
    public delegate void JobExtraChanged(object oldExtra, object newExtra);
    public delegate void JobStatusChanged(JobStatus jobStatus);

    public event JobTypeChanged jobTypeChanged;
    public event JobPositionChanged jobPositionChanged;
    public event JobExtraChanged jobExtraChanged;
    public event JobStatusChanged jobStatusChanged;

    public enum JobType
    {
        HALT,
        MOVE,
        GOTO,
        ACTION,
        GIVEORDER,
        REPORT_BACK,
        WOOD_CUTTING,
        STONE_MINING,
        FARMING,
        HUNTING,
        ITEM_PICKUP,
        ITEM_DROP,
        ITEM_TRANSFER,
        EXAMINE,
        ITEM_RECEIVE
        /*More job type will be avalible*/
    }

    public enum JobStatus
    {
        WORKING,
        QUEUED,
        COMPLETED,
        FAILED,
        CANCELLED
    }

    public JobStatus jobStatus {
        get {
            return _jobStatus;
        }
        set {
            JobStatus old = _jobStatus;
            _jobStatus = value;
            if (jobStatusChanged != null)
                jobStatusChanged.Invoke(_jobStatus);
        }
    }
    private JobStatus _jobStatus;

    public JobType jobType {
        get {
            return _jobType;
        }
        set {
            JobType old = _jobType;
            _jobType = value;
            if (jobTypeChanged != null)
                jobTypeChanged.Invoke(old, _jobType);
        }
    }
    private JobType _jobType;

    public Vector3 position {
        get {
            return _position;
        }
        set {
            Vector3 old = _position;
            _position = value;
            if (jobPositionChanged != null)
                jobPositionChanged.Invoke(old, _position);
        }
    }
    private Vector3 _position;

    public object extra0 {
        get {
            return _extra0;
        }
        set {
            object old = _extra0;
            _extra0 = value;
            if (jobExtraChanged != null)
                jobExtraChanged.Invoke(old, _extra0);
        }
    }
    private object _extra0;

    public object extra1 {
        get {
            return _extra1;
        }
        set {
            object old = _extra1;
            _extra1 = value;
            if (jobExtraChanged != null)
                jobExtraChanged.Invoke(old, _extra1);
        }
    }
    private object _extra1;


    public Job(JobType jobType)
    {
        this.jobType = jobType;
        this.jobStatus = JobStatus.QUEUED;
    }
}
