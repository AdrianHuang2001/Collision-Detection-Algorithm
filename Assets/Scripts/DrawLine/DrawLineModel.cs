using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DrawLine
{
/// <summary>
/// DrawingLine Model - Stores all the data for the drawn shapes.
/// </summary>
public class DrawLineModel
{
    private List<PolygonData> polygons = new();

    public IReadOnlyList<PolygonData> Polygons => polygons.AsReadOnly();
    public PolygonData CurrentPolygon => polygons.Count > 0 ? polygons[^1] : null;
    public int PolygonCount => polygons.Count;

    public event Action<PolygonData> OnPolygonAdded;
    public event Action OnPolygonUpdated;
    public event Action OnDataCleared;
    public event Action OnDataDirty;
    
    public DrawLineModel() {
        this.OnPolygonAdded += _ => NotifyDirty();
        this.OnPolygonUpdated += NotifyDirty;
        this.OnDataCleared += NotifyDirty;
    }

    /// <summary>
    /// 添加新的多边形
    /// </summary>
    public PolygonData AddNewPolygon()
    {
        var polygon = new PolygonData();
        polygons.Add(polygon);
        polygon.Color = new Color(UnityEngine.Random.value, UnityEngine.Random.value, UnityEngine.Random.value, 0.5f);
        OnPolygonAdded?.Invoke(polygon);
        return polygon;
    }

    /// <summary>
    /// 向当前多边形添加顶点
    /// </summary>
    public void AddVertexToCurrentPolygon(Vector3 vertex)
    {
        if (polygons.Count == 0)
            AddNewPolygon();
            
        CurrentPolygon.AddVertex(vertex);
        OnPolygonUpdated?.Invoke();
    }

    /// <summary>
    /// 完成当前多边形的绘制
    /// </summary>
    public void CompleteCurrentPolygon()
    {
        if (CurrentPolygon != null)
        {
            CurrentPolygon.SetCompleted(true);
            OnPolygonUpdated?.Invoke();
        }
    }

    public void AddPolygon(PolygonData polygon)
    {
        if(polygon.VertexCount > 2)
        {
            polygons.Add(polygon);
            polygon.SetCompleted(true);
            polygon.SortVerticesByAngle();
            polygons.Add(polygon);
            OnPolygonAdded?.Invoke(polygon);
        }
    }
    
    public void AddRectangle(Color color, Vector3 center, Vector3 size)
    {
        var polygon = new PolygonData();
        polygon.Color = color;
        Vector3 halfSize = size / 2;
        polygon.AddVertex(new Vector3(center.x - halfSize.x, center.y - halfSize.y, center.z));
        polygon.AddVertex(new Vector3(center.x + halfSize.x, center.y - halfSize.y, center.z));
        polygon.AddVertex(new Vector3(center.x + halfSize.x, center.y + halfSize.y, center.z));
        polygon.AddVertex(new Vector3(center.x - halfSize.x, center.y + halfSize.y, center.z));
        polygon.SetCompleted(true);
        polygons.Add(polygon);
        OnPolygonAdded?.Invoke(polygon);
    }

    public void AddLine(Color color, Vector3 start, Vector3 end)
    {
        var polygon = new PolygonData();
        polygon.Color = color;
        polygon.AddVertex(start);
        polygon.AddVertex(end);
        polygons.Add(polygon);
        OnPolygonAdded?.Invoke(polygon);
    }

    public void AddDirection(Color color, Vector3 point, Vector3 direction, float distance = 1000f)
    {
        var polygon = new PolygonData();
        polygon.Color = color;
        Vector3 dir = direction.normalized;
        Vector3 start = point - dir * distance;
        Vector3 end = point + dir * distance;
        AddLine(color, start, end);
    }

    /// <summary>
    /// 排序当前多边形的顶点（按角度）
    /// </summary>
    public void SortCurrentPolygonVertices()
    {
        CurrentPolygon?.SortVerticesByAngle();
        OnPolygonUpdated?.Invoke();
    }

    /// <summary>
    /// 获取指定索引的多边形
    /// </summary>
    public PolygonData GetPolygon(int index)
    {
        return index >= 0 && index < polygons.Count ? polygons[index] : null;
    }

    /// <summary>
    /// 移除指定多边形
    /// </summary>
    public void RemovePolygon(int index)
    {
        if (index >= 0 && index < polygons.Count)
        {
            polygons.RemoveAt(index);
            OnPolygonUpdated?.Invoke();
        }
    }

    /// <summary>
    /// 清除所有数据
    /// </summary>
    public void Clear()
    {
        polygons.Clear();
        OnDataCleared?.Invoke();
    }

    /// <summary>
    /// 导出所有多边形数据
    /// </summary>
    public List<List<Vector3>> ExportAllVertices()
    {
        return polygons.Select(p => p.GetVerticesCopy()).ToList();
    }
    
    private void NotifyDirty()
    {
        OnDataDirty?.Invoke();
    }
    
    public IEnumerable<PolygonData> GetPolygons()
    {
        return polygons;
    }
}

/// <summary>
/// 单个多边形数据
/// </summary>
[Serializable]
public class PolygonData
{
    private Vector3 center = Vector3.zero;
    private bool centerCached = false;
    private bool isCompleted = false;
    private List<Vector3> vertices = new();

    public IReadOnlyList<Vector3> Vertices => vertices.AsReadOnly();
    public int VertexCount => vertices.Count;
    public bool IsCompleted => isCompleted;
    public Color Color;

    public Vector3 Center
    {
        get
        {
            if (!centerCached)
            {
                CalculateCenter();
            }
            return center;
        }
    }

    public void AddVertex(Vector3 vertex)
    {
        vertices.Add(vertex);
        centerCached = false;
    }

    public void SetCompleted(bool completed)
    {
        isCompleted = completed;
    }

    public List<Vector3> GetVerticesCopy()
    {
        return new List<Vector3>(vertices);
    }

    public void SortVerticesByAngle()
    {
        if (vertices.Count < 3) return;
        
        CalculateCenter();
        vertices = vertices.OrderBy(v => 
            Mathf.Atan2(v.y - center.y, v.x - center.x)
        ).ToList();
    }

    private void CalculateCenter()
    {
        center = Vector3.zero;
        foreach (var v in vertices)
        {
            center += v;
        }
        if (vertices.Count > 0)
        {
            center /= vertices.Count;
        }
        centerCached = true;
    }

    /// <summary>
    /// 检查点是否接近第一个顶点
    /// </summary>
    public bool IsCloseToFirstVertex(Vector3 point, float threshold)
    {
        if (vertices.Count < 3) return false;
        return Vector3.Distance(point, vertices[0]) < threshold;
    }
}
}
    