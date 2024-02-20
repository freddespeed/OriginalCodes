
public struct HitData2D
{
    public readonly HitData1D dimHit1;
    public readonly HitData1D dimHit2;

    public HitData2D(HitData1D firstHit, HitData1D secondHit)
    {
        this.dimHit1 = firstHit;
        this.dimHit2 = secondHit;
    }
}

public struct HitData1D
{
    public readonly V2 firstHitPos;
    public readonly V2I[] firstHitIDs;
    public readonly V2 secondMaxPos;
    public readonly V2 secondHitPos;
    public readonly V2I[] secondHitIDs;
    public readonly int r_xy;
    public static readonly HitData1D Empty = new HitData1D(Rep.Unused, Vector2.zero, null, Vector2.zero, Vector2.zero, null);
    public HitData1D(int r_xy, V2 firstHitPos, V2I[] firstHitIDs, V2 secondMaxPos, V2 secondHitPos, V2I[] secondHitIDs)
    {
        this.firstHitPos = firstHitPos;
        this.firstHitIDs = firstHitIDs;
        this.secondMaxPos = secondMaxPos;
        this.secondHitPos = secondHitPos;
        this.secondHitIDs = secondHitIDs;
        this.r_xy = r_xy;
    }
}