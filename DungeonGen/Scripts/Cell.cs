using System;
using System.Collections.Generic;
using UnityEngine;

namespace WFC
{
    public enum ECellDirection
    {
        North,
        South,
        East,
        West
    }
    public class Cell
    {
        public int x { private set; get; }
        public int y { private set; get; }
        Cell nTop;
        Cell nBottom;
        Cell nLeft;
        Cell nRight;

        public int EntropyValue { private set; get; }
        CollapseOption[] collaspeOptions;
        public CollapseCondition condition { private set; get; }
        public CollapseOption currentOption = null;
        public bool collapsed { private set; get; } = false;
        public GameObject CellObject { private set; get; }

        public Cell(int x, int y, CollapseOption[] options)
        {
            this.x = x;
            this.y = y;
            this.collaspeOptions = new CollapseOption[options.Length];
            for (int i = 0; i < collaspeOptions.Length; i++)
            {
                collaspeOptions[i] = options[i];
            }
            EntropyValue = options.Length;
        }
        public void SetNeighbours(Cell top, Cell bottom, Cell left, Cell right)
        {
            this.nTop = top;
            this.nBottom = bottom;
            this.nRight = right;
            this.nLeft = left;
        }
        public List<Cell> GetConnectedNeibhours(List<ECondition> conds)
        {

            List<Cell> neibhours = new List<Cell>();
            if (nTop != null && conds.Contains(condition.Top) && CollapseCondition.CheckFitCondition(condition.Top, nTop.condition.Bottom))
            {
                neibhours.Add(nTop);
            }
            if (nBottom != null && conds.Contains(condition.Bottom) && CollapseCondition.CheckFitCondition(condition.Bottom, nBottom.condition.Top))
            {
                neibhours.Add(nBottom);
            }
            if (nLeft != null && conds.Contains(condition.Left) && CollapseCondition.CheckFitCondition(condition.Left, nLeft.condition.Right))
            {
                neibhours.Add(nLeft);
            }
            if (nRight != null && conds.Contains(condition.Right) && CollapseCondition.CheckFitCondition(condition.Right, nRight.condition.Left))
            {
                neibhours.Add(nRight);
            }
            return neibhours;
        }

        public List<Cell> GetNeibhoursWithCondition(ECondition cond)
        {
            List<Cell> neibhours = new List<Cell>();
            if (nTop != null && condition.Top == cond && nTop.condition.Bottom == cond)
            {
                neibhours.Add(nTop);
            }
            if (nBottom != null && condition.Bottom == cond && nBottom.condition.Top == cond)
            {
                neibhours.Add(nBottom);
            }
            if (nLeft != null && condition.Left == cond && nLeft.condition.Right == cond)
            {
                neibhours.Add(nLeft);
            }
            if (nRight != null && condition.Right == cond && nRight.condition.Left == cond)
            {
                neibhours.Add(nRight);
            }
            return neibhours;
        }
        public void Collapse(Transform parent, CollapseOption[] options,bool createWorldObject)
        {
            this.collaspeOptions = options;
            Collapse(parent, createWorldObject);
        }
        public void Collapse(Transform parent,bool createWorldObject)
        {
            List<CollapseOption> properOptions = FindProperOptions(collaspeOptions, GetCollapseCondition());
            currentOption = properOptions[UnityEngine.Random.Range(0, properOptions.Count)];

            if (createWorldObject)
            {
                GameObject cellInstance = GameObject.Instantiate(currentOption.Prefab);
                cellInstance.transform.localPosition = new Vector3(x, 0, y);
                cellInstance.transform.localRotation = Quaternion.Euler(0, currentOption.RotatedAngle, 0);
                cellInstance.transform.SetParent(parent);
                cellInstance.name = currentOption.name+"_x"+x+"y"+y;
                CellObject = cellInstance;
            }
            condition = currentOption.Condition;
            collapsed = true;
            UpdateNeighbourEntropy();
        }


