using System;
using V2 = Vector2;

public enum WORLDOBJECTID { Empty, Character, Block  }

class Model
{       
    SquareHitTool squareHitTool;

    public V2 LegalMove(V2 pos, V2 targetPos, float radius, WORLDOBJECTID[,] worldData, out Hits hits)
    {
        WORLDOBJECTID firstHit = WORLDOBJECTID.Empty;
        WORLDOBJECTID firstHit2 = WORLDOBJECTID.Empty;
        WORLDOBJECTID secondHit = WORLDOBJECTID.Empty;

        V2 legalPos = targetPos;

        squareHitTool.Setup(pos, targetPos, radius);

        if (pos.y < 0)
        {
            hits = new Hits(firstHit, firstHit2, secondHit);
            return new V2(Math.Clamp(targetPos.x, radius, Library.fieldWidth - radius), targetPos.y);
        }

        HitData2D[] hitDatas = squareHitTool.GetFinalValues();

        foreach (HitData2D hitData in hitDatas)
        {                        
           bool firstDimHasWall = Utility.HasWall(hitData.dimHit1.firstHitIDs, worldData, hitData.dimHit1.r_xy, out firstHit);
           bool secondDimHasWall = Utility.HasWall(hitData.dimHit2.firstHitIDs, worldData, hitData.dimHit1.r_xy, out firstHit2);
            if (!firstDimHasWall && !secondDimHasWall)
                continue;            
            if(firstDimHasWall && secondDimHasWall)
            {
                legalPos = hitData.dimHit1.firstHitPos;
                break;
            }

            HitData1D checkOneDim;

            if (firstDimHasWall)
                checkOneDim = hitData.dimHit1;
            else
                checkOneDim = hitData.dimHit2;


            if (Utility.HasWall(checkOneDim.secondHitIDs, worldData, Utility.R_XYOpp(hitData.dimHit1.r_xy), out secondHit))
                legalPos = checkOneDim.secondHitPos;             
            else
                legalPos = checkOneDim.secondMaxPos;
            break;
        }

        hits = new Hits(firstHit, firstHit2, secondHit);
        return legalPos;
    }

}
