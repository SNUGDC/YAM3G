
using System.Collections.Generic;
using DG.Tweening;

public static class TweenJoiner {
    public static Sequence JoinAll<T>(this Sequence seq, IEnumerable<T> tweens)
        where T: Tween 
    {
        foreach(var tween in tweens) {
            seq.Join(tween);
        }
        return seq;
    }
}

