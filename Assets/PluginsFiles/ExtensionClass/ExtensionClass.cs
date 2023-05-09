using UnityEngine;
using System.Collections;
using GoogleMobileAds.Api;
using System;
using System.Collections.Generic;
// using Facebook.Unity;
using Facebook.MiniJSON;


public static partial class ExtensionClass
{
    public static T Get<T, T1>(this Dictionary<string, T1> h, string key, bool useDefaultIfNotExist = true)
    {
        if (h == null)
        {
            return default(T);
        }

        if (!h.ContainsKey(key))
        {
            Debug.LogWarning("Can't find key <" + key + ">");
            return default(T);
        }

        object value = h[key];
        if (value == null) return default(T);

        Type valueT = value.GetType();

        if (valueT == typeof(T)) return (T)value;

        Debug.LogWarning("Invalid type");

        return default(T);
    }

    public static T Get<T>(this Dictionary<string, object> h, string key, bool useDefaultIfNotExist = true)
    {
        return Get<T, object>(h, key, useDefaultIfNotExist);
    }

    public static bool ToBool(this int value)
    {
        if (value == 1) { return true; }

        return false;
    }

    public static bool ToBool(this string value)
    {
        if (value == "true") { return true; }

        return false;
    }
}


