using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectSceneObj : MonoBehaviour
{
    static readonly int Character_max = 4;
    public List<GameObject> CharacterList;

    private int curPlayerCnt = 0;

    //todo : server 한테 input 받기.
    private int[] pId = new int[Character_max];

    public SelectSceneObj()
        : this(2)
    {
    }

    public SelectSceneObj(int _playerCnt = 1)
    {
        curPlayerCnt = _playerCnt;
    }

    public void Start()
    {
        var no = 1;
        for (int idx = 0; idx < curPlayerCnt; ++idx)
        {
            pId[idx] = no;
            var character = CharacterList[idx].GetComponent<SelectCharacter>();
            character.SelectPlayer(no);
            no++;
        }
    }
}
