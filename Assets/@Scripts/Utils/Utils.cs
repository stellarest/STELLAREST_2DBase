using System.Diagnostics;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

using Debug = UnityEngine.Debug;
using STELLAREST_2D.Data;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Xml.Schema;
using Assets.HeroEditor.InventorySystem.Scripts.Data;
using UnityEditor.ShaderGraph;
using UnityEngine.UIElements;
//using Unity.Mathematics;

public class ShowOnlyAttribute : PropertyAttribute
{
}

namespace STELLAREST_2D
{
    public class Utils
    {
        public static T GetOrAddComponent<T>(GameObject go) where T : UnityEngine.Component
        {
            T component = go.GetComponent<T>();
            if (component == null)
                component = go.AddComponent<T>();

            return component;
        }

        public static GameObject FindChild(GameObject go, string name = null, bool recursive = false)
        {
            Transform tr = FindChild<Transform>(go, name, recursive);
            if (tr == null)
                return null;

            return tr.gameObject;
        }

        public static T FindChild<T>(GameObject go, string name = null, bool recursive = false) where T : UnityEngine.Object
        {
            if (go == null)
                return null;

            if (recursive == false)
            {
                for (int i = 0; i < go.transform.childCount; ++i)
                {
                    Transform tr = go.transform.GetChild(i);
                    if (string.IsNullOrEmpty(name) || tr.name == name)
                    {
                        T comp = tr.GetComponent<T>();
                        if (comp != null)
                            return comp;
                    }
                }
            }
            else
            {
                foreach (T comp in go.GetComponentsInChildren<T>())
                {
                    if (string.IsNullOrEmpty(name) || comp.name == name)
                        return comp;
                }
            }

            return null;
        }

        public static Vector3 GetRandomPosition(Vector3 fromPos, float fromMinDistance = 10.0f, float fromMaxDistance = 20.0f)
        {
            int maxAttempts = 100;
            int attempts = 0;

            while (attempts < maxAttempts)
            {
                float angle = Random.Range(0, 360) * Mathf.Deg2Rad;
                float distance = Random.Range(fromMinDistance, fromMaxDistance);

                float xDist = Mathf.Cos(angle) * distance;
                float yDist = Mathf.Sin(angle) * distance;

                Vector3 spawnPosition = fromPos + new Vector3(xDist, yDist, 0f);
                if (spawnPosition.x > Managers.Stage.MinimumPosition.x && spawnPosition.x < Managers.Stage.MaximumPosition.x)
                {
                    if (spawnPosition.y > Managers.Stage.MinimumPosition.y && spawnPosition.y < Managers.Stage.MaximumPosition.y)
                        return spawnPosition;
                }

                ++attempts;
            }

            return fromPos;
        }

        public static bool IsArriveToTarget(Transform from, Transform target, float minDistance = 1f)
        {
            if (Managers.Stage.IsOutOfPos(from.position))
                return true;

            if ((target.transform.position - from.position).sqrMagnitude < minDistance)
                return true;

            return false;
        }

        public static bool IsArriveToTarget(Transform from, Vector3 toTargetPoint, float minDistance = 1f)
        {
            if (Managers.Stage.IsOutOfPos(from.position))
                return true;

            if ((toTargetPoint - from.position).sqrMagnitude < minDistance)
                return true;

            return false;
        }

