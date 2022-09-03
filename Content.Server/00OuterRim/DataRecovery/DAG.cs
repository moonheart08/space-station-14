using System.Linq;
using Robust.Shared.Utility;

namespace Content.Server._00OuterRim.DataRecovery;

/// <summary>
/// A Directed Acyclic Graph, which is guaranteed by the API
/// </summary>
/// <typeparam name="T">The IDagNode implementor</typeparam>
[DataDefinition]
public sealed class Dag<T>
{
    /// <summary>
    /// Used solely for deserialization.
    /// </summary>
    [DataField("nodes")] private SerializedDagNode<T>[]? _initialNodes = default!;

    private HashSet<DagNode<T>> _nodes = new();

    [DataDefinition]
    private sealed class SerializedDagNode<T2> : DagNode<T2>
    {
        [DataField("id", required: true)] public string Id = default!;

        [DataField("children")] public string[]? Children;

        public SerializedDagNode(T2 value) : base(value)
        {

        }
    }

    [Virtual]
    public class DagNode<T2>
    {
        public T2 Value;

        /// <summary>
        /// Used during validity checking to assert a valid graph.
        /// </summary>
        internal int _visited;

        internal DagNode(T2 value)
        {
            Value = value;
        }
    }

    public Dag()
    {

    }

    private static void ValidateSerialized(SerializedDagNode<T>[] serialized)
    {
        var mapping = new Dictionary<string, SerializedDagNode<T>>(serialized.Length);
        var toVisit = new List<SerializedDagNode<T>>(serialized.Length);

        foreach (var node in serialized)
        {
            mapping[node.Id] = node;
        }

        while (toVisit.Count != 0)
        {
            var node = toVisit.Pop();

            // okay wait how do i find cycles?
            // OKAY SO
            // Every time you visit a node, add one to the visit count.
            // IF that count ever exceeds the number of parents it has, you have a loop and are free to die
            // Yes, this algorithm is worst-case O(n^2) but i DO NOT CARE it's usually quite close to O(n).
            // now i need lun- din- wait it's 3pm wth

            node._visited += 1;

            if (node.Children is null)
                continue;

            foreach (var child in node.Children)
            {
                toVisit.Add(mapping[child]);
            }
        }

    }
}
