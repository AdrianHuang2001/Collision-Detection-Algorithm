using System.Collections.Generic;
using DrawLine;
using UnityEngine;

namespace CollisionDetection
{
    public class SAT : ICollisionDetection
    {
        private DrawLineModel _demoModel;

        public SAT() {}

        public SAT(DrawLineModel demoModel)
        {
            _demoModel = demoModel;
        }
        public bool IsColliding(PolygonData polyA, PolygonData polyB)
        {
            List<Vector2> normals = new ();
            normals.AddRange(GetEdges(polyB));
            normals.AddRange(GetEdges(polyA));
            for (int i = 0; i < normals.Count; i++)
            {
                normals[i] = new Vector2(-normals[i].y, normals[i].x).normalized;
                _demoModel.AddDirection(Color.black, Vector3.zero, normals[i]);
                List<float> projectA = new ();
                List<float> projectB = new ();
                foreach (Vector2 v in polyA.Vertices)
                {
                    projectA.Add(Vector2.Dot(normals[i], v));
                }

                foreach (Vector2 v in polyB.Vertices)
                {
                    projectB.Add(Vector2.Dot(normals[i], v));
                }
                
                float minA = Mathf.Min(projectA.ToArray());
                float maxA = Mathf.Max(projectA.ToArray());
                _demoModel.AddLine(polyA.Color, normals[i] * minA, normals[i] * maxA);
                float minB = Mathf.Min(projectB.ToArray());
                float maxB = Mathf.Max(projectB.ToArray());
                _demoModel.AddLine(polyB.Color, normals[i] * minB, normals[i] * maxB);
                
                if (maxA < minB || maxB < minA)
                {
                    return false;
                }
            }
            return true;
        }
        
        private List<Vector2> GetEdges(PolygonData poly)
        {
            List<Vector2> edges = new ();
            int count = poly.Vertices.Count;
            for (int i = 0; i < count; i++)
            {
                Vector2 current = poly.Vertices[i];
                Vector2 next = poly.Vertices[(i + 1) % count];
                edges.Add(next - current);
            }
            return edges;
        }
    }
}