        public static Vector3 GetRandomTargetPosition<T>(Vector3 fromPos, float fromMinDistance, float fromMaxDistance,
                                                Define.HitFromType hitFromType = Define.HitFromType.None) where T : BaseController
        {
            Vector3 randomTargetPos = Vector3.zero;
            System.Type type = typeof(T);
            if (type == typeof(MonsterController))
            {
                List<MonsterController> toList = MakeToList_Monsters(hitFromType);
                if (toList.Count > 0)
                {
                    List<MonsterController> filteredList = new List<MonsterController>();
                    for (int i = 0; i < toList.Count; ++i)
                    {
                        if (toList[i].IsValid() == false)
                            continue;

                        float fromDist = (toList[i].transform.position - fromPos).sqrMagnitude;
                        if (fromDist > (fromMinDistance * fromMinDistance) && fromDist < (fromMaxDistance * fromMaxDistance))
                            filteredList.Add(toList[i]);
                    }

                    if (filteredList.Count > 0)
                    {
                        int randomIdx = UnityEngine.Random.Range(0, filteredList.Count);
                        randomTargetPos = filteredList[randomIdx].transform.position;
                    }
                }
            }

            return randomTargetPos;
        }

        public static CreatureController GetClosestCreatureTargetFromAndRange<T>(GameObject from, CreatureController owner, float searchMaxRange)
        {
            CreatureController target = null;
            System.Type type = typeof(T);
            if (owner?.IsPlayer() == true)
            {
                List<MonsterController> toList = MakeToList_Monsters();
                Vector3 fromPos = from.transform.position;
                float closestDist = float.MaxValue;
                for (int i = 0; i < toList.Count; ++i)
                {
                    if (toList[i] == from)
                        continue;

                    if (toList[i].IsValid() == false)
                        continue;

                    float distFrom = (toList[i].Center.position - fromPos).sqrMagnitude;
                    if (distFrom < searchMaxRange * searchMaxRange)
                    {
                        if (distFrom < closestDist)
                        {
                            closestDist = distFrom;
                            target = toList[i];
                        }
                    }
                }
            }

            return target;
        }

        public static GameObject GetClosestTargetFromAndRange_TEMP<T>(GameObject from, float range)
        {
            GameObject target = null;
            System.Type type = typeof(T);
            if (type == typeof(MonsterController))
            {
                List<MonsterController> toList = MakeToList_Monsters();
                Vector3 fromPos = from.transform.position;
                float closestDist = float.MaxValue;
                for (int i = 0; i < toList.Count; ++i)
                {
                    if (toList[i] == from)
                        continue;

                    if (toList[i].IsValid() == false)
                        continue;

                    float distFrom = (toList[i].Center.position - fromPos).sqrMagnitude;
                    if (distFrom < range * range && distFrom < closestDist)
                    {
                        closestDist = distFrom;
                        target = toList[i].gameObject;
                        //target = toList[i].AnimTransform.gameObject;
                    }
                }
            }

            return target;
        }

        public static Vector3 GetClosestFromTargetDirection<T>(CreatureController from, Define.HitFromType hitFromType = Define.HitFromType.None) where T : BaseController
        {
            Vector3 nextDir = Vector3.zero;
            System.Type type = typeof(T);
            if (type == typeof(MonsterController))
            {
                List<MonsterController> toList = MakeToList_Monsters(hitFromType);
                Vector3 fromPos = from.Center.position;
                float closestDist = float.MaxValue;
                for (int i = 0; i < toList.Count; ++i)
                {
                    if (toList[i] == from)
                        continue;

                    if (toList[i].IsValid() == false)
                        continue;

                    float nextDist = (toList[i].Center.position - fromPos).sqrMagnitude;
                    if (nextDist < closestDist)
                    {
                        closestDist = nextDist;
                        nextDir = (toList[i].Center.position - fromPos);
                    }
                }
            }

            return nextDir.normalized;
        }

        private static List<MonsterController> MakeToList_Monsters(Define.HitFromType hitFromType = Define.HitFromType.None)
        {
            List<MonsterController> toList = new List<MonsterController>();
            foreach (var mon in Managers.Object.Monsters)
            {
                if (hitFromType != Define.HitFromType.None)
                {
                    switch (hitFromType)
                    {
                        case Define.HitFromType.ThrowingStar:
                            if (mon.IsHitFrom_ThrowingStar)
                                continue;
                            break;

                        case Define.HitFromType.LazerBolt:
                            if (mon.IsHitFrom_LazerBolt)
                                continue;
                            break;
                    }
                }

                if (mon.IsValid())
                    toList.Add(mon);
            }

            return toList;
        }

#if UNITY_EDITOR
        [Conditional("UNITY_EDITOR")]
        public static void Log(object message)
            => Debug.Log($"<color=white>{message.ToString()}</color>");

