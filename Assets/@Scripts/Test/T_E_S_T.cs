using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

namespace STELLAREST_SFH
{
    public class T_E_S_T : MonoBehaviour
    {
        private void Start()
        {
            // int[] arr = { 7, 4, 5, 1, 3 };
            // BubbleSort(arr);
            // Print(arr);
        }

        private void Print(int[] arr)
        {
            Util.Log("T_E_S_T : PRINT BUBBLE");
            for (int i = 0; i < arr.Length; ++i)
                Util.Log($"{arr[i]}");
        }

        private void BubbleSort(int[] arr)
        {
            int n = arr.Length;
            for (int i = 0; i < n - 1; ++i)
            {
                for (int j = 0; j < n - i - 1; ++j)
                {
                    if (arr[j] > arr[j + 1])
                    {
                        int temp = arr[j];
                        arr[j] = arr[j + 1];
                        arr[j + 1] = temp;
                    }
                }
            }
        }
    }
}
