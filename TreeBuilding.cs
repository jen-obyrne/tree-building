using System;
using System.Collections.Generic;
using System.Linq;

public class TreeBuildingRecord
{
    public int ParentId { get; set; }
    public int RecordId { get; set; }
}

public class Tree
{
    public int Id { get; set; }
    public int ParentId { get; set; }

    public List<Tree> Children { get; set; }

    public bool IsLeaf => Children.Count == 0;
}

public static class TreeBuilder
{
    public static Tree BuildTree(IEnumerable<TreeBuildingRecord> records)
    {
        var trees = AddTrees(records).Values.ToList();
        
        if (trees.Count == 0)
        {
            throw new ArgumentException();
        }

        var parentNode = trees.First(t => t.Id == 0);
        return parentNode;
    }

    private static Dictionary<int, Tree> AddTrees(IEnumerable<TreeBuildingRecord> records){ 
        var trees = new Dictionary<int, Tree>();

        var sortedRecords = records.OrderBy(x => x.RecordId);
        var previousRecordId = -1;
        
        foreach(var record in sortedRecords) 
        {
            var tree = new Tree
            {
                Id = record.RecordId,
                ParentId = record.ParentId,
                Children = new List<Tree>()
            };

            trees.Add(record.RecordId, tree);

            ValidateNode(tree, previousRecordId);
            
            previousRecordId ++;
        }

        AssignChildren(trees);

        return trees;
    }

    private static void AssignChildren(Dictionary<int, Tree> trees) 
    {
        foreach (var item in trees.Values) {
            Tree proposedParent;
            if (item.Id != 0 && trees.TryGetValue(item.ParentId, out proposedParent)) {
                proposedParent.Children.Add(item);
            }
        }
    }

    private static void ValidateNode(Tree tree, int previousRecordId)
    {
        if ((RootNodeConfiguredWrong(tree)) ||
                (NodeIdLargerThanParentId(tree)) ||
                (tree.Id != 0 && tree.Id != previousRecordId + 1))
            {
                throw new ArgumentException();
            }
    }

    private static bool RootNodeConfiguredWrong(Tree tree) 
    {
        return tree.Id == 0 && tree.ParentId != 0;
    }

    private static bool NodeIdLargerThanParentId(Tree tree) 
    {
        return tree.Id != 0 && tree.ParentId >= tree.Id;
    }
}