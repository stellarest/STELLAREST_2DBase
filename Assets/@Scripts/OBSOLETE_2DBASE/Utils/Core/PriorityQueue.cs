using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

namespace STELLAREST_2D
{
    // Search : O(1) <Dequeue>
    // Remove : O(logN) <Dequeue>
    // Same Check : O(N)
    // Insert : O(logN) <Enqueue>
    public class PriorityQueue<T> where T : IComparable<T>
    {
        // 나중에 배열로 바꾸는게 좋음
        public List<T> Data { get; private set; } = new List<T>();

        public void Enqueue(T item)
        {
            if (Data.Contains(item))
                return;

            Data.Add(item);
            BubbleUp();
        }

        private void BubbleUp()
        {
            int childIdx = this.Count - 1;
            while (childIdx > 0) // until child to root
            {
                int parentIdx = (childIdx - 1) / 2;
                if (Data[parentIdx].CompareTo(Data[childIdx]) < 0)
                    break;

                Swap(childIdx, parentIdx);
                childIdx = parentIdx;
            }
        }

        private void Swap(int idx1, int idx2)
        {
            var temp = Data[idx1];
            Data[idx1] = Data[idx2];
            Data[idx2] = temp;
        }

        public T Dequeue()
        {
            if (Count == 0)
            {
                Utils.LogStrong($"Failed to Dequeue. (Count : {Count})");
                return default(T);
            }

            T head = Data[0];
            MoveLastItemToTheTop(Count - 1);
            SinkDown();
            return head;
        }

        private void MoveLastItemToTheTop(int lastIdx)
        {
            Data[0] = Data[lastIdx];
            Data.RemoveAt(lastIdx);
        }

        private void SinkDown()
        {
            int lastIdx = Count - 1;
            int parentIdx = 0;
            while (true)
            {
                int leftChildIdx = (parentIdx * 2) + 1;
                if (leftChildIdx > lastIdx)
                    break;

                int rightChildIdx = leftChildIdx + 1;
                if (rightChildIdx <= lastIdx && Data[rightChildIdx].CompareTo(Data[leftChildIdx]) < 0) // left vs right
                    leftChildIdx = rightChildIdx;

                if (Data[parentIdx].CompareTo(Data[leftChildIdx]) < 0)
                    break;

                Swap(parentIdx, leftChildIdx);
                parentIdx = leftChildIdx;   
            }
        }

        public void Reposition(T item)
        {
            int idx = Data.IndexOf(item);
            while (idx > 0)
            {
                /*
                    CompareTo 결론 : 현재값이 작으면 음수
                    Debug.Log($"10.CompareTo(23) : {10.CompareTo(23)}"); // 현재값(10)이 전달된 값(23)보다 작으므로 음수가 나옴
                */
                int parentIdx = (idx - 1) / 2;

                // 자식의 값이 더 크면 종료
                if (Data[idx].CompareTo(Data[parentIdx]) >= 0)
                    break;

                Swap(idx, parentIdx);
                idx = parentIdx;
            }
        }

        public bool Contains(T item) => Data.Contains(item);
        public int Count => Data.Count;
        public T Head => (Data[0] != null) ? Data[0] : default(T);

        #if UNITY_EDITOR
        [Conditional("UNITY_EDITOR")]
        public void CheckValue()
        {
            if (Count > 0)
            {
                for (int i = 0; i < Count; ++i)
                    Utils.Log($"Data[{i}] : {Data[i]}");
            }
            else
                Utils.LogStrong("Failed to CheckValue. Empty Container Elements.");
        }
        #endif
    }
}
