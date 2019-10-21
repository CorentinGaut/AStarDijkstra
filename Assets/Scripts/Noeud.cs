using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Noeud
{
    public int x;
    public int y;
    public int distance;
    public int heuristique;
    public bool obstacle;
    public Noeud parent;

    public Noeud(int x, int y, int distance,Noeud parent, bool obstacle)
    {
        this.x = x;
        this.y = y;
        this.distance = distance;
        this.parent = parent;
        this.obstacle = obstacle;
    }

    public override bool Equals(object obj)
    {
        return obj is Noeud noeud &&
               base.Equals(obj) &&
               x == noeud.x &&
               y == noeud.y;
    }

    public override int GetHashCode()
    {
        var hashCode = 1559617763;
        hashCode = hashCode * -1521134295 + base.GetHashCode();
        hashCode = hashCode * -1521134295 + x.GetHashCode();
        hashCode = hashCode * -1521134295 + y.GetHashCode();
        hashCode = hashCode * -1521134295 + distance.GetHashCode();
        return hashCode;
    }

}
