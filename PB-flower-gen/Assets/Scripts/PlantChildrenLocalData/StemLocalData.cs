using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StemLocalData : Component
{
    #region Properties

    public bool isBranch = false;

    public Node[] SproutNodes;
    public Node[] Nodes;

    public Vector2 SproutThickness;
    public Vector2 StemThickness;

    public Color Color = Color.green;

    #endregion
    //public StemLocalData() { }
    //public StemLocalData(StemLocalData data)
    //{
    //    isBranch        = data.isBranch;
    //    SproutNodes     = data.SproutNodes;
    //    Nodes           = data.Nodes;
    //    SproutThickness = data.SproutThickness;
    //    StemThickness   = data.StemThickness;
    //    Color           = data.Color;
    //}
    public StemLocalData AssignValues(StemLocalData data)
    {
        isBranch = data.isBranch;
        SproutNodes = data.SproutNodes;
        Nodes = data.Nodes;
        SproutThickness = data.SproutThickness;
        StemThickness = data.StemThickness;
        Color = data.Color;

        return this;
    }
}
