using System;
using System.Collections;
using System.Reflection;
using UnityEngine;

namespace Script.Tools
{
    public class PlayerPrefsDataManager
    {
        private PlayerPrefsDataManager(){}

        private static PlayerPrefsDataManager instance =  new PlayerPrefsDataManager();
        
        public static PlayerPrefsDataManager Instance => instance;

        #region 存储简单数据类型

        public void SaveData(string key, int value)
        {
            PlayerPrefs.SetInt(key,value);
            PlayerPrefs.Save();
        }
        public void SaveData(string key, float value)
        {
            PlayerPrefs.SetFloat(key,value);
            PlayerPrefs.Save();
        }
        public void SaveData(string key, bool value)
        {
            PlayerPrefs.SetInt(key,value ? 1 : 0);
            PlayerPrefs.Save();
        }
        public void SaveData(string key, string value)
        {
            PlayerPrefs.SetString(key,value);
            PlayerPrefs.Save();
        }

        #endregion
        
        #region 存储自定义类型
        public void SaveData<T> (string key, T data)
        {
            if (data == null)
            {
                Debug.LogError("SaveData<T>: data is null");
                return;
            }
            
            Type type = typeof(T);
            FieldInfo[] fields = type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            string saveKey;//存储的键
            
            foreach (FieldInfo field in fields)
            {
                saveKey = $"{key}_{type.Name}_{field.FieldType.Name}_{field.Name}";
                SaveValue(saveKey, field.GetValue(data));
            }
            
            PlayerPrefs.Save();
        }
        private void SaveValue(string key, object value)
        {
            Type type = value.GetType();
            
            //根据不同类型存储
            if (type == typeof(int))
            {
                PlayerPrefs.SetInt(key, (int) value);
#if UNITY_EDITOR
                Debug.Log($"{key}数据存储成功");
#endif
            }
            else if (type == typeof(float))
            {
                PlayerPrefs.SetFloat(key, (float) value);
#if UNITY_EDITOR
                Debug.Log($"{key}数据存储成功");
#endif
            }
            else if (type == typeof(bool))
            {
                PlayerPrefs.SetInt(key, (bool) value ? 1 : 0);
#if UNITY_EDITOR
                Debug.Log($"{key}数据存储成功");
#endif
            }
            else if (type == typeof(string))
            {
                PlayerPrefs.SetString(key, (string) value);
#if UNITY_EDITOR
                Debug.Log($"{key}数据存储成功");
#endif
            }
            //IsAssignable检查当前类型是否可以接受传入类型的实例
            else if (typeof(IList).IsAssignableFrom(type))//判断父子关系
            {
                IList list = value as IList;
                if (list == null)
                {
                    NullReferenceException e = new NullReferenceException("List is null");
                    throw e;
                }
                PlayerPrefs.SetInt(key, list.Count);
                for (int i = 0; i < list.Count; i++)
                {
                    SaveValue(key + "_" + i, list[i]);
                }
#if UNITY_EDITOR
                Debug.Log($"{key}数据存储成功");
#endif
            }
            else if (typeof(IDictionary).IsAssignableFrom(type))//判断父子关系
            {
                IDictionary dictionary = (IDictionary)value;
                PlayerPrefs.SetInt(key, dictionary.Count);
                int index = 0;
                foreach (object k in dictionary.Keys)
                {
                    SaveValue(key + "_key_" + index, k);
                    SaveValue(key + "_value_" + index, dictionary[k]);
                    index++;
                }
#if UNITY_EDITOR
                Debug.Log($"{key}数据存储成功");
#endif
            }
            else//自定义类型存储
            {
                MethodInfo saveMethod = typeof(PlayerPrefsDataManager).GetMethod("SaveData");
                MethodInfo genericSaveMethod = saveMethod?.MakeGenericMethod(type);
                if (genericSaveMethod == null)
                {
                    #if UNITY_EDITOR
                        Debug.LogError($"genericSaveMethod is null.未找到泛型方法");
                    #endif
                    return;
                }
                genericSaveMethod.Invoke(this, new object[] { key, value });
            }
        }
        
        #endregion
        
        #region 加载数据
         public T LoadData<T>(string key)
        {
            Type type = typeof(T);
            
            return (T)LoadDataInstance(type, key);
        }
         
        private object LoadDataInstance(Type type, string key)
        {
            if (type == typeof(int))
            {
#if UNITY_EDITOR
                Debug.Log($"{key}数据加载成功");
#endif
                return PlayerPrefs.GetInt(key);
            }
            else if (type == typeof(float))
            {
#if UNITY_EDITOR
                Debug.Log($"{key}数据加载成功");
#endif
                return PlayerPrefs.GetFloat(key);
            }
            else if (type == typeof(bool))
            {
#if UNITY_EDITOR
                Debug.Log($"{key}数据加载成功");
#endif
                return PlayerPrefs.GetInt(key) == 1;
            }
            else if (type == typeof(string))
            {
#if UNITY_EDITOR
                Debug.Log($"{key}数据加载成功");
#endif
                return PlayerPrefs.GetString(key);
            }
            //GetGetGenericArguments获取泛型参数类型
            else if (typeof(IList).IsAssignableFrom(type))
            {
                int count = PlayerPrefs.GetInt(key);
                IList list = Activator.CreateInstance(type) as IList;
                if (list == null)
                {
                    throw new InvalidOperationException($"无法创建 IList 实例。类型 {type.Name} 必须是具体类且有无参构造函数");
                }
                Type[] types = type.GetGenericArguments();
                for (int i = 0; i < count; i++)
                {
                    list.Add(LoadDataInstance(types[0], key + "_" + i));
                }
#if UNITY_EDITOR
                Debug.Log($"{key}数据加载成功");
#endif
                return list;
            }
            else if (typeof(IDictionary).IsAssignableFrom(type))
            {
                int count = PlayerPrefs.GetInt(key);
                IDictionary dict = Activator.CreateInstance(type) as IDictionary;
                if (dict == null)
                {
                    throw new InvalidOperationException($"无法创建 IDictionary 实例。类型 {type.Name} 必须是具体类且有无参构造函数");
                }
                Type[] types = type.GetGenericArguments();//Dictionary<string,int>,所以types[0]=string,types[1]=int.
                for (int i = 0; i < count; i++)
                {
                    dict.Add(LoadDataInstance(types[0], key + "_key_" + i), LoadDataInstance(types[1], key + "_value_" + i));
                }
#if UNITY_EDITOR
                Debug.Log($"{key}数据加载成功");
#endif
                return dict;
            }
            else
            {
                object ins = Activator.CreateInstance(type);
                
                FieldInfo[] fields = type.GetFields();
                string loadKey;
                foreach (FieldInfo fieldInfo in fields)
                {
                    loadKey = key + "_" + type.Name + "_" + fieldInfo.FieldType.Name + "_" + fieldInfo.Name;
                    fieldInfo.SetValue(ins, LoadDataInstance(fieldInfo.FieldType, loadKey));
                }
#if UNITY_EDITOR
                Debug.Log($"{key}数据加载成功");
#endif
                return ins;
            }
        }
        #endregion
    }
}