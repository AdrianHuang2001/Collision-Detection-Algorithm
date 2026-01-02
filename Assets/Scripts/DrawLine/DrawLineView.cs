using UnityEngine;
using UnityEngine.UI;

namespace DrawLine
{
    /// <summary>
    /// DrawLine View - for drawing lines and shapes on a UI canvas
    /// </summary>
    public class DrawLineView : MaskableGraphic
    {
        [Header("Visual Settings")] public Color LineColor = Color.blue;
        public float LineWidth = 5f;

        private DrawLineModel model;

        public void Initialize(DrawLineModel drawModel)
        {
            model = drawModel;
            model.OnDataDirty += RefreshDisplay;
        }

        private void OnDestroy()
        {
            if (model != null)
            {
                model.OnDataDirty -= RefreshDisplay;
            }
        }

        private void OnModelUpdated(PolygonData polygon = null)
        {
            RefreshDisplay();
        }

        protected override void OnPopulateMesh(VertexHelper vh)
        {
            vh.Clear();

            if (model == null) return;
            
            foreach (var polygon in model.GetPolygons())
            {
                if (polygon.Vertices.Count < 2) continue;
                DrawPolygonLines(vh, polygon);

                if (polygon.IsCompleted)
                {
                    DrawPolygonFill(vh, polygon);
                }
            }
        }

        /// <summary>
        /// Draw the lines of the polygon
        /// </summary>
        private void DrawPolygonLines(VertexHelper vh, PolygonData polygon)
        {
            var vertices = polygon.Vertices;

            // Draw line segments between adjacent vertices.
            for (int i = 0; i < vertices.Count - 1; i++)
            {
                var lineVerts = CalculateLineVertices(vertices[i], vertices[i + 1], polygon.IsCompleted ? LineColor : polygon.Color);
                vh.AddUIVertexQuad(lineVerts);
            }

            // If the polygon is complete, connect the last vertex to the first vertex.
            if (polygon.IsCompleted)
            {
                var closingLine = CalculateLineVertices(vertices[^1], vertices[0], LineColor);
                vh.AddUIVertexQuad(closingLine);
            }
        }

        /// <summary>
        /// Drawing the fill of a polygon
        /// </summary>
        private void DrawPolygonFill(VertexHelper vh, PolygonData polygon)
        {
            var vertices = polygon.Vertices;
            int baseIndex = vh.currentVertCount;

            // Add all vertices
            foreach (var v in vertices)
            {
                UIVertex newVertex = new UIVertex
                {
                    position = v,
                    color = polygon.Color,
                };
                vh.AddVert(newVertex);
            }

            // Create triangles (fan triangulation)
            for (int i = 1; i < vertices.Count - 1; i++)
            {
                vh.AddTriangle(baseIndex, baseIndex + i, baseIndex + i + 1);
            }
        }

        /// <summary>
        /// Calculate the four vertices of the line segment.
        /// </summary>
        private UIVertex[] CalculateLineVertices(Vector3 start, Vector3 end, Color color)
        {
            Vector3 direction = (end - start).normalized;
            Vector3 normal = Vector3.Cross(direction, Vector3.forward).normalized;
            Vector3 offset = normal * LineWidth;

            UIVertex[] verts = new UIVertex[4];

            verts[0] = new UIVertex { position = start + offset, color = color };
            verts[1] = new UIVertex { position = start - offset, color = color };
            verts[2] = new UIVertex { position = end - offset, color = color };
            verts[3] = new UIVertex { position = end + offset, color = color };

            return verts;
        }

        /// <summary>
        /// Update line color
        /// </summary>
        public void SetLineColor(Color color)
        {
            LineColor = color;
            SetVerticesDirty();
        }

        /// <summary>
        /// Update line width
        /// </summary>
        public void SetLineWidth(float width)
        {
            LineWidth = Mathf.Max(0.1f, width);
            SetVerticesDirty();
        }

        /// <summary>
        /// Refresh the display
        /// </summary>
        public void RefreshDisplay()
        {
            SetVerticesDirty();
        }
    }
}