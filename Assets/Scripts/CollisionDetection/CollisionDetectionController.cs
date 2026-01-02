using System;
using DrawLine;
using TMPro;
using UnityEngine;

namespace CollisionDetection
{
    public enum CollisionDetectionType
    {
        AABB = 1,
        SAT = 2,
        GJK = 3
    }
    public class CollisionDetectionController : MonoBehaviour
    {
        [SerializeField] private DrawLineView AABBView;
        [SerializeField] private DrawLineView SATView;
        [SerializeField] private DrawLineView GJKView;
        [SerializeField] private DrawLineController dlc;
        [SerializeField] private TMP_Text infoText;

        private ICollisionDetection dector;

        private DrawLineModel model;

        private void Start()
        {
            Clear();
        }
        
        public void Clear()
        {
            model = new DrawLineModel();
            AABBView.Initialize(model);
            SATView.Initialize(model); 
            GJKView.Initialize(model);
            AABBView.gameObject.SetActive(false);
            SATView.gameObject.SetActive(false);
            GJKView.gameObject.SetActive(false);
            infoText.text = string.Empty;
        }
        
        public void CheckColliding(int type)
        {
            switch ((CollisionDetectionType)type)
            {
                case CollisionDetectionType.AABB:
                    dector = new AABB(model);
                    AABBView.gameObject.SetActive(true);
                    SATView.gameObject.SetActive(false);
                    GJKView.gameObject.SetActive(false);
                    model.Clear();
                    break;
                case CollisionDetectionType.SAT:
                    dector = new SAT(model);
                    AABBView.gameObject.SetActive(false);
                    SATView.gameObject.SetActive(true);
                    GJKView.gameObject.SetActive(false);
                    model.Clear();
                    break;
                case CollisionDetectionType.GJK:
                    dector = new GJK(model);
                    AABBView.gameObject.SetActive(false);
                    SATView.gameObject.SetActive(false);
                    GJKView.gameObject.SetActive(true);
                    model.Clear();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }

            var polygons = dlc.Model.Polygons;

            for (int i = 0; i < polygons.Count; i++)
            {
                if(polygons[i] != null && polygons[i].IsCompleted)
                {
                    for (int j = i + 1; j < polygons.Count; j++)
                    {
                        if(polygons[j] != null && polygons[j].IsCompleted)
                        {
                            bool isColliding = dector.IsColliding(polygons[i], polygons[j]);
                            infoText.text += $"Polygon {i} and Polygon {j} colliding: {isColliding}";
                            Debug.Log($"Polygon {i} and Polygon {j} colliding: {isColliding}");
                        }
                    }
                }
            }
        }
    }
}