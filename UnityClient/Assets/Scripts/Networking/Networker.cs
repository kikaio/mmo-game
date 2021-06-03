using common.Jobs;
using common.Networking;
using common.Protocols;
using common.Sockets;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

//one connection 
public class Networker : CoreNetwork, IDisposable
{
    //ex : lobby connect, battle connect etc....
    public static Dictionary<string, Networker> nameToDic { get; private set; } = new Dictionary<string, Networker>();
    public static Networker CreateNetworker(string _name)
    {
        if (nameToDic.ContainsKey(_name))
        {
            //todo : logging
            return default(Networker);
        }
        var newWorker = new Networker();
        nameToDic[_name] = newWorker;
        return newWorker;
    }
    private bool disposed = false;

    protected virtual void Dispose(bool _isDisposing)
    {
        //close session
        if (_isDisposing)
        {
        }
    }

    public void Dispose()
    {
        if (disposed)
            return;
        Dispose(true);
        disposed = true;
    }

    private Dictionary<string, Worker> nameToWorker = new Dictionary<string, Worker>();
    private CancellationTokenSource cts = new CancellationTokenSource();
    private CoreSession mSession;

    private string ipStr;
    private int port;

    private Networker()
    {

    }

    private void ReadyWorkers()
    {
        nameToWorker["pkg"] = new Worker("pkg");
        nameToWorker["recv"] = new Worker("recv");

        nameToWorker["pkg"].PushJob(new JobOnce(DateTime.UtcNow, ()=> {
            while(cts.IsCancellationRequested == false)
            {
               
            }
        }));
        nameToWorker["recv"].PushJob(new JobOnce(DateTime.UtcNow, () => {
            while (cts.IsCancellationRequested == false)
            {
               
            }
        }));
    }

    public void SetConnToServer(string _Ip, int _port)
    {
        ipStr = _Ip;
        port = _port;
    }

    public override void ReadyToStart()
    {
        ReadyWorkers();
    }

    public override void Start()
    {


        foreach (var w in nameToWorker)
        {
            UnityEngine.Debug.Log($"{w.Key} worker is start");
            w.Value.WorkStart();
        }
    }

    protected override void Analizer_Ans(CoreSession _s, Packet _p)
    {
        throw new System.NotImplementedException();
    }

    protected override void Analizer_Noti(CoreSession _s, Packet _p)
    {
        throw new System.NotImplementedException();
    }

    protected override void Analizer_Req(CoreSession _s, Packet _p)
    {
        throw new System.NotImplementedException();
    }

    protected override void Analizer_Test(CoreSession _s, Packet _p)
    {
        throw new System.NotImplementedException();
    }
}
