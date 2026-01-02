using System;
using System.Collections.Generic;
using CollisionDetection;
using UnityEngine;
using UnityEngine.EventSystems;
namespace DrawLine
{
    /// <summary>
    /// Drawing Controller - Handles user input and business logic.
    /// </summary>
    public class DrawLineController : MonoBehaviour, IPointerDownHandler
    {
        [Header("References")]
        [SerializeField] private DrawLineView view;
        
        [Header("Settings")]
        [SerializeField] private float snapDistance = 20f;
    
        private DrawLineModel model;
        private RectTransform rectTransform;
    
        public DrawLineModel Model => model;
    
        private void Awake()
        {
            rectTransform = GetComponent<RectTransform>();
            
            // Init MVC
            model = new DrawLineModel();
            
            if (view == null)
                view = GetComponent<DrawLineView>();
                
            view.Initialize(model);
            
            // Create the first polygon
            model.AddNewPolygon();
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            // Transform screen point to local point
            if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(
                rectTransform, eventData.position, null, out Vector2 localPos))
            {
                return;
            }
            
            ProcessClick(localPos);
        }
    
        /// <summary>
        /// Handle click at the given position
        /// </summary>
        private void ProcessClick(Vector3 position)
        {
            var currentPolygon = model.CurrentPolygon;
            
            // Check if close the polygon
            if (currentPolygon != null && 
                currentPolygon.IsCloseToFirstVertex(position, snapDistance))
            {
                CompleteCurrentPolygon();
            }
            else
            {
                // Add new vertex
                model.AddVertexToCurrentPolygon(position);
            }
        }
    
        /// <summary>
        /// Complete the current polygon
        /// </summary>
        private void CompleteCurrentPolygon()
        {
            model.SortCurrentPolygonVertices();
            model.CompleteCurrentPolygon();
            model.AddNewPolygon();
        }
    
        /// <summary>
        /// Withdraw the last added vertex
        /// </summary>
        public void UndoLastVertex()
        {
            var current = model.CurrentPolygon;
            if (current != null && current.VertexCount > 0)
            {
                var vertices = current.GetVerticesCopy();
                vertices.RemoveAt(vertices.Count - 1);
                // 需要在 Model 中添加替换顶点的方法
            }
        }
    
        /// <summary>
        /// Clear all polygons
        /// </summary>
        public void ClearAll()
        {
            model.Clear();
            model.AddNewPolygon();
        }
    
        /// <summary>
        /// Set snap distance for closing polygons
        /// </summary>
        public void SetSnapDistance(float distance)
        {
            snapDistance = Mathf.Max(0f, distance);
        }
    
        /// <summary>
        /// Export all polygon vertex data\
        /// </summary>
        public List<List<Vector3>> ExportData()
        {
            return model.ExportAllVertices();
        }
    }
}