using System;
using System.Collections.Generic;
using DrawLine;
using UnityEngine;

namespace CollisionDetection
{
    public class GJK : ICollisionDetection
    {
        public class Simplex
        {
            private List<Vector3> points = new List<Vector3>(3);
            public int Count => points.Count;

            public Vector3 this[int index]
            {
                get => points[index];
                set => points[index] = value;
            }

            public void Add(Vector3 point)
            {
                points.Insert(0, point);
            }

            public void Clear()
            {
                points.Clear();
            }

            public bool ContainsOrigin(ref Vector3 direction)
            {
                if (points.Count == 2)
                {
                    return HandleLine(ref direction);
                }
                else if (points.Count == 3)
                {
                    return HandleTriangle(ref direction);
                }

                return false;
            }
            private bool HandleLine(ref Vector3 direction)
            {
                Vector3 a = points[0]; // 最新点
                Vector3 b = points[1]; // 上一个点

                Vector3 ab = b - a;
                Vector3 perpH = new Vector3(-ab.y, ab.x, 0);
                Vector3 ao = -a;        

                direction = Vector3.Dot(perpH, ao) < 0 ? -perpH : perpH;
                return false; 
            }
            
            private bool HandleTriangle(ref Vector3 direction)
            {
                Vector3 a = points[0]; 
                Vector3 b = points[1];
                Vector3 c = points[2];

                Vector3 ab = b - a;
                Vector3 ac = c - a;
                Vector3 ao = -a;

                // 计算 ab 边的垂线 (在三角形平面内，且指向三角形外)
                // 逻辑: (ac x ab) 得到平面法线，再 x ab 得到指向 ab 外侧的向量
                Vector3 abPerp = Vector3.Cross(Vector3.Cross(ac, ab), ab);
                
                // 计算 ac 边的垂线
                Vector3 acPerp = Vector3.Cross(Vector3.Cross(ab, ac), ac);

                // 1. 检查是否在 AB 外侧
                if (Vector3.Dot(abPerp, ao) > 0)
                {
                    // 在 AB 外侧，C 点没用了，移除 C
                    points.RemoveAt(2); 
                    // 现在的 Simplex 变成了 [A, B] (线段)
                    
                    // 更新方向：垂直于 AB 指向原点
                    // 优化：abPerp 已经是指向外侧了，直接用它作为新方向即可
                    direction = abPerp; 
                    return false;
                }

                // 2. 检查是否在 AC 外侧
                if (Vector3.Dot(acPerp, ao) > 0)
                {
                    // 在 AC 外侧，B 点没用了，移除 B
                    points.RemoveAt(1);
                    // 现在的 Simplex 变成了 [A, C] (线段)
                    
                    direction = acPerp;
                    return false;
                }

                // BUG修复 3: 2D 情况下，如果都在内侧，说明碰撞了
                return true;
            }
            
        }
        private DrawLineModel _demoModel;

        public GJK() {}

        public GJK(DrawLineModel demoModel)
        {
            _demoModel = demoModel;
        }
        public bool IsColliding(PolygonData polyA, PolygonData polyB)
        {
            var simplex = new Simplex();
            
            Vector3 direction = polyA.Center - polyB.Center;
            direction.Normalize();
            direction.z = 0;
            if (direction.magnitude < 0.0001f) direction = Vector3.right;
            Vector3 support = GetSupport(polyA, polyB, direction);
            simplex.Add(support);
            direction = -support;

            while (true)
            {
                support = GetSupport(polyA, polyB, direction);
                if (Vector3.Dot(support, direction) < 0)
                {
                    return false;
                }
                simplex.Add(support);
                if (simplex.ContainsOrigin(ref direction))
                {
                    return true;
                }
            }
        }
        
        private Vector3 GetSupport(PolygonData polyA, PolygonData polyB, Vector3 direction)
        {
            Vector3 supportA = Support(polyA, direction);
            Vector3 supportB = Support(polyB, -direction);
            return supportA - supportB;
        }
        
        private Vector3 Support(PolygonData poly, Vector3 direction)
        {
            float max = float.MinValue;
            Vector3 support = poly.Vertices[0];
            foreach (var point in poly.Vertices)
            {
                float project = Vector3.Dot(point, direction);
                if (project > max)
                {
                    support = point;
                    max = project;
                }
            }
            return support;
        }
    }
}