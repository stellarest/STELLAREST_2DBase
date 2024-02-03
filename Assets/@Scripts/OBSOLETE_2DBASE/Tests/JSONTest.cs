using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace STELLAREST_2D.Test
{
    public class JSONTest : MonoBehaviour
    {
        [ContextMenu("JSON_TEST")]
        private void JSON_TEST()
        {
            // JObject
            // - key : string
            // - value : JToken(premitive, JObject, JArray 등등)

            // JArray
            // - value만 있음(JToken)

            /*
                {
                    "id": "Luna",
                    "name": "Silver",
                    "age": 19,
                    "blog": "devluna.blogspot.kr",
                    "Friends": [
                        {
                        "id": "Luna",
                        "name": "Silver",
                        "age": 19
                        },
                        {
                        "id": "SJ",
                        "name": "Philip",
                        "age": 25
                        }
                    ]
                }
            */

            // JObject jObj = new JObject();
            // jObj = JObject.FromObject(new {id = "Luna", name = "Silver", age = 19, blog = "devluna.blogspot.kr"});

            // JArray jArr = new JArray();
            // jArr.Add(JObject.Parse("{id : \"Luna\", name : \"Silver\", age : 19}"));
            // jArr.Add(JObject.Parse("{id : \"SJ\", name : \"Philip\", age : 25}"));

            // jObj.Add("Friends", jArr);
            // Debug.Log(jObj.ToString());
        }
    }
}
