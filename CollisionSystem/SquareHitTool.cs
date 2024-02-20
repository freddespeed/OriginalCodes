using V2 = Vector2;
using V2I = Vector2Int;

static class Rep
{
    public const int X = 0;
    public const int Y = 1;
    public const int BothXAndY = 2;
    public const int True = 1;
    public const int False = -1;
    public const int NeitherTrueNorFalse = 0;
    public const int Unused = -1;
}

public class SquareHitTool
{
    const int X = 0;
    const int Y = 1;
    const int BothXAndY = 2;
    const int Unused = -1;

    int hitDataCount;
    struct BaseValues
    {
        public readonly V2 pos;
        public readonly V2 targetPos;
        public readonly float radius;

        public BaseValues(V2 pos, V2 targetPos, float radius)
        {
            this.pos = pos;
            this.targetPos = targetPos;
            this.radius = radius;
        }
    }
    BaseValues bv;
    V2 safetyMargin;
    float yFactor;
    float xFactor;
    float totalXDist;
    float totalYDist;
    int xDir;
    int yDir;
    public SquareHitTool(V2 pos, V2 targetPos, float radius) => Setup(pos, targetPos, radius);

    public SquareHitTool() { }
    
    public void Setup(V2 pos, V2 targetPos, float radius)
    {
        if (pos.x - radius <= 0 || pos.y - radius <= 0)
            throw new ArgumentException("Player cannot exist within negative values of the field!!!!!!!!");

        bv = new BaseValues(pos, targetPos, radius);
        totalXDist = MathF.Abs(pos.x - targetPos.x);
        totalYDist = MathF.Abs(pos.y - targetPos.y);
        xFactor = totalXDist / totalYDist;
        yFactor = totalYDist / totalXDist;
        xDir = MathF.Sign(targetPos.x - pos.x);
        yDir = MathF.Sign(targetPos.y - pos.y);
        hitDataCount = 0;
    }

    public HitData2D[] GetFinalValues()
    {
        List<HitData2D> finalValues = new();

        int[] xIntHits = FindIntHits(bv.pos.x, bv.targetPos.x, xDir);
        int[] yIntHits = FindIntHits(bv.pos.y, bv.targetPos.y, yDir);

        int xi = 0;
        int yi = 0;
        while (true)
        {
            int nextIntHit;
            Tuple<int, int> doubleIntHit = null;
            int r_xy;
            if (xi < xIntHits.Length && yi < yIntHits.Length)
            {
                int compareClosestX = XIsClosest(xIntHits[xi], yIntHits[yi]);
                if (compareClosestX == Rep.True)
                {
                    nextIntHit = xIntHits[xi];
                    xi ++;
                    r_xy = Rep.X;
                }
                else if(compareClosestX == Rep.False)
                {
                    nextIntHit = yIntHits[yi];
                    yi ++;
                    r_xy = Rep.Y;
                }
                else
                {
                    nextIntHit = Rep.Unused;
                    doubleIntHit = new(xIntHits[xi], yIntHits[yi]);
                    r_xy = Rep.BothXAndY;
                    xi ++;
                    yi ++;
                }

            }
            else if (xi < xIntHits.Length)
            {
                nextIntHit = xIntHits[xi];
                xi ++;
                r_xy = Rep.X;
            }
            else if (yi < yIntHits.Length)
            {
                nextIntHit = yIntHits[yi];
                yi ++;
                r_xy = Rep.Y;
            }
            else
                break;

            if (doubleIntHit == null)
                finalValues.Add(new(GetHitdata(nextIntHit, r_xy), HitData1D.Empty));
            else
                finalValues.Add(new(GetHitdata(doubleIntHit.Item1, Rep.X), GetHitdata(doubleIntHit.Item2, Rep.Y)));
        }
        
        return finalValues.ToArray();
    }

    int XIsClosest(int xHit, int yHit)
    {
        float xHitPos = xHit - (bv.radius * xDir);
        float yHitPos = yHit - (bv.radius * yDir);
        float xDistToHit = MathF.Abs(xHitPos - bv.pos.x);
        float yDistToHit = MathF.Abs(yHitPos - bv.pos.y);

        float finalXDist = totalXDist / xDistToHit;
        float finalYDist = totalYDist / yDistToHit;

        if (finalXDist > finalYDist)
            return Rep.True;
        else if (finalXDist < finalYDist)
            return Rep.False;
        else
            return Rep.NeitherTrueNorFalse;
    }

