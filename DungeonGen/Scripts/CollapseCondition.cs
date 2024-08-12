using System;


namespace WFC
{
    [System.Flags]
    public enum ECondition
    {
        Wall = 2,
        Pass = 4,
        RoomL = 8,
        RoomR = 16,
        Room = 32,
        Any = ~0,
    }

    [System.Serializable]
    public class CollapseCondition
    {
        public ECondition Top;
        public ECondition Bottom;
        public ECondition Left;
        public ECondition Right;
        public CollapseConditionOptional[] Optionals;
        public CollapseCondition(ECondition top, ECondition bottom, ECondition left, ECondition right)
        {
            Top = top;
            Bottom = bottom;
            Left = left;
            Right = right;
        }
        public bool CheckCondition(CollapseCondition other)
        {
            bool tempTop = (CompareMask(Top, other.Top) || this.Top == ECondition.Any || other.Top == ECondition.Any);
            bool tempBottom = (CompareMask(Bottom, other.Bottom) || this.Bottom == ECondition.Any || other.Bottom == ECondition.Any);
            bool tempLeft = (CompareMask(Left, other.Left) || this.Left == ECondition.Any || other.Left == ECondition.Any);
            bool tempRight = (CompareMask(Right, other.Right) || this.Right == ECondition.Any || other.Right == ECondition.Any);

            return tempTop && tempBottom && tempLeft && tempRight;
        }
        static bool CompareMask(Enum a, Enum b)
        {

            ConditionsConfig config = DungeonManager.Instance.conditionConfig;

            int aMask = config.GetMask((ECondition)a);
            ECondition aEnum = (ECondition)aMask;
            int bMask = config.GetMask((ECondition)b);
            ECondition bEnum = (ECondition)bMask;

            bool result = aEnum.HasFlag(b);
            bool reverse = bEnum.HasFlag(a);
            return result || reverse;
        }
        public bool CheckAnySideCondition(ECondition condition)
        {
            bool haveCondition = Top == condition || Bottom == condition || Left == condition || Right == condition;
            return haveCondition;
        }
        public void RotateOptionals()
        {
            if (Optionals == null) return;
            for (int i = 0; i < Optionals.Length; i++)
            {
                switch (Optionals[i].dir)
                {
                    case ECellDirection.North:
                        Optionals[i].dir = ECellDirection.East;
                        break;
                    case ECellDirection.South:
                        Optionals[i].dir = ECellDirection.West;
                        break;
                    case ECellDirection.East:
                        Optionals[i].dir = ECellDirection.South;
                        break;
                    case ECellDirection.West:
                        Optionals[i].dir = ECellDirection.North;
                        break;
                    default:
                        break;
                }
            }
        }
        public CollapseConditionOptional[] GetOptionals()
        {
            CollapseConditionOptional[] temp = new CollapseConditionOptional[Optionals.Length];
            for (int i = 0; i < temp.Length; i++)
            {
                temp[i] = new CollapseConditionOptional();
                temp[i].condition = Optionals[i].condition;
                temp[i].dir = Optionals[i].dir;
            }
            return temp;
        }
        public static bool CheckFitCondition(ECondition a, ECondition b)
        {
            return CompareMask(a, b);

        }
        public int GetConditionAmount(ECondition cond)
        {
            int amount = 0;
            if (CheckFitCondition(Top, cond)) amount++;
            if (CheckFitCondition(Bottom, cond)) amount++;
            if (CheckFitCondition(Left, cond)) amount++;
            if (CheckFitCondition(Right, cond)) amount++;
            return amount;
        }
    }
}