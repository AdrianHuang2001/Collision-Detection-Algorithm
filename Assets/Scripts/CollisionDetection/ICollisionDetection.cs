using System;
using DrawLine;

namespace CollisionDetection
{
    public interface ICollisionDetection
    {
        public bool IsColliding(PolygonData polyA, PolygonData polyB);
    }
}