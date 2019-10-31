using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate int stUpdate();
public delegate void stBegin();
public delegate void stEnd();
public delegate IEnumerator stCoroutine();

class StateValue
{
    public stUpdate update { get;}
    public stBegin begin { get; }
    public stEnd end { get; }
    public stCoroutine coroutine { get; }

    public StateValue(stUpdate update, stCoroutine coroutine, stBegin begin, stEnd end)
    {
        this.update = update;
        this.coroutine = coroutine;
        this.begin = begin;
        this.end = end;
    }
}


public class StateMachine
{
    Dictionary<int, StateValue> dicState = new Dictionary<int, StateValue> { };
    public int curState { get; private set; }

    public void Update()
    {
        int ret = dicState[curState].update.Invoke();
        if (curState != ret)
        {
            dicState[curState].end?.Invoke();

            curState = ret;
            
            dicState[curState].begin?.Invoke();

            Debug.Log(curState);

            //StartCoroutine(dicState[curState].coroutine?.Invoke());
        }
    }

    public void SetCallback(int type_, stUpdate update_, stBegin begin_, stEnd end_)
    {
        dicState.Add(type_, new StateValue(update_, null, begin_, end_));
    }

    public void changeState(int type_)
    {
        dicState[curState].end?.Invoke();

        curState = type_;

        dicState[curState].begin?.Invoke();
    }

}