        [Conditional("UNITY_EDITOR")]
        public static void Log(object calledByMethodOnly, object message = null)
        {
            if (message == null)
                message = $"{calledByMethodOnly}";
            else
                Utils.Log($"By : {calledByMethodOnly}, {message}");
        }

        [Conditional("UNITY_EDITOR")]
        public static void Log(object called, object byMethod, object target, object message, bool clearPrevLogs = false)
        {
            if (clearPrevLogs)
                ClearLog();

            Utils.Log(" ↓ [         LOG         ] ↓");
            Utils.Log($"Called : {called}");
            Utils.Log($"By : {byMethod}");
            if (target != null)
            {
                target = $"<color=cyan>[{target}]</color>";
                message = $"{target} : {message}";
                Utils.Log(message);
            }
            else
                Utils.Log(message);
            Utils.Log(" ↑ [         LOG         ] ↑");
        }

        [Conditional("UNITY_EDITOR")]
        public static void LogBreak(object message)
        {
            Utils.Log("[\n");
            Utils.Log(message);
            Utils.Log("\n                   ] : Paused");
            Debug.Break();
        }

        [Conditional("UNITY_EDITOR")]
        public static void LogBreak(object calledByMethodOnly, object message)
        {
            Utils.Log("[\n");
            Utils.Log(message);
            Utils.Log($"\n                   ] : Paused by {calledByMethodOnly}");
            Debug.Break();
        }

        [Conditional("UNITY_EDITOR")]
        public static void LogStrong(object calledByMethodOnly, object message = null)
        {
            Debug.Log($"<color=yellow> ↓ [   LOG STRONG   ] ↓</color>");
            if (message == null)
                message = $"{calledByMethodOnly}";
            else
                message = $"{calledByMethodOnly}, {message}";

            Debug.LogWarning($"<color=yellow>{message}</color>");
            Debug.Log($"<color=yellow> ↑ [   LOG STRONG   ] ↑</color>");
        }

        [Conditional("UNITY_EDITOR")]
        public static void LogStrong(object calledBy, object calledByMethod, object message = null)
        {
            Debug.Log($"<color=yellow> ↓ [   LOG STRONG   ] ↓</color>");
            if (message == null)
                message = $"{calledBy}, {calledByMethod}";
            else
                message = $"{calledBy}, {calledByMethod}, {message}";

            Debug.LogWarning($"<color=yellow>{message}</color>");
            Debug.Log($"<color=yellow> ↑ [   LOG STRONG   ] ↑</color>");
        }

        [Conditional("UNITY_EDITOR")]
        public static void LogCritical(object calledBy, object calledByMethod, object message = null)
        {
            Debug.Log($"<color=red> ↓↓↓ [   LOG CRITICAL   ] ↓↓↓</color>");

            Debug.LogError($"<color=red>Called : {calledBy}</color>");
            Debug.LogError($"<color=red>By : {calledByMethod}</color>");
            if (message != null)
                Debug.LogError($"<color=red>{message}</color>");

            Debug.Log($"<color=red> ↑↑↑ [   LOG CRITICAL   ] ↑↑↑</color>");
            Debug.Break();
        }

        [Conditional("UNITY_EDITOR")]
        public static void ClearLog()
        {
            var assembly = Assembly.GetAssembly(typeof(UnityEditor.Editor));
            var type = assembly.GetType("UnityEditor.LogEntries");
            var method = type.GetMethod("Clear");
            method.Invoke(new object(), null);
            Debug.Log("##### [[[ Debug Message ]]] #####");
        }
#endif
    }
}
