using System;

namespace Microsoft.XmlDiffPatch
{
    internal class MinimalTreeDistanceAlgo
    {
        private XmlDiff _xmlDiff;

        private XmlDiffNode[] _sourceNodes;

        private XmlDiffNode[] _targetNodes;

        private MinimalTreeDistanceAlgo.Distance[,] _treeDist;

        private MinimalTreeDistanceAlgo.Distance[,] _forestDist;

        private readonly static EditScriptEmpty EmptyEditScript;

        internal readonly static int[] OperationCost;

        static MinimalTreeDistanceAlgo()
        {
            MinimalTreeDistanceAlgo.EmptyEditScript = new EditScriptEmpty();
            MinimalTreeDistanceAlgo.OperationCost =
                new int[] { 0, 4, 4, 1, 1, 2, 3, 2, 3, 4, 4, 4, 4, 4, 4, 1073741823 };
        }

        internal MinimalTreeDistanceAlgo(XmlDiff xmlDiff)
        {
            this._xmlDiff = xmlDiff;
        }

        private void ComputeTreeDistance(int sourcePos, int targetPos)
        {
            int i;
            int j;
            int left = this._sourceNodes[sourcePos].Left;
            int num = this._targetNodes[targetPos].Left;
            EditScriptAddOpened editScriptAddOpened =
                new EditScriptAddOpened(num, MinimalTreeDistanceAlgo.EmptyEditScript);
            EditScriptRemoveOpened editScriptRemoveOpened =
                new EditScriptRemoveOpened(left, MinimalTreeDistanceAlgo.EmptyEditScript);
            this._forestDist[left - 1, num - 1]._cost = 0;
            this._forestDist[left - 1, num - 1]._editScript = MinimalTreeDistanceAlgo.EmptyEditScript;
            for (i = left; i <= sourcePos; i++)
            {
                this._forestDist[i, num - 1]._cost = (i - left + 1) * MinimalTreeDistanceAlgo.OperationCost[2];
                this._forestDist[i, num - 1]._editScript = editScriptRemoveOpened;
            }

            for (j = num; j <= targetPos; j++)
            {
                this._forestDist[left - 1, j]._cost = (j - num + 1) * MinimalTreeDistanceAlgo.OperationCost[1];
                this._forestDist[left - 1, j]._editScript = editScriptAddOpened;
            }

            for (i = left; i <= sourcePos; i++)
            {
                for (j = num; j <= targetPos; j++)
                {
                    int left1 = this._sourceNodes[i].Left;
                    int num1 = this._targetNodes[j].Left;
                    int operationCost = this._forestDist[i - 1, j]._cost + MinimalTreeDistanceAlgo.OperationCost[2];
                    int operationCost1 = this._forestDist[i, j - 1]._cost + MinimalTreeDistanceAlgo.OperationCost[1];
                    if (left1 != left || num1 != num)
                    {
                        int num2 = left1 - 1;
                        int num3 = num1 - 1;
                        if (num2 < left - 1)
                        {
                            num2 = left - 1;
                        }

                        if (num3 < num - 1)
                        {
                            num3 = num - 1;
                        }

                        int num4 = this._forestDist[num2, num3]._cost + this._treeDist[i, j]._cost;
                        if (num4 >= operationCost1)
                        {
                            if (operationCost1 >= operationCost)
                            {
                                this.OpRemove(i, j, operationCost);
                            }
                            else
                            {
                                this.OpAdd(i, j, operationCost1);
                            }
                        }
                        else if (num4 >= operationCost)
                        {
                            this.OpRemove(i, j, operationCost);
                        }
                        else if (this._treeDist[i, j]._editScript != MinimalTreeDistanceAlgo.EmptyEditScript)
                        {
                            this.OpConcatScripts(i, j, num2, num3);
                        }
                        else
                        {
                            this.OpCopyScript(i, j, num2, num3);
                        }
                    }
                    else
                    {
                        XmlDiffOperation diffOperation =
                            this._sourceNodes[i].GetDiffOperation(this._targetNodes[j], this._xmlDiff);
                        if (diffOperation != XmlDiffOperation.Match)
                        {
                            int operationCost2 = this._forestDist[i - 1, j - 1]._cost +
                                                 MinimalTreeDistanceAlgo.OperationCost[(int)diffOperation];
                            if (operationCost2 < operationCost1)
                            {
                                if (operationCost2 >= operationCost)
                                {
                                    this.OpRemove(i, j, operationCost);
                                }
                                else
                                {
                                    this.OpChange(i, j, diffOperation, operationCost2);
                                }
                            }
                            else if (operationCost1 >= operationCost)
                            {
                                this.OpRemove(i, j, operationCost);
                            }
                            else
                            {
                                this.OpAdd(i, j, operationCost1);
                            }
                        }
                        else
                        {
                            this.OpNodesMatch(i, j);
                        }

                        this._treeDist[i, j]._cost = this._forestDist[i, j]._cost;
                        this._treeDist[i, j]._editScript = this._forestDist[i, j]._editScript.GetClosedScript(i, j);
                    }
                }
            }
        }

