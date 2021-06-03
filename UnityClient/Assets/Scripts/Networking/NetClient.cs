using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using UnityEngine;
//for p2p?
public class NetClient
{
    public static NetClient Inst { get; private set; }

    public static void Init(string _ip, int _port, string _pip, int _pport)
    {
        Inst = new NetClient();
        Inst.PublicIp = _ip;
        Inst.PublicPort = _port;
        Inst.PrivateIp = _pip;
        Inst.PrivatePort = _pport;
    }

    //todo : CoreNet 참조 추가할 것.

    public string PublicIp { get; private set; }
    public int PublicPort { get; private set; }
    public string PrivateIp{ get; private set; }
    public int PrivatePort { get; private set; }

    private NetClient()
    {
    }

}
