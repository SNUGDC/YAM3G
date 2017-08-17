using UnityEngine;

public class Barrier
{
    public bool value;
    public GameObject barrierObject;
    public Barrier(bool value, GameObject barrierObject)
    {
        this.value = value;
        this.barrierObject = barrierObject;
    }
}
public class BarrierWithPos
{
    public Barrier barrier;
    public IntVector2 pos;
    public BarrierWithPos(Barrier barrier, IntVector2 pos)
    {
        this.barrier = barrier;
        this.pos = pos;
    }
}