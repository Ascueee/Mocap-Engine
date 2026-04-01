using Galaxy3D.ECS;
using Galaxy3D.ECS.Components.Physics;
using OpenTK.Mathematics;

namespace Galaxy3D.EngineSpecific.PhysicsAdditions;

public class Raycast
{
    List<Entity> _colliders = new List<Entity>();
    public bool Hit( Vector3 origin, Vector3 dir, float length)
    {
        float tMin;
        float tMax;

        
        foreach (Entity aabb in _colliders)
        {
            tMin = float.NegativeInfinity;
            tMax = float.PositiveInfinity;
            AABB currentAABBCheck = aabb.GetComponent<AABB>();

            Vector3 aabbMax = currentAABBCheck.max;
            Vector3 aabbMin = currentAABBCheck.min;
            
            float tx1 = (aabbMin.X - origin.X) / dir.X;
            float tx2 = (aabbMax.X - origin.X) / dir.X;
            tMin = MathF.Max(tMin, MathF.Min(tx1, tx2));
            tMax = MathF.Min(tMax, MathF.Max(tx1, tx2));

            float ty1 = (aabbMin.Y - origin.Y) / dir.Y;
            float ty2 = (aabbMax.Y - origin.Y) / dir.Y;
            tMin = MathF.Max(tMin, MathF.Min(ty1, ty2));
            tMax = MathF.Min(tMax, MathF.Max(ty1, ty2));

            float tz1 = (aabbMin.Z - origin.Z) / dir.Z;
            float tz2 = (aabbMax.Z - origin.Z) / dir.Z;
            tMin = MathF.Max(tMin, MathF.Min(tz1, tz2));
            tMax = MathF.Min(tMax, MathF.Max(tz1, tz2));
        
            if (tMax >= MathF.Max(tMin, 0.0f) && tMin <= length)
            {
                Console.WriteLine($"RayCast Hit at distance {tMin}");
                return true;
            }
        }

        return false;
    }
    
    
    public List<Entity> colliders => _colliders;
}