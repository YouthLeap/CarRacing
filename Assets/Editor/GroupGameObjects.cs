using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class GroupGameObjects:MonoBehaviour
{
    private static Vector3 Vector3Max = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
    private static Vector3 Vector3Min = new Vector3(float.MinValue, float.MinValue, float.MinValue);
    [MenuItem("CZQ/Group打包(中心) %g")]//crtl+g
    public static void Run()
    {
        Object[] objs = Selection.objects;
        List<Transform> list = new List<Transform>();
        
        foreach (GameObject obj1 in objs)
        {
            bool add = true;
            foreach (GameObject obj2 in objs)
            {
                if (obj1.transform.parent == obj2.transform)
                {
                    add = false; 
                    break;
                }
            }
            if (add == true)
                list.Add(obj1.transform);
        }
        Vector3 Min=Vector3Max;
        Vector3 Max = Vector3Min;
        foreach (Transform tf in list)
        {
            GetMinMax(tf, ref Min,ref Max);
        }

        if (Max == Vector3Max)
        {
            Debug.LogError("没有Meshes");
            return;
        }
        
        GameObject group=new GameObject();
        group.name = "Group";
        group.transform.position = (Min + Max) / 2;

        foreach (Transform tf in list)
            tf.parent = group.transform;

         Debug.LogWarning("GroupGameObjects finished");
    }

    static void GetMinMax(Transform parent, ref Vector3 vector3Min,ref Vector3 vector3Max)
    {
        Mesh mesh = null;
        if (parent.GetComponent<MeshFilter>() != null && parent.GetComponent<MeshFilter>().sharedMesh != null)
            mesh = parent.GetComponent<MeshFilter>().sharedMesh;
        
        
        
        if(mesh!=null)
        {
            List<Vector3>vectors=GetBoundsVectors(mesh.bounds);
            foreach(Vector3 vector in vectors)
            {
                Vector3 v = parent.localToWorldMatrix * vector;
                v = v + parent.position;
                vector3Min = new Vector3(Mathf.Min(v.x, vector3Min.x), Mathf.Min(v.y, vector3Min.y), Mathf.Min(v.z, vector3Min.z));
                vector3Max = new Vector3(Mathf.Max(v.x, vector3Max.x), Mathf.Max(v.y, vector3Max.y), Mathf.Max(v.z, vector3Max.z));
            }
        }
        if (parent.childCount == 0)
            return;
        for(int i=0;i<parent.childCount;i++)
        {
            GetMinMax(parent.GetChild(i), ref vector3Min, ref vector3Max);
        }
    }

    static List<Vector3> GetBoundsVectors(Bounds bounds)
    {
        List<Vector3> list = new List<Vector3>();
        Vector3 min = bounds.min;
        Vector3 max = bounds.max;
        list.Add(new Vector3(min.x, min.y,min.z));
        list.Add(new Vector3(min.x, min.y, max.z));
        list.Add(new Vector3(min.x, max.y, min.z));
        list.Add(new Vector3(min.x, max.y, max.z));

        list.Add(new Vector3(max.x, min.y, min.z));
        list.Add(new Vector3(max.x, min.y, max.z));
        list.Add(new Vector3(max.x, max.y, min.z));
        list.Add(new Vector3(max.x, max.y, max.z));

        return list;
    }



    [MenuItem("CZQ/Group打包(非中心) &g")]//alt+g
    public static void Run2()
    {
        Object[] objs = Selection.objects;
        List<Transform> list = new List<Transform>();

        foreach (GameObject obj1 in objs)
        {
            bool add = true;
            foreach (GameObject obj2 in objs)
            {
                if (obj1.transform.parent == obj2.transform)
                {
                    add = false;
                    break;
                }
            }
            if (add == true)
                list.Add(obj1.transform);
        }
        

        GameObject group = new GameObject();
        group.name = "Group";
        

        foreach (Transform tf in list)
            tf.parent = group.transform;

        Debug.LogWarning("GroupGameObjects finished");
    }

    [MenuItem("CZQ/Group打包(再次调整中心) %&g")]//crtl+alt+g
    public static void Run3()
    {
        Object[] objs = Selection.objects;

        GameObject group = objs[0] as GameObject;

        List<Transform> children = new List<Transform>();
        for (int i = 0; i < group.transform.childCount; i++)
            children.Add(group.transform.GetChild(i));

        Vector3 Min = Vector3Max;
        Vector3 Max = Vector3Min;
        GetMinMax(group.transform, ref Min, ref Max);
        if (Max == Vector3Max)
        {
            Debug.LogError("没有Meshes");
            return;
        }


        group.transform.DetachChildren();

        group.transform.position = (Min + Max) / 2;

        foreach (Transform tf in children)
                tf.parent = group.transform;

        Debug.LogWarning("GroupGameObjects finished");
    }

    
    

}
