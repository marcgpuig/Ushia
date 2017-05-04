using System.Collections;
using System;

/// <summary>
/// Helper class to make threading easier.
/// From: http://answers.unity3d.com/questions/357033/unity3d-and-c-coroutines-vs-threading.html
/// </summary>
public class UThreadWrapper
{
    private bool m_IsDone = false;
    private object m_Handle = new object();
    private System.Threading.Thread m_Thread = null;

    public DateTime startTime, endTime;

    public bool IsDone
    {
        get
        {
            bool tmp;
            lock (m_Handle)
            {
                tmp = m_IsDone;
            }
            return tmp;
        }
        set
        {
            lock (m_Handle)
            {
                m_IsDone = value;
            }
        }
    }

    public virtual void Start()
    {
        m_Thread = new System.Threading.Thread(Run);
        m_Thread.Start();
    }

    public virtual void Abort()
    {
        m_Thread.Abort();
    }

    protected virtual void ThreadFunction() { }

    protected virtual void OnFinished() { }

    public virtual bool Update()
    {
        if (IsDone)
        {
            OnFinished();
            return true;
        }
        return false;
    }

    public IEnumerator WaitFor()
    {
        while (!Update())
        {
            yield return null;
        }
    }

    private void Run()
    {
        startTime = DateTime.Now;
        ThreadFunction();
        endTime = DateTime.Now;
        IsDone = true;
    }
}
