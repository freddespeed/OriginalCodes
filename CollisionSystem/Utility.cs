using V2I = Vector2Int;
using System;

static class Utility
{    

    public static bool IsWall(WORLDOBJECTID id)
    {
        return id switch
        {            
            WORLDOBJECTID.Block => true,
            _ => false,
        };
    }

    public static int R_XYOpp(int r_xy) => (r_xy + 1) % 2;

    
    public static bool HasWall(V2I[] blockIDs, WORLDOBJECTID[,] worldData, int r_xy, out WORLDOBJECTID hit)
    {
        hit = WORLDOBJECTID.Empty;
        if (blockIDs == null)
            return false;
        foreach (V2I blockID in blockIDs)
        {
            if (blockID.x < 0 || blockID.x > Library.fieldWidth - 1)
            {
                if (r_xy == Rep.X)
                    hit = WORLDOBJECTID.Block;
                else
                    hit = worldData[Math.Clamp(blockID.x, 0, 19), blockID.y];
            }
            else
                hit = worldData[blockID.x, blockID.y];

            if (IsWall(hit))
            {
                return true;
            }                
        }
        return false;
    }

}


