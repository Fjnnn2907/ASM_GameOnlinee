using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Obsever : MonoBehaviour
{
    public static Dictionary<string, List<Action>> listen = new();

    public static void AddObsever(string _name, Action _callback)
    {
        if (!listen.ContainsKey(_name))
        {
            listen.Add(_name, new List<Action>());
        }

        listen[_name].Add(_callback);
    }

    public static void RemoveObsever(string _name, Action _callback)
    {
        if (!listen.ContainsKey(_name))
            return;

        listen[_name].Remove(_callback);

    }

    public static void Notify(string _name)
    {
        if (!listen.ContainsKey(_name))
            return;

        foreach (var item in listen[_name].ToList())
        {
            try
            {
                item?.Invoke();
            }
            catch (Exception e)
            {
                Debug.Log(e.Message);
            }
        }
    }
}