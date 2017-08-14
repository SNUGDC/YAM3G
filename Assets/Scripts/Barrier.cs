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