    public int[] FindIntHits(float pos, float targetPos, int dir)
    {
        List<int> hits = new List<int>();
        if (dir == 0)
            return hits.ToArray();

        targetPos += (bv.radius * dir);
        pos += (bv.radius * dir);

        if (dir < 0)
            targetPos += 1;
        
        int startI = (int)pos + Math.Max(dir, 0);
        if ((int)pos == pos)
            startI -= Math.Max(dir, 0);

        for (int i = startI; (i * dir) <= ((int)targetPos * dir); i += dir)
        {
            hits.Add(i);
        }

        return hits.ToArray();
    }

    HitData1D GetHitdata(int hit, int r_xy)
    {
        hitDataCount += 1;
        float pos = GetValue(r_xy, bv.pos);
        int r_xyOpp = (r_xy + 1) % 2;
        int dir = GetDir(r_xy);
        int otherDir = GetDir(r_xyOpp);
        float hitPos = hit - (bv.radius * dir);
        float dist = MathF.Abs(pos - hitPos);
        float otherHitPos = GetValue(r_xyOpp, bv.pos) + (dist * GetFactor(r_xyOpp) * otherDir);        

        V2 firstHitPos = SetValue(r_xy, hitPos, otherHitPos);
        V2I[] firstHitIDs = HitBlockIDs(firstHitPos, hit, r_xy);
        float otherTargetPos = GetValue(r_xyOpp, bv.targetPos);
        float otherMaxPos = (int)(otherHitPos + (bv.radius * otherDir)) + MathF.Max(otherDir * 2, -1) - (bv.radius * otherDir);
        if (otherTargetPos * otherDir <= otherMaxPos * otherDir)
            otherMaxPos = otherTargetPos;

        V2 secondMaxPos = SetValue(r_xy, hitPos, otherMaxPos);        
        V2 secondHitPos = V2.zero;
        V2I[] secondHitIDs = null;


        int otherHit = (int)(otherHitPos + (bv.radius * otherDir)) + Math.Max(otherDir, 0);
        if ((int)(otherHitPos + (bv.radius * otherDir)) != (int)(otherHitPos))
        {
            otherHit -= otherDir;
        }

        float secondOtherHitPos = otherHit - (bv.radius * otherDir);
        if (secondOtherHitPos * otherDir < otherMaxPos * otherDir)
        {
            secondHitPos = SetValue(r_xy, hitPos, secondOtherHitPos);
            secondHitIDs = HitBlockIDs(secondHitPos, otherHit, r_xyOpp);            
        }

        return new HitData1D(r_xy, firstHitPos, firstHitIDs, secondMaxPos, secondHitPos, secondHitIDs);

    }

    V2I[] HitBlockIDs(V2 hitPos, int hit, int r_xy)
    {
        List<V2I> blockIDs = new();
        if (r_xy == Rep.X)
        {
            int xID = hit + Math.Min(xDir, 0);
            foreach (int yID in GetIntTouches(hitPos.y))
                blockIDs.Add(new V2I(xID, yID));
        }
        else
        {
            int yID = hit + Math.Min(yDir, 0);
            foreach (int xID in GetIntTouches(hitPos.x))
                blockIDs.Add(new V2I(xID, yID));
        }

        return blockIDs.ToArray();
    }

    int[] GetIntTouches(float position)
    {
        List<int> result = new List<int>();

        int lastTouch = (int)(position + bv.radius);

        for (int i = (int)(position - bv.radius); i <= lastTouch; i++)
        {
            if (i == lastTouch)
            {
                if (lastTouch == position + bv.radius)
                    break;
            }
            result.Add(i);
        }

        return result.ToArray();
    }

    int GetDir(int r_xy)
    {
        if (r_xy == Rep.X)
            return xDir;
        else
            return yDir;
    }


    V2 SetValue(int r_xy, float pos, float otherPos)
    {
        if (r_xy == Rep.X)
            return new V2(pos, otherPos);
        else
            return new V2(otherPos, pos);
    }

    float GetFactor(int r_xy)
    {
        if (r_xy == Rep.X)
            return xFactor;
        else
            return yFactor;
    }

    float GetValue(int r_xy, V2 pos)
    {
        if (r_xy == Rep.X)
            return pos.x;
        else
            return pos.y;
    }

}

