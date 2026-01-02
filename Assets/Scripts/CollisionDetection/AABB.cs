using System.Linq;
using DrawLine;
using UnityEngine;

namespace CollisionDetection 
{
    public class AABB : ICollisionDetection
    {
        private DrawLineModel _demoModel;

        public AABB() {}

        public AABB(DrawLineModel demoModel)
        {
            _demoModel = demoModel;
        }
        
        public bool IsColliding(PolygonData polyA, PolygonData polyB)
        {
            float minAx = polyA.Vertices.Min(v => v.x);
            float maxAx = polyA.Vertices.Max(v => v.x);
            float minAy = polyA.Vertices.Min(v => v.y);
            float maxAy = polyA.Vertices.Max(v => v.y);
            
            if(_demoModel != null)
            {
                Vector3 center = new Vector3((minAx + maxAx)/2, (minAy + maxAy)/2);
                Vector3 size = new Vector3(maxAx - minAx, maxAy - minAy, 0);
                _demoModel.AddRectangle(polyA.Color, center, size);
            }
    
            float minBx = polyB.Vertices.Min(v => v.x);
            float maxBx = polyB.Vertices.Max(v => v.x);
            float minBy = polyB.Vertices.Min(v => v.y);
            float maxBy = polyB.Vertices.Max(v => v.y);
            
            if(_demoModel != null)
            {
                Vector3 center = new Vector3((minBx + maxBx)/2, (minBy + maxBy)/2);
                Vector3 size = new Vector3(maxBx - minBx, maxBy - minBy, 0);
                _demoModel.AddRectangle(polyB.Color, center, size);
            }
    
            bool overlapX = maxAx >= minBx && maxBx >= minAx;
            bool overlapY = maxAy >= minBy && maxBy >= minAy;
    
            return overlapX && overlapY;
        }
    }
}