        private void UpdateNeighbourEntropy()
        {
            if (!this.nTop?.collapsed ?? false) this.nTop.UpdateEntropy();
            if (!this.nBottom?.collapsed ?? false) this.nBottom.UpdateEntropy();
            if (!this.nLeft?.collapsed ?? false) this.nLeft.UpdateEntropy();
            if (!this.nRight?.collapsed ?? false) this.nRight.UpdateEntropy();

            if (condition.Optionals == null) return;

            for (int i = 0; i < condition.Optionals.Length; i++)
            {
                switch (condition.Optionals[i].dir)
                {
                    case ECellDirection.North:
                        nTop?.RemoveOptions(condition.Optionals[i].condition);
                        break;
                    case ECellDirection.South:
                        nBottom?.RemoveOptions(condition.Optionals[i].condition);
                        break;
                    case ECellDirection.East:
                        nRight?.RemoveOptions(condition.Optionals[i].condition);
                        break;
                    case ECellDirection.West:
                        nLeft?.RemoveOptions(condition.Optionals[i].condition);
                        break;
                    default:
                        break;
                }
            }
        }
        public void RemoveOptions(ECondition condition)
        {
            List<CollapseOption> tempOptions = new List<CollapseOption>();
            foreach (var item in collaspeOptions)
            {
                bool topRequire = nTop?.condition?.Bottom == condition;
                bool bottomRequire = nBottom?.condition?.Top == condition;
                bool leftRequire = nLeft?.condition?.Right == condition;
                bool rightRequire = nRight?.condition?.Left == condition;

                bool neibhourRequire = topRequire || bottomRequire || leftRequire || rightRequire;


                if (neibhourRequire || !item.Condition.CheckAnySideCondition(condition))
                {
                    tempOptions.Add(item);
                }
            }
            this.collaspeOptions = tempOptions.ToArray();
            UpdateEntropy();
        }
        private void UpdateEntropy()
        {

            collaspeOptions = FindProperOptions(collaspeOptions, GetCollapseCondition()).ToArray();
            EntropyValue = collaspeOptions.Length;
        }
        public CollapseCondition GetCollapseCondition()
        {
            Enum top = CheckNeighbhourCondition(this.nTop, ECellDirection.North);
            Enum bottom = CheckNeighbhourCondition(this.nBottom, ECellDirection.South);
            Enum left = CheckNeighbhourCondition(this.nLeft, ECellDirection.West);
            Enum right = CheckNeighbhourCondition(this.nRight, ECellDirection.East);
            CollapseCondition cond = new CollapseCondition((ECondition)top, (ECondition)bottom, (ECondition)left, (ECondition)right);

            return cond;
        }
        public List<CollapseOption> FindProperOptions(CollapseOption[] options, CollapseCondition cond)
        {
            List<CollapseOption> tempOptions = new List<CollapseOption>();
            for (int i = 0; i < options.Length; i++)
            {
                if (options[i].Condition.CheckCondition(cond))
                {

                    tempOptions.Add(options[i]);
                }
            }
            return tempOptions;
        }
        Enum CheckNeighbhourCondition(Cell cell, ECellDirection dir)
        {
            Enum cellCondition = ECondition.Any;
            if (cell == null || cell.condition == null)
            {
                return ECondition.Any;
            }
            switch (dir)
            {
                case ECellDirection.North:
                    cellCondition = cell.currentOption.Condition.Bottom;
                    break;
                case ECellDirection.South:
                    cellCondition = cell.currentOption.Condition.Top;
                    break;
                case ECellDirection.East:
                    cellCondition = cell.currentOption.Condition.Left;
                    break;
                case ECellDirection.West:
                    cellCondition = cell.currentOption.Condition.Right;
                    break;
                default:
                    break;
            }
            return cellCondition;
        }

        public void Destroy()
        {
#if UNITY_EDITOR
            GameObject.DestroyImmediate(CellObject);
#else
        GameObject.Destroy(CellObject);
#endif
        }
    }
}