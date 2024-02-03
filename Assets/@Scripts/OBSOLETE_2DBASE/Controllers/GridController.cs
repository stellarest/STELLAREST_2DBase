using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace STELLAREST_2D
{
    public class Cell
    {
        public HashSet<GameObject> Gems { get; } = new HashSet<GameObject>();
    }

    public class GridController : BaseController
    {
        private Grid _grid = null;
        private Dictionary<Vector3Int, Cell> _dictGemsInTheCell = new Dictionary<Vector3Int, Cell>();

        private void Awake() => _grid = gameObject.GetOrAddComponent<Grid>();

        public void Add(Define.ObjectType type, GameObject go)
        {
            Vector3Int cellPos = _grid.WorldToCell(go.transform.position);
            Cell cell = GetCell(type, cellPos);

            if (type == Define.ObjectType.Gem)
                cell.Gems.Add(go);
        }

        private Cell GetCell(Define.ObjectType type, Vector3Int cellPos)
        {
            if (type == Define.ObjectType.Gem)
            {
                if (_dictGemsInTheCell.TryGetValue(cellPos, out Cell cell) == false)
                {
                    cell = new Cell();
                    _dictGemsInTheCell.Add(cellPos, cell);
                }

                return cell;
            }

            return null;
        }

        public void Remove(Define.ObjectType type, GameObject go)
        {
            Vector3Int cellPos = _grid.WorldToCell(go.transform.position);
            Cell cell = GetCell(type, cellPos);
            cell.Gems.Remove(go);
        }

        public List<GameObject> Gather(Define.ObjectType type, Vector3 from, float range)
        {
            List<GameObject> objects = new List<GameObject>();

            Vector3Int left = _grid.WorldToCell(from + new Vector3(-range, 0, 0));
            Vector3Int right = _grid.WorldToCell(from + new Vector3(range, 0, 0));
            Vector3Int bottom = _grid.WorldToCell(from + new Vector3(0, -range, 0));
            Vector3Int top = _grid.WorldToCell(from + new Vector3(0, range, 0));

            int minX = left.x;
            int maxX = right.x;
            int minY = bottom.y;
            int maxY = top.y;

            for (int x = minX; x <= maxX; ++x)
            {
                for (int y = minY; y <= maxY; ++y)
                {
                    if (type == Define.ObjectType.Gem)
                    {
                        if (_dictGemsInTheCell.ContainsKey(new Vector3Int(x, y, 0)) == false)
                            continue;

                        var hashObjs = _dictGemsInTheCell[new Vector3Int(x, y, 0)].Gems;
                        objects.AddRange(hashObjs);
                    }
                }
            }

            return objects;
        }
    }
}

// -------------------------------------------------------------------------------------
// -------------------------------------------------------------------------------------
// public void AddGemInTheCell(GameObject go)
// {
//     Vector3Int cellPos = _grid.WorldToCell(go.transform.position);
//     Cell cell = GetCellOfTheGem(cellPos); // Add to Dictionary
//     cell.Gems.Add(go);
// }

// private Cell GetCellOfTheGem(Vector3Int cellPos)
// {
//     Cell cell = null;
//     if (_dictGemsInTheCell.TryGetValue(cellPos, out cell) == false)
//     {
//         cell = new Cell();
//         _dictGemsInTheCell.Add(cellPos, cell);
//     }

//     return cell;
// }

// public List<GameObject> GatherGemsInTheCell(Vector3 from, float range)
//     {
//         List<GameObject> objects = new List<GameObject>();

//         Vector3Int left = _grid.WorldToCell(from + new Vector3(-range, 0, 0));
//         Vector3Int right = _grid.WorldToCell(from + new Vector3(range, 0, 0));
//         Vector3Int bottom = _grid.WorldToCell(from + new Vector3(0, -range, 0));
//         Vector3Int top = _grid.WorldToCell(from + new Vector3(0, range, 0));

//         int minX = left.x;
//         int maxX = right.x;
//         int minY = bottom.y;
//         int maxY = top.y;

//         for (int x = minX; x <= maxX; ++x)
//         {
//             for (int y = minY; y <= maxY; ++y)
//             {
//                 if (_dictGemsInTheCell.ContainsKey(new Vector3Int(x, y, 0)) == false)
//                     continue;

//                 var hashObjs = _dictGemsInTheCell[new Vector3Int(x, y, 0)].Gems;
//                 objects.AddRange(hashObjs);
//             }
//         }

//         return objects;
//     }
// }