        internal EditScript FindMinimalDistance()
        {
            EditScript editScript = null;
            try
            {
                this._sourceNodes = this._xmlDiff._sourceNodes;
                this._targetNodes = this._xmlDiff._targetNodes;
                this._treeDist = new MinimalTreeDistanceAlgo.Distance[checked((uint)((int)this._sourceNodes.Length)),
                    checked((uint)((int)this._targetNodes.Length))];
                this._forestDist = new MinimalTreeDistanceAlgo.Distance[checked((uint)((int)this._sourceNodes.Length)),
                    checked((uint)((int)this._targetNodes.Length))];
                for (int i = 1; i < (int)this._sourceNodes.Length; i++)
                {
                    if (this._sourceNodes[i].IsKeyRoot)
                    {
                        for (int j = 1; j < (int)this._targetNodes.Length; j++)
                        {
                            if (this._targetNodes[j].IsKeyRoot)
                            {
                                this.ComputeTreeDistance(i, j);
                            }
                        }
                    }
                }

                editScript = this._treeDist[(int)this._sourceNodes.Length - 1, (int)this._targetNodes.Length - 1]
                    ._editScript;
            }
            finally
            {
                this._forestDist = null;
                this._treeDist = null;
                this._sourceNodes = null;
                this._targetNodes = null;
            }

            return MinimalTreeDistanceAlgo.NormalizeScript(editScript);
        }

        private static EditScript NormalizeScript(EditScript es)
        {
            EditScript editScript = es;
            EditScript editScript1 = es;
            EditScript editScript2 = null;
            while (editScript1 != MinimalTreeDistanceAlgo.EmptyEditScript)
            {
                if (editScript1.Operation == EditScriptOperation.EditScriptReference)
                {
                    EditScriptReference editScriptReference = editScript1 as EditScriptReference;
                    EditScript editScript3 = editScriptReference._editScriptReference;
                    while (editScript3.Next != MinimalTreeDistanceAlgo.EmptyEditScript)
                    {
                        editScript3 = editScript3._nextEditScript;
                    }

                    editScript3._nextEditScript = editScript1._nextEditScript;
                    editScript1 = editScriptReference._editScriptReference;
                    if (editScript2 != null)
                    {
                        editScript2._nextEditScript = editScript1;
                    }
                    else
                    {
                        editScript = editScript1;
                    }
                }
                else
                {
                    editScript2 = editScript1;
                    editScript1 = editScript1._nextEditScript;
                }
            }

            if (editScript2 == null)
            {
                editScript = null;
            }
            else
            {
                editScript2._nextEditScript = null;
            }

            return editScript;
        }

        private void OpAdd(int i, int j, int cost)
        {
            EditScriptAddOpened editScriptAddOpened = this._forestDist[i, j - 1]._editScript as EditScriptAddOpened ??
                                                      new EditScriptAddOpened(j,
                                                          this._forestDist[i, j - 1]._editScript
                                                              .GetClosedScript(i, j - 1));
            this._forestDist[i, j]._editScript = editScriptAddOpened;
            this._forestDist[i, j]._cost = cost;
        }

        private void OpChange(int i, int j, XmlDiffOperation changeOp, int cost)
        {
            this._forestDist[i, j]._editScript = new EditScriptChange(i, j, changeOp,
                this._forestDist[i - 1, j - 1]._editScript.GetClosedScript(i - 1, j - 1));
            this._forestDist[i, j]._cost = cost;
        }

        private void OpConcatScripts(int i, int j, int m, int n)
        {
            this._forestDist[i, j]._editScript = new EditScriptReference(this._treeDist[i, j]._editScript,
                this._forestDist[m, n]._editScript.GetClosedScript(m, n));
            this._forestDist[i, j]._cost = this._treeDist[i, j]._cost + this._forestDist[m, n]._cost;
        }

        private void OpCopyScript(int i, int j, int m, int n)
        {
            this._forestDist[i, j]._cost = this._forestDist[m, n]._cost;
            this._forestDist[i, j]._editScript = this._forestDist[m, n]._editScript.GetClosedScript(m, n);
        }

        private void OpNodesMatch(int i, int j)
        {
            EditScriptMatchOpened editScriptMatchOpened =
                this._forestDist[i - 1, j - 1]._editScript as EditScriptMatchOpened ?? new EditScriptMatchOpened(i, j,
                    this._forestDist[i - 1, j - 1]._editScript.GetClosedScript(i - 1, j - 1));
            this._forestDist[i, j]._editScript = editScriptMatchOpened;
            this._forestDist[i, j]._cost = this._forestDist[i - 1, j - 1]._cost;
        }

        private void OpRemove(int i, int j, int cost)
        {
            EditScriptRemoveOpened editScriptRemoveOpened =
                this._forestDist[i - 1, j]._editScript as EditScriptRemoveOpened ?? new EditScriptRemoveOpened(i,
                    this._forestDist[i - 1, j]._editScript.GetClosedScript(i - 1, j));
            this._forestDist[i, j]._editScript = editScriptRemoveOpened;
            this._forestDist[i, j]._cost = cost;
        }

        private struct Distance
        {
            internal int _cost;

            internal EditScript _editScript;
        }
    }
}