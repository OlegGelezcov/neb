// ObjectCache.cs
// Nebula
//
// Created by Oleg Zheleztsov on Thursday, September 10, 2015 10:14:09 PM
// Copyright (c) 2015 KomarGames. All rights reserved.
//
namespace Nebula.Resources {

    using System.Collections.Generic;
    using UnityEngine;

    public static class ObjectCache<T> where T : Object {

        private readonly static Dictionary<string, T> objectCache = new Dictionary<string, T>();

        public static T GetObject(string path) {
            if (objectCache.ContainsKey(path)) {
                return objectCache[path];
            } else {
                try {
                    T obj = Resources.Load<T>(path);
                    if (((object)obj) != null) {
                        objectCache.Add(path, obj);
                    }
                    return obj;
                } catch (System.Exception exception) {
                    Debug.Log("Cast at path: " + path);
                    object sourceObj = Resources.Load(path);
                    if (sourceObj != null) {
                        Debug.Log("source object type is: " + sourceObj.GetType().FullName);
                    }
                    Debug.LogError(exception.Message);
                    return default(T);
                }
            }
        }


    }

}